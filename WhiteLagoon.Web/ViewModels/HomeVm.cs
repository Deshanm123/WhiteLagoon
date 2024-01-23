using System.ComponentModel.DataAnnotations;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.ViewModels
{
    public class HomeVm
    {
        public IEnumerable<Villa> Villas { get; set; }

        [Required]
        [Range(0,int.MaxValue)]
        public int Occupancy { get; set; }
        public DateOnly? CheckInDate { get; set; } 
        public DateOnly? CheckOutDate { get; set; } 

    }
}
