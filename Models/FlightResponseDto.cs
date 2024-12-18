public class FlightResponseDto
{
    public int FlightID { get; set; }
    public string FlightNumber { get; set; }
    public string FlightName { get; set; }
    public string DepartureTime { get; set; } // As string formatted from TimeOnly
    public string ArrivalTime { get; set; }   // As string formatted from TimeOnly
    public string DepartureLocation { get; set; }
    public string? ArrivalLocation { get; set; }
    public int AvailableSeats { get; set; }
    public decimal Price { get; set; }
}
