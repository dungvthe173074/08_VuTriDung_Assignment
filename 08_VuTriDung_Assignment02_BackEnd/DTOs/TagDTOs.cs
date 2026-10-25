using System.ComponentModel.DataAnnotations;

namespace _08_VuTriDung_Assignment02.DTOs
{
    public class TagDTO
    {
        public int TagID { get; set; }

        [Required]
        [StringLength(50)]
        public string? TagName { get; set; }

        [StringLength(400)]
        public string? Note { get; set; }
    }
}
