using WebApplication3.Data;

namespace WebApplication3.Models
{
    public class Employe
    {
        public int Id { get; set; }
        public required string FullName {  get; set; }
        public required int Age { get; set; }
        public required int Experience { get; set; }
        public required DateTime DateOfRegistration { get; set; }
        public required string Education { get; set; }
        public required string Skills { get; set; }
        public required string Gender { get; set; }
        public required string EmployementType { get; set; }
        public required decimal Salary { get; set; }
        //public required string Currency { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Country { get; set; }


        public int? ManagerId { get; set; }
        public User? Manager { get; set; }
        public ICollection<Attendance>? Attendances { get; set; }

        // New Features for Prediction
        //public double WorkHours { get; set; } // Total hours worked
        //public double TaskCompletionRate { get; set; } // Percentage of tasks completed
        //public double AttendanceRate { get; set; } // Attendance percentage
        //public double PerformanceScore { get; set; } // Performance evaluation score
        //public double OvertimeHours { get; set; } // Overtime hours
        //public int ProjectCount { get; set; } // Number of projects completed
        //public string SkillLevel { get; set; } // Skill proficiency level
        //public double TrainingHours { get; set; } // Hours spent owhat will n training
        //public double FeedbackScore { get; set; } // Average feedback score


    }
}