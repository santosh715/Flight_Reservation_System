using System;
using System.Collections.Generic;

namespace Flight_Reservation_System.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public string UserId { get; set; }

    public int FlightId { get; set; }

    public int NumberofSeats { get; set; }

    public string SeatNumbers { get; set; } = null!;

    public DateTime? BookingDate { get; set; }

    public decimal TotalAmount { get; set; }

    public bool PaymentStatus { get; set; }

    public bool? CancellationStatus { get; set; }

    public decimal? RefundAmount { get; set; }

    public bool Status { get; set; }

    public string? FlightNumber { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    // public virtual User User { get; set; } = null!;
}
