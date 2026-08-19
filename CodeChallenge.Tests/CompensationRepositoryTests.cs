using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Repositories;
using CodeChallenge.Data;
using CodeChallenge.Models;

namespace CodeChallenge.Tests.Integration
{
    // Unit test generated via Copilot, which I verified before committing.
    [TestClass]
    public class CompensationRepositoryTests
    {
        private ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [TestMethod]
        public void Add_GetById_GetByEmployeeId_Remove_Works()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = CreateContext(dbName))
            {
                var loggerFactory = LoggerFactory.Create(b => { });
                var logger = loggerFactory.CreateLogger<ICompensationRepository>();
                var repo = new CompensationRespository(logger, context);

                var comp = new Compensation { EmployeeId = "emp-x", Salary = 123m, EffectiveDate = DateTime.UtcNow };

                // Act
                var added = repo.Add(comp);
                var beforeSave = repo.GetById(added.CompensationId);
                repo.SaveAsync().Wait();
                var byId = repo.GetById(added.CompensationId);
                var byEmployee = repo.GetByEmployeeId("emp-x");
                var removed = repo.Remove(byId);
                repo.SaveAsync().Wait();
                var afterRemove = repo.GetById(added.CompensationId);

                // Assert
                Assert.IsNotNull(added.CompensationId);
                Assert.IsNull(beforeSave);
                Assert.IsNotNull(byId);
                Assert.IsNotNull(byEmployee);
                Assert.AreEqual(removed.CompensationId, byId.CompensationId);
                Assert.IsNull(afterRemove);
            }
        }
    }
}
