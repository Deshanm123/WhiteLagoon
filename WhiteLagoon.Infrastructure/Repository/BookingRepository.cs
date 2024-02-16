using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking> , IBookingRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public BookingRepository(ApplicationDbContext dbContext): base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(Booking booking)
        {
            _dbContext.Update(booking);
        }

        public void UpdateBookingStatus(int bookingId , string bookingStatus)
        {
            Booking booking = _dbContext.Bookings.Find(bookingId);
            if (booking != null)
            {
                booking.Status = bookingStatus;
                if(booking.Status == BookingStatus.StatusCheckedIn)
                {
                    booking.ActualCheckingDate = DateTime.Now;
                }
                 if(booking.Status == BookingStatus.StatusCompleted)
                {
                    booking.ActualCheckOutDate = DateTime.Now;
                }
                _dbContext.Update(booking);
                
            }
              
        }

        public void  UpdateStripePaymentDetails(int bookingId, string sessionId, string paymentIntentId)
        {
            Booking booking = _dbContext.Bookings.Find(bookingId);
            if(booking != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    booking.StripesSessionId = sessionId;
                }
                if(!string.IsNullOrEmpty(paymentIntentId))
                {
                    booking.StripePaymentIntentId = sessionId;
                    booking.PaymentDate = DateTime.Now;
                    booking.IsPaymentSuccessful = true;
                }
                _dbContext.Update(booking);
            }
        }
    }
}
