
using System.ComponentModel.DataAnnotations;

namespace PocketBookServer.Models;

public class PocketTransaction
{
    [Key]
    public string Id { get; set; }
    [Required]
    public string UserId { get; set; } = string.Empty;
    [Required]
    public string UserGroupId { get; set; } = string.Empty;
    public double Amount { get; set; }
    public bool IsIncome { get; set; }

    [Required]
    [MaxLength(30)] // Adjust the length as needed
    public string Category { get; set; } = string.Empty;
    [Required]
    public string Date { get; set; } = DateTime.Now.ToString();

    public string Identification { get; set; } = string.Empty;
    [MaxLength(100)] // Adjust the length as needed
    public string Comments { get; set; } = string.Empty;

}
