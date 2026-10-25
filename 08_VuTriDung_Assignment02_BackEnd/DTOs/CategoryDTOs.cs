using System.ComponentModel.DataAnnotations;

namespace _08_VuTriDung_Assignment02.DTOs
{
    public class CategoryDTO
    {
        public short CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string CategoryDesciption { get; set; } = string.Empty;

        public short? ParentCategoryID { get; set; }

        public string? ParentCategoryName { get; set; }

        public bool? IsActive { get; set; }
    }
}
