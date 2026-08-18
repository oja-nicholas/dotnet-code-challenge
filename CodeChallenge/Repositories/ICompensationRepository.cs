using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    // Create repository to store and retrieve compensation logic.
    public interface ICompensationRepository
    {
        Compensation GetById(String id);
        // Ability to retrieve the compensation by employee ID.
        Compensation GetByEmployeeId(String employeeId);
        Compensation Add(Compensation compensation);
        Compensation Remove(Compensation Compensation);
        Task SaveAsync();
    }
}