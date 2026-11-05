"use client";


import Link from "next/link";

export default function Header() {
    return (
        <header className="site-header">
            <Link className="brand" href="/">
                <span className="brand-mark" aria-hidden="true">H</span>
                <span>HORIZONTE <span className="brand-light">HOTELES</span></span>
            </Link>
            <nav aria-label="Navegación principal">
                <a href="#hoteles">Nuestros hoteles</a>
                <Link className="nav-admin" href="/admin">Acceso administrador</Link>
            </nav>
        </header>
    )
}