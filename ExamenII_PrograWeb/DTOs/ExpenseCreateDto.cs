using System.ComponentModel.DataAnnotations;

namespace ExamenII_PrograWeb.DTOs;

public class ExpenseCreateDto
{

    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public decimal Amount { get; set; } 
    [Required]
    public string Category { get; set; } = string.Empty;

}