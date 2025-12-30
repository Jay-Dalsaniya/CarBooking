using System.ComponentModel.DataAnnotations;

namespace CarBooking.ViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
    public class Verify2FALoginViewModel
    {
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }
    }
    public class Forgot2FAViewModel
    {
        public string Email { get; set; }
    }

}
