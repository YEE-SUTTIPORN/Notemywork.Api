using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Notemywork.Api.Entities
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        public string NoteTitle { get; set; } = string.Empty;
        public string NoteDescription { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [JsonIgnore]
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        [ForeignKey("Category")]
        public required int CategoryId { get; set; }
        //public Category Category { get; set; }

        [ForeignKey("User")]
        public required int UserId { get; set; }
        //public User User { get; set; }
    }
}
