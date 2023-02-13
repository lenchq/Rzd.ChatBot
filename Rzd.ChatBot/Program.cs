using Rzd.ChatBot.Extensions;

namespace Rzd.ChatBot;

static class Program
{
    public static void Main(string[] args)
        => CreateHostBuilder(args).Build().Run();

    static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
            {
                var localizationFilePath = "localization.yml";
                var appsettingsFilePath = $"appsettings.{
                    hostingContext.HostingEnvironment
                        .EnvironmentName.Capitalize()
                }.json";

                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    //resolve config files in src directory instead of /bin/debug/...
                    // to support hot reload
                    localizationFilePath = Path.GetFullPath("..\\..\\..\\" + localizationFilePath);
                    appsettingsFilePath = Path.GetFullPath("..\\..\\..\\" + appsettingsFilePath);
                }
                
                configurationBuilder.AddYamlFile(localizationFilePath, false, true);
                configurationBuilder
                    .AddJsonFile(appsettingsFilePath,
                        false, true);
            })
            .ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }
            );
}