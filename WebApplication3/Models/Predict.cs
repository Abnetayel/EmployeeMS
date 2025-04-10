namespace WebApplication3.Models
{
    public class Predict
    {
        public int Id { get; set; }
        public required int Age { get; set; }
        public required int Experience { get; set; }
        public required string Education { get; set; }
        public required string Skills { get; set; }
        public required decimal Salary { get; set; }
        public required string Gender { get; set; }
        public required string Country { get; set; }
        public required string EmployementType { get; set; }
    }
}
