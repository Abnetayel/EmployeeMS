using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using DbUpdateException = Microsoft.EntityFrameworkCore.DbUpdateException;
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
       
    }
    // Change the type of Id in RegistrationViewModel to in
}