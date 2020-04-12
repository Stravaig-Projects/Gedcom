using System.Linq;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public class ApplicationHandler : RequestHandler<Application>
    {
        private readonly ILogger<ApplicationHandler> _logger;
        private readonly GedcomDatabase _database;
        private readonly CommandLineOptions _options;
        private readonly IAmACommandProcessor _commander;
        
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
                _commander.Publish(new RenderIndividual(individual));
            } 
            _commander.Publish(new RenderIndex(individuals));
            return base.Handle(command);
        }
    }
}