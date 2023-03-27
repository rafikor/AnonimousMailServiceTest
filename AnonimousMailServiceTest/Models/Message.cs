using System.ComponentModel.DataAnnotations;

namespace AnonimousMailServiceTest.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Body { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Recipient { get; set; }
        [Required]
        public DateTime TimeSent { get; set; }
    }
}
