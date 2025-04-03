using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class RegistrationViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Emial is required")]
        [MaxLength(50)]
        [EmailAddress(ErrorMessage ="please inter the valid email")]
        //[RegularExpression(@"")]
        //[DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [MaxLength(20)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Compare("Password",ErrorMessage ="Please confirm your password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    } 
}
