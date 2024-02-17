using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
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
            booking.BookingDate = DateTime.Now;

            //saving in db 
            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Booking.Save();

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    PriceData =  new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)booking.TotalCost * 100, //unit ammount is expressed in cents ;hence converting cents to pounds
                        Currency = "gbp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = selectedVilla.Name,
                        }

                    },
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"booking/FinalizeBooking?villaId={booking.VillaId}&checkInDate={booking.CheckInDate}&checkOutDate={booking.CheckOutDate}",
            };

            var service = new SessionService();
            Session session = service.Create(options);
            //saving sessionId in  database
            _unitOfWork.Booking.UpdateStripePaymentDetails(booking.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Booking.Save();
            //below is the checkouturl
            Response.Headers.Add("Location", session.Url);


            //saving sessionId in the database
           
           return new StatusCodeResult(303);
           
        }

        [Authorize]
        //confirm payment is successfull and update that on db
        public IActionResult BookingConfirmation(int bookingId )
        {
            Booking booking = _unitOfWork.Booking.Get(booking => booking.Id == bookingId);
            if (booking != null ) 
            {
                if(booking.Status == BookingStatus.StatusPending)
                {
                    var service = new SessionService();
                    Session sessionFromStripe = service.Get(booking.StripesSessionId);

                    //making booking pending satus(before checkout)  to completed status (after succesfull checkout)
                    _unitOfWork.Booking.UpdateBookingStatus(bookingId, BookingStatus.StatusCompleted);
                    _unitOfWork.Booking.UpdateStripePaymentDetails(bookingId, sessionFromStripe.Id, sessionFromStripe.PaymentIntentId);

                    _unitOfWork.Booking.Update(booking);
                    _unitOfWork.Booking.Save();

                }
            }
            return View(bookingId);
        }
    }
}
