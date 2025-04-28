namespace WebApplication3.Models
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public decimal AverageSalary { get; set; }
        public List<GenderDistribution> GenderDistribution { get; set; }
    }

    public class GenderDistribution
    {
        public string Gender { get; set; }
        public int Count { get; set; }
    }
}
