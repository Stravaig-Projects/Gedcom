using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Stravaig.FamilyTreeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            AddLoggingServices(services);
            services.AddTransient<Application>();
            using var provider = services.BuildServiceProvider();
            var app = provider.GetRequiredService<Application>();
            app.Run(args);
        }

        private static void AddLoggingServices(ServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole(options =>
                {
                    options.TimestampFormat = "HH:mm:ss.fff ";
                    options.LogToStandardErrorThreshold = LogLevel.Critical;
                });
                builder.AddDebug();
            });
        }
    }
}