using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WhiteLagoon.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WhiteLagoon.Web.ViewModels
{
    public class AmenityVm
    {
        // public Amenity? Amenity { get; set; }

        
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        [ForeignKey("Villa")]
        public int VillaId { get; set; }
        [ValidateNever]
        public List<SelectListItem> VillaSelectList { get; set; }

    }
}
