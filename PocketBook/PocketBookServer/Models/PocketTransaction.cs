
using System.ComponentModel.DataAnnotations;

namespace PocketBookServer.Models;

public class PocketTransaction
{
    [Key]
    public string Id { get; set; }
    public string UserId { get; set; }
    public double Amount { get; set; }
    public bool IsIncome { get; set; }
    public string Category { get; set; }
    public string Date { get; set; }
}
