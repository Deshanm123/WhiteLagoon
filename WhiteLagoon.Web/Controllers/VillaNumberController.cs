using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        //private readonly IVillaNumberRepository _villaNumRepo;
        //private readonly IVillaRepository _villaRepo;

        //public VillaNumberController(IVillaNumberRepository villaNumRepo,IVillaRepository villaRepo)
        //{
        //   _villaNumRepo = villaNumRepo;
        //    _villaRepo  = villaRepo;
        //}

        private readonly IUnitOfWork _unitOfWork;
        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //var villaNumberList = _context.VillaNumbers.Include(villaNum => villaNum.Villa ).ToList();
           // var villaNumberList = _villaNumRepo.GetAll(null,"Villa" );
            var villaNumberList = _unitOfWork.VillaNumber.GetAll(null,"Villa" );
            return View(villaNumberList);
        }

        private IEnumerable<SelectListItem> extractVillaToSelectList()
        {
            IEnumerable<SelectListItem> drpDown = _unitOfWork.Villa
                                                                .GetAll()
                                                                .Select(s => 
                                                                          new SelectListItem {
                                                                              Text = s.Name,  
                                                                              Value = s.Id.ToString() 
                                                                          }
                                                                 );
            return drpDown;
            //    //method syntax
            //    IEnumerable<SelectListItem> drpDown = _context.Villas
            //                                            .ToList()
            //                                            .Select(s => new SelectListItem
            //                                            {
            //                                                Text = s.Name,
            //                                                Value = s.Id.ToString(),
            //                                            });
            //    return drpDown;
            //    //query syntax
            //    //IEnumerable<SelectListItem> villaNumberListDrpDwn =
            //    //       (from drpItem in _context.Villas
            //    //       select
            //    //       new SelectListItem
            //    //       {
            //    //           Text = drpItem.Name,
            //    //           Value = drpItem.Id.ToString(),
            //    //           //Selected =

            //    //       });


        }
        public IActionResult Create()
        {
            VillaNumberVm villaCreateVm = new VillaNumberVm();
            villaCreateVm.villaNameDropDown = extractVillaToSelectList();
            return View(villaCreateVm);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVm obj)
        {
            bool assignedVillaNumberExists = _unitOfWork.VillaNumber.GetAll().Any(record => record.Villa_Number == obj.villaNumber.Villa_Number);
            if (assignedVillaNumberExists)
            {
                TempData["error"] = "Errror! Villa Number can't be assigned. It already Exists!";
            }
            else if (ModelState.IsValid && !assignedVillaNumberExists)
            {
                //_villaNumRepo.Add(obj.villaNumber);
                _unitOfWork.VillaNumber.Add(obj.villaNumber);
                _unitOfWork.VillaNumber.Save();
                TempData["success"] = "Villa Number has been sucessfully assigned";
                return RedirectToAction("Index", "VillaNumber");
            }
            else
            {
                TempData["error"] = "Errror! Villa Number can't be assigned.SOMETHING IS WRONG!";
            }
            obj.villaNameDropDown = extractVillaToSelectList();
            return View(obj);

        }


        public IActionResult Update(int villaNumberId)
        {
            VillaNumber? villaNumObj = _unitOfWork.VillaNumber.Get(villaNumRecord => villaNumRecord.Villa_Number == villaNumberId);

            VillaNumberVm villaNumberUpdateVM = new VillaNumberVm()
            {
                villaNumber = villaNumObj,
                villaNameDropDown = extractVillaToSelectList()
            };

            if (villaNumObj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberUpdateVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVm obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.VillaNumber.Update(obj.villaNumber);
                _unitOfWork.VillaNumber.Save();
                TempData["success"] = $"The {obj.villaNumber.Villa_Number} Villa Record Number is successfully updated";
                return RedirectToAction("Index", "VillaNumber");
            }
            obj.villaNameDropDown = extractVillaToSelectList();
            TempData["error"] = $"Error! Couldn't Update the {obj.villaNumber.Villa_Number} Villa Number Record ";
            return View(obj);


        }

        [HttpGet]
        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVm deleteVillaNumberVm = new VillaNumberVm();

            VillaNumber villaNumberObj = _unitOfWork.VillaNumber.Get(villaNum => villaNum.Villa_Number == villaNumberId);
            if (villaNumberObj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            deleteVillaNumberVm.villaNumber = villaNumberObj;
            deleteVillaNumberVm.villaNameDropDown = extractVillaToSelectList();
            return View(deleteVillaNumberVm);
        }

        [HttpPost]
        public IActionResult DeleteVillaNumber(int villaNumberId)
        {
            try
            {
                VillaNumber? selectedVillaNumber = _unitOfWork.VillaNumber.Get(villaNum=> villaNum.Villa_Number  == villaNumberId);
                if (selectedVillaNumber is not null)
                {
                    _unitOfWork.VillaNumber.Remove(selectedVillaNumber);
                    _unitOfWork.VillaNumber.Save();
                    TempData["success"] = "Villa Number has been deleted successfully.";
                    return RedirectToAction("Index", "VillaNumber");
                }
                else
                {
                    // Villa not found, return a 404 Not Found response or handle it appropriately.
                    TempData["error"] = $"Villa with ID {villaNumberId} not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error! Villa Number Entry couldn't be deleted /n {ex.ToString()}";
                // Log the exception or handle it appropriately.
                return RedirectToAction("Index", "VillaNumber");
            }
            return RedirectToAction("Index", "VillaNumber");
        }

    }
}
