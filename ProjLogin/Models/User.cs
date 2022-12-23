using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjLogin.Models
{
    [Table("online_user")]
    public class User
    {
        public User() { }
       public User(int user_id, string user_name, string email, 
           string password, string salt,
           int customer_id,bool active, DateTime? created_at)
        {
            User_id = user_id;
            User_name = user_name;
            Email = email;
            Password = password;
            Salt = salt;
            Customer_id = -1;
            Active = false;
            Customer_id = customer_id;
            Active = active;
            Created_at = created_at;
        }
        [Key]
        public int User_id { get; set; }
        public string? User_name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Salt { get; set; }
        public int Customer_id { get; set; }
        public bool Active { get; set; } = false;
        public DateTime? Created_at { get; set; } = DateTime.Now;


    }

    public static class UserExtensions
    {
        public static void WriteLine(this User user)
        {
            string s = user.User_id.ToString() + ",";
            s += user.User_name ?? "null";
            s += ",";
            s += user.Email ?? "null";
            s += ",";
            s += user.Password ?? "null";
            s += ",";
            s += user.Salt ?? "null";
            s += ",";
            Console.WriteLine(s);
        }
    }
}
