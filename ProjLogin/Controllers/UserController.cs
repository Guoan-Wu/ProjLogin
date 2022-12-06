using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjLogin.Encrypt;
using ProjLogin.Models;
using System.Xml.Linq;

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
            User? item;
            if(FunLogin(email, password,out item)) {
                if(item is not null)
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

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ApplyResetPassword/{email}")]
        public async Task<ActionResult<User>> ApplyResetPassword(string email)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ProjLoginDBContext.User'  is null.");
            }
            try
            {
                var user = _context.Users.First(a => a.Email == email);
                string password = GeneralMethods.CreateRandomPassword();
                //send an email.

                //update db.
                string dbPassword = new("");
                string dbSalt = new("");
                GeneralMethods.HashPassword(password, out dbPassword, out dbSalt);
                user.Password = dbPassword;
                user.Salt = dbSalt;
                await _context.SaveChangesAsync();

                //rewrite user to respond.
                User newUser = user;
                newUser.Password = password;
                newUser.Salt = "";
                return CreatedAtAction(nameof(ApplyResetPassword), new { id = newUser.User_id }, newUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("ResetPassword/{email},{oldPassword},{newPassword}")]
        public async Task<ActionResult<User>> ResetPassword(string email, 
            string oldPassword,    string newPassword)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ProjLoginDBContext.User'  is null.");
            }
            User? item;
            if (FunLogin(email, oldPassword, out item))
            {
                if (item is not null)
                {
                    string dbPassword = new("");
                    string dbSalt = new("");
                    GeneralMethods.HashPassword(newPassword, out dbPassword, out dbSalt);
                    item.Password = dbPassword;
                    item.Salt = dbSalt;
                    await _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(ResetPassword), new { id = item.User_id }, item);
                }
            }
            return NotFound();
        }

        protected bool FunLogin(string email, string password,out User? user)
        {
            if (_context.Users == null)
            {
                user= null;
                return false;
            }
            int count = _context.Users.Count();
            //_context.Users.Add(User);
            var queryUsers = _context.Users.First(a => a.Email == email);
            user = queryUsers;

            if (user == null)
                return false;

            if (GeneralMethods.VerifyOnlineUser(password, user.Password, user.Salt))
            {
                return true;
            }
            return false;
        }
    }
}
