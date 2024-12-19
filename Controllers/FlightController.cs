using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flight_Reservation_System.Data;
using Flight_Reservation_System.Models;

using Microsoft.AspNetCore.Authorization;
    

namespace Flight_Reservation_System
{
    [Authorize(Roles = "Admin")]
    [ApiController] // Marks this as an API controller
    [Route("api/[controller]")]
    public class FlightController : Controller
    {
        private readonly IDataContext _context;
    
        public FlightController(IDataContext context)
        {
            _context = context;
        }
    
        [HttpGet("GetFlights")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            try
            {
                return await _context.Flights.ToListAsync();
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
    
            }
        }
    
        // GET: api/Flight/{id}
        [HttpGet("Search flight by Id")]
        public async Task<ActionResult<Flight>> GetFlight(int id)
        {
            try
            {
                var flight = await _context.Flights.FindAsync(id);
    
                if (flight == null)
                {
                    return NotFound();
                }
    
                return flight;
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        
        }
    
        // POST: api/Flight
        [HttpPost("Create")]
        public async Task<ActionResult<Flight>> CreateFlight([FromBody]CreateFlightDto createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest("Flight details are required");
                }
    
                    var flight = new Flight
                {
                    FlightNumber = createDto.FlightNumber,
                    FlightName = createDto.FlightName,
                    FlightDate = createDto.FlightDate,
                    DepartureTime = createDto.DepartureTime,
                    ArrivalTime = createDto.ArrivalTime,
                    DepartureLocation = createDto.DepartureLocation,
                    ArrivalLocation = createDto.ArrivalLocation,
                    TotalSeats = createDto.TotalSeats,
                    AvailableSeats = createDto.AvailableSeats,
                    Price = createDto.Price,
                    isActive = createDto.isActive
                };
    
                _context.Flights.Add(flight);
                await _context.SaveChangesAsync();
    
                return CreatedAtAction(nameof(GetFlight), new { id = flight.FlightID }, flight);
            }
        
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    
            // PUT: api/Flight/Update
        [HttpPut("UpdateFlightDetails")]
        public async Task<IActionResult> UpdateFlightDetails(int id, string flightNumber, [FromBody] UpdateFlightDetailsDto updateDto)
        {
            try
            {
                if (string.IsNullOrEmpty(flightNumber))
                {
                    return BadRequest("FlightNumber is required.");
                }
    
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightID == id && f.FlightNumber == flightNumber);
    
                if (flight == null)
                {
                    return NotFound($"No flight found with the specified ID and FlightNumber.");
                }
    
                // Update only the specified fields if they are provided
                if (updateDto.FlightDate.HasValue)
                {
                    flight.FlightDate = updateDto.FlightDate.Value;
                }
                if (updateDto.DepartureTime.HasValue)
                {
                    flight.DepartureTime = new TimeOnly(
                        updateDto.DepartureTime.Value.Hour,
                        updateDto.DepartureTime.Value.Minute,
                        updateDto.DepartureTime.Value.Second
                    );
                }
                if (updateDto.ArrivalTime.HasValue)
                {
                    flight.ArrivalTime = new TimeOnly(
                        updateDto.ArrivalTime.Value.Hour,
                        updateDto.ArrivalTime.Value.Minute,
                        updateDto.ArrivalTime.Value.Second
                    );
                }
                if (updateDto.AvailableSeats.HasValue)
                {
                    flight.AvailableSeats = updateDto.AvailableSeats.Value;
                }
                if (updateDto.Price.HasValue)
                {
                    flight.Price = updateDto.Price.Value;
                }
    
                await _context.SaveChangesAsync();
    
                return NoContent();
            }
    
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    
            // DELETE: api/Flight/{flightNumber}
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteFlight(int id, string flightNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(flightNumber))
                {
                    return BadRequest("FlightNumber is required.");
                }
            
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightID == id && f.FlightNumber == flightNumber);
    
                if (flight == null)
                {
                    return NotFound($"Flight with id {id} and FlightNumber '{flightNumber}' not found.");
                }
    
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync();
    
                return NoContent();
            }
    
            catch (Exception ex)
            {
            
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
