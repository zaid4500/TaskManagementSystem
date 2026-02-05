using ManagementBE.Kernel.Core.Configurations;
using ManagementBE.Kernel.Host.Base;
using ManagementBE.Kernel.Host.Base.Middlewares;
using ManagementBE.Kernel.Infrastructure.Persistence.Initialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var config = builder.ConfigureServices<Configuration>();

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = long.MaxValue;
});
builder.Services.AddHttpClient("Waqtak_KeygenClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44339/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(48));
var app = builder.Build();

await app.Services.InitializeDatabasesAsync();


app.UseInfrastructure(config);

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Xss-Protection", "1");
    await next();
});

app.MapEndpoints();
app.Run();


// setup app's root folders
AppDomain.CurrentDomain.SetData("ContentRootPath", app.Environment.ContentRootPath);
AppDomain.CurrentDomain.SetData("WebRootPath", app.Environment.WebRootPath);
