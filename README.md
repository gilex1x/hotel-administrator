# hotel-administrator


## Backend authentication

The API seeds an admin account into the SQLite `Users` table on its first start. Configure these environment variables before running the API:

- `Authentication__AdminUsername`
- `Authentication__AdminPassword` (at least 12 characters)
- `Authentication__SigningKey` (at least 32 bytes; keep it secret)

For local PowerShell use, generate a signing key and set the credentials in the current shell:

```powershell
$env:Authentication__SigningKey = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
$env:Authentication__AdminUsername = "admin"
$env:Authentication__AdminPassword = "replace-with-a-long-local-password"
```

Sign in with `POST /api/auth/login` and the configured credentials. Send the returned access token as a bearer token to create, update, or delete hotels. Hotel reads and login remain public. The configured admin username must match the seeded user on subsequent starts; password changes require an explicit account-management/migration process.


#### Guides and tutorials used for build this

[ https://learn.microsoft.com/es-es/shows/on-dotnet/how-do-i-use-csharp-and-dotnet-with-sqlite ]( How do i use csharp and dotnet whit sqlite? )

( https://youtu.be/38GNKtclDdE?si=TE6SdrW5wSKyOtXS )[ Buils REST API in .NET 9 ]