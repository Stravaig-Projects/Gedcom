using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Humanizer;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;

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
            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);
            WriteHeader(writer, command.Individual);
            WriteTimeline(writer, command.Individual);
            WriteFooter(writer, command.Individual);
            
            return base.Handle(command);
        }
        
        private void WriteHeader(TextWriter writer, GedcomIndividualRecord subject)
        {
            var name = subject.NameWithoutMarker;
            writer.WriteLine($"# {name}");
            var birthDate = subject.BirthEvent?.Date;
            var deathDate = subject.DeathEvent?.Date;

            if (birthDate != null || deathDate != null)
            {
                writer.Write("(");
                if (birthDate != null)
                {
                    DateTime? exactDate = birthDate.ExactDate;
                    if (exactDate.HasValue)
                    {
                        writer.Write($"{exactDate:d MMMM, yyyy}");
                    }
                    else
                    {
                        writer.Write(birthDate.RawDateValue);
                    }
                }
                else
                {
                    writer.Write("?");
                }
                writer.Write(" - ");
                if (deathDate != null)
                {
                    DateTime? exactDate = deathDate.ExactDate;
                    if (exactDate.HasValue)
                    {
                        writer.Write($"{exactDate:d MMMM, yyyy}");
                    }
                    else
                    {
                        writer.Write(deathDate.RawDateValue);
                    }
                }
                else
                {
                    writer.Write("?");
                }
                writer.WriteLine(")");
            }
        }

        private void WriteTimeline(TextWriter writer, GedcomIndividualRecord subject)
        {
            writer.WriteLine("## Timeline");

            WriteTimeLineBirth(writer, subject);
        }

        private void WriteTimeLineBirth(TextWriter writer, GedcomIndividualRecord subject)
        {
            var name = subject.NameWithoutMarker;
            var birthday = subject.BirthEvent?.Date;
            writer.Write($"* **{name}** was born");
            if (birthday != null)
            {
                bool displayDate = false;
                switch (birthday.ApproximationType)
                {
                    case GedcomDateApproximationType.About:
                        writer.Write(" about");
                        displayDate = true;
                        break;
                    case GedcomDateApproximationType.Calculated:
                        writer.Write(" calculated to");
                        displayDate = true;
                        break;
                    case GedcomDateApproximationType.Estimated:
                        writer.Write(" estimated as");
                        displayDate = true;
                        break;
                    case GedcomDateApproximationType.Exact:
                        if (birthday.Day.HasValue)
                            writer.Write(" on");
                        else
                            writer.Write(" in");
                        break;
                }

                if (displayDate)
                {
                    if (birthday.Day.HasValue)
                        writer.Write($" {birthday.Day.Value.Ordinalize()} of");

                    if (birthday.Month.HasValue)
                        writer.Write($" {new DateTime(1, birthday.Month.Value, 1):MMMM}");

                    if (birthday.Year.HasValue)
                        writer.Write($" {birthday.Year.Value}");
                }

                var parentFamily = subject.ChildToFamilies.FirstOrDefault();
                if (parentFamily != null)
                {
                    var parents = parentFamily.Spouses;
                    if (parents.Any())
                    {
                        var link = _fileNamer.GetIndividualFile(parents[0], subject);
                        writer.Write($" to [{parents[0].NameWithoutMarker}]({link})");
                        if (parents.Length > 1)
                        { 
                            link = _fileNamer.GetIndividualFile(parents[1], subject);
                            writer.Write($" and [{parents[1].NameWithoutMarker}]({link})");
                        }
                    }
                }
                writer.WriteLine(".");
            }
            else
                writer.WriteLine(".");
        }

        private void WriteFooter(TextWriter writer, GedcomIndividualRecord subject)
        {
            var thisDirectory = new FileInfo(_fileNamer.GetIndividualFile(subject)).DirectoryName;
            var indexByNameFile = _fileNamer.GetByNameIndexFile(thisDirectory);
            writer.WriteLine();
            writer.WriteLine("## See also");
            writer.WriteLine();
            writer.WriteLine("- Indexes");
            writer.WriteLine($"  - [By family name]({indexByNameFile})");
        }
    }
}