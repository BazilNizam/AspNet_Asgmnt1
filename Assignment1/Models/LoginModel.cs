using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models
{
    // model for login functionality
    public class LoginModel
    {
        // email property
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        // password property
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}