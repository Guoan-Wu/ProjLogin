using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjLogin.Controllers.controllerResults;
using ProjLogin.DTO;
using ProjLogin.Encrypt;
using ProjLogin.Middleware;
using ProjLogin.Models;
using ProjLogin.Services;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
        [HttpPost("Login")]
        public  IActionResult Login(LoginDTO loginDTO)
            //string email, string password)
        {
            if (!ModelState.IsValid)
            {
                //throw modelstate 's message
                return BadRequest(new ActionResultBasic<string> { Success = false, ErrorMessage = "Login failed because of Invalid parameters." }) ;
            }
            
            User? item = UserLogic.Login(loginDTO);
            return item != null ? Ok(new ActionResultBasic<string> { Success = true }) : 
                BadRequest(new ActionResultBasic<string> { Success = false, ErrorMessage= "Login failed because of Invalid parameters." });
                //throw new BusinessException(HttpStatusCode.BadRequest, "Login failed because of invalid parameters.");
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO regsiterDTO)
        { 
            if (!ModelState.IsValid)
            {
                return BadRequest("Register failed because of Invalid parameters.");
            }
            string? errorMsg = await UserLogic.Register(regsiterDTO);
            
            return (errorMsg == null) ? Ok(new ActionResultBasic<string> { Success = true }) :
                BadRequest(new ActionResultBasic<string> { Success = false, ErrorMessage = errorMsg });

        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ApplyResetPassword/{email}")]
        [Obsolete("ApplyResetPassword() is obsoleted. Please use ApplyForResetPassToken()")]
        public async Task<IActionResult> ApplyResetPassword( string email)
        {
            if (_context.Users == null)
            {
                return BadRequest(new ActionResultBasic<string> { Success = false, ErrorMessage = "Entity set 'ProjLoginDBContext.User'  is null." });
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
                return Ok(new ActionResultBasic<string> { Success = true });
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
        [ServiceFilter(typeof(ValidationFilter))]
        public IActionResult ApplyForResetPassToken([EmailAddress] [Required] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ActionResultBasic<string> { Success = false, ErrorMessage = "ApplyForResetPassToken token failed because of Invalid parameters." });
            }
            Tuple<string?, string?, string?>result = UserLogic.ApplyForResetPassToken(email);

#pragma warning disable CS8601 // Possible null reference assignment.
            return (result.Item2 != null) ?
                Ok(new ActionResultBasic<ActionResultApplyForResetPassToken> {
                    Success = true,
                    Value = new ActionResultApplyForResetPassToken { Password = result.Item2, Token = result.Item3 } }) :                    
                BadRequest(new ActionResultBasic<ActionResultApplyForResetPassToken>
                {
                    Success = false,
                    ErrorMessage = "ApplyForResetPassToken failed: Unknown errors"
                });
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("ResetPassword")]
        [Authorize(Policy = "Admin")]
        [ServiceFilter(typeof(TokenFilter))]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            string? errorMsg = await UserLogic.ResetPassword(dto);
            return (errorMsg == null) ? Ok(new ActionResultBasic<string> { Success = true, Value = $"email:{dto.Email}" }) :
                BadRequest(new ActionResultBasic<string> { Success = false, Value = errorMsg });
                
        }

    }
}
