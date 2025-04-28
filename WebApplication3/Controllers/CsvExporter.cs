//using System.IO;
//using System.Linq;
//using WebApplication3.Data;

//namespace WebApplication3.Controllers
//{
//    public class CsvExporter
//    {
//        private ApplicationDbContext context;

//        public CsvExporter(ApplicationDbContext context)
//        {
//            this.context = context;
//        }

//        internal void ExportEmployeesToCsv(string filePath)
//        {
//            var employees = context.Employee.Select(e => new
//            {
//                e.Id,
//                e.FullName,
//                e.Age,
//                e.Experience,
//                e.DateOfRegistration,
//                e.Education,
//                e.Skills,
//                e.Gender,
//                e.EmployementType,
//                e.Salary,
//                e.Currency,
//                e.PhoneNumber,
//                e.Country,
//                e.ManagerId,
//                e.WorkHours,
//                e.TaskCompletionRate,
//                e.AttendanceRate,
//                e.PerformanceScore,
//                e.OvertimeHours,
//                //e.ProjectCount,
//                e.SkillLevel,
//                e.TrainingHours,
//                e.FeedbackScore
//            }).ToList();

//            using (var writer = new StreamWriter(filePath))
//            {
//                // Write the header row
//                writer.WriteLine("Id,FullName,Age,Experience,DateOfRegistration,Education,Skills,Gender,EmployementType,Salary,Currency,PhoneNumber,Country,ManagerId,WorkHours,TaskCompletionRate,AttendanceRate,PerformanceScore,OvertimeHours,ProjectCount,SkillLevel,TrainingHours,FeedbackScore");

//                // Write each employee's data
//                foreach (var employee in employees)
//                {
//                    writer.WriteLine($"{employee.Id},{employee.FullName},{employee.Age},{employee.Experience},{employee.DateOfRegistration:yyyy-MM-dd},{employee.Education},{employee.Skills},{employee.Gender},{employee.EmployementType},{employee.Salary},{employee.Currency},{employee.PhoneNumber},{employee.Country},{employee.ManagerId},{employee.WorkHours},{employee.TaskCompletionRate},{employee.AttendanceRate},{employee.PerformanceScore},{employee.OvertimeHours},{employee.ProjectCount},{employee.SkillLevel},{employee.TrainingHours},{employee.FeedbackScore}");
//                }
//            }
//        }
//    }
//}
