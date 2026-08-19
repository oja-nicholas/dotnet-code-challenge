using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    // Provide the controller the ability to work with compensation.
    public interface ICompensationService
    {
        // The Compensation ID isn't required when pulling up the data by employee ID.
        Compensation GetByEmployeeId(String employeeId);
        // Create a new compensation record for an employee. If one exists, remove it and replace with the new one.
        Compensation Create(Compensation compensation);
    }
}
