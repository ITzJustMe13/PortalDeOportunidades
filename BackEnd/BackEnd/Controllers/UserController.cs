using BackEnd.Controllers.Data;
using BackEnd.Models;
using BackEnd.Models.BackEndModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ApplicationDbContext dbContext;
        public UserController(ApplicationDbContext dbContext) => this.dbContext = dbContext;

        // GET: api/users/2
        [HttpGet("{id}")]
        public ActionResult<UserModel> GetUserByID(int id)
        {
            if (dbContext.Users == null)
                return NotFound();

            var user = dbContext.Users.SingleOrDefault(u=>u.UserId == id);

            if (user == null) 
                return NotFound();

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<UserModel> CreateNewUser(UserModel user)
        {
            // business rules validation
            // (...)

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return CreatedAtAction(nameof(CreateNewUser), new { id = user.UserId }, user);
        }
    }
}
