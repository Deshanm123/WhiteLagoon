using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Implementation;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WhiteLagoon.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
       // private readonly DateTime  currentDate = DateTime.Now;

        //datebefore30days
        //private readonly DateOnly DateBefore30Days = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)); 
        private readonly DateTime DateBefore30Days = DateTime.Now.AddDays(-30); 
       // static int currentMonth = Date.Now();
        public DashboardController(IDashboardService dashboardService) {

            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            //RadialBarChartVm radialBarChartBookings = await GetTotalBookingRadialChartData();
            return View("Index");
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
           var result = await _dashboardService.GetTotalBookingRadialChartData();
            return Json(result);
        }


        public async Task<IActionResult> GetRegisteredUsersRadialChartData()
        {
            return Json(await _dashboardService.GetRegisteredUsersRadialChartData());
        }


        public async Task<IActionResult> GetRevenueRadialChartData()
        {
            var result = await _dashboardService.GetRevenueRadialChartData();
            return Json(result);
        }

        public async Task<IActionResult> GetCustomerBookingPieChartData()
        {
            var result = await _dashboardService.GetCustomerBookingPieChartData();
            return Json(result);
            

        }
    }
}
 