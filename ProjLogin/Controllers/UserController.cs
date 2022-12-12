using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjLogin.Encrypt;
using ProjLogin.Models;
using ProjLogin.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Xml.Linq;

namespace ProjLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ProjLoginDBContext _context;
        private IUserRepository _userRepository;

        public UserController(ProjLoginDBContext context)
        {
            _context = context;
            _userRepository = new UserRepository();
            _userRepository.SetContext(_context);
            UserLogic.SetDBContext(_userRepository);
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Login/{email},{password}")]
        public  IActionResult Login(string email, string password)
        {           
            User? item = UserLogic.Login(email, password);            
            return item != null? Ok(): Problem("Invalid parameters.");           
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Register/{name}, {email},{password}")]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            string? errorMsg = await UserLogic.Register(name, email, password);
            
            return (errorMsg == null) ? Ok() : Problem(errorMsg);
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ApplyResetPassword/{email}")]
        [Obsolete("ApplyResetPassword() is obsoleted. Please use ApplyForResetPassToken()")]
        public async Task<IActionResult> ApplyResetPassword(string email)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ProjLoginDBContext.User'  is null.");
            }
            try
            {
                var user = _context.Users.First(a => a.Email == email);
                string password = HashMethods.CreateRandomPassword();
                //send an email.

                //update db.
                string dbPassword = new("");
                string dbSalt = new("");
                HashMethods.HashPassword(password, out dbPassword, out dbSalt);
                user.Password = dbPassword;
                user.Salt = dbSalt;
                await _context.SaveChangesAsync();

                //rewrite user to respond.
                User newUser = user;
                newUser.Password = password;
                newUser.Salt = "";
                return Ok();
                //return CreatedAtAction(nameof(ApplyResetPassword), new { id = newUser.User_id }, newUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ApplyForResetPassToken/{email}")]
        public IActionResult ApplyForResetPassToken(string email)
        {
            Tuple<string?, string?, string?>result = UserLogic.ApplyForResetPassToken(email);

            return (result.Item1 == null) ?
                Ok(new { success = true, pwd = result.Item2, token = result.Item3 }) :
                Problem(result.Item1 ?? "unknown error.");            
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("ResetPassword/{email},{oldPassword},{newPassword}")]
        [Authorize(Policy = "Admin")]
        [ServiceFilter(typeof(TokenFilter))]
        public async Task<IActionResult> ResetPassword(string email,
            string oldPassword, string newPassword)
        {
            string? errorMsg = await UserLogic.ResetPassword(email, newPassword);
            return (errorMsg == null)? Ok($"email:{email}"):Problem(errorMsg);
        }

    }
}
