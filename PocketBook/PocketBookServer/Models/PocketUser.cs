using System.ComponentModel.DataAnnotations;

namespace PocketBookServer.Models
{
    public class PocketUser
    {
        [Key]
        public string Id { get; set; }

        //same group Id can share record and review ,operate pocket book
        //GUID
        [Required]
        [MaxLength(40)] // guid length is 36
        public string GroupId { get; set; }

        [Required]
        [MaxLength(20)] // 
        public string Name { get; set; }

        [Required]
        [MaxLength(length: 256)] // 
        public string Password { get; set; }
        [Required]
        [MaxLength(50)] // 
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 
    }
}
