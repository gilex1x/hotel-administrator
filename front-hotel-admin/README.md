# Horizonte Hoteles

Frontend Next.js para explorar hoteles y administrar el catálogo conectado a la API .NET.

## Requisitos

- Node.js y npm
- API .NET ejecutándose en `http://localhost:5002` o en la URL configurada

## Desarrollo local

Desde esta carpeta instala dependencias e inicia Next.js:

```bash
npm install
npm run dev
```

Abre [http://localhost:3000](http://localhost:3000). La página pública permite listar hoteles y filtrar por nombre y calificación promedio mínima. En `/admin`, el administrador puede iniciar sesión y crear, editar o eliminar hoteles.

La URL predeterminada de la API es `http://localhost:5002`. Para cambiarla, crea `.env.local`:

```env
NEXT_PUBLIC_API_URL=https://localhost:7208
```

Configura `Frontend:Origin` en la API para que coincida con el origen del frontend (por defecto `http://localhost:3000`). La API también debe tener configuradas sus credenciales iniciales de administrador y la clave de firma JWT.
