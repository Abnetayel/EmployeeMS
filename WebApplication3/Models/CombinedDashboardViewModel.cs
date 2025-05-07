namespace WebApplication3.Models
{
    public class GenderDistribution1
    {
        public string Gender { get; set; }
        public int Count { get; set; }
    }

    public class SkillExperienceData
    {
        public string? Skill { get; set; }
        public int AverageExperience { get; set; }
    }
    public class MonthlyAttendanceTrend
    {
        public string Month { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
    }

    public class CombinedDashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public decimal AverageSalary { get; set; }
        public List<GenderDistribution1>? GenderDistribution { get; set; }
        public List<dynamic>? TopCountries { get; set; }
        public List<dynamic>? RecentHires { get; set; }

        public List<SkillExperienceData>? SkillsExperience { get; set; }
        public List<MonthlyAttendanceTrend>? MonthlyAttendanceTrends { get; set; }
    }

}
