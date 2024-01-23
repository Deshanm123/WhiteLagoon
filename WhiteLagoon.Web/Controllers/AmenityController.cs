using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;
using WhiteLagoon.Web.ViewModels;
using static WhiteLagoon.Application.Common.Utility.Enum;

namespace WhiteLagoon.Web.Controllers
{
    [Authorize(Roles = nameof(UserRoles.Admin))] //only accessible to the admin role
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //List<Amenity> amenityList = _context.Amenities.Include(amenity => amenity.Viilla).ToList();
            IEnumerable<Amenity> amenityList = _unitOfWork.Amenity.GetAll();
            return View(amenityList);
        }

        public List<SelectListItem> GetVillaSelectedList()
        {
            List<SelectListItem> VillaSelectList = _unitOfWork.Villa.GetAll().Select(
                                                   v => new SelectListItem { Value = v.Id.ToString(), Text = v.Name }
                                                ).ToList();
            return VillaSelectList;
        } 
        public IActionResult Create()
        {
            AmenityVm  amenityVm = new AmenityVm();
            amenityVm.VillaSelectList = GetVillaSelectedList();
            return View(amenityVm);
        }

        [HttpPost]
        public IActionResult Create(AmenityVm amenityVm)
        {
            if (ModelState.IsValid)
            {
                Amenity newRecord = new Amenity {  Name = amenityVm.Name, Description = amenityVm.Description, VillaId = amenityVm.VillaId };
                _unitOfWork.Amenity.Add(newRecord);
                _unitOfWork.Amenity.Save();

                TempData["success"] = $"You have successfully added {amenityVm.Name}";

                return RedirectToAction("Index","Amenity");

            }

            var firstError = ModelState.Values.First().Errors[0].ErrorMessage;
            TempData["error"] = $"Error! Unable to add amenity. {firstError}";
            return RedirectToAction("Index","Amenity");
        }

        public IActionResult Update(int amenityId)
        {
            Amenity? selectedAmenity =  _unitOfWork.Amenity.Get(Amenity => Amenity.Id == amenityId);
            if(selectedAmenity != null)
            {
                AmenityVm amenityVm = new AmenityVm()
                {
                    Id = selectedAmenity.Id,
                    Name = selectedAmenity.Name,
                    Description = selectedAmenity.Description,
                    VillaId = selectedAmenity.VillaId,
                };
                amenityVm.VillaSelectList = GetVillaSelectedList();
                return View(amenityVm);
            }
            TempData["error"] = $"Error ! Update Failed. The selected Amenity is missing.";
            return RedirectToAction("Index", "Amenity");
        }

        [HttpPost]
        public IActionResult Update(AmenityVm updateAmenityVm)
        {
            if(ModelState.IsValid)
            {
                Amenity? selectedAmenity = _unitOfWork.Amenity.Get(Amenity => Amenity.Id == updateAmenityVm.Id);
                if(selectedAmenity != null)
                {
                    selectedAmenity.Name = updateAmenityVm.Name;
                    selectedAmenity.Description = updateAmenityVm.Description;
                    selectedAmenity.VillaId = updateAmenityVm.VillaId;

                    _unitOfWork.Amenity.Update(selectedAmenity);
                    _unitOfWork.Amenity.Save();

                    TempData["success"] = $"You have successfully Updated {updateAmenityVm.Name}";
                }
                else
                {
                    TempData["error"] = $"Error ! Update Failed. The selected Amenity is missing.";
                }
                return RedirectToAction("Index", "Amenity");
            }
            var firstError = ModelState.Values.First().Errors[0].ErrorMessage;
            TempData["error"] = $"Error ! Update Failed. Please check the inputs.";
            return RedirectToAction("Index", "Amenity");
            
        }

        public IActionResult Delete(int amenityId)
        {
            Amenity? selectedAmenity = _unitOfWork.Amenity.Get(Amenity =>Amenity.Id ==amenityId);
            if(selectedAmenity != null)
            {
                AmenityVm amenityDeleteVm = new AmenityVm() { 
                                            Id = selectedAmenity.Id, 
                                            Name = selectedAmenity.Name,
                                            Description = selectedAmenity.Description,
                                            VillaId = selectedAmenity.VillaId,
                                            };
                amenityDeleteVm.VillaSelectList = GetVillaSelectedList();
                return View(amenityDeleteVm);
            }
            TempData["error"] = $"Error ! Delete Failed. The selected Amenity is missing.";
            return RedirectToAction("Index", "Amenity");
        }



        //In DElete Form all inputs are disabled and AmenityId  is hidden
        //Disabled Input Values are not passed by the form
        //even input property  is  hidden , its value get passed to the destination
        //tharts why only amenityId is passed  in http post
        [HttpPost]
        public IActionResult DeleteAmenity(int Id)
        {
            Amenity? selectedAmenity = _unitOfWork.Amenity.Get(Amenity => Amenity.Id == Id);
            if(selectedAmenity != null)
            {
                _unitOfWork.Amenity.Remove(selectedAmenity);
                _unitOfWork.Amenity.Save();

                TempData["success"] = $"You have successfully Deleted {selectedAmenity.Name}";
                return RedirectToAction("Index", "Amenity");
            }

            TempData["error"] = $"Error ! Delete Failed. The selected Amenity is missing.";
            return RedirectToAction("Index", "Amenity");
        }



    }
}
