using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    // Provide the controller the ability to work with compensation. The prompt did not include the ability to replace, but it isn't much more work to support it.
    public interface ICompensationService
    {
        // The Compensation ID isn't required when pulling up the data by employee ID.
        Compensation GetByEmployeeId(String employeeId);
        Compensation Create(Compensation compensation);
        Compensation Replace(Compensation originalCompensation, Compensation newCompensation);
    }
}
