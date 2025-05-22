using Scalar.AspNetCore;
using Verx.Autentication.Service.Api;
using Verx.Authentication.Service.Crosscuting;
using Verx.Autentication.Service.Api.Middleware;
using Verx.Authentication.Service.Common.Logging;
using Verx.Authentication.Service.Infrastructure.Data;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddDefaultLogging();
builder.RegisterDependencies();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.MapV1Endpoints();

if (app.Environment.IsDevelopment())
{
    app.ExecuteMigration();

    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.Run();