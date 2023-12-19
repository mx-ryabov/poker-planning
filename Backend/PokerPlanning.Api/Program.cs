using PokerPlanning.Api;
using PokerPlanning.Application;
using PokerPlanning.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services
        .AddPresentation()
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    /*builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();*/
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        /*app.UseSwagger();
        app.UseSwaggerUI();*/
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseExceptionHandler("/error");
    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
}

