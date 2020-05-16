using System.IO;
using System.Text;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Stravaig.FamilyTreeGenerator.Requests;
using Stravaig.FamilyTreeGenerator.Requests.Handlers;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = ExtractCommandLineInfo(args);
            if (options == null)
                return;
            
            using var provider = BuildServiceProvider(options);
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
            }).Handlers(registry =>
            {
                registry.Register<Application, ApplicationHandler>();
                registry.Register<InitFileSystem, InitFileSystemForMarkdownHandler>();
                registry.Register<RenderIndividual, RenderIndividualAsMarkdownHandler>();
                registry.Register<RenderPersonIndex, RenderPersonIndexByNameAsMarkdownHandler>();
                registry.Register<RenderPersonIndex, RenderPersonIndexByDateOfBirthAsMarkdownHandler>();
                registry.Register<RenderSourceIndex, RenderSourceIndexAsMarkdownHandler>();
                registry.Register<RenderSource, RenderSourceAsMarkdownHandler>();
            });
            services.AddTransient<IFootnoteOrganiser, MarkdownFootnoteOrganiser>();
            services.AddTransient<IAssociatesOrganiser, AssociatesOrganiser>();
            services.AddTransient<IIndividualNameRenderer, IndividualNameMarkdownRenderer>();
            services.AddTransient<ITimelineRenderer, TimelineMarkdownRenderer>();
            services.AddTransient<IRelationshipRenderer, RelationshipRenderer>();
            
            
            GedcomDatabase database = GetDatabase(options.SourceFile);
            services.AddSingleton<CommandLineOptions>(options);
            services.AddSingleton<GedcomDatabase>(p=>
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
            GedcomSettings.LineLength = LineLengthSettings.ValueUpTo255;
            using FileStream gedcomFileStream = new FileStream(optionsSourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using TextReader streamReader = new StreamReader(gedcomFileStream, Encoding.UTF8);
            using GedcomLineReader lineReader = new GedcomLineReader(streamReader);
            GedcomRecordReader recordReader = new GedcomRecordReader(lineReader);
            
            GedcomDatabase result = new GedcomDatabase();
            result.Populate(recordReader);
            return result;
        }
    }
}