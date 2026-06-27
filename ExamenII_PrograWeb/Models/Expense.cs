namespace ExamenII_PrograWeb.Models;

public class Expense
{
    public string Id { get; set; }          
    public string Description { get; set; } 
    public decimal Amount { get; set; }      
    public string Category { get; set; }     
    public string UserId { get; set; }       
    public DateTime CreatedAt { get; set; } 
}