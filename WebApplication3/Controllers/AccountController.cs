using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Data;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Identity;
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
                    UserName = model.UserName
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
                        //success
                        var Claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("Name", user.UserName),
                        new Claim(ClaimTypes.Role, "user")
                    };
                        var ClaimIdentity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(ClaimIdentity));
                        return RedirectToAction("Index", "Employes");
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
        //[Authorize]
        //    public IActionResult SecurePage()
        //{
        //    ViewBag.Name = HttpContext.User.Identity.Name;
        //    return View();
        //}
    }
    // Change the type of Id in RegistrationViewModel to in
}
