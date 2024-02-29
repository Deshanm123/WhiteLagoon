using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.Text.Json;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using static WhiteLagoon.Application.Common.Utility.Enum;

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
        public IActionResult Index()
        {
            return View("Index");
        }

        [Authorize]
        public IActionResult FinalizeBooking(int villaId,DateOnly checkInDate, DateOnly checkOutDate)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            AppUser loggedUser = _unitOfWork.AppUser.Get(user =>user.Id == userId);

            Villa villa = _unitOfWork.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenities");
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
            //TODO : when finishng the booking check whether villa is available or not
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
        
        [Authorize]
        //confirm payment is successfull and update that on db
        public IActionResult BookingDetails(int bookingId )
        {
            Booking booking = _unitOfWork.Booking.Get(booking => booking.Id == bookingId,includeProperties:"SelectedVilla");
             return View(booking);
        }


        [HttpPost]
        [Authorize(Roles = nameof(UserRoles.Admin))]
        public IActionResult CheckInBooking(int bookingId)
        {
            _unitOfWork.Booking.UpdateBookingStatus(bookingId, BookingStatus.StatusCheckedIn);
            _unitOfWork.Booking.Save();
            TempData["success"] = "Booking has been checked In Successfully";
           return RedirectToAction(nameof(BookingDetails), "Booking", new { bookingId = bookingId });
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRoles.Admin))]
        public IActionResult CheckOutBooking(int bookingId)
        {
            _unitOfWork.Booking.UpdateBookingStatus(bookingId, BookingStatus.StatusCompleted);
            _unitOfWork.Booking.Save();
            TempData["success"] = "Booking has been checked out Successfully";
            return RedirectToAction(nameof(BookingDetails), "Booking", new { bookingId = bookingId });
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRoles.Admin))]
        public IActionResult CancelBooking(int bookingId)
        {
            _unitOfWork.Booking.UpdateBookingStatus(bookingId, BookingStatus.StatusCancelled);
            _unitOfWork.Booking.Save();
            TempData["success"] = "Booking Cancellation Successfull";
            return RedirectToAction(nameof(BookingDetails), "Booking", new { bookingId = bookingId });
        }



        #region ApiEndCalls
        [HttpGet]
       [Authorize]
        public IActionResult GetAllBookings(string? status )
        {
            if (string.IsNullOrEmpty(status) || status == "null")
            {
                status = BookingStatus.StatusPending.ToString();
            }
            IEnumerable<Booking> bookings = null;
            if (User.IsInRole(UserRoles.Admin.ToString()))
            {
                bookings = _unitOfWork.Booking.GetAll(b => b.Status == status,includeProperties:"User");
            }
            if (User.IsInRole(UserRoles.Customer.ToString()))
            {

                var claimIdentityInstance = (ClaimsIdentity)User.Identity;
                string loggedUserId = claimIdentityInstance.FindFirst(ClaimTypes.NameIdentifier).Value;
                bookings = _unitOfWork.Booking.GetAll(booking => booking.UserId == loggedUserId && booking.Status == status, includeProperties: "User");

            }
            return Json(new { data = bookings });
        }

        #endregion
    }
}
