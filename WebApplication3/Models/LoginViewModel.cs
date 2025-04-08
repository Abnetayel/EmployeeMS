using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "UserName And Email is required")]
        [MaxLength(50)]
        [DisplayName("username or Password ")]
        public string UserNameOrEmail { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(20)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        //public string UserNameOrEmail { get; internal set; }
    }
}