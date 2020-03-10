using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.Gedcom;

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
            int counter = 0;
            foreach (var individual in _database.IndividualRecords.Values)
            {
                counter++;
                _logger.LogInformation($"Processing #{counter}: {individual.Name}...");
                _commander.Publish(new RenderIndividual(individual));
                         }
            return base.Handle(command);
        }
    }
}