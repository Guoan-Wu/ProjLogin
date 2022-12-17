using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjLogin.Encrypt;
using ProjLogin.Models;
using System.ComponentModel;

namespace ProjLogin.Services
{
    public class UserLogic
    {
        private static IUserRepository? _UserRepository;
        public static void SetDBContext(IUserRepository respo) {
            _UserRepository = respo;
        }
       
        public static User? Login(string email, string password )
        {
            User? user = null;
            if (_UserRepository == null)
            {
                return user;
            }
            
            try
            {
                user = _UserRepository ? .GetID(email);

                if (user == null)
                    return user;

                if (HashMethods.VerifyOnlineUser(password, user.Password, user.Salt))
                {
                    return user;
                }
            }
            catch (Exception ex)
            {
                user = null;
                Console.WriteLine(ex.Message);
            }
            return user;

        }
        public static async Task<string?> Register(string name, string email, string password)
        {
            string? errorMsg = null;
            if (_UserRepository is null)
            {
                errorMsg = "Entity set 'ProjLoginDBContext.User'  is null.";
                return errorMsg;
            }

            try
            {
                string dbPassword = new("");
                string dbSalt = new("");
                HashMethods.HashPassword(password, out dbPassword, out dbSalt);
                User newUser = new(0, name, email, dbPassword, dbSalt, 110, false, null);//110 is fixed.
                newUser.WriteLine();
                bool ok = await _UserRepository.Add(newUser);
                if(!ok) { return errorMsg; }
                
                return errorMsg;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return errorMsg;
            }
        }
        public static bool VerifyToken(string token, string password,out string? errorMsg)
        {
            if (token.Length <= 0)
            {
                errorMsg = "Error, without an authorization token.";
                return false;
            }
            
            Tuple<string?, string?> userInfo = JwtHelper.SerializeJwtStr(token);
            if (userInfo.Item1 is null || userInfo.Item2 is null)
            {
                errorMsg = "Error! The token's content is invalid.";
                return false;
            }
            if (userInfo.Item2 != password)
            {
                errorMsg = "Error! The token is fake.";
                return false;
            }
            errorMsg = null;
            return true;
        }
        public static Tuple<string?,string?,string?> ApplyForResetPassToken(string email)
        {
            string? errorMsg = null;
            if (_UserRepository == null)
            {
                errorMsg = "Entity set 'ProjLoginDBContext.User'  is null.";
                return new Tuple<string?, string?, string?>(errorMsg,null,null);
            }

            //1. creae random password
            try
            {
                var user = _UserRepository.GetID(email);
                string password = HashMethods.CreateRandomPassword();
                string dbPassword = new("");
                string dbSalt = new("");
                HashMethods.HashPassword(password, out dbPassword, out dbSalt);
                user.Password = dbPassword;
                user.Salt = dbSalt;

                //create token
                string jwtStr = JwtHelper.IssueJwt(email, password);//sent original password rather than dbpassword.

                return new Tuple<string?, string?, string?>(errorMsg, password, jwtStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Tuple<string?, string?, string?>(errorMsg,null,null);
            }
        }
        public static async Task<string?> ResetPassword(string email,string newPassword)
        {
            string? errorMsg= null;
            
            try
            {
                //update db.
                string dbPassword = new("");
                string dbSalt = new("");
                string password = newPassword;
                HashMethods.HashPassword(password, out dbPassword, out dbSalt);
                if (_UserRepository is null)
                {
                    errorMsg = "Entity set 'ProjLoginDBContext.User'  is null.";
                    return errorMsg;
                }
                if (! await _UserRepository.Update(email, dbPassword, dbSalt))
                    return errorMsg;

                return errorMsg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                errorMsg = ex.Message + " " + ex.StackTrace;
                return errorMsg;
            }
        }
    }
}

