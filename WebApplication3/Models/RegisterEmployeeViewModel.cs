using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class RegisterEmployeeViewModel
    {
        [Required]
        public string FullName { get; set; }

        //[Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public int Experience { get; set; }

        [Required]
        public string Education { get; set; }

        [Required]
        public string Skills { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string EmployementType { get; set; }
    }

}
