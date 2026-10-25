using System.ComponentModel.DataAnnotations;

namespace _08_VuTriDung_Assignment02_FrontEnd.Models
{
    public class ReportFilterViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public List<NewsArticleViewModel> Articles { get; set; } = new List<NewsArticleViewModel>();
    }
}
