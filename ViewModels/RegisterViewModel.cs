using System.ComponentModel.DataAnnotations;

namespace CarBooking.ViewModels
{
    public class RegisterViewModel
    {
        public int userId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNo { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public Guid? ActivetionCode { get; set; }
    }
    public class UserProfileViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        public string Email { get; set; } 

        [Required]
        public string PhoneNo { get; set; }

      
    }
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }
    }
    public class VerifyOtpViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        [StringLength(6, ErrorMessage = "OTP must be 6 digits", MinimumLength = 6)]
        public string Otp { get; set; }
    }
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}