using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Diagnostics;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.Models;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HomeVm homevm = new HomeVm();
            homevm.Villas = _unitOfWork.Villa.GetAll(includeProperties:"VillaAmenities");
            return View(homevm);
        }

       

        public IActionResult CheckAvialabilityByDate(string CheckInDate , string CheckOutDate, int Occupancy)
        {
            HomeVm homeVm = new HomeVm();

            homeVm.CheckInDate = DateOnly.Parse(CheckInDate);
            homeVm.CheckOutDate = DateOnly.Parse(CheckOutDate);
            homeVm.Occupancy = Occupancy;

            var villaCollection = _unitOfWork.Villa.GetAll(villa => villa.Occupancy > Occupancy, "VillaAmenities");
            if (villaCollection?.Any() == true)
            {
                foreach (var villa in villaCollection)
                {
                    if (BookingStatus.GetVillaWithAvaialability(villa, homeVm.Occupancy))
                    {
                        List<Booking> existingBookings = _unitOfWork.Booking.GetAll(b => b.VillaId == villa.Id).ToList();
                        bool result = BookingStatus.GetVillaAvialabilityDate(existingBookings, homeVm.CheckInDate, homeVm.CheckOutDate);
                        villa.IsAvialable = result;
                    }
                    else
                    {
                        villa.IsAvialable = false;
                    }
                }
                homeVm.Villas = villaCollection;
            }
            return PartialView("_VillaShowCase", homeVm);
        }

        //During bookinb past days should be mentioned as sold out and future dates are as available



        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View();
        //}

    }
    
}
