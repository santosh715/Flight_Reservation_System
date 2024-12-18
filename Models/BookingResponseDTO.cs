public class BookingResponseDto
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int FlightId { get; set; }
    public string FlightName { get; set; }
    public int NumberOfSeats { get; set; }
    public string SeatNumbers { get; set; }
    public DateTime? BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool PaymentStatus { get; set; }
    public bool? CancellationStatus { get; set; }
    public decimal? RefundAmount { get; set; }
    public bool Status { get; set; }

    
}
