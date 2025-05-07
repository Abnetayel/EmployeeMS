using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using WebApplication3.Data;
using WebApplication3.Models;
//using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize(Policy = "Employee")]
    public class EmployeePortalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeePortalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmployeePortal/Index
        [HttpGet]
        public IActionResult Index()
        {
            var loggedInUserName = User.Identity?.Name;
            Console.WriteLine(loggedInUserName);
            var employee = _context.Employee.FirstOrDefault(e => e.FullName == loggedInUserName);

            if (employee == null)
            {
                return Unauthorized();
            }

            var viewModel = new EmployeePortalViewModel
            {
                Employee = employee,
                AttendanceSummary = new AttendanceSummary
                {
                    PresentDays = _context.Attendances.Count(a => a.EmployeeId == employee.Id && a.IsPresent),
                    AbsentDays = _context.Attendances.Count(a => a.EmployeeId == employee.Id && !a.IsPresent)
                },
                OnboardingTasks = _context.OnboardingTasks
                    .Where(t => t.EmployeeId == employee.Id)
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: EmployeePortal/EditProfile
        [HttpGet]
        public IActionResult EditProfile()
        {
            var loggedInUserName = User.Identity?.Name;
            var employee = _context.Employee.FirstOrDefault(e => e.FullName == loggedInUserName);

            if (employee == null)
            {
                return Unauthorized();
            }

            var model = new EditEmployeeProfileViewModel
            {
                FullName = employee.FullName,
                PhoneNumber = employee.PhoneNumber,
                Country = employee.Country
            };

            return View(model);
        }

        // POST: EmployeePortal/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditEmployeeProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var loggedInUserName = User.Identity?.Name;
                        var employee = _context.Employee.FirstOrDefault(e => e.FullName == loggedInUserName);
                        var user = _context.Users.FirstOrDefault(e => e.UserName == loggedInUserName);

                        if (employee == null || user == null)
                        {
                            return Unauthorized();
                        }

                        employee.FullName = model.FullName;
                        employee.PhoneNumber = model.PhoneNumber;
                        employee.Country = model.Country;
                        user.UserName = model.FullName;

                        _context.SaveChanges();
                        transaction.Commit();

                        TempData["SuccessMessage"] = "Profile updated successfully.";
                        return RedirectToAction("Index", "EmployeePortal");
                    }
                    catch
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "An error occurred while updating the profile.");
                    }
                }
            }

            return View(model);
        }

        // GET: EmployeePortal/Attendance
        [HttpGet]
        public IActionResult Attendance()
        {
            var loggedInUserName = User.Identity?.Name;
            var employee = _context.Employee.FirstOrDefault(e => e.FullName == loggedInUserName);

            if (employee == null)
            {
                return Unauthorized();
            }

            var attendanceRecords = _context.Attendances
                .Where(a => a.EmployeeId == employee.Id)
                .ToList();

            return View(attendanceRecords);
        }

        // GET: EmployeePortal/OnboardingTasks
        [HttpGet]
        public IActionResult OnboardingTask()
        {
            var loggedInUserName = User.Identity?.Name;
            var employee = _context.Employee.FirstOrDefault(e => e.FullName == loggedInUserName);

            if (employee == null)
            {
                return Unauthorized();
            }

            var tasks = _context.OnboardingTasks
                .Where(t => t.EmployeeId == employee.Id)
                .ToList();

            return View(tasks);
        }
    }
  
   
}
