using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    //AllowAnonymous
    [Authorize]
    public class AppUsersController : BaseApiController
    {
        private readonly DataContext _context;

        public AppUsersController(DataContext context)
        {
           _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAppUsers()
        {
            return await _context.AppUsers.ToListAsync(); 
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetAppUser(int id)
        {
            return await _context.AppUsers.Where(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}
