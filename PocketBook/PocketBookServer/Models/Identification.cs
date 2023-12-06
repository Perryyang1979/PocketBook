using System.ComponentModel.DataAnnotations;

namespace PocketBookServer.Models
{
    public class Identification
    {
        [Key] 
        public string Id { get; set; }

        [Required]
        [MaxLength(length: 30)]
        public string Name { get; set; }
    }
}
