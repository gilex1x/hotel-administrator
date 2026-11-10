namespace hotel_admin_api.DTOs;

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);
