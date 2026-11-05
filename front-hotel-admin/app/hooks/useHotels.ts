"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import { getHotels } from "../lib/api";
import type { Hotel } from "../lib/api";

export type HotelFilters = {
  name?: string;
  rating?: string;
};

export function useHotels(filters: HotelFilters) {
  const { name, rating } = filters;
  const [hotels, setHotels] = useState<Hotel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [debouncedName, setDebouncedName] = useState(name?.trim() ?? "");
  const [refreshVersion, setRefreshVersion] = useState(0);
  const activeRequest = useRef<AbortController | null>(null);
  const previousName = useRef(name?.trim() ?? "");

  useEffect(() => {
    const nextName = name?.trim() ?? "";
    if (nextName === previousName.current) return;
    previousName.current = nextName;

    activeRequest.current?.abort();
    setLoading(true);
    setError("");

    const timeoutId = window.setTimeout(() => {
      setDebouncedName(nextName);
      setRefreshVersion((version) => version + 1);
    }, 500);

    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [name]);

  useEffect(() => {
    const controller = new AbortController();
    activeRequest.current = controller;
    setLoading(true);
    setError("");

    getHotels({ name: debouncedName, rating }, controller.signal)
      .then((results) => {
        if (!controller.signal.aborted) setHotels(results);
      })
      .catch((requestError: unknown) => {
        if (!controller.signal.aborted) {
          setError(requestError instanceof Error ? requestError.message : "No se pudieron cargar los hoteles.");
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });

    return () => {
      controller.abort();
      if (activeRequest.current === controller) activeRequest.current = null;
    };
  }, [debouncedName, rating, refreshVersion]);

  const refresh = useCallback(() => {
    setRefreshVersion((version) => version + 1);
  }, []);

  return { hotels, loading, error, refresh };
}