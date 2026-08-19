using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, IEmployeeRepository employeeRepository, ICompensationRepository compensationRepository)
        {
            _employeeRepository = employeeRepository;
            _compensationRepository = compensationRepository;
            _logger = logger;
        }

        public Compensation Create(Compensation compensation)
        {
            // Only create the compensation and employee ID are not null.
            if (compensation == null || string.IsNullOrEmpty(compensation.EmployeeId))
            {
                return null;
            }

            // Check if the employee exists before creating the compensation
            var employee = _employeeRepository.GetById(compensation.EmployeeId);
            // If employee is null, return null to indicate that the compensation cannot be created for a non-existent employee
            if (employee == null)
            {
                return null;
            }

            // Check if existing compensation exists. If it does, remove it and wait for the removal to be complete.
            var existingCompensation = _compensationRepository.GetByEmployeeId(compensation.EmployeeId);
            if (existingCompensation != null)
            {
                _compensationRepository.Remove(existingCompensation);
                _compensationRepository.SaveAsync().Wait();
            }

            _compensationRepository.Add(compensation);
            _compensationRepository.SaveAsync().Wait();

            return compensation;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            // Straight-forward method to retrieve the compensation by employee ID if it exists
            if(!String.IsNullOrEmpty(employeeId))
            {
                return _compensationRepository.GetByEmployeeId(employeeId);
            }

            return null;
        }
    }
}
