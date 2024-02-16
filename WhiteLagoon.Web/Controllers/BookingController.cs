using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult FinalizeBooking(int villaId,DateOnly checkInDate, DateOnly checkOutDate)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            AppUser loggedUser = _unitOfWork.AppUser.Get(user =>user.Id == userId);


            Villa villa = _unitOfWork.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenity");
            Booking booking = new Booking()
            {
                VillaId = villaId,
                // SelectedVilla = _unitOfWork.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenity"),
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                UserId = loggedUser.Id,
                PhoneNumber = loggedUser.PhoneNumber,
                Email = loggedUser.Email,
                Name = loggedUser.Name,

            };
            booking.SelectedVilla = villa;
            var days = (checkOutDate.ToDateTime(TimeOnly.MinValue) - checkInDate.ToDateTime(TimeOnly.MinValue)).TotalDays;
            booking.TotalCost = days * villa.Price;
            return View(booking);
        }

        [HttpPost]
        [Authorize]
        public IActionResult FinalizeBooking(Booking booking )
        {
            var selectedVilla = _unitOfWork.Villa.Get(villa => villa.Id == booking.VillaId);
            booking.Status = BookingStatus.StatusPending;

            //var days = (booking.CheckOutDate.ToDateTime(TimeOnly.MinValue) - booking.CheckInDate.ToDateTime(TimeOnly.MinValue)).TotalDays;
            //booking.TotalCost = days * selectedVilla.Price;
            
            //We are not saving in db untill the purchase is complete
            //_unitOfWork.Booking.Add(booking);
            //_unitOfWork.Booking.Save();
            return RedirectToAction(nameof(BookingConfirmation), new {bookingId = booking.Id});
           
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId )
        {
            return View(bookingId);
        }
    }
}
