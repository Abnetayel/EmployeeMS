using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication3.Models;

namespace WebApplication3.Data
{
    [Microsoft.EntityFrameworkCore.Index(nameof(Email), IsUnique = true)]
    [Microsoft.EntityFrameworkCore.Index(nameof(UserName), IsUnique = true)]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        [MaxLength(20)]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(50)]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(200)]
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        [Required]
        public string Role { get; set; } = "User";
        public ICollection<Employe>? ManagedEmployees { get; set; }
    }
}