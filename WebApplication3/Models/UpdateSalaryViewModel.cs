using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class UpdateSalaryViewModel
    {
        public int EmployeeId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public decimal Salary { get; set; }
    }

}
