using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Notemywork.Api.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
