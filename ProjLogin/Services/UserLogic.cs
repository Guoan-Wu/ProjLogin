using ProjLogin.DTO;
using ProjLogin.Encrypt;
using ProjLogin.Models;
namespace ProjLogin.Services
{
    public class UserLogic
    {
        private static IUserRepository? _UserRepository;
        public static void SetDBContext(IUserRepository respo)
        {
            _UserRepository = respo;
        }

        public static User? Login(LoginDTO loginDTO)
        {
            User? user = null;
            if (_UserRepository == null)
            {
                return user;//throw exception.
            }

            user = _UserRepository?.GetID(loginDTO.Email);

            if (user == null)
                return user;

            if (HashMethods.VerifyOnlineUser(loginDTO.Password, user.Password, user.Salt))
            {
                return user;
            }

            return user;

        }
        public static async Task<string?> Register(RegisterDTO regsiterDTO)
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
                HashMethods.HashPassword(regsiterDTO.Password, out dbPassword, out dbSalt);

                User newUser = MapContainer.Map<RegisterDTO, User>(regsiterDTO);
                newUser.Salt = dbSalt;
                newUser.User_id = 0;
                newUser.Password = dbPassword;
                newUser.Customer_id = 110; //110 is fixed.
                newUser.Active = false;
                newUser.Created_at = null;

                newUser.WriteLine();
                bool ok = await _UserRepository.Add(newUser);
                if (!ok) { return errorMsg; }

                return errorMsg;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return errorMsg;
            }
        }
        public static bool VerifyToken(string token, string password, out string? errorMsg)
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
        public static Tuple<string?, string?, string?> ApplyForResetPassToken(string email)
        {
            string? errorMsg = null;
            if (_UserRepository == null)
            {
                errorMsg = "Entity set 'ProjLoginDBContext.User'  is null.";
                return new Tuple<string?, string?, string?>(errorMsg, null, null);
            }

            //1. creae random password
            try
            {
                var user = _UserRepository.GetID(email);
                string password = HashMethods.CreateRandomPassword();
                string dbPassword = new("");
                string dbSalt = new("");
                HashMethods.HashPassword(password, out dbPassword, out dbSalt);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                user.Password = dbPassword;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                user.Salt = dbSalt;

                //create token
                string jwtStr = JwtHelper.IssueJwt(email, password);//sent original password rather than dbpassword.

                return new Tuple<string?, string?, string?>(errorMsg, password, jwtStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Tuple<string?, string?, string?>(errorMsg, null, null);
            }
        }
        public static async Task<string?> ResetPassword(ResetPasswordDTO dto)
        //string email,string newPassword)
        {
            string? errorMsg = null;

            try
            {
                //update db.
                string dbPassword = new("");
                string dbSalt = new("");
                string password = dto.NewPassword;
                HashMethods.HashPassword(password, out dbPassword, out dbSalt);
                if (_UserRepository is null)
                {
                    errorMsg = "Entity set 'ProjLoginDBContext.User'  is null.";
                    return errorMsg;
                }
                if (!await _UserRepository.Update(dto.Email, dbPassword, dbSalt))
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

