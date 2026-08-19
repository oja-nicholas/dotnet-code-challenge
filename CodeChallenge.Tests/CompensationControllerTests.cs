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

        public Compensation Create(Compensation compensation)
        {
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

        [TestMethod]
        public void ReplaceCompensation_ReturnsNotFound_WhenOriginalMissing()
        {
            // Arrange
            var employeeId = "nope";
            var newComp = new Compensation { CompensationId = "new", Salary = 1m, EffectiveDate = DateTime.UtcNow };

            // Act
            var result = _controller.ReplaceCompensation(employeeId, newComp);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void ReplaceCompensation_ReplacesAndReturnsOk_WhenOriginalExists()
        {
            // Arrange
            var employeeId = "emp-9";
            var original = new Compensation { CompensationId = "orig-1", EmployeeId = employeeId, Salary = 10m, EffectiveDate = DateTime.UtcNow };
            _service.StoredCompensation = original;

            var newComp = new Compensation { CompensationId = "new-9", EmployeeId = employeeId, Salary = 20m, EffectiveDate = DateTime.UtcNow.AddDays(1) };
            // Act
            var result = _controller.ReplaceCompensation(employeeId, newComp) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var returned = result.Value as Compensation;
            Assert.IsNotNull(returned);
            Assert.AreEqual(original.CompensationId, returned.CompensationId);
            Assert.AreEqual(original.EmployeeId, returned.EmployeeId);
            Assert.AreEqual(20m, returned.Salary);

            // Verify fake service was updated
            Assert.IsNotNull(_service.StoredCompensation);
            Assert.AreEqual(returned.CompensationId, _service.StoredCompensation.CompensationId);
            Assert.AreEqual(returned.EmployeeId, _service.StoredCompensation.EmployeeId);
        }

        [TestMethod]
        public void ReplaceCompensation_WhenNewCompensationIsNull_RemovesOriginalAndReturnsNull()
        {
            // Arrange
            var employeeId = "emp-null";
            var original = new Compensation { CompensationId = "orig-null", EmployeeId = employeeId, Salary = 50m, EffectiveDate = DateTime.UtcNow };
            _service.StoredCompensation = original;

            // Act
            var result = _controller.ReplaceCompensation(employeeId, null) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);

            // Verify fake service removed the original
            Assert.IsNull(_service.StoredCompensation);
        }
    }
}
