using System;

using CodeChallenge.Data;
using CodeChallenge.Repositories;
using CodeChallenge.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeChallenge.Config
{
    public class App
    {
        // Updating database name to be a constant on App to be used elsewhere
        public static readonly string DB_NAME = "CodeChallengeDB";

        public WebApplication Configure(string[] args)
        {
            args ??= Array.Empty<string>();

            var builder = WebApplication.CreateBuilder(args);

            builder.UseApplicationDb();
            
            AddServices(builder.Services);

            var app = builder.Build();

            var env = builder.Environment;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                SeedApplicationDB();
            }

            app.UseAuthorization();

            app.MapControllers();

            return app;
        }

        private void AddServices(IServiceCollection services)
        {

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IEmployeeRepository, EmployeeRespository>();

            services.AddControllers();
        }

        private void SeedApplicationDB()
        {
            new EmployeeDataSeeder(
                new ApplicationDbContext(
                    new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(DB_NAME).Options
            )).Seed().Wait();
        }
    }
}
