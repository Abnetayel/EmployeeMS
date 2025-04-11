using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using StudentPortal.Web.Data;
//using StudentPortal.Web.Models;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class AI : Controller
    {
        private readonly ApplicationDbContext _context;

        public AI(ApplicationDbContext context)
        {
            _context = context;
        }

        // Existing action methods...

        [HttpGet]
        public async Task<IActionResult> ExportEmployeToCsv()
        {
            // Get the students data
            var employees = await _context.Employee
                //.Include(s => s.User)
                .OrderBy(s => s.Experience)
                .ToListAsync();

            // Create CSV content
            var csv = new StringBuilder();

            // Add header row
            csv.AppendLine("FullName,FullName,Experience,Education,Gender,EmployementType,Skills,Country,Salary");

            // Add data rows
            foreach (var Employee in employees)
            {
                csv.AppendLine($"\"{Employee.FullName}\",\"{Employee.Age}\",\"{Employee.Experience}\",{Employee.Education},\"{Employee.Gender}\",\"{Employee.EmployementType}\",\"{Employee.Skills}\",\"{Employee.Country}\",{Employee.Salary}");
            }

            // Create the file content
            byte[] fileContents = Encoding.UTF8.GetBytes(csv.ToString());

            // Return the file
            return File(fileContents, "text/csv", "EmployeesExport.csv");
        }
    }
}