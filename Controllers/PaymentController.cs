using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Flight_Reservation_System.Models;
using Microsoft.EntityFrameworkCore;
using Flight_Reservation_System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Flight_Reservation_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly IDataContext _context;
 
        private readonly IEmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;
 
        public PaymentController(IDataContext context, IEmailService emailService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
        }
 
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentDto paymentRequest)
        {
            // if(paymentRequest==null||paymentRequest.Amount<=0)
            // {
            //     return BadRequest("Invalid payment details");
            // }
 
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
 
            var booking = await _context.Bookings
            .Include(b=>b.Flight)
            .FirstOrDefaultAsync(b=>b.BookingId==paymentRequest.BookingId);
 
            if(booking==null)
            {
                return NotFound("Booking not Found");
            }
 
            // if (booking.PaymentStatus)
            // {
            //     return BadRequest(new { Message = "This booking has already been paid for." });
            // }
 
            var existingPayment = _context.Payments.FirstOrDefault(p=>p.BookingId==paymentRequest.BookingId && p.PaymentStatus);
            if (existingPayment != null)
            {
                return BadRequest("Payment for this booking has already been processed.");
            }
            try
            {
                var amount = booking.TotalAmount;
                bool isPaymentSuccessful = SimulatePaymentGateway(amount);
                // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // var Email = User.FindFirstValue(ClaimTypes.Name);

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

                var payment = new Payment
                {
                    BookingId = booking.BookingId,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = isPaymentSuccessful
                };
               
                if(isPaymentSuccessful)
                {
                    booking.PaymentStatus = true;
                    booking.Status = true;
                    await _emailService.SendEmailAsync(userEmail, "Payment Successful",
                    $"Your payment of {amount:C} for booking ID:{booking.BookingId} was successful");
                }
                else
                {
                    return StatusCode(402,"Payment failed. Please try again.");
                    await _emailService.SendEmailAsync(userEmail, "Payment Failed",
                    $"Your payment of {amount:C} for booking ID {booking.BookingId} please try again");
                }
 
                _context.Payments.Add(payment);    
                _context.Bookings.Update(booking);
                _context.SaveChanges();
 
                return Ok(new
                {
                    Message = "Payment processed successfully!",
                    PaymentID = payment.PaymentId,
                    BookingID = payment.BookingId,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    PaymentStatus = payment.PaymentStatus
                });
 
            }
            catch (Exception ex)
            {
                return StatusCode(500,$"An error occured: {ex.Message}");
            }
        }
 
        private bool SimulatePaymentGateway(decimal amount)
        {return true;}
    }
}