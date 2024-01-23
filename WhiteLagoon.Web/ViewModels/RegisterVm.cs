using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WhiteLagoon.Web.ViewModels
{
    public class RegisterVm
    {
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",ErrorMessage ="Password are Mismatching")]
        public string ConfirmPassword { get; set; }


        [Display(Name="Phone Number")]
        public string PhoneNumeber { get; set; }

        public string? RedirectionUrl { get; set; }
        public string? Role {  get; set; }

        [ValidateNever]
        public List<SelectListItem> RoleSelectList { get; set; }

    }
}
