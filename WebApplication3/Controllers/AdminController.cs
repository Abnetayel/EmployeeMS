using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

[Authorize]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    //private readonly UserManager<User> _userManager;

    public AdminController(ApplicationDbContext context)
        //, UserManager<User> userManager)
    {
        _context = context;
        //_userManager = userManager;
    }

    [Authorize(Policy = "SuperAdminOnly")]
    public IActionResult SuperAdminDashboard()
    {
        var users = _context.Users.ToList();
        return View(users);
    }

    [Authorize(Policy = "AdminOrHigher")]
    public IActionResult AdminDashboard()
    {
        var users = _context.Users.Where(u => u.Role != "SuperAdmin").ToList();
        return View(users);
    }
    [HttpGet]
    [Authorize(Policy = "User")]
    public IActionResult UserDashboard()
    {
        
        var loggedInUserName = User.Identity?.Name;

        // Find the logged-in user
        var loggedInUser = _context.Users.FirstOrDefault(u => u.UserName == loggedInUserName);

        if (loggedInUser == null)
        {
            return Unauthorized();
        }

        
        var employees = _context.Employee
            .Where(e => e.ManagerId == loggedInUser.Id)
            .ToList();

        return View(employees);
    }

    [HttpGet]
    [Authorize(Policy = "SuperAdminOnly")]
    public IActionResult AddAdmin()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> AddAdmin(RegistrationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            Role = "Admin"
        };

        var passwordHasher = new PasswordHasher<User>();
        user.Password = passwordHasher.HashPassword(user, model.Password);

        _context.Users.Add(user);
        await SendEmployeeDetailsEmail(model);
        await _context.SaveChangesAsync();

        return RedirectToAction("SuperAdminDashboard");
    }

    [HttpGet]
    [Authorize(Policy = "AdminOrHigher")]
    public IActionResult AddUser()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrHigher")]
    public async Task<IActionResult> AddUser(RegistrationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            Role = "User",
            PasswordResetToken = Guid.NewGuid().ToString()
        };

        var passwordHasher = new PasswordHasher<User>();
        user.Password = passwordHasher.HashPassword(user, model.Password);

        _context.Users.Add(user);
        await SendEmployeeDetailsEmail(model);
        await _context.SaveChangesAsync();

        return RedirectToAction("AdminDashboard");
    }


    private async Task SendEmployeeDetailsEmail(RegistrationViewModel user)
    {
      
        // Email subject and body
        var subject = " You Registerd ";
        var body = $@"
        <h1>Your Information </h1>
        <p><strong>UserName:</strong> {user.UserName}</p>    
        <p><strong>Your Role:</strong> {user.Role}</p>    
        <p><strong>Password:</strong> {user.Password}</p>";

        using var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("abnetayele694@gmail.com", "emcfhgtxvskobzwy"),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("abnetayele694@gmail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(user.Email);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email to {user.Email}: {ex.Message}");
        }
        //}
    }



    //[HttpGet]
    //[Authorize(Policy = "SuperAdminOnly")]
    //public async Task<IActionResult> EditUser(int? id)
    //{
    //    if (id == null)
    //    {
    //        return NotFound();
    //    }

    //    var user = await _context.Users.FindAsync(id);
    //    if (user == null)
    //    {
    //        return NotFound();
    //    }

    //    // Map the User entity to EditUserModel
    //    var editUserModel = new EditUserModel
    //    {
    //        Id = user.Id,
    //        UserName = user.UserName,
    //        Email = user.Email,
    //        Role = user.Role
           
    //    };

    //    return View(editUserModel);
    //}



    [HttpGet]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> DeleteUser(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrHigher")]
    public async Task<IActionResult> AssignEmployeesToUser(int userId, List<int> employeeIds)
    {
        var user = await _context.Users.Include(u => u.ManagedEmployees).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var employees = await _context.Employee.Where(e => employeeIds.Contains(e.Id)).ToListAsync();
        foreach (var employee in employees)
        {
            employee.ManagerId = userId;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Employees assigned successfully.";
        return RedirectToAction("AdminDashboard");
    }

    [HttpGet]
    [Authorize(Policy = "AdminOrHigher")]
    public IActionResult AssignEmployees()
    {
        var users = _context.Users.Select(u => new { u.Id, u.UserName }).ToList();

        
        var unassignedEmployees = _context.Employee
            .Where(e => e.ManagerId == null)  
            .Select(e => new { e.Id, e.FullName })
            .ToList();

        var model = new AssignEmployeesViewModel
        {
            Users = new SelectList(users, "Id", "UserName"),
            Employees = new SelectList(unassignedEmployees, "Id", "FullName")
        };

        return View(model);
    }
    [Authorize(Policy = "AdminOrHigher")]
    public async Task<IActionResult> DetailUser(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(m => m.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }


    [HttpPost, ActionName("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _context.Users.FindAsync(id);
            if(user!= null)
        {
            _context.Users.Remove(user);
        }
            await _context.SaveChangesAsync();
        return RedirectToAction("SuperAdminDashboard");

    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserModel user)
    
    {
       

        if (user.Role == "SuperAdmin")
        {
            TempData["ErrorMessage"] ="Not recommended";
            return RedirectToAction("SuperAdminDashboard");
        }
        
        TryValidateModel(user);

        if (ModelState.IsValid)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    return NotFound();
                }                             
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.Role = user.Role;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User updated successfully";
                return RedirectToAction("SuperAdminDashboard");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. The record was modified by another user.");
                    
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred while saving.");
                
            }
        } 
        return View(user);
    }

    [HttpGet]
    [Authorize(Policy = "UserOrHigher")]
    public IActionResult AddEmployee()
    {
        return RedirectToAction("Create", "Employes");
    }
    [HttpGet]
    [Authorize(Policy = "User")]
    public IActionResult Empolylist()
    {
        return RedirectToAction("List", "Employes");
    }

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }


    [Authorize(Policy = "SuperAdminOnly")]
    [HttpGet]
    public IActionResult UpdateSalary(int employeeId)
    {
        var employee = _context.Employee.FirstOrDefault(e => e.Id == employeeId);
        if (employee == null)
        {
            return NotFound();
        }
        var model = new UpdateSalaryViewModel
        {
            EmployeeId = employee.Id,
            Salary = employee.Salary
        };
        return View(model);
    }
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateSalary(UpdateSalaryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var employee = _context.Employee.FirstOrDefault(e => e.Id == model.EmployeeId);
            if (employee == null)
            {
                return NotFound();
            }

            employee.Salary = model.Salary;
            _context.SaveChanges();

            TempData["Message"] = "Salary updated successfully.";
            return RedirectToAction("List", "Employes");
        }
        return View(model);
    }

}