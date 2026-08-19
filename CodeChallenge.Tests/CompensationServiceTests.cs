using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Repositories;
using CodeChallenge.Models;

namespace CodeChallenge.Tests.Integration
{
    internal class FakeCompensationRepository : ICompensationRepository
    {
        public System.Collections.Generic.List<Compensation> Store { get; } = new System.Collections.Generic.List<Compensation>();
        public int SaveAsyncCount { get; private set; }

        public Compensation Add(Compensation compensation)
        {
            if (string.IsNullOrEmpty(compensation.CompensationId))
                compensation.CompensationId = Guid.NewGuid().ToString();

            Store.Add(compensation);
            return compensation;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            return Store.Find(c => c.EmployeeId == employeeId);
        }

        public Compensation GetById(string id)
        {
            return Store.Find(c => c.CompensationId == id);
        }

        public Compensation Remove(Compensation Compensation)
        {
            Store.Remove(Compensation);
            return Compensation;
        }

        public System.Threading.Tasks.Task SaveAsync()
        {
            SaveAsyncCount++;
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }

    internal class FakeEmployeeRepository : IEmployeeRepository
    {
        public System.Collections.Generic.List<Employee> Store { get; } = new System.Collections.Generic.List<Employee>();
        public int SaveAsyncCount { get; private set; }

        public Employee Add(Employee employee)
        {
            Store.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return Store.Find(e => e.EmployeeId == id);
        }

        public Employee Remove(Employee employee)
        {
            Store.Remove(employee);
            return employee;
        }

        public System.Threading.Tasks.Task SaveAsync()
        {
            SaveAsyncCount++;
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }

    // Unit test generated via Copilot, which I verified before committing.
    [TestClass]
    public class CompensationServiceTests
    {
        private CompensationService _service;
        private FakeCompensationRepository _repo;
        private FakeEmployeeRepository _employeeRepo;

        [TestInitialize]
        public void Setup()
        {
            _repo = new FakeCompensationRepository();
            _employeeRepo = new FakeEmployeeRepository();
            var loggerFactory = LoggerFactory.Create(builder => { });
            var logger = loggerFactory.CreateLogger<CompensationService>();
            _service = new CompensationService(logger, _employeeRepo, _repo);
        }

        [TestMethod]
        public void Create_AddsAndSaves()
        {
            // Arrange
            var comp = new Compensation { EmployeeId = "e1", Salary = 100m, EffectiveDate = DateTime.UtcNow };
            // Ensure employee exists in the fake employee repo
            _employeeRepo.Store.Add(new Employee { EmployeeId = "e1", FirstName = "Test", LastName = "User", Department = "Dev", Position = "Engineer" });

            // Act
            var result = _service.Create(comp);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(comp, result);
            Assert.AreEqual(1, _repo.Store.Count);
            Assert.AreEqual(1, _repo.SaveAsyncCount);
        }

        [TestMethod]
        public void Create_WhenExistingCompensationExists_RemovesAndAddsAndSavesTwice()
        {
            // Arrange
            var comp = new Compensation { EmployeeId = "dup", Salary = 200m, EffectiveDate = DateTime.UtcNow };
            // existing compensation for same employee
            var existing = new Compensation { CompensationId = "existing-1", EmployeeId = "dup", Salary = 100m, EffectiveDate = DateTime.UtcNow.AddDays(-1) };
            _repo.Store.Add(existing);
            // Ensure employee exists
            _employeeRepo.Store.Add(new Employee { EmployeeId = "dup", FirstName = "D", LastName = "U", Department = "X", Position = "Y" });

            // Act
            var result = _service.Create(comp);

            // Assert
            Assert.IsNotNull(result);
            // existing should be removed and new comp added
            Assert.IsFalse(_repo.Store.Contains(existing));
            Assert.IsTrue(_repo.Store.Contains(comp));
            // SaveAsync should be called twice: once for removal, once for final save
            Assert.AreEqual(2, _repo.SaveAsyncCount);
        }

        [TestMethod]
        public void Create_ReturnsNull_WhenCompensationIsNull()
        {
            // Arrange

            // Act
            var result = _service.Create(null);

            // Assert
            Assert.IsNull(result);
            Assert.AreEqual(0, _repo.SaveAsyncCount);
        }

        [TestMethod]
        public void Create_ReturnsNull_WhenEmployeeIdIsEmpty()
        {
            // Arrange
            var comp = new Compensation { EmployeeId = string.Empty, Salary = 1m, EffectiveDate = DateTime.UtcNow };

            // Act
            var result = _service.Create(comp);

            // Assert
            Assert.IsNull(result);
            Assert.AreEqual(0, _repo.SaveAsyncCount);
        }

        [TestMethod]
        public void Create_ReturnsNull_WhenEmployeeNotFound()
        {
            // Arrange
            var comp = new Compensation { EmployeeId = "missing-emp", Salary = 1m, EffectiveDate = DateTime.UtcNow };

            // Act
            var result = _service.Create(comp);

            // Assert
            Assert.IsNull(result);
            Assert.AreEqual(0, _repo.SaveAsyncCount);
        }
    }
}
