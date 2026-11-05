
"use client";

import { useState } from "react";
import type { ReactNode } from "react";
import HotelSearchBar from "./components/hotel-search-bar";
import { useHotels } from "./hooks/useHotels";
import HotelCard from "./components/hotelCard";

export default function HomeContent({ footerYear }: { footerYear: ReactNode }) {
  const [filters, setFilters] = useState({ name: "", rating: "" });
  const { hotels, loading, error } = useHotels(filters);

  function clearSearch() {
    setFilters({ name: "", rating: "" });
  }

  return (
    <main>

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
              <HotelCard key={hotel.id} hotel={hotel} index={index} />
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
