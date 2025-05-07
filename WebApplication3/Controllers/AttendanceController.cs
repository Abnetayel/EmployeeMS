using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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


        public IActionResult Analytics()
        {
            // Fetch attendance data grouped by employee and month           

            var attendanceData = _context.Attendances
                .GroupBy(a => new { a.EmployeeId, a.Date.Month })
                .Select(g => new
                {
                    g.Key.EmployeeId,
                    g.Key.Month,
                    PresentDays = g.Count(a => a.IsPresent),
                    AbsentDays = g.Count(a => !a.IsPresent)
                })
                .ToList();

            // Prepare data for heatmap
            var heatmapData = attendanceData
                .GroupBy(a => a.EmployeeId)
                .Select(g => new HeatmapData
                {
                    EmployeeName = _context.Employee.FirstOrDefault(e => e.Id == g.Key)?.FullName,
                    MonthlyAttendance = g.Select(a => new MonthlyAttendance
                    {
                        Month = a.Month,
                        PresentDays = a.PresentDays,
                        AbsentDays = a.AbsentDays
                    }).ToList()
                })
                .ToList();

            // Identify frequent absentees
            var frequentAbsentees = attendanceData
                .Where(a => a.AbsentDays > 5) // Example threshold
                .Select(a => new FrequentAbsentee
                {
                    EmployeeName = _context.Employee.FirstOrDefault(e => e.Id == a.EmployeeId)?.FullName,
                    AbsentDays = a.AbsentDays
                })
                .ToList();

            // Pass data to the view
            var viewModel = new AttendanceAnalyticsViewModel
            {
                HeatmapData = heatmapData,
                FrequentAbsentees = frequentAbsentees
            };
            Console.WriteLine($"HeatmapData: {JsonConvert.SerializeObject(viewModel.HeatmapData)}");

            return View(viewModel);
        }
    }
}
