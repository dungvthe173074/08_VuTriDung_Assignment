using System.ComponentModel.DataAnnotations;

namespace _08_VuTriDung_Assignment02_FrontEnd.Models
{
    public class AccountViewModel
    {
        public short AccountID { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters.")]
        public string AccountEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        [Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer).")]
        public int AccountRole { get; set; } = 1; // 1 = Staff, 2 = Lecturer

        [StringLength(70, ErrorMessage = "Password cannot exceed 70 characters.")]
        public string? AccountPassword { get; set; }
    }

    public class ProfileViewModel
    {
        public short AccountID { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [StringLength(70)]
        public string AccountEmail { get; set; } = string.Empty;

        public int AccountRole { get; set; }
        public string RoleName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(70, ErrorMessage = "Password cannot exceed 70 characters.")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(70, MinimumLength = 2, ErrorMessage = "Password must be at least 2 characters.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Confirm password does not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
