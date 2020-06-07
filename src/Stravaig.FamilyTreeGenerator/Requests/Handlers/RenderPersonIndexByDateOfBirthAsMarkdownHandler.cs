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
    public class RenderPersonIndexByDateOfBirthAsMarkdownHandler : RenderPersonIndexBaseHandler
    {
        private readonly IIndividualNameRenderer _nameRenderer;

        public RenderPersonIndexByDateOfBirthAsMarkdownHandler(
            ILogger<RenderPersonIndexByDateOfBirthAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
            : base(logger, fileNamer)
        {
            _nameRenderer = nameRenderer;
        }

        protected override string FileName => _fileNamer.GetByDateOfBirthIndexFile();
        
        protected override void WriteIndex(TextWriter writer, GedcomIndividualRecord[] subjects)
        {
            var knowns = subjects
                .Where(s => s.IsBirthDateKnown())
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
                            string name = _nameRenderer.RenderLinkedNameWithLifespan(subject, boldName:true, familyNameFirst:true);
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