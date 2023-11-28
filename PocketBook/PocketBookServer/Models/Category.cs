using System.ComponentModel.DataAnnotations;

namespace PocketBookServer.Models
{
    public class Category
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public bool IsIncome { get; set; }

        [Required]
        [MaxLength(30)] // Adjust the length as needed
        public string Description { get; set; }

        [MaxLength(300)] // Adjust the length as needed
        public string DetailedInfo { get; set; }

    }
}
