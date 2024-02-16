using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Domain.Entities
{
    public class Villa
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(60)]
        public required string Name {  get; set; }
        [Required]
        public string? Description { get; set; }

        [Display(Name="Price per Day")]

        public double Price {  get; set; }
        public int SqFt {  get; set; }

        [Range(1,10)]
        public int Occupancy {  get; set; }

        [NotMapped]
        public IFormFile? VillaImage { get; set; }

        [Display(Name="Image Url")]
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;

        [NotMapped]
        public bool IsAvialable { get; set; } = true;

        // Navigation property
        public virtual ICollection<Amenity> VillaAmenities { get; set; }
    }
}
