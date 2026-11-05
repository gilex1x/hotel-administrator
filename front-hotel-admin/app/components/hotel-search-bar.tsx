"use client";

type HotelSearchBarProps = {
  name: string;
  rating: string;
  onNameChange: (name: string) => void;
  onRatingChange: (rating: string) => void;
  onClear: () => void;
};

export default function HotelSearchBar({
  name,
  rating,
  onNameChange,
  onRatingChange,
  onClear,
}: HotelSearchBarProps) {
  return (
    <div className="filter-bar">
      <label className="filter-field">
        <span>Nombre del hotel</span>
        <input
          type="search"
          placeholder="Ej. Hotel Sol"
          value={name}
          onChange={(event) => onNameChange(event.target.value)}
        />
      </label>
      <label className="filter-field filter-rating">
        <span>Valoración mínima</span>
        <select value={rating} onChange={(event) => onRatingChange(event.target.value)}>
          <option value="">Cualquier valoración</option>
          <option value="1">1 estrella o más</option>
          <option value="2">2 estrellas o más</option>
          <option value="3">3 estrellas o más</option>
          <option value="4">4 estrellas o más</option>
          <option value="5">5 estrellas</option>
        </select>
      </label>
      <button className="button button-quiet" type="button" onClick={onClear}>Limpiar</button>
    </div>
  );
}
