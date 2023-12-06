
using System.ComponentModel.DataAnnotations;

namespace PocketBookServer.Models;

public class PocketTransaction
{
    [Key]
    public string Id { get; set; }
    public string UserId { get; set; }
    public double Amount { get; set; }
    public bool IsIncome { get; set; }

    [Required]
    [MaxLength(30)] // Adjust the length as needed
    public string Category { get; set; }
    [Required]
    public string Date { get; set; }

    public string Identification { get; set; }
    [MaxLength(100)] // Adjust the length as needed
    public string Comments { get; set; }

}
