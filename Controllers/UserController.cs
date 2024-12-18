using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Flight_Reservation_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        // Inject UserManager
        public UserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // API endpoint to fetch the user ID
        [HttpGet("userId")]
        public async Task<IActionResult> GetUserId()
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Get the user ID
                var userId = _userManager.GetUserId(User);

                // Return the user ID as a JSON response
                return Ok(new { UserId = userId });
            }
            else
            {
                // If the user is not authenticated, return an Unauthorized response
                return Unauthorized(new { Message = "User is not authenticated" });
            }
            
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user and clear the cookies
            await HttpContext.SignOutAsync();

            // Return a successful response
            return Ok(new { Message = "User logged out successfully" });
        }
        
    }
}
