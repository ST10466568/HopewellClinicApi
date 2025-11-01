using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;

namespace HopewellClinicApi.Controllers
{
    /// <summary>
    /// Controller for providing user data in array format for frontend compatibility
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserArrayController : ControllerBase
    {
        private readonly HopewellDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserArrayController(HopewellDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get all users as a simple array (for frontend compatibility)
        /// This endpoint returns a direct array without pagination metadata
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<List<object>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Patient)
                    .Include(u => u.Staff)
                    .ToListAsync();

                var usersWithRoles = new List<object>();

                foreach (var user in users)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    string displayRole;
                    if (userRoles.Contains("admin"))
                        displayRole = "admin";
                    else if (userRoles.Contains("doctor"))
                        displayRole = "doctor";
                    else if (userRoles.Contains("staff"))
                        displayRole = "staff";
                    else if (userRoles.Contains("patient"))
                        displayRole = "patient";
                    else if (user.Staff != null)
                        displayRole = "staff";
                    else if (user.Patient != null)
                        displayRole = "patient";
                    else
                        displayRole = "user";

                    usersWithRoles.Add(new
                    {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        phoneNumber = user.PhoneNumber,
                        role = displayRole,
                        isActive = user.IsActive,
                        createdAt = user.CreatedAt,
                        updatedAt = user.UpdatedAt
                    });
                }

                return Ok(usersWithRoles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all users: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint to verify controller is working
        /// </summary>
        [HttpGet("test")]
        [AllowAnonymous]
        public ActionResult Test()
        {
            return Ok(new { message = "UserArrayController is working!", timestamp = DateTime.Now });
        }
    }
}










