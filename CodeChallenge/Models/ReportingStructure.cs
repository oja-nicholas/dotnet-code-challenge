namespace CodeChallenge.Models
{
    // Since ReportingStructure is calculated on demand, the class is immutable
    public class ReportingStructure
    {
        public Employee Employee { get; }
        public int NumberOfReports { get; }

        public ReportingStructure(Employee employee, int numberOfReports)
        {
            Employee = employee;
            NumberOfReports = numberOfReports;
        }
    }
}
