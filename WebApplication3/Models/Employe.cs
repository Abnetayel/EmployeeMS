namespace WebApplication3.Models
{
    public class Employe
    {
        public int Id { get; set; }
        public required string FullName {  get; set; }
        public required int Age { get; set; }
        public required int Experience { get; set; }
        public required DateTime DateOfRegistration { get; set; }
        public required string Education { get; set; }
        public required string Skills { get; set; }
        public required string Gender { get; set; }
        public required string EmployementType { get; set; }
        public required decimal Salary { get; set; }
        public required string Currency { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Country { get; set; }
    }
}