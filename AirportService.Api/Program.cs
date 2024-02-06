using AirportService.Api.Services;
using AirportService.Application.Interfaces;
using AirportService.Application.Services;
using System.Reflection;
using MediatR;
using FluentValidation;
using System;
using AirportService.Api.Models;

namespace AirportService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);
        //app.UseExceptionHandler();

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddMvc();
        builder.Services.AddControllers();
        builder.Services.AddValidatorsFromAssemblyContaining<DistanceRequestValidator>();

        builder.Services.AddScoped<IAirportDataService, AirportDataService>();
        builder.Services.AddMemoryCache();
        var assemblies = Assembly.Load("AirportService.Application");

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

        var distanceSettings = builder.Configuration.GetSection("DistanceSettings").Get<DistanceSettings>();

        builder.Services.AddHttpClient("AirportClient", client =>
        {
            client.BaseAddress = new Uri(distanceSettings.AirportApiUrl);
        });
        builder.Services.AddSingleton<AirportHttpClient>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(
        //    setupAction =>
        //{
        //    setupAction.SwaggerDoc(
        //        "AirportServiceOpenAPISpecification",
        //            new Microsoft.OpenApi.Models.OpenApiInfo()
        //            {
        //                Title = "Airport Service API",
        //                Version = "v1",
        //                Description = "Through this API you can access airport data and calculate distance between airports.",
        //                Contact = new Microsoft.OpenApi.Models.OpenApiContact()
        //                {
        //                    Name = "Airport Service API",
        //                    Email = "",
        //                },
        //                License = new Microsoft.OpenApi.Models.OpenApiLicense()
        //                {
        //                    Name = "MIT License",
        //                    Url = new Uri("https://opensource.org/licenses/MIT")
        //                },

        //            });
        //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        //    setupAction.IncludeXmlComments(xmlPath);
        //}
        );
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(
                    "/swagger/AirportServiceOpenAPISpecification/swagger.json",
                    "Airport Service API");
                setupAction.RoutePrefix = "";
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}
