using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public class RenderIndividualAsMarkdownHandler : RequestHandler<RenderIndividual>
    {
        private readonly ILogger<RenderIndividualAsMarkdownHandler> _logger;
        private readonly IFileNamer _fileNamer;

        public RenderIndividualAsMarkdownHandler(
            ILogger<RenderIndividualAsMarkdownHandler> logger,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _fileNamer = fileNamer;
        }

        public override RenderIndividual Handle(RenderIndividual command)
        {
            _logger.LogInformation(
                $"Render As Markdown {command.Individual.CrossReferenceId} : {command.Individual.Name}");

            var fileName = _fileNamer.GetIndividualFile(command.Individual);
            using FileStream fs = new FileStream(fileName.FullName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);
            WriteHeader(writer, command.Individual);
            
            return base.Handle(command);
        }

        private void WriteHeader(TextWriter writer, GedcomIndividualRecord commandIndividual)
        {
            var name = commandIndividual.Name.Replace("/", "");
            writer.WriteLine($"# {name}");
        }   
    }
}