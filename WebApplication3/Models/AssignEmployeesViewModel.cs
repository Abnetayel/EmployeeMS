using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication3.Models
{
    public class AssignEmployeesViewModel
    {
        public int UserId { get; set; }
        public List<int> EmployeeIds { get; set; }
        public SelectList Users { get; set; }
        public SelectList Employees { get; set; }
    }

}
