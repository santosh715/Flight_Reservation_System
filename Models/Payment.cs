using System;
using System.Collections.Generic;

namespace Flight_Reservation_System.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public bool PaymentStatus { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
