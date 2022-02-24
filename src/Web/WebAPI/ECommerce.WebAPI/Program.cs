using System;
using System.Reflection;
using ECommerce.Abstractions.Messages;
using ECommerce.JsonConverters;
using ECommerce.WebAPI.DataTransferObjects.ShoppingCarts;
using ECommerce.WebAPI.DependencyInjection.Extensions;
using ECommerce.WebAPI.DependencyInjection.Options;
using ECommerce.WebAPI.DependencyInjection.ParameterTransformers;
using FluentValidation.AspNetCore;
using MassTransit;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    options.ValidateOnBuild = true;
});

builder.Host.ConfigureAppConfiguration(configurationBuilder =>
{
    configurationBuilder
        .AddUserSecrets(Assembly.GetExecutingAssembly())
        .AddEnvironmentVariables();
});

builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log.Logger = new LoggerConfiguration().ReadFrom
        .Configuration(context.Configuration)
        .CreateLogger();

    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

builder.Services
    .AddCors(options
        => options.AddPolicy(
            name: "cors",
            configurePolicy: policyBuilder =>
            {
                policyBuilder.AllowAnyHeader();
                policyBuilder.AllowAnyMethod();
                policyBuilder.AllowAnyOrigin();
            }));

builder.Services
    .AddRouting(options
        => options.LowercaseUrls = true)
    .AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        options.SuppressAsyncSuffixInActionNames = true;
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new DateOnlyJsonConverter());
        options.SerializerSettings.Converters.Add(new ExpirationDateOnlyJsonConverter());
    })
    .AddFluentValidation(cfg =>
    {
        cfg.RegisterValidatorsFromAssemblyContaining(typeof(IMessage));
        cfg.RegisterValidatorsFromAssemblyContaining(typeof(Requests));
    });

builder.Services
    .AddSwaggerGenNewtonsoftSupport()
    .AddFluentValidationRulesToSwagger()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new() { Title = "WebAPI", Version = "v1" });
        options.MapType<DateOnly>(() => new() { Format = "date", Example = new OpenApiString(DateOnly.MinValue.ToString()) });
        options.CustomSchemaIds(type => type.ToString());
    });

builder.Services.AddMassTransitWithRabbitMq();

builder.Services.AddAutoMapper();

builder.Services.ConfigureRabbitMqOptions(
    builder.Configuration.GetSection(nameof(RabbitMqOptions)));

builder.Services.ConfigureMassTransitHostOptions(
    builder.Configuration.GetSection(nameof(MassTransitHostOptions)));

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce.WebAPI v1"));
}

app.UseCors("cors");

app.UseRouting();

app.UseEndpoints(endpoints
    => endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller:slugify}/{action:slugify}"));

try
{
    await app.RunAsync();
    Log.Information("Stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    await app.StopAsync();
}
finally
{
    Log.CloseAndFlush();
    await app.DisposeAsync();
}

namespace ECommerce.WebAPI
{
    public partial class Program { }
}