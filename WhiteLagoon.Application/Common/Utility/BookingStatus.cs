using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Utility
{
    public static class BookingStatus
    {
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusCheckedIn = "CheckedIn";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static bool GetVillaWithAvaialability(Villa villa, int peopleCount)
        {
            if (villa.Occupancy >= peopleCount)
                return true;
            else
                return false;
        }

        public static bool GetVillaAvialabilityDate(List<Booking> existingBookingsInaSelectedVilla, DateOnly? checkInDate, DateOnly? checkOutDate)
        {
            //get all bookings related to villa
            if (existingBookingsInaSelectedVilla.Count > 0)
            {
                foreach (Booking booking in existingBookingsInaSelectedVilla)
                {
                    if (checkInDate <= booking.CheckInDate)
                    {
                        if (checkOutDate >= booking.CheckOutDate)
                        {
                            return false;
                        }
                    }
                    //early to bookings checking date 
                    else if (checkInDate < booking.CheckInDate)
                    {
                        if (checkOutDate >= booking.CheckInDate)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }

    }
}
