using Microsoft.Extensions.Logging;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTree.Standardiser
{
    public class Application
    {
        private readonly ILogger<Application> _logger;
        private readonly CommandLineOptions _options;
        private readonly GedcomDatabase _database;

        public Application(
            ILogger<Application> logger, 
            CommandLineOptions options,
            GedcomDatabase database)
        {
            _logger = logger;
            _options = options;
            _database = database;
        }

        public void Run()
        {
            
        }
    }
}