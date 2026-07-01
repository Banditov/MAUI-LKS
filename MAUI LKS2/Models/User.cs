using System.Text.Json.Serialization;

namespace MAUI_LKS2.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = "User";

        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }
    }
}