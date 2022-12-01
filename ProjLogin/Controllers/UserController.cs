using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjLogin.Encrypt;
using ProjLogin.Models;

namespace ProjLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ProjLoginDBContext _context;
        

        public UserController(ProjLoginDBContext context)
        {
            _context = context;
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Login/{email},{password}")]
        public async Task<ActionResult<User>> Login(string email, string password)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ProjLoginDBContext.User'  is null.");
            }
            int count = _context.Users.Count();
            //_context.Users.Add(User);
            var queryUsers = from b in _context.Users
                             where b.Email == email
                             select b;
            foreach (var item in queryUsers)
            {
                Console.WriteLine(item.Email);
                if (GeneralMethods.VerifyOnlineUser(password, item.Password, item.Salt))
                {
                    return CreatedAtAction(nameof(Login), new { id = item.User_id }, item);
                }
            }
            return NotFound();
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Register/{name}, {email},{password}")]
        public async Task<ActionResult<User>> Register(string name, string email, string password)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ProjLoginDBContext.User'  is null.");
            }

            string dbPassword = new ("");
            string dbSalt = new("");
            GeneralMethods.HashPassword(password, out dbPassword, out dbSalt);
            User newUser = new (0,name,email,dbPassword,dbSalt,110,false,null);//110 is fixed.
            newUser.WriteLine();
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(Register), new { id = newUser.User_id }, newUser);
        }




        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User User)
        {
            if (id != User.User_id)
            {
                return BadRequest();
            }

            _context.Entry(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

       
        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var User = await _context.Users.FindAsync(id);
            if (User == null)
            {
                return NotFound();
            }

            _context.Users.Remove(User);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(long id)
        {
            return (_context.Users?.Any(e => e.User_id == id)).GetValueOrDefault();
        }
    }
}
