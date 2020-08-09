﻿using System.IO;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            _logger = provider.GetService<ILogger<Program>>();
            _logger.LogInformation("Application bootstrapping complete.");
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
            services.AddSingleton<CommandLineOptions>(options);
            services.AddSingleton<GedcomDatabase>(p=>
            {
                CommandLineOptions opts = p.GetRequiredService<CommandLineOptions>();
                return GetDatabase(opts.SourceFile);
            });
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
        
        private static GedcomDatabase GetDatabase(string optionsSourceFile)
        {
            GedcomSettings.LineLength = LineLengthSettings.ValueUpTo255;
            using FileStream gedcomFileStream = new FileStream(optionsSourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            GedcomDatabase result = new GedcomDatabase();
            result.Populate(gedcomFileStream);
            return result;
        }
    }
}