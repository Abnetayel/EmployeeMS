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
        await _context.SaveChangesAsync();

        return RedirectToAction("AdminDashboard");
    }


    [HttpGet]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> EditUser(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Map the User entity to EditUserModel
        var editUserModel = new EditUserModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role
            // Map any other properties that exist in both models
        };

        return View(editUserModel);
    }



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

    //[HttpGet]
    //[Authorize(Policy = "AdminOrHigher")]
    //public IActionResult AssignEmployees()
    //{
    //    var users = _context.Users.Select(u => new { u.Id, u.UserName }).ToList();
    //    var employees = _context.Employee.Select(e => new { e.Id, e.FullName }).ToList();

    //    var model = new AssignEmployeesViewModel
    //    {
    //        Users = new SelectList(users, "Id", "UserName"),
    //        Employees = new SelectList(employees, "Id", "FullName")
    //    };

    //    return View(model);
    //}

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

    //[HttpPost]
    //[Authorize(Policy = "SuperAdminOnly")]
    //public async Task<IActionResult> EditUser(User model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(model);
    //    }

    //    var user = await _context.Users.FindAsync(model.Id);
    //    if (user == null)
    //    {
    //        return NotFound();
    //    }

    //    user.UserName = model.UserName;
    //    user.Email = model.Email;

    //    await _context.SaveChangesAsync();

    //    TempData["SuccessMessage"] = "User updated successfully";
    //return RedirectToAction("SuperAdminDashboard");
    //}

    //public async Task<IActionResult> EditUser(int id, string newRole, [Bind("Id,UserName,Email")] User user)
    //{
    //    if (id != user.Id)
    //    {
    //        // Should return an error, not just log to console
    //        TempData["ErrorMessage"] = "ID mismatch";
    //        return RedirectToAction("SuperAdminDashboard");
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            // Get the existing user from database
    //            var existingUser = await _context.Users.FindAsync(id);
    //            if (existingUser == null)
    //            {
    //                return NotFound();
    //            }

    //            // Update only the allowed properties (to prevent overposting)
    //            existingUser.UserName = user.UserName;
    //            existingUser.Email = user.Email;

    //            // Update the role if newRole parameter is provided
    //            if (!string.IsNullOrEmpty(newRole))
    //            {
    //                existingUser.Role = newRole;
    //            }

    //            _context.Update(existingUser);
    //            await _context.SaveChangesAsync();

    //            return RedirectToAction("SuperAdminDashboard");
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!UserExists(user.Id))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //    }

    //    // If we got this far, something failed, redisplay form
    //    return View(user);
    //}

    //private bool UserExists(int id)
    //{
    //    throw new NotImplementedException();
    //}



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserModel user)
     //int id,
     //[Bind("Id,UserName,Email,Role")] EditUserModel user,  
     ////string Password,
     //String Role)  
    {
        //if (id != user.Id)
        //{
        //    TempData["ErrorMessage"] = "ID mismatch";
        //    return RedirectToAction("SuperAdminDashboard");
        //}

        if (user.Role == "SuperAdmin")
        {
            TempData["ErrorMessage"] ="Not recommended";
            return RedirectToAction("SuperAdminDashboard");
        }
        //if (user.Password=="")
        //{

        //}
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








    //[HttpPost]
    //[Authorize(Policy = "UserOrHigher")]
    //public async Task<IActionResult> AddEmployee(Employe model)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(model);
    //    }

    //    _context.Employee.Add(model);
    //    await _context.SaveChangesAsync();

    //    return RedirectToAction("UserDashboard");
    //}

    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}