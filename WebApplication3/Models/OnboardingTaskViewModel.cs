namespace WebApplication3.Models;

public class OnboardingTaskViewModel
{
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public List<OnboardingTask>? Tasks { get; set; }
}
