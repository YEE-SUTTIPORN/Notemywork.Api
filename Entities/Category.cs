using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Notemywork.Api.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public string CategoryDescription { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [JsonIgnore]
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public required int UserId { get; set; }
        //public User User { get; set; }
    }
}
