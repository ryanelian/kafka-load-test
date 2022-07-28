using CapMemoryTest.HostedServices;
using Microsoft.EntityFrameworkCore;

namespace CapMemoryTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHostedService<SubscribeKafkaHostedService>();

            builder.Services.AddCap(capOptions =>
            {
                capOptions.UseKafka(builder.Configuration.GetConnectionString("Kafka"));
                capOptions.UseSqlServer(builder.Configuration.GetConnectionString("CAP"));
                capOptions.UseDashboard();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}