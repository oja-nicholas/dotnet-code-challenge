using System;

namespace CodeChallenge.Models
{
    // Creating the class to be mutable for persistence with entity framework
    public class Compensation
    {
        public string CompensationId { get; set; }
        public string EmployeeId { get; set; }
        public decimal Salary { get; set; }
        public DateTime EffectiveDate { get; set; }

        public Employee Employee { get; set; }
    }
}
