using Rzd.ChatBot.Extensions;
using Serilog;

namespace Rzd.ChatBot;

static class Program
{
    public static void Main(string[] args)
        => CreateHostBuilder(args).Build().Run();

    static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
            {
                var isDev = hostingContext.HostingEnvironment.IsDevelopment();
                
                var sep = Path.DirectorySeparatorChar;

                var localizationFilePath = $"Resources{sep}localization.yml";
                var picsFilePath = $"Resources{sep}pics.yml";
                var appsettingsFilePath = $"appsettings.{ hostingContext.HostingEnvironment.EnvironmentName.Capitalize() }.json";

                if (isDev)
                {
                    // resolve config files in src directory instead of /bin/debug/...
                    // to support hot reload
                    localizationFilePath = Path.GetFullPath("..\\..\\..\\" + localizationFilePath);
                    picsFilePath = Path.GetFullPath("..\\..\\..\\" + picsFilePath);
                    appsettingsFilePath = Path.GetFullPath("..\\..\\..\\" + appsettingsFilePath);
                }
                
                // Hot reload only makes sense in development environment
                configurationBuilder.AddYamlFile(localizationFilePath, false, isDev);
                configurationBuilder.AddYamlFile(picsFilePath, false, isDev);
                
                configurationBuilder
                    .AddJsonFile(appsettingsFilePath,
                        false, isDev);
            })
            .ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }
            );
}