using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notemywork.Api.Entities
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        public string NoteTitle { get; set; } = string.Empty;
        public string NoteDescription { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        [ForeignKey(nameof(Category))]
        public required int CategoryId { get; set; }

        [ForeignKey(nameof(User))]
        public required int UserId { get; set; }
    }
}
