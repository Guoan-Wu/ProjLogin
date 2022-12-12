using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace ProjLogin.Models
{
    public interface IUserRepository
    { 
        void SetContext(ProjLoginDBContext context);

        User? GetID(string email);
        Task<bool> Add(User newUser);
        Task<bool>  Update(string email, string newPassword, string salt);
        Task<bool>  Delete(string email, string password);

    }
    public class UserRepository : IUserRepository
    {
        private  ProjLoginDBContext? _context;
        public  void SetContext(ProjLoginDBContext context)
        {
            _context = context;
        }
        public User? GetID(string email)
        {
            if (_context is null)
                return null;
            var queryUsers = _context.Users.First(a => a.Email == email);
            return queryUsers;
        }
        public  async Task<bool> Add(User newUser)
        {
            if (_context is null)
                return false;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return true;
        }
        public  async Task<bool> Update(string email, string newPassword,string salt)
    {
            if (_context is null)
                return false;

            var user = _context.Users.First(a => a.Email == email);
            if(user is null)
                return false;

            user.Password = newPassword;
            user.Salt = salt;
            await _context.SaveChangesAsync();
            return true;
    }

    public  async Task<bool> Delete(string email, string password)
    {
            Console.WriteLine(email + password);
            return false;
    }

}
}
