namespace WebApplication3.Models
{
    public class EmployeePortalViewModel
    {
        public Employe Employee { get; set; }
        public AttendanceSummary AttendanceSummary { get; set; }
        public List<OnboardingTask>? OnboardingTasks { get; set; }
    }

    public class AttendanceSummary
    {
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
    }
}
