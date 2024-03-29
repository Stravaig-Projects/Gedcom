﻿using System;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTree.Standardiser
{
    class Program
    {
        private static ILogger<Program> _logger;
        static void Main(string[] args)
        {
            var options = ExtractCommandLineInfo(args);
            if (options == null)
                return;
            
            using var provider = BuildServiceProvider(options);
            _logger = provider.GetRequiredService<ILogger<Program>>();
            _logger.LogInformation("Application bootstrapping complete.");
            provider.GetRequiredService<Application>().Run();
        }
        
        private static CommandLineOptions ExtractCommandLineInfo(string[] args)
        {
            CommandLineOptions options = Parser.Default
                .ParseArguments<CommandLineOptions>(args)
                .MapResult(
                    clo => clo,
                    errors => null);
            return options;
        }
        
        private static ServiceProvider BuildServiceProvider(CommandLineOptions options)
        {
            var services = new ServiceCollection();
            AddLoggingServices(services);
            AddApplicationServices(services, options);
            var provider = services.BuildServiceProvider();
            return provider;
        }

        private static void AddApplicationServices(ServiceCollection services, CommandLineOptions options)
        {
            services.AddSingleton(options);
            services.AddSingleton(p=>
            {
                CommandLineOptions opts = p.GetRequiredService<CommandLineOptions>();
                return GetDatabase(opts.SourceFile);
            });
            services.AddSingleton<FileNamer>();
            services.AddSingleton<Func<DateTimeOffset>>(p => () => DateTimeOffset.Now);
            services.AddSingleton<Application>();
        }

        private static void AddLoggingServices(ServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "HH:mm:ss.fff ";
                    options.ColorBehavior = LoggerColorBehavior.Enabled;
                });
                builder.AddDebug();
            });
        }
        
        private static GedcomDatabase GetDatabase(string optionsSourceFile)
        {
            GedcomSettings.LineLength = LineLengthSettings.Any;
            GedcomDatabase result = new GedcomDatabase();
            result.PopulateFromFile(optionsSourceFile);
            return result;
        }
    }
}