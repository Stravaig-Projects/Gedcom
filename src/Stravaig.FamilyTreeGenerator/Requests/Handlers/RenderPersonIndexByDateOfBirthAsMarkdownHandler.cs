using System.IO;
using System.Linq;
using System.Text;
using Humanizer;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderPersonIndexByDateOfBirthAsMarkdownHandler : RequestHandler<RenderPersonIndex>
    {
        private readonly ILogger<RenderPersonIndexByDateOfBirthAsMarkdownHandler> _logger;
        private readonly IIndividualNameRenderer _nameRenderer;
        private readonly IFileNamer _fileNamer;

        public RenderPersonIndexByDateOfBirthAsMarkdownHandler(
            ILogger<RenderPersonIndexByDateOfBirthAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _nameRenderer = nameRenderer;
            _fileNamer = fileNamer;
        }

        public override RenderPersonIndex Handle(RenderPersonIndex command)
        {
            _logger.LogInformation("Rendering Index by name.");

            var fileName = _fileNamer.GetByDateOfBirthIndexFile();
            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);
            WriteHeader(writer);
            WriteIndex(writer, command.Individuals);
            
            return base.Handle(command);
        }
        
        private void WriteHeader(TextWriter writer)
        {
            writer.WriteLine("---");
            writer.WriteLine("layout: page");
            writer.WriteLine("permalink: /indexes/by-date-of-birth");
            writer.WriteLine("---");
            writer.WriteLine();

            writer.WriteLine("# Index - By Date of Birth");
            writer.WriteLine();
        }
        
        private void WriteIndex(TextWriter writer, GedcomIndividualRecord[] subjects)
        {
            var knowns = subjects
                .Where(s => IndividualAlivenessExtensions.IsBirthDateKnown(s))
                .Where(s => s.BirthEvent.Date.BeginningOfExtent.HasValue)
                .GroupBy(s => s.BirthEvent.Date.BeginningOfExtent.Value.Year);

            var byDecade = knowns.GroupBy(g => g.Key / 10);
            var byCentury = byDecade.GroupBy(g => g.Key / 10);

            foreach (var century in byCentury.OrderBy(g=>g.Key))
            {
                writer.WriteLine($"## {(century.Key+1).Ordinalize()} Century");
                writer.WriteLine();
                foreach (var decade in century.OrderBy(g=>g.Key))
                {
                    writer.WriteLine($"### {decade.Key}0s");
                    writer.WriteLine();
                    foreach (var year in decade.OrderBy(g => g.Key))
                    {
                        writer.WriteLine($"* **Born in {year.Key}.**");
                        foreach (var subject in year.OrderByDateThenFamilyName())
                        {
                            string name = _nameRenderer.RenderLinkedNameWithLifespan(subject);
                            writer.WriteLine($"  * {name}");
                        }
                    }
                    writer.WriteLine();
                }
            }
            
            WriteUnknownDoBSubjects(writer, subjects);
        }

        private void WriteUnknownDoBSubjects(TextWriter writer, GedcomIndividualRecord[] subjects)
        {
            var unknowns = subjects.Where(s => s.IsBirthDateKnown() == false);
            writer.WriteLine("## Unknown Date of Birth");
            writer.WriteLine();
            foreach (var subject in unknowns.OrderByStandardSort())
            {
                string name = _nameRenderer.RenderLinkedNameWithLifespan(subject);
                writer.WriteLine($"* {name}");
            }
        }
    }
}