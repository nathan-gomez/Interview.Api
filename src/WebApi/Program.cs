using Application;
using Microsoft.OpenApi.Models;
using Infrastructure;
using Serilog;
using WebApi.Controllers;
using WebApi.Middleware;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Interview API"
        });

        // var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

    builder.Services.AddApplication().AddInfrastructure();

    builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddScoped<UsersController>();

    builder.Services.AddSingleton<ValidateApiKey>();
    builder.Services.AddSingleton<ValidateSession>();

    builder.Services.AddHealthChecks();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHealthChecks("/health");
    app.UseMiddleware<ValidateApiKey>();
    app.UseWhen(
        context => !context.Request.Path.Value.StartsWith("/v1/users/login") && !context.Request.Path.Value.StartsWith("/swagger"),
        appBuilder => { appBuilder.UseMiddleware<ValidateSession>(); });

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();

    app.MapControllers();

    app.Logger.LogInformation("API Running");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}