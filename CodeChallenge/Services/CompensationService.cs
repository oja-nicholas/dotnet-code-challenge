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
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository)
        {
            _compensationRepository = compensationRepository;
            _logger = logger;
        }

        public Compensation Create(Compensation compensation)
        {
            // Straight-forward method to add the compensation and save as long as it is not null.
            if(compensation != null)
            {
                _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
            }

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

        public Compensation Replace(Compensation originalCompensation, Compensation newCompensation)
        {
            // Replace the compensation only when the original exists.
            // If the original compensation is null, return null to indicate nothing was replaced.
            if (originalCompensation == null)
            {
                return null;
            }

            // Remove the original compensation
            _compensationRepository.Remove(originalCompensation);

            if (newCompensation != null)
            {
                // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                _compensationRepository.SaveAsync().Wait();

                _compensationRepository.Add(newCompensation);
                // overwrite the new compensation id with the previous compensation id
                newCompensation.CompensationId = originalCompensation.CompensationId;
                // overwrite the new employee id with previous employee id
                newCompensation.EmployeeId = originalCompensation.EmployeeId;
            }

            _compensationRepository.SaveAsync().Wait();

            return newCompensation;
        }
    }
}
