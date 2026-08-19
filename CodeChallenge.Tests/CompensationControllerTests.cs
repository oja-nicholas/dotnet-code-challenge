using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using CodeChallenge.Controllers;
using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeChallenge.Tests.Integration
{
    // Simple in-memory fake for ICompensationService to verify controller behavior
    public class FakeCompensationService : ICompensationService
    {
        public Compensation StoredCompensation { get; set; }
        public Compensation ReplacedWith { get; set; }
        public bool ReturnNullOnCreate { get; set; } = false;

        public Compensation Create(Compensation compensation)
        {
            if (ReturnNullOnCreate)
            {
                return null;
            }

            StoredCompensation = compensation;
            return StoredCompensation;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            if (StoredCompensation != null && StoredCompensation.EmployeeId == employeeId)
                return StoredCompensation;

            return null;
        }

        public Compensation Replace(Compensation originalCompensation, Compensation newCompensation)
        {
            if (originalCompensation == null)
                return null;

            if (newCompensation == null)
            {
                // emulate service behavior: remove the original
                StoredCompensation = null;
                return null;
            }

            ReplacedWith = newCompensation;
            ReplacedWith.CompensationId = originalCompensation.CompensationId;
            ReplacedWith.EmployeeId = originalCompensation.EmployeeId;

            StoredCompensation = ReplacedWith;
            return ReplacedWith;
        }
    }

    // Unit test generated via Copilot, which I verified before committing.
    [TestClass]
    public class CompensationControllerTests
    {
        private CompensationController _controller;
        private FakeCompensationService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new FakeCompensationService();
            var loggerFactory = LoggerFactory.Create(builder => { });
            var logger = loggerFactory.CreateLogger<CompensationController>();
            _controller = new CompensationController(logger, _service);
        }

        [TestMethod]
        public void CreateCompensation_SetsEmployeeId_ReturnsCreated()
        {
            // Arrange
            var routeEmployeeId = "emp-123";
            var compensation = new Compensation
            {
                CompensationId = "c1",
                EmployeeId = "different",
                Salary = 100m,
                EffectiveDate = new DateTime(2025, 1, 1)
            };
            // Act
            var result = _controller.CreateCompensation(routeEmployeeId, compensation) as CreatedAtRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("getCompensationByEmployeeId", result.RouteName);
            Assert.AreEqual(routeEmployeeId, ((System.Collections.Generic.IDictionary<string, object>)result.RouteValues)["employeeId"]);
            var returned = result.Value as Compensation;
            Assert.IsNotNull(returned);
            Assert.AreEqual(routeEmployeeId, returned.EmployeeId);
            Assert.AreEqual(100m, returned.Salary);

            // Verify fake service was updated
            Assert.AreEqual(compensation, _service.StoredCompensation);
        }

        [TestMethod]
        public void CreateCompensation_ServiceReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var routeEmployeeId = "emp-null-create";
            var compensation = new Compensation
            {
                CompensationId = "c-null",
                EmployeeId = "different",
                Salary = 100m,
                EffectiveDate = new DateTime(2025, 1, 1)
            };
            _service.ReturnNullOnCreate = true;

            // Act
            var result = _controller.CreateCompensation(routeEmployeeId, compensation);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            // Ensure fake service was not updated
            Assert.IsNull(_service.StoredCompensation);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_ReturnsOk_WhenFound()
        {
            // Arrange
            var employeeId = "emp-42";
            _service.StoredCompensation = new Compensation
            {
                CompensationId = "c42",
                EmployeeId = employeeId,
                Salary = 250m,
                EffectiveDate = DateTime.UtcNow
            };
            // Act
            var result = _controller.GetCompensationByEmployeeId(employeeId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var returned = result.Value as Compensation;
            Assert.IsNotNull(returned);
            Assert.AreEqual(employeeId, returned.EmployeeId);
            Assert.AreEqual(250m, returned.Salary);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var employeeId = "missing";

            // Act
            var result = _controller.GetCompensationByEmployeeId(employeeId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
