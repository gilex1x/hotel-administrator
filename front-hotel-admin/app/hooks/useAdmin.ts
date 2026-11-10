"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import {
  createHotel as createHotelRequest,
  deleteHotel as deleteHotelRequest,
  getHotels,
  login as loginRequest,
  updateHotel as updateHotelRequest,
} from "../lib/api";
import type { HotelInput, Hotel } from "../lib/api";

export function useAdmin() {
  const [token, setToken] = useState("");
  const [hotels, setHotels] = useState<Hotel[]>([]);
  const [loading, setLoading] = useState(false);
  const [authenticating, setAuthenticating] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const requestId = useRef(0);

  const refreshHotels = useCallback(async () => {
    if (!token) return false;

    const currentRequestId = ++requestId.current;
    setLoading(true);
    setError("");

    try {
      const results = await getHotels();
      if (requestId.current === currentRequestId) setHotels(results);
      return true;
    } catch (requestError: unknown) {
      if (requestId.current === currentRequestId) {
        setError(requestError instanceof Error ? requestError.message : "No se pudieron cargar los hoteles.");
      }
      return false;
    } finally {
      if (requestId.current === currentRequestId) setLoading(false);
    }
  }, [token]);

  useEffect(() => {
    if (token) {
      void refreshHotels();
    }
  }, [token, refreshHotels]);

  const signIn = useCallback(async (username: string, password: string) => {
    setError("");
    setAuthenticating(true);

    try {
      const result = await loginRequest(username, password);
      setToken(result.accessToken);
      return true;
    } catch {
      setError("No se pudo iniciar sesión. Revisa el usuario y la contraseña.");
      return false;
    } finally {
      setAuthenticating(false);
    }
  }, []);

  const signOut = useCallback(() => {
    requestId.current += 1;
    setToken("");
    setHotels([]);
    setLoading(false);
    setError("");
  }, []);

  const saveHotel = useCallback(async (id: number | null, hotel: HotelInput) => {
    if (!token) {
      setError("Inicia sesión para administrar hoteles.");
      return false;
    }

    setError("");
    setSaving(true);
    try {
      if (id === null) {
        await createHotelRequest(hotel, token);
      } else {
        await updateHotelRequest(id, hotel, token);
      }
      await refreshHotels();
      return true;
    } catch (requestError: unknown) {
      setError(requestError instanceof Error ? requestError.message : "No se pudo guardar el hotel.");
      return false;
    } finally {
      setSaving(false);
    }
  }, [refreshHotels, token]);

  const removeHotel = useCallback(async (id: number) => {
    if (!token) {
      setError("Inicia sesión para administrar hoteles.");
      return false;
    }

    setError("");
    try {
      await deleteHotelRequest(id, token);
      await refreshHotels();
      return true;
    } catch (requestError: unknown) {
      setError(requestError instanceof Error ? requestError.message : "No se pudo eliminar el hotel.");
      return false;
    }
  }, [refreshHotels, token]);

  const clearError = useCallback(() => setError(""), []);

  return {
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
  };
}