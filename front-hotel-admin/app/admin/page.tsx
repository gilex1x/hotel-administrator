"use client";

import { useRef, useState } from "react";
import type { ChangeEvent, FormEvent } from "react";
import { useAdmin } from "../hooks/useAdmin";
import { getHotelImageUrl } from "../lib/api";
import type { Hotel, HotelInput } from "../lib/api";
import { compressHotelImage } from "../lib/image";

type HotelForm = {
    name: string;
    address: string;
    reviews: string;
};

const emptyForm: HotelForm = { name: "", address: "", reviews: "" };

function toHotelInput(form: HotelForm): HotelInput {
    return {
        name: form.name.trim(),
        address: form.address.trim(),
        reviews: form.reviews
            .split(",")
            .map((value) => value.trim())
            .filter(Boolean)
            .map(Number),
    };
}

export default function AdminPage() {
    const {
        token,
        hotels,
        loading,
        authenticating,
        saving,
        error,
        signIn,
        signOut,
        refreshHotels,
        saveHotel,
        removeHotel,
        clearError,
    } = useAdmin();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [form, setForm] = useState<HotelForm>(emptyForm);
    const [editingId, setEditingId] = useState<number | null>(null);
    const [formError, setFormError] = useState("");
    const [message, setMessage] = useState("");
    const [imageDataUri, setImageDataUri] = useState<string | null>(null);
    const [removeExistingImage, setRemoveExistingImage] = useState(false);
    const [processingImage, setProcessingImage] = useState(false);
    const imageInputRef = useRef<HTMLInputElement>(null);
    const editingHotel = hotels.find((hotel) => hotel.id === editingId);
    const previewImageUrl = imageDataUri
        ?? (editingHotel?.imageUrl && !removeExistingImage ? getHotelImageUrl(editingHotel.id) : null);

    async function handleLogin(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        setMessage("");
        if (await signIn(username, password)) {
            setPassword("");
        }
    }

    function beginEdit(hotel: Hotel) {
        setEditingId(hotel.id);
        setForm({
            name: hotel.name,
            address: hotel.address,
            reviews: hotel.reviews.join(", "),
        });
        setImageDataUri(null);
        setRemoveExistingImage(false);
        if (imageInputRef.current) imageInputRef.current.value = "";
        setMessage("");
        setFormError("");
        clearError();
        window.scrollTo({ top: 0, behavior: "smooth" });
    }

    function resetForm() {
        setEditingId(null);
        setForm(emptyForm);
        setImageDataUri(null);
        setRemoveExistingImage(false);
        if (imageInputRef.current) imageInputRef.current.value = "";
    }

    async function handleImageSelection(event: ChangeEvent<HTMLInputElement>) {
        const file = event.target.files?.[0];
        if (!file) return;

        setFormError("");
        setProcessingImage(true);
        try {
            setImageDataUri(await compressHotelImage(file));
            setRemoveExistingImage(false);
        } catch (imageError: unknown) {
            setImageDataUri(null);
            if (imageInputRef.current) imageInputRef.current.value = "";
            setFormError(imageError instanceof Error ? imageError.message : "No se pudo procesar la imagen.");
        } finally {
            setProcessingImage(false);
        }
    }

    function handleRemoveImage() {
        setImageDataUri(null);
        setRemoveExistingImage(editingId !== null && Boolean(hotels.find((hotel) => hotel.id === editingId)?.imageUrl));
        if (imageInputRef.current) imageInputRef.current.value = "";
    }

    async function handleSave(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        setFormError("");
        clearError();
        setMessage("");

        const reviews = form.reviews
            .split(",")
            .map((value) => value.trim())
            .filter(Boolean);
        if (reviews.some((value) => !/^[1-5]$/.test(value))) {
            setFormError("Las valoraciones deben ser números del 1 al 5 separados por comas.");
            return;
        }

        if (processingImage) return;

        const saved = await saveHotel(editingId, {
            ...toHotelInput(form),
            imageDataUri,
            removeImage: removeExistingImage,
        });
        if (saved) {
            if (editingId === null) {
                setMessage("Hotel creado correctamente.");
            } else {
                setMessage("Cambios guardados correctamente.");
            }
            resetForm();
        }
    }

    async function handleDelete(hotel: Hotel) {
        if (!window.confirm(`¿Eliminar ${hotel.name}? Esta acción no se puede deshacer.`)) return;

        clearError();
        setFormError("");
        setMessage("");
        if (await removeHotel(hotel.id)) {
            if (editingId === hotel.id) resetForm();
            setMessage("Hotel eliminado.");
        }
    }

    function handleSignOut() {
        signOut();
        resetForm();
        setMessage("");
        setFormError("");
    }

    return (
        <main className="admin-page">
            {!token ? (
                <section className="login-wrap">
                    <div className="login-aside">
                        <p className="eyebrow">PANEL DE GESTIÓN</p>
                        <h1>Todo listo para<br />recibir a tus huéspedes.</h1>
                        <p>Administra el catálogo de hoteles de Horizonte desde un solo lugar.</p>
                        <span className="login-aside-mark" aria-hidden="true">H</span>
                    </div>
                    <form className="login-card" onSubmit={handleLogin}>
                        <p className="eyebrow eyebrow-dark">BIENVENIDO DE NUEVO</p>
                        <h2>Iniciar sesión</h2>
                        <p className="login-copy">Ingresa tus credenciales de administrador para continuar.</p>
                        <label className="form-field">
                            <span>Usuario</span>
                            <input
                                autoComplete="username"
                                required
                                value={username}
                                onChange={(event) => setUsername(event.target.value)}
                            />
                        </label>
                        <label className="form-field">
                            <span>Contraseña</span>
                            <input
                                autoComplete="current-password"
                                required
                                type="password"
                                value={password}
                                onChange={(event) => setPassword(event.target.value)}
                            />
                        </label>
                        {error && <p className="notice notice-error" role="alert">{error}</p>}
                        <button className="button button-primary button-full" disabled={authenticating} type="submit">
                            {authenticating ? "Verificando…" : "Entrar al panel"}
                        </button>
                        <p className="login-footnote">Acceso exclusivo para administradores.</p>
                    </form>
                </section>
            ) : (
                <section className="admin-content">
                    <div className="admin-title-row">
                        <div>
                            <p className="eyebrow eyebrow-dark">ADMINISTRACIÓN</p>
                            <h1>Gestiona tus hoteles</h1>
                            <p className="section-copy">Crea, actualiza o elimina hoteles de la colección.</p>
                        </div>
                        <button className="button button-quiet" onClick={handleSignOut} type="button">Cerrar sesión</button>
                    </div>

                    {error && <p className="notice notice-error" role="alert">{error}</p>}
                    {formError && <p className="notice notice-error" role="alert">{formError}</p>}
                    {message && <p className="notice notice-success" role="status">{message}</p>}

                    <div className="admin-grid">
                        <form className="admin-form-card" onSubmit={handleSave}>
                            <p className="eyebrow eyebrow-dark">{editingId === null ? "NUEVA PROPIEDAD" : `EDITANDO HOTEL #${editingId}`}</p>
                            <h2>{editingId === null ? "Añadir un hotel" : "Editar hotel"}</h2>
                            <label className="form-field">
                                <span>Nombre del hotel</span>
                                <input
                                    required
                                    maxLength={120}
                                    value={form.name}
                                    onChange={(event) => setForm({ ...form, name: event.target.value })}
                                    placeholder="Ej. Hotel Sol"
                                />
                            </label>
                            <label className="form-field">
                                <span>Dirección</span>
                                <input
                                    required
                                    maxLength={240}
                                    value={form.address}
                                    onChange={(event) => setForm({ ...form, address: event.target.value })}
                                    placeholder="Calle y ciudad"
                                />
                            </label>
                            <label className="form-field">
                                <span>Valoraciones <span className="field-hint">(opcional, del 1 al 5)</span></span>
                                <input
                                    inputMode="decimal"
                                    value={form.reviews}
                                    onChange={(event) => setForm({ ...form, reviews: event.target.value })}
                                    placeholder="Ej. 5, 4, 5"
                                />
                            </label>
                            <label className="form-field">
                                <span>Imagen <span className="field-hint">(opcional, máximo 400 KB)</span></span>
                                <input
                                    ref={imageInputRef}
                                    accept="image/*"
                                    onChange={(event) => void handleImageSelection(event)}
                                    disabled={saving || processingImage}
                                    type="file"
                                />
                            </label>
                            {processingImage && <p className="field-hint">Procesando imagen…</p>}
                            {previewImageUrl && (
                                <div className="image-preview-wrap">
                                    <img
                                        className="image-preview"
                                        src={previewImageUrl}
                                        alt="Vista previa de la imagen del hotel"
                                    />
                                    <button className="text-button text-danger" disabled={saving || processingImage} onClick={handleRemoveImage} type="button">
                                        Quitar imagen
                                    </button>
                                </div>
                            )}
                            <div className="form-actions">
                                <button className="button button-primary" disabled={saving || processingImage} type="submit">
                                    {processingImage ? "Procesando imagen…" : saving ? "Guardando…" : editingId === null ? "Crear hotel" : "Guardar cambios"}
                                </button>
                                {editingId !== null && (
                                    <button className="button button-quiet" onClick={resetForm} type="button">Cancelar</button>
                                )}
                            </div>
                        </form>

                        <section className="admin-list">
                            <div className="admin-list-heading">
                                <div>
                                    <p className="eyebrow eyebrow-dark">COLECCIÓN</p>
                                    <h2>Hoteles <span className="hotel-count">({hotels.length})</span></h2>
                                </div>
                                <button className="button button-quiet button-small" disabled={loading} onClick={() => void refreshHotels()} type="button">
                                    {loading ? "Cargando…" : "Actualizar"}
                                </button>
                            </div>
                            {hotels.length === 0 && !loading ? (
                                <p className="empty-state compact">Todavía no hay hoteles en la colección.</p>
                            ) : (
                                <div className="admin-hotel-list">
                                    {hotels.map((hotel) => (
                                        <article className="admin-hotel-row" key={hotel.id}>
                                            <div className="admin-hotel-info">
                                                <span className="admin-hotel-id">#{String(hotel.id).padStart(2, "0")}</span>
                                                <div>
                                                    <h3>{hotel.name}</h3>
                                                    <p>{hotel.address}</p>
                                                    <span className="admin-hotel-rating">★ {hotel.averageReviewScore.toFixed(1)} · {hotel.reviews.length} valoraciones</span>
                                                </div>
                                            </div>
                                            <div className="admin-row-actions">
                                                <button className="text-button" onClick={() => beginEdit(hotel)} type="button">Editar</button>
                                                <button className="text-button text-danger" onClick={() => void handleDelete(hotel)} type="button">Eliminar</button>
                                            </div>
                                        </article>
                                    ))}
                                </div>
                            )}
                        </section>
                    </div>
                </section>
            )}
        </main>
    );
}