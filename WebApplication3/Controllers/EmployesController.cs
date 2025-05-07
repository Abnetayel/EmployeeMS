using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Azure.Core;
using ISO3166;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
  
    public class EmployesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        //GET: Employes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employee.ToListAsync());
        }

        //public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        //{
        //    var totalEmployees = await _context.Employee.CountAsync();
        //    var employees = await _context.Employee
        //        .OrderBy(e => e.FullName)
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var viewModel = new EmployeeIndexViewModel
        //    {
        //        Employees = employees,
        //        TotalEmployees = totalEmployees,
        //        PageNumber = pageNumber,
        //        PageSize = pageSize
        //    };

        //    return View(viewModel);
        //}





        [Authorize]

       

            // GET: Onboarding Tasks for an Employee
            [HttpGet]
            public async Task<IActionResult> OnboardingTasks(int employeeId)
            {
                var employee = await _context.Employee
                    .FirstOrDefaultAsync(e => e.Id == employeeId);

                if (employee == null)
                {
                    return NotFound();
                }

                var tasks = await _context.OnboardingTasks
                    .Where(t => t.EmployeeId == employeeId)
                    .ToListAsync();

                var viewModel = new OnboardingTaskViewModel
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.FullName,
                    Tasks = tasks
                };

                return View("OnboardingTasks", viewModel); // Explicitly return OnboardingTasks.cshtml
            }

            // POST: Add a New Onboarding Task
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AddOnboardingTask(OnboardingTask task)
            {
                if (ModelState.IsValid)
                {
                    _context.OnboardingTasks.Add(task);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(OnboardingTasks), new { employeeId = task.EmployeeId });
                }

                // If validation fails, reload the OnboardingTasks view
                var employee = await _context.Employee
                    .FirstOrDefaultAsync(e => e.Id == task.EmployeeId);

                if (employee == null)
                {
                    return NotFound();
                }

                var tasks = await _context.OnboardingTasks
                    .Where(t => t.EmployeeId == task.EmployeeId)
                    .ToListAsync();

                var viewModel = new OnboardingTaskViewModel
                {
                    EmployeeId = task.EmployeeId,
                    EmployeeName = employee.FullName,
                    Tasks = tasks
                };

                return View("OnboardingTasks", viewModel);
            }

            // POST: Mark Task as Completed
            [HttpPost]
            public async Task<IActionResult> CompleteTask(int taskId)
            {
                var task = await _context.OnboardingTasks.FindAsync(taskId);
                if (task == null)
                {
                    return NotFound();
                }

                task.IsCompleted = true;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(OnboardingTasks), new { employeeId = task.EmployeeId });
            }






















        [Authorize]
        public async Task<IActionResult> MyEmployees()
        {
            // Get the currently logged-in user's Id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // Return 401 if the user is not logged in
            }

            // Retrieve employees assigned to the logged-in user
            var employees = await _context.Employee
                .Where(e => e.ManagerId.ToString() == userId)
                .ToListAsync();

            return View(employees);
        }


        [Authorize]
        //GET: Employes
        public async Task<IActionResult> List()
        {
            return View(await _context.Employee.ToListAsync());
        }

        [Authorize]
        //GET: Employes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employe = await _context.Employee
                .Include(e => e.Attendances) // Include attendance records
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employe == null)
            {
                return NotFound();
            }

            return View(employe);
        }
        [Authorize]
        // GET: Employes/Create
        public IActionResult Create()
        {
            var countries = Country.List.Select(c => new { c.Name }).ToList();
            ViewBag.Countries = new SelectList(countries, "Name", "Name");
            var users = _context.Users
                    .Where(u => u.Role == "User")
                    .Select(u => new {
                        u.Id,
                        DisplayName = $"{u.UserName} ({u.Email})"
                    })
                    .ToList();
            
            ViewBag.Users = new SelectList(users, "Id", "DisplayName");
            return View();
        }


        //[HttpGet]
        //public IActionResult ExportToCsv()
        //{
        //    try
        //    {
        //        var exporter = new CsvExporter(_context);
        //        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //        var filePath = Path.Combine(desktopPath, "Employees1.csv");

        //        exporter.ExportEmployeesToCsv(filePath);

        //        return File(System.IO.File.ReadAllBytes(filePath), "text/csv", "Employees1.csv");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error and return an error message
        //        return BadRequest(new { message = "Error exporting employees: " + ex.Message });
        //    }
        //}
 
        public IActionResult CombinedDashboard()
        {
            // Ensure _context.Employee is not null before proceeding
            var employees = _context.Employee?.ToList();
            if (employees == null || !employees.Any())
            {
                // Handle the case where there are no employees
                return View(new CombinedDashboardViewModel
                {
                    TotalEmployees = 0,
                    AverageSalary = 0,
                    GenderDistribution = new List<GenderDistribution1>(),
                    TopCountries = new List<dynamic>(),
                    RecentHires = new List<dynamic>(),
                    SkillsExperience = new List<SkillExperienceData>(),
                    MonthlyAttendanceTrends = new List<MonthlyAttendanceTrend>()
                });
            }

            // Ensure _context.Attendances is not null before proceeding
            var monthlyAttendance = _context.Attendances?.AsEnumerable()
                .GroupBy(a => a.Date.ToString("MMMM"))
                .Select(g => new MonthlyAttendanceTrend
                {
                    Month = g.Key,
                    PresentDays = g.Count(a => a.IsPresent),
                    AbsentDays = g.Count(a => !a.IsPresent)
                })
                .ToList() ?? new List<MonthlyAttendanceTrend>();

            var viewModel = new CombinedDashboardViewModel
            {
                TotalEmployees = employees.Count,
                AverageSalary = employees.Average(e => e.Salary),
                GenderDistribution = employees
                    .GroupBy(e => e.Gender)
                    .Select(g => new GenderDistribution1
                    {
                        Gender = g.Key,
                        Count = g.Count()
                    }).ToList(),
                TopCountries = employees
                    .GroupBy(e => e.Country)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new { Country = g.Key, Count = g.Count() })
                    .ToList<dynamic>(),
                RecentHires = employees
                    .OrderByDescending(e => e.DateOfRegistration)
                    .Take(5)
                    .Select(e => new { e.FullName, e.Skills, e.DateOfRegistration, e.Country })
                    .ToList<dynamic>(),
                SkillsExperience = employees
                    .GroupBy(e => e.Skills)
                    .Select(g => new SkillExperienceData
                    {
                        Skill = g.Key,
                        AverageExperience = (int)g.Average(e => e.Experience)
                    }).ToList(),
                MonthlyAttendanceTrends = monthlyAttendance
            };

            return View(viewModel);
        }

        //public class TopCountry
        //{
        //    public string Country { get; set; }
        //    public int Count { get; set; }
        //}







        [HttpGet]
        public IActionResult AdvancedDashboard()
        {
            // Example: Fetch data for visualizations
            var totalEmployees = _context.Employee.Count();
            var topCountries = _context.Employee
                .GroupBy(e => e.Country)
                .Select(g => new { Country = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToList();

            var recentHires = _context.Employee
                .OrderByDescending(e => e.DateOfRegistration)
                .Take(5)
                .ToList();

            ViewBag.TotalEmployees = totalEmployees;
            ViewBag.TopCountries = topCountries;
            ViewBag.RecentHires = recentHires;

            return View();
        }






        [HttpGet]
        public IActionResult Dashboard()
        {
            var totalEmployees = _context.Employee.Count();
            var averageSalary = _context.Employee.Average(e => e.Salary);
            var genderDistribution = _context.Employee
                .GroupBy(e => e.Gender)
                .Select(static g => new GenderDistribution
                {
                    Gender = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var viewModel = new DashboardViewModel
            {
                TotalEmployees = totalEmployees,
                AverageSalary = averageSalary,
                GenderDistribution = genderDistribution
            };

            return View(viewModel);
        }






        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Age,Experience,DateOfRegistration,Education,Skills,EmployementType,Salary,Currency,Gender,PhoneNumber,Country,ManagerId")] Employe employe)
        {
            if (ModelState.IsValid)
            {              
                if (employe.ManagerId == null || employe.ManagerId == 0)
                {
                    //var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    //employe.ManagerId = currentUserId;
                    employe.ManagerId = null;
                }
                _context.Add(employe);
                await _context.SaveChangesAsync();
                await SendEmployeeDetailsEmail(employe);
                return RedirectToAction(nameof(Index));
            }
            return View(employe);
        }


        private async Task SendEmployeeDetailsEmail(Employe employe)
        {
            // Get all assigned users (e.g., Admins or SuperAdmins)
            // Get the assigned user (Manager) based on the ManagerId
            var assignedUser = _context.Users.FirstOrDefault(u => u.Id == employe.ManagerId);

            if (assignedUser == null)
            {
                Console.WriteLine("No assigned user found for this employee.");
                return;
            }

            // Email subject and body
            var subject = "New Employee Created";
            var body = $@"
        <h1>New Employee Details</h1>
        <p><strong>Full Name:</strong> {employe.FullName}</p>
        <p><strong>Age:</strong> {employe.Age}</p>
        <p><strong>Experience:</strong> {employe.Experience} years</p>
        <p><strong>Education:</strong> {employe.Education}</p>
        <p><strong>Skills:</strong> {employe.Skills}</p>
        <p><strong>Employment Type:</strong> {employe.EmployementType}</p>
        <p><strong>Salary:</strong> {employe.Salary:C}</p>
        <p><strong>Gender:</strong> {employe.Gender}</p>
        <p><strong>Phone Number:</strong> {employe.PhoneNumber}</p>
        <p><strong>Country:</strong> {employe.Country}</p>";

            // SMTP configuration
            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("abnetayele694@gmail.com", "emcfhgtxvskobzwy"),
                EnableSsl = true
            };

            //foreach (var user in assignedUser)
            //{
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("abnetayele694@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(assignedUser.Email);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email to {assignedUser.Email}: {ex.Message}");
                }
            //}
        }







        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PredictSalary([FromBody] SalaryPredictionRequest request)
        {
            try
            {
                // Input Validation
                if (request.Age <= 18 || request.Age > 70)
                {
                    return BadRequest(new { success = false, message = "Age must be between 18 and 70." });
                }

                if (request.Experience < 0 || request.Experience > 50)
                {
                    return BadRequest(new { success = false, message = "Experience must be between 0 and 50 years." });
                }

                if (string.IsNullOrWhiteSpace(request.Education) ||
                    string.IsNullOrWhiteSpace(request.Skills) ||
                    string.IsNullOrWhiteSpace(request.Gender) ||
                    string.IsNullOrWhiteSpace(request.EmployementType) ||
                    string.IsNullOrWhiteSpace(request.Country))
                {
                    return BadRequest(new { success = false, message = "All fields are required." });
                }

                // Encode the input
                var encodedRequest = EncodeRequest(request);

                // Try ML Prediction
                try
                {
                    var mlInput = new MLModel3.ModelInput()
                    {
                        Age = (float)encodedRequest.Age,
                        Experience = (float)encodedRequest.Experience,
                        Education = float.Parse(encodedRequest.Education),
                        Skills = float.Parse(encodedRequest.Skills),
                        //Gender = float.Parse(encodedRequest.Gender),
                        EmployementType = float.Parse(encodedRequest.EmployementType),
                        Country = encodedRequest.Country
                    };

                    var prediction = MLModel3.Predict(mlInput);
                    var predictedSalary = (decimal)prediction.Score;

                    return Ok(new
                    {
                        success = true,
                        predictedSalary = predictedSalary.ToString("N2"),
                        method = "ML Prediction"
                    });
                }
                catch (Exception mlEx)
                {
                    // Fallback to Database Average if ML fails
                    try
                    {
                        var averageSalary = await _context.Employee
                            .Where(e => e.Country == request.Country &&
                                       e.Education == request.Education)
                            .AverageAsync(e => e.Salary);
                        return Ok(new
                        {
                            success = true,
                            predictedSalary = averageSalary.ToString("N2"),
                            method = "Database Fallback"
                        });
                    }
                    catch (Exception dbEx)
                    {
                        return StatusCode(500, new
                        {
                            success = false,
                            message = $"ML Prediction failed: {mlEx.Message}. Fallback also failed: {dbEx.Message}"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Unexpected error: {ex.Message}"
                });
            }
        }


        private SalaryPredictionRequest EncodeRequest(SalaryPredictionRequest request)
        {
            return new SalaryPredictionRequest
            {
                Age = request.Age,
                Experience = request.Experience,
                Education = request.Education switch
                {
                    "High School" => "1",
                    "Bachelor" => "2",
                    "Master" => "3",
                    "PhD" => "4",
                    _ => "0"
                },
                //Skills = request.Skills, // Assuming Skills is already encoded or not required for encoding

                Skills = request.Skills switch
                {
                    "Frontend" => "1",
                    "Backend" => "2",
                    "Fullstack" => "3",
                    "Mobile"=>"4",
                    "Machine Learning"=>"5",
                    "DevOps"=>"6",
                    "Cloud"=>"7",
                    _=>"0"
                },

                Gender = request.Gender switch
                {
                    "Male" => "1",
                    "Female" => "0",
                    "Other" => "2",
                    _ => "0"
                },
                EmployementType = request.EmployementType switch
                {
                    "Full-time" => "1",
                    "Part-time" => "2",
                    "Contract" => "3",
                    "Freelance" => "4",
                    _ => "0"
                },
                Country = request.Country // Assuming no encoding is needed for Country
            };
        }
        public class SalaryPredictionRequest
        {         
            public int Age { get; set; }           
            public int Experience { get; set; }          
            public string? Education { get; set; }           
            public string? Skills { get; set; }   
            public string? Gender { get; set; }
            public string? EmployementType { get; set; }
            public string? Country { get; set; }
        }
        //private async Task<decimal> PredictSalaryAsync(Employe employe)
        //{
        //    try
        //    {
        //        var mlInput = new MLModel3.ModelInput()
        //        {
        //            Age = (float)employe.Age,
        //            Experience = (float)employe.Experience,
        //            Education = employe.Education,
        //            Skills = employe.Skills,
        //            Gender = employe.Gender,
        //            EmployementType = employe.EmployementType,
        //            Country = employe.Country
        //        };

        //        var prediction = MLModel3.Predict(mlInput);
        //        return (decimal)prediction.Score;
        //    }
        //    catch
        //    {
        //        // Fallback to average salary calculation if prediction fails
        //        return await _context.Employee
        //            .Where(e => e.Country == employe.Country &&
        //                      e.Education == employe.Education)
        //            .AverageAsync(e => e.Salary);
        //    }
        //}
        [Authorize]
        [HttpGet]
        public IActionResult Predict()
        {
            var countries = Country.List.Select(c => new { c.Name }).ToList();
            ViewBag.Countries = new SelectList(countries, "Name", "Name");

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Predict([Bind("Id,Age,Experience,Education,Skills,EmployementType,Salary,Gender,PhoneNumber,Country")] Predict employe)
        
        {
            if (ModelState.IsValid)
            {
                
                var encodedEmploye = EncodeUserInput(employe);

                
                var predictedSalary = PredictSalary(encodedEmploye);
               
                ViewBag.PredictionResult = predictedSalary;
                return View("PredictResult", employe);
            }
            
            return View(employe);
        }
        private Predict EncodeUserInput(Predict employe)
        {
            return new Predict
            {
                Id = employe.Id,
                Age = employe.Age,
                Experience = employe.Experience,
                Education = employe.Education switch
                {
                    "High School" => "1",
                    "Bachelor" => "2",
                    "Master" => "3",
                    "PhD" => "4",
                    _ => "0"
                },
                //Skills = employe.Skills,

                Skills = employe.Skills switch
                {
                    "Frontend" => "1",
                    "Backend" => "2",
                    "Fullstack" => "3",
                    "Mobile" => "4",
                    "Machine Learning" => "5",
                    "DevOps" => "6",
                    "Cloud" => "7",
                    _ => "0"
                },

                Gender = employe.Gender switch
                {
                    "Male" => "1",
                    "Female" => "0",
                    "Other" => "2",
                    _ => "0"
                },
                EmployementType = employe.EmployementType switch
                {
                    "Full-time" => "1",
                    "Part-time" => "2",
                    "Contract" => "3",
                    "Freelance" => "4",
                    _ => "0"
                },
                Country = employe.Country, // No encoding needed if already preprocessed
                Salary = employe.Salary // Keep as is
            };
        }


        private decimal PredictSalary(Predict employe)
        {
            try
            {
                var mlInput = new MLModel3.ModelInput()
                {
                    Age = employe.Age,
                    Experience =employe.Experience,
                    Education = float.Parse(employe.Education),
                    Skills = float.Parse(employe.Skills),
                    //Gender = float.Parse(employe.Gender),
                    EmployementType =float.Parse(employe.EmployementType),
                    Country = employe.Country
                };

                var prediction = MLModel3.Predict(mlInput);
                return (decimal)prediction.Score;
            }
            catch
            {
                // Fallback to simple calculation if prediction fails
                return (employe.Experience * 5000) + 30000; // Example formula
            }
        }









        [Authorize]
        [HttpGet]
        public IActionResult PredictResult(Predict employe)
        {
            if (employe == null || TempData["PredictionResult"] == null)
            {
                return RedirectToAction("Predict");
            }

            // Retrieve the prediction result from TempData
            ViewBag.PredictionResult = TempData["PredictionResult"];

            // Ensure TempData persists for the next request if needed
            TempData.Keep("PredictionResult");

            return View(employe);
        }


        [Authorize]

        // GET: Employes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employe = await _context.Employee.FindAsync(id);
            if (employe == null)
            {
                return NotFound();
            }
            var countries = Country.List.Select(c => new { c.Name }).ToList();
            ViewBag.Countries = new SelectList(countries, "Name", "Name");

            var users = _context.Users
                   .Where(u => u.Role == "User")
                   .Select(u => new {
                       u.Id,
                       DisplayName = $"{u.UserName} ({u.Email})"
                   })
                   .ToList();

            ViewBag.Users = new SelectList(users, "Id", "DisplayName");

            //var countries = Country.List.Select(c => new { c.Name, c.TwoLetterCode }).ToList();
            //ViewBag.Countries = countries;
            //var countries = Country.List.Select(c => new { c.Name, c.TwoLetterCode }).ToList();
            //ViewBag.Countries = new SelectList(countries, "TwoLetterCode", "Name");

            return View(employe);
        }

        // POST: Employes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Age,Experience,DateOfRegistration,Education,Skills,EmployementType,Salary,Currency,Gender,PhoneNumber,Country,ManagerId")] Employe employe)
        {
            if (id != employe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeExists(employe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employe);
        }

        // GET: Employes/Delete/5

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employe = await _context.Employee
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employe == null)
            {
                return NotFound();
            }

            return View(employe);
        }
        [Authorize]
        // POST: Employes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employe = await _context.Employee.FindAsync(id);
            if (employe != null)
            {
                _context.Employee.Remove(employe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
    }
}
