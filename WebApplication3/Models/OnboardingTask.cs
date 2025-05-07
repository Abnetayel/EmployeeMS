namespace WebApplication3.Models;


public class OnboardingTask

{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? TaskName { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime DueDate { get; set; }

    // Navigation property
    public Employe? Employee { get; set; }
}
