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
    public class CompensationRespository : ICompensationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRespository(ILogger<ICompensationRepository> logger, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Compensation Add(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();
            _dbContext.Compensations.Add(compensation);
            return compensation;
        }

        public Compensation GetById(string id)
        {
            return _dbContext.Compensations
                .SingleOrDefault(c => c.CompensationId == id);
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            return _dbContext.Compensations
                .SingleOrDefault(c => c.EmployeeId == employeeId);
        }

        public Task SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public Compensation Remove(Compensation compensation)
        {
            return _dbContext.Remove(compensation).Entity;
        }
    }
}
