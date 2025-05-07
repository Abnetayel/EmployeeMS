using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

public class EmployeeSubmissionController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmployeeSubmissionController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Submit Employee Information
    [HttpGet]
    public IActionResult Submit()
    {
        return View();
    }

    // POST: Submit Employee Information
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(EmployeeSubmission submission)
    {
        if (ModelState.IsValid)
        {
            _context.EmployeeSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your submission has been sent for approval.";
            return RedirectToAction("Submit");
        }

        return View(submission);
    }

    [Authorize(Policy = "AdminOrHigher")]
    public async Task<IActionResult> PendingSubmissions()
    {
        var submissions = await _context.EmployeeSubmissions
            .Where(s => !s.IsApproved)
            .ToListAsync();

        // Directly project into a List<User>
        var users = _context.Users
            .Where(u => u.Role == "User")
            .Select(u => new User
            {
                Id = u.Id,
                UserName = $"{u.UserName} ({u.Email})" // Combine UserName and Email for display
            })
            .ToList();

        ViewBag.Users = new SelectList(users, "Id", "UserName");
        var model = new MnSubmitViewModel
        {
            EmployeeSubmission = submissions,
            Users = users // Pass the list of users directly
        };

        return View(model);
    }


    // POST: Approve Submission
    [HttpPost]
    [Authorize(Policy = "AdminOrHigher")]
    public async Task<IActionResult> ApproveSubmission(int id, int managerId)
    {
        var submission = await _context.EmployeeSubmissions.FindAsync(id);
        if (submission == null)
        {
            return NotFound();
        }

        // Create a new employee based on the submission
        var employee = new Employe
        {
            FullName = submission.FullName,
            Age = submission.Age,
            Experience = submission.Experience,
            DateOfRegistration = DateTime.Now,
            Education = submission.Education,
            Skills = submission.Skills,
            Gender = submission.Gender,
            EmployementType = submission.EmployementType,
            Salary = submission.Salary,
            PhoneNumber = submission.PhoneNumber,
            Country = submission.Country,
            ManagerId = managerId // Assign the selected manager
        };

        _context.Employee.Add(employee);
        submission.IsApproved = true;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Employee has been created successfully.";
        return RedirectToAction("PendingSubmissions");
    }
}
