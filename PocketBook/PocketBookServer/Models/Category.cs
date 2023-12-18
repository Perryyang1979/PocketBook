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
        public string Description { get; set; } = string.Empty;

        [MaxLength(300)] // Adjust the length as needed
        public string DetailedInfo { get; set; } =string.Empty;
        [Required]
        public string UserGroupId { get; set; }= string.Empty;

    }
}
