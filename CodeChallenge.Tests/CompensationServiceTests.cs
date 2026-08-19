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

    // Unit test generated via Copilot, which I verified before committing.
    [TestClass]
    public class CompensationServiceTests
    {
        private CompensationService _service;
        private FakeCompensationRepository _repo;

        [TestInitialize]
        public void Setup()
        {
            _repo = new FakeCompensationRepository();
            var loggerFactory = LoggerFactory.Create(builder => { });
            var logger = loggerFactory.CreateLogger<CompensationService>();
            _service = new CompensationService(logger, _repo);
        }

        [TestMethod]
        public void Create_AddsAndSaves()
        {
            // Arrange
            var comp = new Compensation { EmployeeId = "e1", Salary = 100m, EffectiveDate = DateTime.UtcNow };

            // Act
            var result = _service.Create(comp);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(comp, result);
            Assert.AreEqual(1, _repo.Store.Count);
            Assert.AreEqual(1, _repo.SaveAsyncCount);
        }

        [TestMethod]
        public void GetByEmployeeId_ReturnsNullForEmptyId()
        {
            // Arrange
            var emptyId = string.Empty;

            // Act
            var result = _service.GetByEmployeeId(emptyId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByEmployeeId_ReturnsValueFromRepo()
        {
            // Arrange
            var comp = new Compensation { CompensationId = "c1", EmployeeId = "e2", Salary = 5m, EffectiveDate = DateTime.UtcNow };
            _repo.Store.Add(comp);

            // Act
            var result = _service.GetByEmployeeId("e2");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(comp, result);
        }

        [TestMethod]
        public void Replace_WhenOriginalIsNull_ReturnsNullAndDoesNotCallRepo()
        {
            // Arrange
            var newComp = new Compensation { CompensationId = "n1", EmployeeId = "e3", Salary = 1m, EffectiveDate = DateTime.UtcNow };

            // Act
            var result = _service.Replace(null, newComp);

            // Assert
            Assert.IsNull(result);
            Assert.AreEqual(0, _repo.SaveAsyncCount);
            Assert.AreEqual(0, _repo.Store.Count);
        }

        [TestMethod]
        public void Replace_WhenOriginalExists_RemovesAndAddsAndSaves()
        {
            // Arrange
            var original = new Compensation { CompensationId = "orig", EmployeeId = "e4", Salary = 10m, EffectiveDate = DateTime.UtcNow };
            var replacement = new Compensation { CompensationId = "new", EmployeeId = "e4", Salary = 20m, EffectiveDate = DateTime.UtcNow };

            _repo.Store.Add(original);

            // Act
            var result = _service.Replace(original, replacement);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(original.CompensationId, replacement.CompensationId);
            Assert.AreEqual(original.EmployeeId, replacement.EmployeeId);
            // SaveAsync should be called twice per implementation
            Assert.AreEqual(2, _repo.SaveAsyncCount);
            Assert.IsFalse(_repo.Store.Contains(original));
            Assert.IsTrue(_repo.Store.Contains(replacement));
        }

        [TestMethod]
        public void Replace_WhenOriginalExistsAndNewCompIsNull_RemovesOriginalAndReturnsNull()
        {
            // Arrange
            var original = new Compensation { CompensationId = "orig-null-new", EmployeeId = "e5", Salary = 15m, EffectiveDate = DateTime.UtcNow };
            _repo.Store.Add(original);

            // Act
            var result = _service.Replace(original, null);

            // Assert
            Assert.IsNull(result);
            // original should be removed from the repository store
            Assert.IsFalse(_repo.Store.Contains(original));
            // SaveAsync should be called once by implementation (final save)
            Assert.AreEqual(1, _repo.SaveAsyncCount);
        }
    }
}
