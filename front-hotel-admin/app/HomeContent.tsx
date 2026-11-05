
"use client";

import { useState } from "react";
import type { ReactNode } from "react";
import HotelSearchBar from "./components/hotel-search-bar";
import { useHotels } from "./hooks/useHotels";

export default function HomeContent({ footerYear }: { footerYear: ReactNode }) {
  const [filters, setFilters] = useState({ name: "", rating: "" });
  const { hotels, loading, error } = useHotels(filters);

  function clearSearch() {
    setFilters({ name: "", rating: "" });
  }

  return (
    <main>
      <section className="hero">
        <div className="hero-content">
          <p className="eyebrow">UNA ESTANCIA PARA RECORDAR</p>
          <h1>Tu próximo destino<br />empieza aquí.</h1>
          <p className="hero-copy">
            Lugares únicos, hospitalidad cercana y momentos que se quedan contigo.
            Encuentra el hotel ideal para tu próxima escapada.
          </p>
          <a className="button button-light" href="#hoteles">Explorar hoteles <span aria-hidden="true">↓</span></a>
        </div>
        <div className="hero-note">
          <span className="hero-note-line" />
          <span>Descubre tu lugar en el mundo</span>
        </div>
      </section>

      <section className="hotel-section" id="hoteles">
        <div className="section-heading">
          <div>
            <p className="eyebrow eyebrow-dark">DESCUBRE HORIZONTE</p>
            <h2>Hoteles para cada viaje</h2>
            <p className="section-copy">Busca tu próxima estancia por nombre o por la valoración de nuestros huéspedes.</p>
          </div>
          <span className="hotel-count">{hotels.length} {hotels.length === 1 ? "hotel" : "hoteles"}</span>
        </div>

        <HotelSearchBar
          name={filters.name}
          rating={filters.rating}
          onNameChange={(name) => setFilters((current) => ({ ...current, name }))}
          onRatingChange={(rating) => setFilters((current) => ({ ...current, rating }))}
          onClear={clearSearch}
        />

        {error && <p className="notice notice-error" role="alert">{error}</p>}
        {loading ? (
          <p className="empty-state" role="status">Cargando hoteles…</p>
        ) : hotels.length > 0 ? (
          <div className="hotel-grid">
            {hotels.map((hotel, index) => (
              <article className="hotel-card" key={hotel.id}>
                <div className={`hotel-card-art hotel-art-${(index % 4) + 1}`}>
                  <span className="hotel-card-number">{String(hotel.id).padStart(2, "0")}</span>
                  <span className="hotel-art-caption">COLECCIÓN HORIZONTE</span>
                </div>
                <div className="hotel-card-body">
                  <div className="hotel-card-title">
                    <h3>{hotel.name}</h3>
                    <span className="rating"><span aria-hidden="true">★</span> {hotel.averageReviewScore.toFixed(1)}</span>
                  </div>
                  <p className="hotel-address">{hotel.address}</p>
                  <p className="review-count">
                    {hotel.reviews.length
                      ? `${hotel.reviews.length} ${hotel.reviews.length === 1 ? "valoración" : "valoraciones"}`
                      : "Aún sin valoraciones"}
                  </p>
                </div>
              </article>
            ))}
          </div>
        ) : !error ? (
          <div className="empty-state">
            <span className="empty-icon" aria-hidden="true">⌕</span>
            <h3>No encontramos hoteles</h3>
            <p>Prueba con otro nombre o reduce la valoración mínima.</p>
          </div>
        ) : null}
      </section>
    </main>
  );
}
