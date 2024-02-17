using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Domain.Entities
{
    public  class Booking
    {
        [Key]
        public int Id { get; set; }


        //UserTable
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        //VillaTable
        [Required]
        public int VillaId {  get; set; }
        [ForeignKey("VillaId")]
        public Villa SelectedVilla { get; set; }



        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }



        [Required]
        public double TotalCost { get; set; }
        //public int Nights { get; set; }
        public string? Status { get; set; }


        [Required]
        public DateTime? BookingDate { get; set; }
        [Required]
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }


        public bool IsPaymentSuccessful { get; set; } = false;
        public DateTime PaymentDate { get; set; } 

        //start of stripe payment integration
        public string? StripesSessionId { get; set; }

        //Stripe paymentIntentId can be used to uniquely identify the transaction;then it can be used refund process
        public string? StripePaymentIntentId { get; set; }
        //end of stripe payment integration

        public DateTime ActualCheckingDate { get; set; }
        public DateTime ActualCheckOutDate { get; set; }
        public int VillaNumber { get; set; }

    }
}
