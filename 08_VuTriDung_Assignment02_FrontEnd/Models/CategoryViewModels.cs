using System.ComponentModel.DataAnnotations;

namespace _08_VuTriDung_Assignment02_FrontEnd.Models
{
    public class CategoryViewModel
    {
        public short CategoryID { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Description is required.")]
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string CategoryDesciption { get; set; } = string.Empty;

        public short? ParentCategoryID { get; set; }

        public string? ParentCategoryName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
