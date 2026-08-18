using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _dbContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            // Updating the call for retrieving employee by ID to include direct reports
            return _dbContext.Employees
                .Include(e => e.DirectReports)
                .SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _dbContext.Remove(employee).Entity;
        }
    }
}
