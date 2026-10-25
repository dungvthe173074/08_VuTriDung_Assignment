using System.ComponentModel.DataAnnotations;

namespace _08_VuTriDung_Assignment02.DTOs
{
    public class SystemAccountDTO
    {
        public short AccountID { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(70)]
        public string AccountEmail { get; set; } = string.Empty;

        [Required]
        public int AccountRole { get; set; } // 1 = Staff, 2 = Lecturer

        [StringLength(70)]
        public string? AccountPassword { get; set; }
    }
}
