using Microsoft.AspNetCore.Mvc;
using Flight_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;
using Flight_Reservation_System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
namespace Flight_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IDataContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public BookingsController(UserManager<IdentityUser> userManager, IDataContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        
        [HttpGet("search")]
public IActionResult SearchFlights(string departureLocation, string? arrivalLocation, DateOnly? departureDate)
{
    // Query for available flights
    var availableFlights = _context.Flights
    .Where(f => (string.IsNullOrEmpty(departureLocation) || f.DepartureLocation == departureLocation) && // Handle departure location filter
                (string.IsNullOrEmpty(arrivalLocation) || f.ArrivalLocation == arrivalLocation) &&  // Handle arrival location filter
                f.AvailableSeats > 0 &&
                f.isActive &&
                (!departureDate.HasValue || f.FlightDate == departureDate.Value))  // Match only the time of day
        .Select(f => new FlightResponseDto  // Assuming you use a DTO for returning response
        {
            FlightID = f.FlightID,
            FlightNumber = f.FlightNumber,
            FlightName = f.FlightName,
            DepartureTime = f.DepartureTime.ToString("HH:mm"),  // Format TimeOnly to string if needed
            ArrivalTime = f.ArrivalTime.ToString("HH:mm"),  // Format TimeOnly to string if needed
            DepartureLocation = f.DepartureLocation,
            ArrivalLocation = f.ArrivalLocation,
            AvailableSeats = f.AvailableSeats,
            Price = f.Price
        })
        .ToList();

    if (availableFlights.Count == 0)
    {
        return NotFound("No flights found matching your search criteria.");
    }

    // Return the available flights as a response
    return Ok(availableFlights);
}


        // POST: api/Bookings
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingCreateDto)
        {
            try
            {
                if (bookingCreateDto == null)
                {
                    return BadRequest("Booking data is required.");
                }

                // Validate Number of Seats (Cannot book more than 3 tickets)
                if (bookingCreateDto.NumberOfSeats > 3)
                {
                    return BadRequest("You cannot book more than 3 tickets.");
                }
                // Have to Book At least one seat
                if (bookingCreateDto.NumberOfSeats <= 0)
                {
                    return BadRequest("You must book at least one seat.");
                }

                // Validate that SeatNumbers is not null or empty and matches the number of seats
                if (bookingCreateDto.SeatNumbers == null || !bookingCreateDto.SeatNumbers.Any())
                {
                    return BadRequest("Seat numbers are required.");
                }
                var seatNumbersList = bookingCreateDto.SeatNumbers.Split(',');
                if (seatNumbersList.Count() != bookingCreateDto.NumberOfSeats)
                {
                    return BadRequest("The number of seat numbers must match the number of seats booked.");
                }

                // Check if Seat Numbers are unique
                if (seatNumbersList.Distinct().Count() != seatNumbersList.Count())
                {
                    return BadRequest("Seat numbers cannot be duplicated.");
                }



                // Check if Flight exists
                var flight = await _context.Flights
                    .FirstOrDefaultAsync(f => f.FlightID == bookingCreateDto.FlightId);

                if (flight == null)
                {
                    return NotFound("Flight not found.");
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                // Check if User exists (Optional)
                var userExists = await _context.Users
                .AnyAsync(u => u.Id == userId);  // Use u.Id instead of u.UserId

                if (!userExists)
                {
                    return NotFound("User not found.");
                }


                // Create the Booking object
                var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var booking = new Booking
                {
                    FlightId = bookingCreateDto.FlightId,
                    FlightNumber = bookingCreateDto.FlightNumber,
                    NumberofSeats = bookingCreateDto.NumberOfSeats,
                    SeatNumbers = bookingCreateDto.SeatNumbers,
                    BookingDate = bookingCreateDto.BookingDate,
                    TotalAmount = 0,  // Will calculate later
                    PaymentStatus = false, // Assuming booking is not paid
                    CancellationStatus = false, // Assuming booking is not cancelled
                    RefundAmount = 0, // No refund initially
                    Status = true, // Assuming booking is active
                    UserId = userid
                };

                // Calculate the total amount (Number of Seats * Flight Price)
                booking.TotalAmount = bookingCreateDto.NumberOfSeats * flight.Price;

                // Add booking to the context
                _context.Bookings.Add(booking);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Return the created booking details
                return CreatedAtAction(nameof(CreateBooking), new { id = booking.BookingId }, new
                {
                    FlightId = bookingCreateDto.FlightId,
                    FlightNumber = bookingCreateDto.FlightNumber,
                    NumberofSeats = bookingCreateDto.NumberOfSeats,
                    SeatNumbers = bookingCreateDto.SeatNumbers,
                    BookingDate = bookingCreateDto.BookingDate,
                    TotalAmount = booking.TotalAmount,
                    PaymentStatus = false,
                    CancellationStatus = false,
                    RefundAmount = 0,
                    Status = true,
                    // UserId = bookingCreateDto.UserId,
                    BookingId = booking.BookingId
                });
            }
            catch (Exception ex)
            {

                // Return a generic error message
                return StatusCode(500, "An unexpected error occurred while processing your request.");
            }
        }


        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBooking(int bookingId)
        {
            // Fetch the booking by its ID
            var booking = await _context.Bookings
                .Include(b => b.Flight) // Include Flight to access its Price
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound(); // Return 404 if the booking doesn't exist
            }

            // if ((bool)booking.CancellationStatus)
            // {
            //     return BadRequest("The booking is already cancelled.");
            // }

            // If the booking is not cancelled, cancel it
            // booking.CancellationStatus = true;
            // booking.Status = false; // Mark as inactive since the booking is cancelled

            // Calculate the TotalAmount = NumberOfSeats * Price of the Flight
            booking.TotalAmount = booking.NumberofSeats * booking.Flight.Price;

            // Check if the payment is done, and update PaymentStatus accordingly
            // if (booking.Payments.Any())  // Assuming Payments is a collection, and if it has any entry, payment is made
            // {
            //     booking.PaymentStatus = true;
            // }
            // else
            // {
            //     booking.PaymentStatus = false;
            // }

            // Update the booking in the database if there are any changes (in TotalAmount and PaymentStatus)
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            // Return the updated booking details as response
            var bookingResponse = new BookingResponseDto
            {
                BookingId = booking.BookingId,
                // UserId = booking.UserId,
                FlightId = booking.FlightId,
                FlightName = booking.Flight.FlightName,
                NumberOfSeats = booking.NumberofSeats,
                SeatNumbers = booking.SeatNumbers,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                PaymentStatus = booking.PaymentStatus,
                CancellationStatus = booking.CancellationStatus,
                RefundAmount = booking.RefundAmount,
                Status = booking.Status
            };

            return Ok(new
            {
                BookingId = booking.BookingId,
                // UserId = booking.UserId
                FlightId = booking.FlightId,
                NumberOfSeats = booking.NumberofSeats,
                SeatNumbers = booking.SeatNumbers,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                PaymentStatus = booking.PaymentStatus,
                CancellationStatus = booking.CancellationStatus,
                RefundAmount = booking.RefundAmount,
                Status = booking.Status

            }); // Return the updated booking response
        }

    }
}
