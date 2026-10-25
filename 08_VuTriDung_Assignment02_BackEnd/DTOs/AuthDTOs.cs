using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace _08_VuTriDung_Assignment02.DTOs
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDTO
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("accountId")]
        public short AccountId { get; set; }

        [JsonPropertyName("accountName")]
        public string AccountName { get; set; } = string.Empty;

        [JsonIgnore]
        public short AccountID => AccountId;

        [JsonIgnore]
        public string AccountEmail => Email;

        [JsonIgnore]
        public string RoleName => Role;

        [JsonIgnore]
        public int AccountRole => Role switch
        {
            "Admin" => 0,
            "Staff" => 1,
            "Lecturer" => 2,
            _ => 3
        };
    }
}
