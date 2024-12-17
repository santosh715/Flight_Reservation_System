namespace Flight_Reservation_System
{
    public class UpdateFlightDetailsDto
    {
        public DateOnly? FlightDate { get; set; }
        public TimeOnly? DepartureTime { get; set; }
        public TimeOnly? ArrivalTime { get; set; }
        public int? AvailableSeats { get; set; }
        public decimal? Price { get; set; }
    }
}
 