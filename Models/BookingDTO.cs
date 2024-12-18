using System;
using System.Collections.Generic;
 
namespace FlightReservations.Models;
 
public class BookingDTO
{
    public int BookingId { get; set; }
 
    public string UserId { get; set; }
 
    public int FlightId { get; set; }
 
    public int NumberOfSeats { get; set; }
 
    public string SeatNumbers { get; set; } = null!;
 
    public DateTime BookingDate { get; set; }
 
    public decimal TotalAmount { get; set; }
 
    public bool PaymentStatus { get; set; }
 
    public bool? CancellationStatus { get; set; }
 
    public decimal? RefundAmount { get; set; }
}
 