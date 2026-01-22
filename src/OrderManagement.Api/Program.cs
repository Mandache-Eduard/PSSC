using Microsoft.EntityFrameworkCore;
using OrderManagement.Data;
using OrderManagement.Data.Repositories;
using OrderManagement.Domain.Repositories;
using OrderManagement.Domain.Workflows;

namespace OrderManagement.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Database context
            builder.Services.AddDbContext<OrderManagementContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
                    ?? @"Data Source=C:\Users\manda\OneDrive\Desktop\PSSC\Data\OrderManagement.db"));

            // Repository registration
            builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
            builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();

            // Workflow registration
            builder.Services.AddTransient<PlaceOrderWorkflow>();
            builder.Services.AddTransient<ModifyOrderWorkflow>();
            builder.Services.AddTransient<CancelOrderWorkflow>();
            builder.Services.AddTransient<ReturnOrderWorkflow>();

            // HTTP Client for potential external API calls
            builder.Services.AddHttpClient();

            // Controllers
            builder.Services.AddControllers();

            // Swagger/OpenAPI configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management API v1");
                    c.RoutePrefix = string.Empty; // Serve Swagger UI at root (http://localhost:PORT/)
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
