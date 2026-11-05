using System.ComponentModel.DataAnnotations;

namespace hotel_admin_api.DTOs;

public sealed class LoginRequest
{
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string Password { get; set; } = string.Empty;
}
