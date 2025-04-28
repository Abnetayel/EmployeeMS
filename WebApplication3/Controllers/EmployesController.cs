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

        [HttpGet]
        public IActionResult CombinedDashboard()
        {
            var totalEmployees = _context.Employee.Count();
            var averageSalary = _context.Employee.Average(e => e.Salary);
            var genderDistribution = _context.Employee
                .GroupBy(e => e.Gender)
                .Select(g => new GenderDistribution1
                {
                    Gender = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var topCountries = _context.Employee
                .GroupBy(e => e.Country)
                .Select(g => new TopCountry
                {
                    Country = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToList();

            var recentHires = _context.Employee
                .OrderByDescending(e => e.DateOfRegistration)
                .Take(5)
                .ToList();

            var viewModel = new CombinedDashboardViewModel
            {
                TotalEmployees = totalEmployees,
                AverageSalary = averageSalary,
                GenderDistribution = genderDistribution,
                TopCountries = topCountries.Cast<dynamic>().ToList(),
                RecentHires = recentHires.Cast<dynamic>().ToList()
            };

            return View(viewModel);
        }

        public class TopCountry
        {
            public string Country { get; set; }
            public int Count { get; set; }
        }







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
                return RedirectToAction(nameof(Index));
            }
            return View(employe);
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
