export type Hotel = {
  id: number;
  name: string;
  address: string;
  reviews: number[];
  averageReviewScore: number;
};

export type HotelInput = {
  name: string;
  address: string;
  reviews: number[];
};

const apiBaseUrl = (process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5002").replace(/\/+$/, "");

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...init,
    headers: {
      ...(init?.body ? { "Content-Type": "application/json" } : {}),
      ...init?.headers,
    },
  });

  if (!response.ok) {
    const message = (await response.text()).trim();
    throw new Error(message || `La solicitud falló (${response.status}).`);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export function getHotels(filters: { name?: string; rating?: string } = {}, signal?: AbortSignal) {
  const query = new URLSearchParams();
  if (filters.name?.trim()) query.set("name", filters.name.trim());
  if (filters.rating) query.set("rating", filters.rating);
  const suffix = query.size > 0 ? `?${query.toString()}` : "";

  return request<Hotel[]>(`/api/hotels${suffix}`, { signal });
}

export function login(username: string, password: string) {
  return request<{ accessToken: string; expiresAtUtc: string }>("/api/auth/login", {
    method: "POST",
    body: JSON.stringify({ username, password }),
  });
}

export function createHotel(hotel: HotelInput, token: string) {
  return request<Hotel>("/api/hotels", {
    method: "POST",
    headers: { Authorization: `Bearer ${token}` },
    body: JSON.stringify(hotel),
  });
}

export function updateHotel(id: number, hotel: HotelInput, token: string) {
  return request<void>(`/api/hotels/${id}`, {
    method: "PUT",
    headers: { Authorization: `Bearer ${token}` },
    body: JSON.stringify(hotel),
  });
}

export function deleteHotel(id: number, token: string) {
  return request<void>(`/api/hotels/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });
}
