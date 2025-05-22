using Verx.Consolidated.Infra;
using Verx.Consolidated.Infra.IoC;
using Verx.Consolidated.Common.Logging;
using Verx.Consolidated.Common.Tracing;
using Verx.Consolidated.Onboarding.Command.Api.Middware;

var builder = WebApplication.CreateBuilder(args);

var environmentName = builder.Environment.EnvironmentName;

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddBasicTracing(builder.Configuration);
builder.Services.SetupApplication(builder.Configuration);

builder.Logging.ConfigureEnterpriceLog(builder.Configuration, "ApplicationName");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.Run();