using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using DbUpdateException = Microsoft.EntityFrameworkCore.DbUpdateException;
using Microsoft.CodeAnalysis.Scripting;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ISO3166;
using Microsoft.AspNetCore.Mvc.Rendering;
using CloudinaryDotNet;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
namespace WebApplication3.Controllers

{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
     

        private List<User> GetSuperAdmins()
        {
            return _context.Users.Where(u => u.Role == "SuperAdmin").ToList();
        }

        private async Task NotifySuperAdminsAboutNewEmployee(Employe employee)
        {
            var superAdmins = GetSuperAdmins();
            if (!superAdmins.Any())
            {
                Console.WriteLine("No SuperAdmins found to notify.");
                return;
            }

            var subject = "New Employee Registration";
            var body = $@"
        <h1>New Employee Registered</h1>
        <p><strong>Full Name:</strong> {employee.FullName}</p>
        <p><strong>Email:</strong> {employee.PhoneNumber}</p>
        <p><strong>Country:</strong> {employee.Country}</p>
        <p><strong>Registration Date:</strong> {employee.DateOfRegistration}</p>";

            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("abnetayele694@gmail.com", "emcfhgtxvskobzwy"),
                EnableSsl = true
            };

            foreach (var superAdmin in superAdmins)
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("abnetayele694@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(superAdmin.Email);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email to {superAdmin.Email}: {ex.Message}");
                }
            }
        }


        //[Authorize]
        [HttpGet]


        public IActionResult RegisterEmployee()
        {
            var countries = Country.List.Select(c => new { c.Name }).ToList();
            ViewBag.Countries = new SelectList(countries, "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEmployee(RegisterEmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email is already registered
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Email is already registered.");
                    return View(model);
                }

                var passwordHasher = new PasswordHasher<User>();
                // Create a new user
                var user = new User
                {
                    UserName = model.Email, // Use email as username
                    Email = model.Email,
                    //Password = BCrypt.Net.BCrypt.HashPassword(model.Password), // Hash the password
                    Role = "Employee" // Assign the "Employee" role
                };

                user.Password = passwordHasher.HashPassword(user, model.Password); // Hash the password
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create an associated employee record
                var employee = new Employe
                {
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Country = model.Country,
                    DateOfRegistration = DateTime.Now,
                    Age = model.Age,
                    Experience = model.Experience,
                    Education = model.Education,
                    Skills = model.Skills,
                    Gender = model.Gender,
                    EmployementType = model.EmployementType,
                    Salary = 0, // Default salary; to be updated by SuperAdmin
                    ManagerId = null // No manager assigned initially
                };

                _context.Employee.Add(employee);
                await _context.SaveChangesAsync();

                await NotifySuperAdminsAboutNewEmployee(employee);

                return RedirectToAction("Login", "Account");
            }

            var countries = Country.List.Select(c => new { c.Name }).ToList();
            ViewBag.Countries = new SelectList(countries, "Name", "Name");
            return View(model);
        }






        public IActionResult ViewProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var user = _context.Users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound();

            }
            var model = new UserProfileViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                Role = user.Role
            };

            return View(model);
        }

        [Authorize]
        [HttpGet]
        [Route("Account/EditProfile")]
        public IActionResult EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Use ClaimTypes.Email
            if (userId==null)
            {
                return Unauthorized(); // Return 401 if the email claim is missing
            }

            var user = _context.Users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound(); // Return 404 if the user is not found
            }
            var model = new EditUserModel1
            {
                Id = user.Id,
                Email = user.Email,
                UserName=user.UserName
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditUserModel1 model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Use ClaimTypes.Email
            if (userId==null)
            {
                return Unauthorized(); 
            }

            var user = _context.Users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound(); 
            }

            // Verify current password
            var passwordHasher = new PasswordHasher<User>();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, model.CurrentPassword);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View(model);
            }

            // Update email and password
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Password = passwordHasher.HashPassword(user, model.NewPassword);

            _context.SaveChanges();

            TempData["Message"] = "Profile updated successfully.";
            return RedirectToAction("ViewProfile");

        }

   

        [HttpGet]
        public IActionResult Registation()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registation(RegistrationViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                 
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return View(model);
            }
            if (_context.Users.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "UserName is already in use.");
                return View(model);
            }
            try
            {

                var passwordHasher = new PasswordHasher<User>();
                User account = new User
                {
                    Id = model.Id,
                    Email = model.Email,
                    UserName = model.UserName,
                    Role=model.Role,
                    PasswordResetToken = Guid.NewGuid().ToString()
                };
                account.Password = passwordHasher.HashPassword(account, model.Password); // Hash the password


                _context.Users.Add(account);
                _context.SaveChanges();


                ModelState.Clear();


                //return RedirectToAction("Login", "Account");
                TempData["Message"] = $"{account.UserName} registered successfully. Please login.";
                return RedirectToAction("Login");


            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);


                ModelState.AddModelError("", "An error occurred while saving. Please enter a unique Email and Username.");

                return View(model);
            }
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(x => x.UserName == model.UserNameOrEmail || x.Email == model.UserNameOrEmail);
                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<User>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role) 
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


                        if (user.Role == "SuperAdmin")
                            return RedirectToAction("SuperAdminDashboard", "Admin");
                        else if (user.Role == "Admin")
                            return RedirectToAction("AdminDashboard", "Admin");
                        else if (user.Role == "User")
                            return RedirectToAction("UserDashboard", "Admin");                    
                        else
                            return RedirectToAction("Index", "EmployeePortal");
                    }
                    else
                    {
                        ModelState.AddModelError("UserNameOrEmail", "Username or Email is not correct");
                        ModelState.AddModelError("Password", "Password is not correct");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Password is not correct");
                }
            }
            return View(model);
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            return RedirectToAction("Login", "Account");
        }



        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Email is required.");
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No user found with this email.");
                return View();
            }

            // Generate a password reset token
            var token = Guid.NewGuid().ToString();
            Console.WriteLine("the reset link is " + token);
            // Save the token in the database (or a secure store)
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour
            await _context.SaveChangesAsync();

            // Generate the reset link
            var resetLink = Url.Action("ResetPassword", "Account", new { token }, Request.Scheme);
            Console.WriteLine("the reset link is "+resetLink);
            // Send the reset link via email
            await SendResetLinkEmail(user.Email, resetLink);

            ViewBag.Message = "Password reset link has been sent to your email.";
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid password reset token.");
            }

            var model = new ResetPasswordViewModel { Token = token };
            
            return View(model);
        }

        // Fix for the CS7036 error in the ResetPassword method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == model.Token && u.PasswordResetTokenExpiry > DateTime.UtcNow);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired password reset token.");
                return View(model);
            }

            // Update the user's password
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, model.NewPassword); // Pass the user and the new password
            //user.PasswordResetToken = null; // Clear the token
            //user.PasswordResetTokenExpiry = null;
            await _context.SaveChangesAsync();

            ViewBag.Message = "Your password has been reset successfully.";
            return RedirectToAction("Login");
        }

        private async Task SendResetLinkEmail(string email, string resetLink)
        {
            var subject = "Password Reset Request";
            var body = $"<p>Click the link below to reset your password:</p><p><a href='{resetLink}'>{resetLink}</a></p>";

            // Example using SMTP
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
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }










    }
    // Change the type of Id in RegistrationViewModel to in
}