using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee/{employeeId}/compensation")]
    public class CompensationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        // The route already includes the employee ID
        // When saving compensation, EffectiveDate should be saved with the format: 'YYYY-MM-DDTHH:MI:SS'
        [HttpPost]
        public IActionResult CreateCompensation(String employeeId, [FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create request for employee ID '{compensation.EmployeeId}'");

            // Update EmployeeId on the compensation object to match what was passed into the controller
            compensation.EmployeeId = employeeId;
            _compensationService.Create(compensation);

            // Since we are only supporting returning compensation by employee ID, we need the route to reflect the employee ID
            return CreatedAtRoute("getCompensationByEmployeeId", new { employeeId = compensation.EmployeeId }, compensation);
        }

        // The route already includes the employee ID
        [HttpGet(Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String employeeId)
        {
            _logger.LogDebug($"Received compensation get request for '{employeeId}'");

            var compensation = _compensationService.GetByEmployeeId(employeeId);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }

        // The route already includes the employee ID
        // When saving compensation, EffectiveDate should be saved with the format: 'YYYY-MM-DDTHH:MI:SS'
        [HttpPut]
        public IActionResult ReplaceCompensation(String employeeId, [FromBody] Compensation newCompensation)
        {
            _logger.LogDebug($"Recieved compensation update request for '{employeeId}'");

            var existingCompensation = _compensationService.GetByEmployeeId(employeeId);
            if (existingCompensation == null)
                return NotFound();

            // Update EmployeeId on the new compensation object to match what was passed into the controller
            newCompensation.EmployeeId = employeeId;
            _compensationService.Replace(existingCompensation, newCompensation);

            return Ok(newCompensation);
        }
    }
}
