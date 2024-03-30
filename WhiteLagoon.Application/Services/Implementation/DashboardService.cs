using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        // private readonly DateTime  currentDate = DateTime.Now;
        private readonly DateTime DateBefore30Days = DateTime.Now.AddDays(-30);

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PieChartDTO> GetCustomerBookingPieChartData()
        {
            PieChartDTO pieChartVm = new PieChartDTO();
            IEnumerable<Booking> bookings = _unitOfWork.Booking.GetAll(b => b.Status != BookingStatus.StatusCancelled || b.Status != BookingStatus.StatusPending, includeProperties: "User");
            //consider new customer as customer with only one booking
            var newCustomerBookings = bookings.GroupBy(booking => booking.User.Id).Where(grpContent => grpContent.Count() == 1);
            var existingCustomerBookingsCount = bookings.Count() - newCustomerBookings.Count();
            pieChartVm.Labels = ["New Customer Bookings", "Existing Cujstomer Bookings"];
            pieChartVm.Series = [newCustomerBookings.Count(), existingCustomerBookingsCount];
            return pieChartVm;
        }

        public async Task<RadialBarChartDTO> GetRegisteredUsersRadialChartData()
        {
            RadialBarChartDTO radialBarChartDto = new RadialBarChartDTO();
            // radialBarChartVm.TotalValue
            IEnumerable<AppUser> Users = _unitOfWork.AppUser.GetAll();
            IEnumerable<AppUser> UsersbeforeLast30Days = Users.Where(booking => booking.CreatedAt < DateBefore30Days);
            int currentBookingCount = Users.Count();

            //int bookingsCountBefore30Days = bookingsBefore30Days.Count();
            int usersDifference = currentBookingCount - UsersbeforeLast30Days.Count();
            decimal calculatedPercentage = (decimal)100 * usersDifference / UsersbeforeLast30Days.Count();


            //radialBarChartVm.TotalValue = calculatedPercentage;
            radialBarChartDto.Labels = new string[] { "Total Users" };
            radialBarChartDto.ValueChangeLabel = usersDifference;
            radialBarChartDto.TotalCountLabel = currentBookingCount.ToString();
            radialBarChartDto.Series = new decimal[] { decimal.Round(calculatedPercentage, 2) };
            return radialBarChartDto;
        }

        public async Task<RadialBarChartDTO> GetRevenueRadialChartData()
        {
            RadialBarChartDTO radialBarChartDto = new RadialBarChartDTO();
            IEnumerable<Booking> bookings = _unitOfWork.Booking.GetAll(b => b.Status != BookingStatus.StatusCancelled || b.Status != BookingStatus.StatusPending);
            int RevenueInBookings = (int)bookings.Sum(booking => booking.TotalCost);
            IEnumerable<Booking> bookingsInLast30Days = bookings.Where(booking => booking.BookingDate < DateBefore30Days);
            int RevenueBookingsInLast30Days = (int)bookingsInLast30Days.Sum(booking => booking.TotalCost);

            //int bookingsCountBefore30Days = bookingsBefore30Days.Count();
            int revenueDifference = RevenueInBookings - RevenueBookingsInLast30Days;
            decimal calculatedPercentage = (decimal)100 * revenueDifference / RevenueBookingsInLast30Days;


            //radialBarChartVm.TotalValue = calculatedPercentage;
            radialBarChartDto.Labels = new string[] { "Total Revenue" };
            radialBarChartDto.ValueChangeLabel = revenueDifference;
            radialBarChartDto.TotalCountLabel = RevenueInBookings.ToString();
            radialBarChartDto.Series = new decimal[] { decimal.Round(calculatedPercentage, 2) };
            return radialBarChartDto;
        }

        public async Task<RadialBarChartDTO> GetTotalBookingRadialChartData()
        {
            RadialBarChartDTO radialBarChartDto = new RadialBarChartDTO();
            // radialBarChartVm.TotalValue
            IEnumerable<Booking> bookings = _unitOfWork.Booking.GetAll(b => b.Status != BookingStatus.StatusCancelled || b.Status != BookingStatus.StatusPending);
            IEnumerable<Booking> bookingsInLast30Days = bookings.Where(booking => booking.BookingDate < DateBefore30Days);
            int currentBookingCount = bookings.Count();

            //int bookingsCountBefore30Days = bookingsBefore30Days.Count();
            int bookingsDifference = currentBookingCount - bookingsInLast30Days.Count();
            decimal calculatedPercentage = (decimal)100 * bookingsDifference / bookingsInLast30Days.Count();


            //radialBarChartVm.TotalValue = calculatedPercentage;
            radialBarChartDto.Labels = new string[] { "Total Bookings" };
            radialBarChartDto.ValueChangeLabel = bookingsDifference;
            radialBarChartDto.TotalCountLabel = currentBookingCount.ToString();
            radialBarChartDto.Series = new decimal[] { decimal.Round(calculatedPercentage, 2) };
            return radialBarChartDto;

        }
    }
}
