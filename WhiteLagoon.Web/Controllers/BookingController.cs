using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
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
        public IActionResult FinalizeBooking(int villaId,DateOnly checkInDate, DateOnly checkOutDate)
        {
            Villa villa = _unitOfWork.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenity");
            Booking booking = new Booking()
            {
                VillaId = villaId,
               // SelectedVilla = _unitOfWork.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenity"),
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate
            };
            booking.SelectedVilla = villa;
            return View(booking);
         } 
    }
}
