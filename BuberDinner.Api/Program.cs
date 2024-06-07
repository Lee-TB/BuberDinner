using BuberDinner.Application;
using BuberDinner.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services
        .AddApplication()
        .AddInfrastructure();
}

var app = builder.Build();
{
    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
}


