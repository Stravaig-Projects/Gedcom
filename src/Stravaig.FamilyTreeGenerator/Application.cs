using System.IO;
using System.Text;
using System.Threading.Tasks;
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

        public async Task Run(string[] args)
        {
            CommandLineOptions options = Parser.Default
                .ParseArguments<CommandLineOptions>(args)
                .MapResult(
                    clo => clo,
                    errors => null);
            if (options == null)
                return;

            GedcomDatabase database = await GetDatabaseAsync(options.SourceFile);

            foreach (var individual in database.IndividualRecords)
            {
                _logger.LogInformation($"Processing {individual.Value.Name}...");                
            }
        }

        private async Task<GedcomDatabase> GetDatabaseAsync(string optionsSourceFile)
        {
            GedcomSettings.LineLength = LineLengthSettings.ValueUpTo255;
            await using FileStream gedcomFileStream = new FileStream(optionsSourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using TextReader streamReader = new StreamReader(gedcomFileStream, Encoding.UTF8);
            using GedcomLineReader lineReader = new GedcomLineReader(streamReader);
            GedcomRecordReader recordReader = new GedcomRecordReader(lineReader);
            
            GedcomDatabase result = new GedcomDatabase();
            await result.PopulateAsync(recordReader);
            return result;
        }
    }
}