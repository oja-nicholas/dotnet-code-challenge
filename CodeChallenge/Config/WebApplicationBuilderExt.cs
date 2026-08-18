using CodeChallenge.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodeChallenge.Config
{
    public static class WebApplicationBuilderExt
    {
        public static void UseApplicationDb(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(App.DB_NAME);
            });
        }
    }
}
