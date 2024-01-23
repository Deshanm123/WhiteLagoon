using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Diagnostics;
using WhiteLagoon.Application.Common.Interfaces;
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
            homevm.Villas = _unitOfWork.Villa.GetAll();
            return View(homevm);
        }

        public Villa MarkVillaAvialabilityDate(Villa villa, DateOnly? checkInDate, DateOnly? checkOutDate)
        {
            if (checkInDate > checkOutDate || checkInDate < DateOnly.FromDateTime(DateTime.Now) || checkOutDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                villa.IsAvialable = false;
            }
            else
            {
                villa.IsAvialable = true;
            }
            return villa;
        }

        public IActionResult CheckAvialabilityByDate(HomeVm homeVm)
        {
            var villaList = _unitOfWork.Villa.GetAll();
            foreach (var villa in villaList)
            {
                if (villa.Occupancy >= homeVm.Occupancy)
                {
                    MarkVillaAvialabilityDate(villa, homeVm.CheckInDate, homeVm.CheckOutDate);
                }
                else
                {
                    //villa 
                    villa.IsAvialable = false;
                }
            }
            homeVm.Villas = villaList;
            //homeVm.CheckInDate = checkInDate;
            //homeVm.CheckOutDate = checkOutDate;
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