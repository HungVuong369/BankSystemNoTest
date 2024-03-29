using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankSystem.Dtos.Response
{
    public class RefreshTokenResponse
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Token { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public bool IsExpired => DateTime.Now >= Expires;
    }
}
