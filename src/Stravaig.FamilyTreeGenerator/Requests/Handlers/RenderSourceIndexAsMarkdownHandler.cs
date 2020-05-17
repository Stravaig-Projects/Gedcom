using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Requests.Models;
using Stravaig.FamilyTreeGenerator.Services;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderSourceIndexAsMarkdownHandler : RequestHandler<RenderSourceIndex>
    {
        private readonly ILogger<RenderSourceIndexAsMarkdownHandler> _logger;
        private readonly IIndividualNameRenderer _nameRenderer;
        private readonly IFileNamer _fileNamer;
        

        public RenderSourceIndexAsMarkdownHandler(
            ILogger<RenderSourceIndexAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _fileNamer = fileNamer;
            _nameRenderer = nameRenderer;
        }
        public override RenderSourceIndex Handle(RenderSourceIndex command)
        {
            _logger.LogInformation("Rendering Source Index.");

            var fileName = _fileNamer.GetSourceIndexFile();
            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);
            WriteHeader(writer);
            WriteIndex(writer, command.SourceEntries);

            return base.Handle(command);
        }

        private void WriteIndex(TextWriter writer, SourceEntry[] entries)
        {
            var orderedEntries = entries.OrderBy(s => s.Source.Title);
            foreach (var entry in orderedEntries)
            {
                string basePath = _fileNamer.BaseDirectory().FullName;
                string filePath = _fileNamer.GetSourceFile(entry.Source, basePath);
                writer.WriteLine($"* [{entry.Source.Title.RemoveNamesOfTheLiving(entry.Source.ReferencedBy)}]({filePath})");
                var references = entry.ReferencedByIndividuals
                    .OrderByStandardSort()
                    .ToArray();
                foreach (var person in references)
                {
                    writer.Write("  * Referenced from entry about: ");
                    string name = _nameRenderer.RenderLinkedNameWithLifespan(person);
                    writer.WriteLine(name);
                }
            }
        }

        private void WriteHeader(TextWriter writer)
        {
            writer.WriteLine("---");
            writer.WriteLine("layout: page");
            writer.WriteLine("permalink: /indexes/by-source-title");
            writer.WriteLine("---");
            writer.WriteLine();
            writer.WriteLine("# Index - Sources");
            writer.WriteLine();
        }
    }
}