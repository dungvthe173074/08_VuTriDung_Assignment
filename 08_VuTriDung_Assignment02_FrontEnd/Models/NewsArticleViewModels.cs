using System.ComponentModel.DataAnnotations;

namespace _08_VuTriDung_Assignment02_FrontEnd.Models
{
    public class TagViewModel
    {
        public int TagID { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
    }

    public class NewsArticleViewModel
    {
        public string NewsArticleID { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CreatedByID { get; set; }
        public string? CreatedByName { get; set; }
        public short? UpdatedByID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<int> TagIDs { get; set; } = new List<int>();
        public List<TagViewModel> Tags { get; set; } = new List<TagViewModel>();
    }

    public class NewsArticleCreateEditViewModel
    {
        [Required(ErrorMessage = "Article ID is required.")]
        [StringLength(20, ErrorMessage = "Article ID cannot exceed 20 characters.")]
        public string NewsArticleID { get; set; } = string.Empty;

        [StringLength(400, ErrorMessage = "Title cannot exceed 400 characters.")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required.")]
        [StringLength(150, ErrorMessage = "Headline cannot exceed 150 characters.")]
        public string Headline { get; set; } = string.Empty;

        [StringLength(4000, ErrorMessage = "Content cannot exceed 4000 characters.")]
        public string? NewsContent { get; set; }

        [StringLength(400, ErrorMessage = "Source cannot exceed 400 characters.")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Please select a Category.")]
        public short? CategoryID { get; set; }

        public bool NewsStatus { get; set; } = true;

        public short? CreatedByID { get; set; }

        public short? UpdatedByID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public List<int> SelectedTagIDs { get; set; } = new List<int>();

        // For rendering dropdowns / checkboxes in modal
        public List<CategoryViewModel> AvailableCategories { get; set; } = new List<CategoryViewModel>();
        public List<TagViewModel> AvailableTags { get; set; } = new List<TagViewModel>();
    }
}
