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
namespace WebApplication3.Controllers

{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
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
                        else
                            return RedirectToAction("UserDashboard", "Admin");
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
            return RedirectToAction("Login");
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