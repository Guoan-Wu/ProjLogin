using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjLogin.DTO
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class ResetPasswordDTO
    {       
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [PasswordPropertyText]
        [Required]
        public string OldPassword { get; set; }
        [PasswordPropertyText]
        [Required]
        public string NewPassword { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
