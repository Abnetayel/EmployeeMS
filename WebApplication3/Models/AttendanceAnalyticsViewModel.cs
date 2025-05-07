namespace WebApplication3.Models
{
    using System.Collections.Generic;

    public class AttendanceAnalyticsViewModel
    {
        public List<HeatmapData>? HeatmapData { get; set; }
        public List<FrequentAbsentee>? FrequentAbsentees { get; set; }
    }

    public class HeatmapData
    {
        public string? EmployeeName { get; set; }
        public List<MonthlyAttendance>? MonthlyAttendance { get; set; }
    }

    public class MonthlyAttendance
    {
        public int Month { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
    }

    public class FrequentAbsentee
    {
        public string? EmployeeName { get; set; }
        public int AbsentDays { get; set; }
    }
}
