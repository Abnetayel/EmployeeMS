using ISO3166;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class EmployesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employee.ToListAsync());
        }
        //GET: Employes
        public async Task<IActionResult> List()
        {
            return View(await _context.Employee.ToListAsync());
        }
        //GET: Employes/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Employes/Create
        public IActionResult Create()
        {
            var countries = Country.List.Select(c => new { c.Name }).ToList();
            ViewBag.Countries = new SelectList(countries, "Name", "Name");

          

            return View();
        }

        //POST: Employes/Create
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Age,Experience,DateOfRegistration,Education,Skills,EmployementType,Salary,Currency,Gender,PhoneNumber,Country")] Employe employe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employe);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,FullName,Age,Experience,DateOfRegistration,Education,Skills,EmployementType,Salary,Currency,Gender,PhoneNumber,Country")] Employe employe)
        //{
        //    if (ModelState.IsValid)
        //    {
        //         If salary wasn't set, predict it
        //        if (employe.Salary == 0)
        //        {
        //            employe.Salary = await PredictSalaryAsync(employe);
        //            TempData["PredictionInfo"] = $"System predicted salary: {employe.Salary:C}";
        //        }

        //        _context.Add(employe);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //     Reload form data if validation fails
        //    var countries = Country.List.Select(c => new { c.Name }).ToList();
        //    ViewBag.Countries = new SelectList(countries, "Name", "Name");
        //    return View(employe);
        //}

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

                // Try ML Prediction
                try
                {
                    var mlInput = new MLModel3.ModelInput()
                    {
                        Age = (float)request.Age,
                        Experience = (float)request.Experience,
                        Education = request.Education,
                        Skills = request.Skills,
                        Gender = request.Gender,
                        EmployementType = request.EmployementType,
                        Country = request.Country
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

        public class SalaryPredictionRequest
        {         
            public int Age { get; set; }           
            public int Experience { get; set; }          
            public string Education { get; set; }           
            public string Skills { get; set; }   
            public string Gender { get; set; }          
            public string EmployementType { get; set; }
            public string Country { get; set; }
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

        [HttpGet]
        public IActionResult Predict()
        {
            var countries = Country.List.Select(c => new { c.Name }).ToList();
            ViewBag.Countries = new SelectList(countries, "Name", "Name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Predict([Bind("Id,Age,Experience,Education,Skills,EmployementType,Salary,Gender,PhoneNumber,Country")] Predict employe)
        //public IActionResult Predict(Employe employe)
        {
            if (ModelState.IsValid)
            {
                // Generate salary prediction
                var predictedSalary = PredictSalary(employe);

                // Pass results to the view
                ViewBag.PredictionResult = predictedSalary;
                return View("PredictResult", employe); // New view for displaying results
            }

            // If validation fails, return to input form
            return View(employe);
        }
        private decimal PredictSalary(Predict employe)
        {
            try
            {
                var mlInput = new MLModel3.ModelInput()
                {
                    Age = (float)employe.Age,
                    Experience = (float)employe.Experience,
                    Education = employe.Education,
                    Skills = employe.Skills,
                    Gender = employe.Gender,
                    EmployementType = employe.EmployementType,
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

            //var countries = Country.List.Select(c => new { c.Name, c.TwoLetterCode }).ToList();
            //ViewBag.Countries = countries;
            //var countries = Country.List.Select(c => new { c.Name, c.TwoLetterCode }).ToList();
            //ViewBag.Countries = new SelectList(countries, "TwoLetterCode", "Name");

            return View(employe);
        }

        // POST: Employes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Age,Experience,DateOfRegistration,Education,Skills,EmployementType,Salary,Currency,Gender,PhoneNumber,Country")] Employe employe)
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
