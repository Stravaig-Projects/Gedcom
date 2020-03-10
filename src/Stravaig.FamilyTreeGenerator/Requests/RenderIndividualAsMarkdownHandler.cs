using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public class RenderIndividualAsMarkdownHandler : RequestHandler<RenderIndividual>
    {
        private readonly ILogger<RenderIndividualAsMarkdownHandler> _logger;

        public RenderIndividualAsMarkdownHandler(ILogger<RenderIndividualAsMarkdownHandler> logger)
        {
            _logger = logger;
        }

        public override RenderIndividual Handle(RenderIndividual command)
        {
            _logger.LogInformation(
                $"Render As Markdown {command.Individual.CrossReferenceId} : {command.Individual.Name}");
            
            
            
            return base.Handle(command);
        }
    }
}