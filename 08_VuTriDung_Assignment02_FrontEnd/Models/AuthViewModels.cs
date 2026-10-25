using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace _08_VuTriDung_Assignment02_FrontEnd.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
    }

    public class UserSessionViewModel
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
