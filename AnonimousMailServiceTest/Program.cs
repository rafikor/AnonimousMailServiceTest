using AnonimousMailServiceTest.Hubs;
using AnonimousMailServiceTest.MiddlewareExtensions;
using AnonimousMailServiceTest.SubscribeTableDependencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AnonimousMailServiceTest.Data;

namespace AnonimousMailServiceTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AnonimousMailServiceTestContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AnonimousMailServiceTestContext") ?? throw new InvalidOperationException("Connection string 'AnonimousMailServiceTestContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<MailHub>();
            builder.Services.AddSingleton<SubscribeMessageTableDependency>();
            builder.Services.AddSingleton<SubscribeUniqueUserTableDependency>();

            var app = builder.Build();
            var connectionString = app.Configuration.GetConnectionString("AnonimousMailServiceTestContext");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<MailHub>("/MailHub");

            app.UseSqlTableDependency<SubscribeMessageTableDependency>(connectionString);
            app.UseSqlTableDependency<SubscribeUniqueUserTableDependency>(connectionString);
            app.Run();
        }
    }
}