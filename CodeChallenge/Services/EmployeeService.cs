using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        // Calculating the reporting structure at the service layer for the most flexibility with data access. If I calculated this
        // lower in the stack, I might run into problems if the data model became more complicated in the future.
        public ReportingStructure GetReportingStructure(string employeeId)
        {
            var employee = _employeeRepository.GetById(employeeId);
            // If the employee isn't found, return null so the controller can return the not found message.
            if (employee == null)
            {
                return null;
            }

            // Initialize the HashSet to track visited IDs and start the recursion
            var visitedIds = new HashSet<string>();
            int numberOfReports = CalculateNumberOfReports(employee, visitedIds);

            return new ReportingStructure(employee, numberOfReports);
        }

        private int CalculateNumberOfReports(Employee employee, HashSet<string> visitedIds)
        {
            // If the employee is null, the number of reports is 0.
            // If the ID has already been visited, that means there is a recursive loop in the data, and we should return 0.
            if (employee == null || visitedIds.Contains(employee.EmployeeId))
            {
                return 0;
            }

            // The employee ID has been counted during the recursive calculation.
            visitedIds.Add(employee.EmployeeId);

            // If there are no direct reports, the calculation should return 0.
            if (employee.DirectReports == null || !employee.DirectReports.Any())
            {
                return 0;
            }

            int count = 0;

            // Calculate the number of reports exist within each direct report.
            foreach (var report in employee.DirectReports)
            {
                // We don't want to count the same employee multiple times.
                if (!visitedIds.Contains(report.EmployeeId))
                {
                    // Count the immediate direct report
                    count++;

                    // Fetch the full report details from the repository to get their nested reports
                    var fullReport = _employeeRepository.GetById(report.EmployeeId);

                    // Recursively count the nested reports, passing down the visited state
                    count += CalculateNumberOfReports(fullReport, visitedIds);
                }
            }

            return count;
        }
    }
}
