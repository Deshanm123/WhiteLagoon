using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        void Save();
        void Update(Booking booking);
        void UpdateBookingStatus(int bookingId, string bookingStatus);
        void UpdateStripePaymentDetails(int bookingId, string sessionId, string paymentIntentId);
    }
}
