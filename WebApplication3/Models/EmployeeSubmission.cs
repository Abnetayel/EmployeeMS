
namespace WebApplication3.Models
{

    public class EmployeeSubmission
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public int Experience { get; set; }
        public DateTime DateOfSubmission { get; set; } = DateTime.Now;
        public string Education { get; set; }
        public string Skills { get; set; }
        public string Gender { get; set; }
        public string EmployementType { get; set; }
        public decimal Salary { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string? SubmittedBy { get; set; } // UserName or Email of the submitter
        public bool IsApproved { get; set; } = false;
    }
}