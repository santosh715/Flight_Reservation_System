using System.Security.Claims;
using Flight_Reservation_System.Data;
using FlightReservations.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightReservations.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingCancellationController : Controller
    {
        private readonly IDataContext _context;

        private readonly IEmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;


        public BookingCancellationController(IDataContext context, IEmailService emailService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;

        }

        [HttpGet("{ID}")]
        public IActionResult GetAllBooking(int ID)
        {
            var booking = _context.Bookings.Find(ID);
            if (booking == null)
            {
                return NotFound($"No bookings found with ID {ID}.");
            }
            return Ok(booking);
        }

        [HttpDelete("DeleteSeats")]
        public async Task<IActionResult> DeleteSeats([FromBody] DeleteSeatsRequest request)
        {
            // Step 1: validate the booking by ID
            if (request == null || request.SeatNumbers == null || !request.SeatNumbers.Any())
            {
                return BadRequest("Invalid request. Please provide booking ID and seat numbers.");
            }

            var booking = _context.Bookings
            .Include(b => b.Flight)
            .FirstOrDefault(b => b.BookingId == request.BookingID);

            if (booking == null)
            {
                return NotFound($"No booking found with ID {request.BookingID}.");
            }

            if (string.IsNullOrEmpty(booking.SeatNumbers))
            {
                return BadRequest("No seats are currently booked to delete.");
            }

            // Step 2: Split existing seat numbers into a list
            var existingSeats = booking.SeatNumbers.Split(',').ToList();

            // Step 3: Check and remove the seats requested for deletion
            var seatsToDelete = request.SeatNumbers.Distinct().ToList(); // Avoid duplicates in request
            var deletedSeats = new List<string>();

            foreach (var seat in seatsToDelete)
            {
                if (existingSeats.Contains(seat))
                {
                    existingSeats.Remove(seat);
                    deletedSeats.Add(seat);
                }
            }

            if (deletedSeats.Count == 0)
            {
                return BadRequest("None of the specified seats were found in the booking.");
            }

            decimal refundAmount = 0;
            var user = await _userManager.FindByIdAsync(booking.UserId.ToString());
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var userEmail = user.Email;

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("User email not found.");
            }
            if (!existingSeats.Any()) // No seats left in the booking
            {
                refundAmount = booking.TotalAmount; // Refund the entire amount
                _context.Bookings.Remove(booking);   // Delete the booking row
                
                if (booking.UserId != null && !string.IsNullOrEmpty(userEmail))
                {
                    booking.CancellationStatus=true;
                    await _emailService.SendEmailAsync(
                        userEmail,
                        "Seats Cancellation Successful",
                        $"Your Entire Booking with ID{booking.BookingId} have been cancelled.Your refund amount is {refundAmount:C}.");
                }

                _context.SaveChanges();

                return Ok(new
                {
                    BookingId = booking.BookingId,
                    RefundAmount = refundAmount,
                    DeletedSeats = deletedSeats,
                    Message = "All seats have been cancelled, and the booking has been removed."
                });
            }

            else
            {

                decimal pricePerSeat = booking.TotalAmount / (booking.NumberofSeats + deletedSeats.Count);
                refundAmount = pricePerSeat * deletedSeats.Count;

                booking.SeatNumbers = string.Join(",", existingSeats);
                booking.NumberofSeats = existingSeats.Count;
                booking.TotalAmount -= refundAmount;
                booking.CancellationStatus=true;

                _context.SaveChanges();

                if (booking.UserId != null && !string.IsNullOrEmpty(userEmail))
                {
                    await _emailService.SendEmailAsync(userEmail, "Seats Cancelled.",
                    $"The following seats have been cancelled: {string.Join(", ", deletedSeats)}. Refund Amount: {refundAmount:C}.");
                }

                // Step 9: Return the response
                return Ok(new
                {
                    BookingId = booking.BookingId,
                    RemainingSeats = booking.SeatNumbers,
                    NumberOfRemainingSeats = booking.NumberofSeats,
                    RefundAmount = refundAmount,
                    DeletedSeats = deletedSeats,
                    Message = "Selected seats have been deleted successfully."
                });

            }

        }


        [HttpDelete("DeleteEntireBooking")]
        public async Task<IActionResult> DeleteBooking(int bookingID)
        {

            var booking = _context.Bookings
            .Include(B => B.Payments)
            .Include(B => B.Flight)
            .FirstOrDefault(B => B.BookingId == bookingID);

            var user = await _userManager.FindByIdAsync(booking.UserId.ToString());
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var userEmail = user.Email;

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("User email not found.");
            }

            if (booking == null)
            {
                return NotFound($"No booking found.");
            }

            decimal refundpercentage = 0.7m;
            decimal refundAmount = booking.TotalAmount * refundpercentage;

            booking.RefundAmount = refundAmount;
            booking.CancellationStatus = true;

            if (booking.UserId != null)
            {
                await _emailService.SendEmailAsync(
                    userEmail,
                    "Cancellation Successful ",
                $"Your refund of {refundAmount:C} for booking ID {booking.BookingId} was successful");
            }

            _context.Bookings.Remove(booking);
            _context.SaveChanges();
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookingresponse = new BookingDTO
            {
                BookingId = booking.BookingId,
                UserId = userid,
                FlightId = booking.FlightId,
                NumberOfSeats = booking.NumberofSeats,
                SeatNumbers = booking.SeatNumbers,
                BookingDate = (DateTime)booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                CancellationStatus = booking.CancellationStatus,
                RefundAmount = refundAmount

            };

            return Ok(new
            {
                BookingId = booking.BookingId,
                UserId = booking.UserId,
                FlightId = booking.FlightId,
                NumberOfSeats = booking.NumberofSeats,
                SeatNumbers = booking.SeatNumbers,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                CancellationStatus = booking.CancellationStatus,
                RefundAmount = refundAmount,
                Mesaage = $"Booking has been successfully deleted. Refund Amount is ${refundAmount}."
            });
        }


    }

    public class CancelSeatRequest
    {
        public int BookingID { get; set; }
        public string SeatNumber { get; set; }
    }
    public class DeleteSeatsRequest
    {
        public int BookingID { get; set; }
        public List<string> SeatNumbers { get; set; } // List of seat numbers to delete
    }


}
