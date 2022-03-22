using System;
using CommandLine;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.Policies.Handlers;
using Polly;
using Stravaig.FamilyTree.Common.Humaniser;
using Stravaig.FamilyTreeGenerator.Requests;
using Stravaig.FamilyTreeGenerator.Requests.Handlers;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator
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
            var commander = provider.GetRequiredService<IAmACommandProcessor>();
            commander.Send(new Application());
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
            services.AddBrighter(opts =>
            {
                var policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetry(new[]
                    {
                        1.Seconds(),
                        2.Seconds(),
                        3.Seconds()
                    }, (exception, timeSpan) =>
                    {
                        _logger.LogWarning(exception, $"Operation failed. Will retry in {timeSpan.Humanize()}.");
                    });

                opts.PolicyRegistry.Add(InitFileSystemForMarkdownHandler.InitFileStstemRetryPolicy, policy);
            }).Handlers(registry =>
            {
                registry.Register<Application, ApplicationHandler>();
                
                registry.Register<InitFileSystem, InitFileSystemForMarkdownHandler>();
                
                registry.Register<RenderIndividual, RenderIndividualAsMarkdownHandler>();
                registry.Register<RenderIndividual, RenderPersonAncestorsAsJsonHandler>();
                registry.Register<RenderIndividual, RenderPersonDescendantsAsJsonHandler>();
                registry.Register<RenderIndividual, RenderPersonFamilyTreeAsJsonHandler>();
                
                registry.Register<RenderPersonIndex, RenderPersonIndexByNameAsMarkdownHandler>();
                registry.Register<RenderPersonIndex, RenderPersonIndexByDateOfBirthAsMarkdownHandler>();
                registry.Register<RenderPersonIndex, RenderPersonIndexByBirthLocationHandler>();
                registry.Register<RenderPersonIndex, RenderPersonIndexByDeathLocationHandler>();
                registry.Register<RenderPersonIndex, RenderPersonIndexByResidenceLocationHandler>();
                registry.Register<RenderPersonIndex, RenderPersonIndexByOccupationHandler>();
                registry.Register<RenderPersonIndex, RenderMarriageByDateIndexHandler>();
                registry.Register<RenderPersonIndex, RenderMarriageByNameIndexHandler>();
                
                registry.Register<RenderSourceIndex, RenderSourceIndexAsMarkdownHandler>();

                registry.Register<RenderSource, RenderSourceAsMarkdownHandler>();
            });
            services.AddTransient(typeof(ExceptionPolicyHandler<>));
            
            services.AddTransient<IStaticFootnoteOrganiser, MarkdownFootnoteOrganiser>();
            services.AddTransient<IAssociatesOrganiser, AssociatesOrganiser>();
            services.AddTransient<IIndividualNameRenderer, IndividualNameMarkdownRenderer>();
            services.AddTransient<ITimelineRenderer, TimelineMarkdownRenderer>();
            services.AddTransient<IRelationshipRenderer, EnglishLanguageRelationshipRenderer>();
            services.AddTransient<IResidenceRenderer, ResidenceMarkdownRenderer>();
            services.AddTransient<IOccupationRenderer, OccupationMarkdownRenderer>();

            services.AddSingleton(options);
            services.AddSingleton(p=>
            {
                CommandLineOptions opts = p.GetRequiredService<CommandLineOptions>();
                return GetDatabase(opts.SourceFile);
            });
            services.AddSingleton<IFileNamer, FileNamer>();
            services.AddSingleton<IDateRenderer, DateRenderer>();
            services.AddTransient<ApplicationHandler>();
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
            GedcomSettings.LineLength = LineLengthSettings.Any;
            GedcomDatabase result = new GedcomDatabase();
            result.PopulateFromFile(optionsSourceFile);
            return result;
        }
    }
}