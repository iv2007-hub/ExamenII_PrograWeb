using System.ComponentModel.DataAnnotations;

namespace ExamenII_PrograWeb.DTOs;

public class RegisterRequestDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}