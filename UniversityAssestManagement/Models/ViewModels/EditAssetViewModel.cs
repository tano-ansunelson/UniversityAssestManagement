using System.ComponentModel.DataAnnotations;
using UniversityAssestManagement.Models;

namespace UniversityAssetsManagement.Models.ViewModels
{
    public class EditAssetViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Asset name is required.")]
        [Display(Name ="Asset Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; } = AssetType.Unique;


        public int? Quantity { get; set; }

        [Required(ErrorMessage ="Please select a location.")]
        public int LocationId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage ="Value must be positive.")]
        public decimal Value { get; set; }

        public string? Description { get; set; }

        public List<LocationDropdownItem> Locations { get; set; } = new();

    }
}
