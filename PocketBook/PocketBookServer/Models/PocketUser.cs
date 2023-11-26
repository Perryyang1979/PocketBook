using System.ComponentModel.DataAnnotations;

namespace PocketBookServer.Models
{
    public class PocketUser
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(20)] // Adjust the length as needed
        public string Name { get; set; }

        [Required]
        [MaxLength(length: 12)] // 
        public string Password { get; set; }
        [Required]
        [MaxLength(50)] // Adjust the length as needed
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
       
    }
}
