using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Flight_Reservation_System.Data;

namespace Flight_Reservation_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers and views
            builder.Services.AddControllersWithViews();

            // Swagger setup
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Setup DbContext for Identity
            builder.Services.AddDbContext<IDataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseCon")));

            // Add Identity API endpoints
            builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<IDataContext>()
                .AddApiEndpoints();

            // Configure Authorization
            builder.Services.AddAuthorization();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
            });
 
            var app = builder.Build();

            // Map Identity API endpoints
            app.MapIdentityApi<IdentityUser>();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Enable Swagger middleware
            app.UseSwagger();

            // Enable Swagger UI (interactive documentation)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;  // To access Swagger UI at the root URL
            });


            // Middleware setup
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            // Map static assets and controller routes
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
