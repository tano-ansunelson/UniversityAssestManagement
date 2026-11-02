using System.ComponentModel.DataAnnotations;

namespace UniversityAssestManagement.Models.ViewModels
{
    public class CollegeViewModel
    {

        public List<CollegeCard> Colleges { get; set; } = new();
    }


    public class CollegeCard
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string ColorClass { get; set; } = string.Empty;

        public int AssetCount { get; set; }
        public int LocationCount { get; set; }

    }


    public class AddCollegeViewModel
    {
        [Required(ErrorMessage = "College name is required. ")]
        [Display(Name = "College Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage ="College code is required.")]
        [StringLength(10, ErrorMessage ="Code cannot exceed 10 characters.")]
        public string Code { get; set; } =string.Empty;

        [Required(ErrorMessage ="Description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a color.")]
        [Display(Name = "Color Theme")]
        public string ColorClass { get; set; } = string.Empty;
    }
}
