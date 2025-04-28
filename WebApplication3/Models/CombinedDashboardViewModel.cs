namespace WebApplication3.Models
{
    public class GenderDistribution1
    {
        public string Gender { get; set; }
        public int Count { get; set; }
    }

    public class CombinedDashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public decimal AverageSalary { get; set; }
        public List<GenderDistribution1> GenderDistribution { get; set; }
        public List<dynamic> TopCountries { get; set; }
        public List<dynamic> RecentHires { get; set; }
    }

}
