namespace WebApplication3.Models
{
    public class Employe
    {
        public int Id { get; set; }
        public required string FirstName {  get; set; }
        public required string LastName { get; set; }
        public required string MiddleName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Country { get; set; }
    }
}
