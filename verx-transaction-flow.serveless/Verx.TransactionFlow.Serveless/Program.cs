namespace Verx.TransactionFlow.Serveless;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        builder.Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var environmentName = hostingContext.HostingEnvironment.EnvironmentName;

                config
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}
