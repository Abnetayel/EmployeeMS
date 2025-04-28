using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var loggedInUserName = User.Identity?.Name;

            var loggedInUser = _context.Users.FirstOrDefault(u => u.UserName == loggedInUserName);

            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            var attendances = _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Employee.ManagerId == loggedInUser.Id)
                .ToList();

            return View(attendances);
        }

        // GET: Attendance/Create
        public IActionResult Create()
        {
            var loggedInUserName = User.Identity?.Name;

            var loggedInUser = _context.Users.FirstOrDefault(u => u.UserName == loggedInUserName);

            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            var employees = _context.Employee
                .Where(e => e.ManagerId == loggedInUser.Id)
                .Select(e => new { e.Id, e.FullName })
                .ToList();

            if (!employees.Any())
            {
                Console.WriteLine("No employees found in the database.");
            }

            ViewBag.Employees = employees;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("EmployeeId,Date,IsPresent")] Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                _context.Attendances.Add(attendance);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(attendance);
        }
    }
}
