# Horizonte Hoteles

Aplicación para explorar y administrar un catálogo de hoteles. Incluye una API REST en ASP.NET Core y un frontend en Next.js.

## Funcionalidades

- Listado público de hoteles con filtros por nombre y calificación promedio mínima.
- Panel de administración con inicio de sesión y operaciones para crear, editar y eliminar hoteles.
- Autenticación JWT para proteger las operaciones de escritura.
- Persistencia local en SQLite.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20.9 o posterior](https://nodejs.org/) y npm

## Ejecutar en desarrollo

Se necesitan dos terminales. Desde la raíz del repositorio:

### 1. Iniciar la API

```powershell
dotnet run --project backend\hotel-admin-api\hotel-admin-api\hotel-admin-api.csproj --launch-profile http
```

La API estará disponible en `http://localhost:5002`. En el entorno de desarrollo, el documento OpenAPI está en [http://localhost:5002/openapi/v1.json](http://localhost:5002/openapi/v1.json).

El perfil `http` configura las variables requeridas para desarrollo. En un primer inicio con una base de datos vacía, la API crea las tablas y agrega hoteles de ejemplo. La base de datos SQLite se guarda en `backend/hotel-admin-api/hotel-admin-api/hotels.db`; los cambios del catálogo persisten entre ejecuciones.

### 2. Iniciar el frontend

En una segunda terminal:

```powershell
cd front-hotel-admin
npm ci
npm run dev
```

Abre [http://localhost:3000](http://localhost:3000). El frontend usa `http://localhost:5002` como URL predeterminada de la API. Si la API se ejecuta en otra dirección, configura `NEXT_PUBLIC_API_URL` en `front-hotel-admin\.env.local` y reinicia el servidor de Next.js.

## Probar la aplicación

1. En la página principal, verifica que se listen los hoteles.
2. Escribe parte de un nombre: la búsqueda se aplica automáticamente después de 500 ms. Cambiar la calificación mínima aplica el filtro de inmediato.
3. Abre [http://localhost:3000/admin](http://localhost:3000/admin) e inicia sesión con las credenciales de desarrollo del perfil `http`:
   - Usuario: `admin`
   - Contraseña: `Adminpassword@`
4. Crea un hotel, edítalo, elimínalo y confirma que el catálogo público refleja los cambios.

> Estas credenciales y la clave de firma del perfil de desarrollo son exclusivamente para pruebas locales. No las uses en producción. La cuenta se crea cuando la tabla de usuarios está vacía; si la base ya contiene usuarios, el nombre configurado debe coincidir con una cuenta existente.

## API principal

| Método | Ruta | Acceso | Descripción |
|---|---|---|---|
| `GET` | `/api/hotels` | Público | Lista hoteles. Acepta `name` y `rating` como filtros opcionales. |
| `GET` | `/api/hotels/{id}` | Público | Obtiene un hotel por su identificador. |
| `POST` | `/api/auth/login` | Público | Inicia sesión y devuelve un token JWT. |
| `POST` | `/api/hotels` | Administrador | Crea un hotel. |
| `PUT` | `/api/hotels/{id}` | Administrador | Actualiza un hotel. |
| `DELETE` | `/api/hotels/{id}` | Administrador | Elimina un hotel. |

Ejemplo de filtro:

```text
http://localhost:5002/api/hotels?name=hotel&rating=4
```

## Configuración de la API

La API requiere `Authentication__AdminUsername`, `Authentication__AdminPassword` (al menos 12 caracteres) y `Authentication__SigningKey` (al menos 32 bytes). El perfil de desarrollo `http` proporciona valores locales para facilitar la prueba. Para otros entornos, configúralos mediante variables de entorno o el mecanismo de secretos del entorno; no guardes claves reales en el repositorio.

El origen permitido por CORS es `http://localhost:3000` de forma predeterminada y puede cambiarse con `Frontend__Origin`.

## Proyectos

- API: [backend/hotel-admin-api/hotel-admin-api](./backend/hotel-admin-api/hotel-admin-api)
- Frontend: [front-hotel-admin](./front-hotel-admin)
