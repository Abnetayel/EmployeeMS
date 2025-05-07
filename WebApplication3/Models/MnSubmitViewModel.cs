using System.Collections.Generic;
using WebApplication3.Data;

namespace WebApplication3.Models
{
    public class MnSubmitViewModel
    {
        public IEnumerable<EmployeeSubmission> EmployeeSubmission { get; set; }
        public int? ManagerId { get; set; }
        public List<User> Users { get; set; } // List of managers/users
    }
}
