using System.IO;
using System.Text;
using CommandLine;
using Microsoft.Extensions.Logging;
using Stravaig.Gedcom;

namespace Stravaig.FamilyTreeGenerator
{
    public class Application
    {
        private readonly ILogger<Application> _logger;

        public Application(ILogger<Application> logger)
        {
            _logger = logger;
        }

        public void Run(string[] args)
        {
            CommandLineOptions options = Parser.Default
                .ParseArguments<CommandLineOptions>(args)
                .MapResult(
                    clo => clo,
                    errors => null);
            if (options == null)
                return;

            GedcomDatabase database = GetDatabase(options.SourceFile);

            int counter = 0;
            foreach (var individual in database.IndividualRecords.Values)
            {
                counter++;
                _logger.LogInformation($"Processing #{counter}: {individual.Name}...");                
            }
        }

        private GedcomDatabase GetDatabase(string optionsSourceFile)
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