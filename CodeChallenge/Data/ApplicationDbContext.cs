using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Data
{
    // I renamed this to Application Db Context to denote that it will be storing more than just Employee going forward.
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Compensation> Compensations { get; set; }

        // Adding a one-to-one relationship with compensation and employee
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the 1-to-1 relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Compensation)      // An Employee has one Compensation
                .WithOne(c => c.Employee)         // A Compensation belongs to one Employee
                .HasForeignKey<Compensation>(c => c.EmployeeId) // Foreign key on Compensation back to Employee
                .IsRequired();                    // A Compensation MUST have an Employee
        }
    }
}
