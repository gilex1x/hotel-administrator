

import { getHotelImageUrl } from "../lib/api";

export default function HotelCard({ hotel, index }: { hotel: Hotel; index: number }) {
    return (
        <article className="hotel-card" key={hotel.id}>
            <div className={`hotel-card-art hotel-art-${(index % 4) + 1}${hotel.imageUrl ? " has-image" : ""}`}>
                {hotel.imageUrl && (
                    <img className="hotel-card-image" src={getHotelImageUrl(hotel.id)} alt={`Imagen de ${hotel.name}`} />
                )}
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
    )

}