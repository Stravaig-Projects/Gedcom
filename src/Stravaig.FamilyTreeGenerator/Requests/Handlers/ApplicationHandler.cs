using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class ApplicationHandler : RequestHandler<Application>
    {
        public class SourceEntry
        {
            public GedcomSourceRecord Source { get; }
            public List<GedcomIndividualRecord> ReferencedByIndividuals { get; }

            public SourceEntry(GedcomSourceRecord source, GedcomIndividualRecord individual)
            {
                Source = source;
                ReferencedByIndividuals = new List<GedcomIndividualRecord>();
                ReferencedByIndividuals.Add(individual);
            }
        }
        private readonly ILogger<ApplicationHandler> _logger;
        private readonly GedcomDatabase _database;
        private readonly CommandLineOptions _options;
        private readonly IAmACommandProcessor _commander;
        private readonly Dictionary<GedcomPointer, SourceEntry> _sources;
        
        
        public ApplicationHandler(
            ILogger<ApplicationHandler> logger,
            IAmACommandProcessor commander,
            GedcomDatabase database,
            CommandLineOptions options)
        {
            _logger = logger;
            _commander = commander;
            _database = database;
            _options = options;
            _sources = new Dictionary<GedcomPointer, SourceEntry>();
        }

        public override Application Handle(Application command)
        {
            _commander.Publish(new InitFileSystem());
            int counter = 0;

            var individuals = _database.IndividualRecords.Values
                .Where(ir=>ir.IsDead())
                .ToArray();
            foreach (var individual in individuals)
            {
                counter++;
                _logger.LogInformation($"Processing #{counter}: {individual.Name}...");
                _commander.Publish(new RenderIndividual(individual, AddSource));
            } 
            _commander.Publish(new RenderIndex(individuals));
            _logger.LogInformation("Done!");
            return base.Handle(command);
        }

        private void AddSource(GedcomSourceRecord source, GedcomIndividualRecord individual)
        {
            if (_sources.TryGetValue(source.CrossReferenceId, out var sourceEntry))
            {
                if (!sourceEntry.ReferencedByIndividuals.Any(i => i.CrossReferenceId == individual.CrossReferenceId))
                {
                    sourceEntry.ReferencedByIndividuals.Add(individual);
                }
            }
            else
            {
                sourceEntry = new SourceEntry(source, individual);
                _sources.Add(source.CrossReferenceId, sourceEntry);
            }
        }
    }
}