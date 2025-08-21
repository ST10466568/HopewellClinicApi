using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.DTOs;

namespace HopewellClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly HopewellDbContext _context;

        public StaffController(HopewellDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffResponse>>> GetStaff()
        {
            try
            {
                var staff = await (from s in _context.Staff
                                   join u in _context.Users on s.UserId equals u.Id
                                   join ur in _context.UserRoles on u.Id equals ur.UserId
                                   join r in _context.Roles on ur.RoleId equals r.Id
                                   where u.IsActive
                                   select new StaffResponse
                                   {
                                       Id = s.Id,
                                       UserId = s.UserId,
                                       StaffNumber = s.StaffNumber,
                                       FirstName = u.FirstName,
                                       LastName = u.LastName,
                                       Role = r.Name,
                                       Phone = u.PhoneNumber,
                                       IsActive = u.IsActive
                                   }).ToListAsync();

                return Ok(staff);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}