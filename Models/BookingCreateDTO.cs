namespace Flight_Reservation_System.Models
{
    public class BookingCreateDto
    {
        public int FlightId { get; set; }
        public int NumberOfSeats { get; set; }
        public string SeatNumbers { get; set; } = null!;
        public DateTime BookingDate { get; set; }

       // public int UserId { get; set; }

        public string FlightNumber{get;set;}
        
    }
}
