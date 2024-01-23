using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.ViewModels
{
    public class VillaNumberVm
    {
        public VillaNumber? villaNumber {  get; set; }
        public IEnumerable<SelectListItem>? villaNameDropDown { get; set; }
    }
}
