using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WhiteLagoon.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
       // private readonly DateTime  currentDate = DateTime.Now;

        //datebefore30days
        //private readonly DateOnly DateBefore30Days = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)); 
        private readonly DateTime DateBefore30Days = DateTime.Now.AddDays(-30); 
       // static int currentMonth = Date.Now();
        public DashboardController(IUnitOfWork unitOfWork ) {

            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            //RadialBarChartVm radialBarChartBookings = await GetTotalBookingRadialChartData();
            return View("Index");
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            RadialBarChartVm radialBarChartVm = new RadialBarChartVm(); 
           // radialBarChartVm.TotalValue
            IEnumerable<Booking> bookings = _unitOfWork.Booking.GetAll(b => b.Status != BookingStatus.StatusCancelled || b.Status != BookingStatus.StatusPending);
            IEnumerable<Booking> bookingsInLast30Days = bookings.Where(booking => booking.BookingDate < DateBefore30Days);
            int currentBookingCount = bookings.Count();

            //int bookingsCountBefore30Days = bookingsBefore30Days.Count();
            int bookingsDifference = currentBookingCount - bookingsInLast30Days.Count();
            decimal calculatedPercentage = (decimal)100 * bookingsDifference / bookingsInLast30Days.Count();


            //radialBarChartVm.TotalValue = calculatedPercentage;
            radialBarChartVm.Labels = new string[] { "Total Bookings" };
            radialBarChartVm.ValueChangeLabel = bookingsDifference;
            radialBarChartVm.TotalCountLabel = currentBookingCount.ToString();
            radialBarChartVm.Series =  new decimal[] { decimal.Round(calculatedPercentage,2) };
            return Json(radialBarChartVm);

        }


        public async Task<IActionResult> GetRegisteredUsersRadialChartData()
        {
            RadialBarChartVm radialBarChartVm = new RadialBarChartVm();
            // radialBarChartVm.TotalValue
            IEnumerable<AppUser> Users = _unitOfWork.AppUser.GetAll();
            IEnumerable<AppUser> UsersbeforeLast30Days = Users.Where(booking => booking.CreatedAt < DateBefore30Days);
            int currentBookingCount = Users.Count();

            //int bookingsCountBefore30Days = bookingsBefore30Days.Count();
            int usersDifference = currentBookingCount - UsersbeforeLast30Days.Count();
            decimal calculatedPercentage = (decimal)100 * usersDifference / UsersbeforeLast30Days.Count();


            //radialBarChartVm.TotalValue = calculatedPercentage;
            radialBarChartVm.Labels = new string[] { "Total Users" };
            radialBarChartVm.ValueChangeLabel = usersDifference;
            radialBarChartVm.TotalCountLabel = currentBookingCount.ToString();
            radialBarChartVm.Series = new decimal[] { decimal.Round(calculatedPercentage, 2) };
            return Json(radialBarChartVm);

        }


        public async Task<IActionResult> GetRevenueRadialChartData()
        {
            RadialBarChartVm radialBarChartVm = new RadialBarChartVm();
            IEnumerable<Booking> bookings = _unitOfWork.Booking.GetAll(b => b.Status != BookingStatus.StatusCancelled || b.Status != BookingStatus.StatusPending);
            int RevenueInBookings = (int) bookings.Sum(booking => booking.TotalCost);
            IEnumerable<Booking> bookingsInLast30Days = bookings.Where(booking => booking.BookingDate < DateBefore30Days);
            int RevenueBookingsInLast30Days = (int)bookingsInLast30Days.Sum(booking => booking.TotalCost);

            //int bookingsCountBefore30Days = bookingsBefore30Days.Count();
            int revenueDifference = RevenueInBookings - RevenueBookingsInLast30Days;
            decimal calculatedPercentage = (decimal)100 * revenueDifference / RevenueBookingsInLast30Days;


            //radialBarChartVm.TotalValue = calculatedPercentage;
            radialBarChartVm.Labels = new string[] { "Total Revenue" };
            radialBarChartVm.ValueChangeLabel = revenueDifference;
            radialBarChartVm.TotalCountLabel = RevenueInBookings.ToString();
            radialBarChartVm.Series = new decimal[] { decimal.Round(calculatedPercentage, 2) };
            return Json(radialBarChartVm);

        }

        public async Task<IActionResult> GetCustomerBookingPieChartData()
        {
            PieChartVM pieChartVm = new PieChartVM();
            IEnumerable<Booking> bookings = _unitOfWork.Booking.GetAll(b => b.Status != BookingStatus.StatusCancelled || b.Status != BookingStatus.StatusPending,includeProperties:"User");
            //consider new customer as customer with only one booking
            var newCustomerBookings = bookings.GroupBy(booking => booking.User.Id).Where(grpContent => grpContent.Count() == 1);
            var existingCustomerBookingsCount = bookings.Count() - newCustomerBookings.Count();
            pieChartVm.Labels =["New Customer Bookings" ,"Existing Cujstomer Bookings"];
            pieChartVm.Series = [newCustomerBookings.Count(), existingCustomerBookingsCount];
            return Json(pieChartVm);

        }
    }
}
 