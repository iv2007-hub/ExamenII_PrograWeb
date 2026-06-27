using System.ComponentModel.DataAnnotations;

namespace ExamenII_PrograWeb.DTOs;

public class ExpenseCreateDto
{

    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string Amount { get; set; } = string.Empty;
    [Required]
    public string Category { get; set; } = string.Empty;

}