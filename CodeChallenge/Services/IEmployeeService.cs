using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Employee GetById(String id);
        Employee Create(Employee employee);
        Employee Replace(Employee originalEmployee, Employee newEmployee);

        // Calculating the reporting structure at the service layer for the most flexibility with data access. If I calculated this
        // lower in the stack, I might run into problems if the data model became more complicated in the future.
        ReportingStructure GetReportingStructure(string employeeId);
    }
}
