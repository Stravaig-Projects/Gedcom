using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Humanizer;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderIndividualImmediateFamilyAsSvgHandler : RequestHandler<RenderIndividual>
    {
        private readonly ILogger<RenderIndividualImmediateFamilyAsSvgHandler> _logger;
        private readonly IFileNamer _fileNamer;

        public RenderIndividualImmediateFamilyAsSvgHandler(
            ILogger<RenderIndividualImmediateFamilyAsSvgHandler> logger,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _fileNamer = fileNamer;
        }

        public override RenderIndividual Handle(RenderIndividual command)
        {
            
            return base.Handle(command);
        }
    }
    
    public class RenderIndividualAsMarkdownHandler : RequestHandler<RenderIndividual>, IDisposable
    {
        private readonly ILogger<RenderIndividualAsMarkdownHandler> _logger;
        private readonly IDateRenderer _dateRenderer;
        private readonly IFootnoteOrganiser _footnoteOrganiser;
        private readonly IAssociatesOrganiser _associatesOrganiser;
        private readonly ITimelineRenderer _timelineRenderer;
        private readonly IIndividualNameRenderer _nameRenderer;
        private readonly IFileNamer _fileNamer;
        private readonly IRelationshipRenderer _relationshipRenderer;
        private FileStream _fs;
        private TextWriter _writer;
        private bool _hasSources;
        
        public RenderIndividualAsMarkdownHandler(
            ILogger<RenderIndividualAsMarkdownHandler> logger,
            IDateRenderer dateRenderer,
            IFootnoteOrganiser footnoteOrganiser,
            IAssociatesOrganiser associatesOrganiser,
            ITimelineRenderer timelineRenderer,
            IIndividualNameRenderer nameRenderer,
            IRelationshipRenderer relationshipRenderer,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _dateRenderer = dateRenderer;
            _footnoteOrganiser = footnoteOrganiser;
            _associatesOrganiser = associatesOrganiser;
            _timelineRenderer = timelineRenderer;
            _nameRenderer = nameRenderer;
            _fileNamer = fileNamer;
            _relationshipRenderer = relationshipRenderer;
            _hasSources = false;
        }

        public override RenderIndividual Handle(RenderIndividual command)
        {
            _logger.LogInformation(
                $"Render As Markdown {command.Individual.CrossReferenceId} : {command.Individual.Name}");

            InitHandler(command);
            WriteHeader(command.Individual);
            WriteNames(command.Individual);
            WriteImmediateFamily(command.Individual);
            _timelineRenderer.WriteTimeline(_writer, command.Individual, _footnoteOrganiser, _associatesOrganiser);
            WriteNotes(command.Individual);
            WriteAssociations(command.Individual);
            _footnoteOrganiser.WriteFootnotes(_writer, command.Individual);
            WriteFooter(command.Individual);
            
            return base.Handle(command);
        }

        private void InitHandler(RenderIndividual command)
        {
            void AddSource(GedcomSourceRecord record, GedcomIndividualRecord individualRecord)
            {
                _hasSources = true;
                command.AddSource(record, individualRecord);
            }

            _footnoteOrganiser.InitFootnotes(AddSource, command.Individual);
            var fileName = _fileNamer.GetIndividualFile(command.Individual);
            _logger.LogInformation($"Writing to file: {fileName}");
            _fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            _writer = new StreamWriter(_fs, Encoding.UTF8);
        }
        
        private void WriteImmediateFamily(GedcomIndividualRecord subject)
        {
            if (subject.FamilyLinks.NotAny())
                return;
            
            _writer.WriteLine("## Immediate Family");
            _writer.WriteLine();

            var immediateRelatives = subject.GetImmediateRelatives()
                .OrderBy(r => r.Relative.BirthEvent?.Date);

            foreach (var relative in immediateRelatives)
            {
                string relationship = _relationshipRenderer.HumanReadable(relative.TypeOfRelationship, true).Titleize();
                string name = relative.Relative.IsAlive()
                    ? "X"
                    : _nameRenderer.RenderLinkedNameWithLifespan(relative.Relative, subject);
                _writer.WriteLine($"* {relationship}: {name}");
            }
            _writer.WriteLine();
        }
        
        private void WriteNames(GedcomIndividualRecord subject)
        {
            if (subject.Names.Length > 1)
            {
                _writer.WriteLine("## Names");
                _writer.WriteLine();
                var orderedNames = subject.Names
                    .OrderBy(n=>n.Type??string.Empty)
                    .ThenBy(n=>n.WholeName);
                foreach (var name in orderedNames)
                {
                    _writer.Write($"* {name.WholeName.Trim()}");
                    if (name.Type.HasContent())
                    {
                        switch (name.Type)
                        {
                            case "variation":
                                _writer.Write(" (variation)");
                                break;
                            case "married":
                                _writer.Write(" (married name)");
                                break;
                            case "nick":
                                _writer.Write(" (nickname)");
                                break;
                            default:
                                _writer.Write($" ({name.Type})");
                                break;
                        }
                    }
                    _writer.WriteLine();
                }
                _writer.WriteLine();
            }
        }

        private void WriteNotes(GedcomIndividualRecord subject)
        {
            if (subject.Notes.Any())
            {
                _writer.WriteLine("## Notes");
                for (int i = 0; i < subject.Notes.Length; i++)
                {
                    if (subject.Notes.Length > 1)
                    {
                        _writer.WriteLine();
                        _writer.WriteLine($"### Note #{i+1}");
                    }
                    _writer.WriteLine();
                    var note = subject.Notes[i];
                    _writer.WriteMarkdownBlockQuote(note.Text);
                    _writer.WriteLine();
                }
            }
        }

        private void WriteAssociations(GedcomIndividualRecord subject)
        {
            List<GedcomIndividualRecord> parentsAndGuardians = new List<GedcomIndividualRecord>();
            List<GedcomIndividualRecord> siblings = new List<GedcomIndividualRecord>();
            foreach (var family in subject.ChildToFamilies)
            {
                siblings.AddRange(family.Children);
                parentsAndGuardians.AddRange(family.Spouses);
            }
            
        }

        private void WriteHeader(GedcomIndividualRecord subject)
        {
            _writer.WriteLine("---");
            _writer.WriteLine("layout: person");
            _writer.WriteLine($"subject_key: {subject.CrossReferenceId.ToString().Trim('@').ToLowerInvariant()}");
            _writer.WriteLine($"permalink: /people/{subject.CrossReferenceId.ToString().Trim('@').ToLowerInvariant()}");
            _writer.WriteLine("---");
            _writer.WriteLine();

            
            var name = subject.NameWithoutMarker;
            _writer.WriteLine($"# {name}");
            var birthDate = subject.BirthEvent?.Date;
            var deathDate = subject.DeathEvent?.Date;

            if (birthDate != null || deathDate != null)
            {
                _writer.Write("(");
                if (birthDate != null)
                {
                    DateTime? exactDate = birthDate.ExactDate1;
                    if (exactDate.HasValue)
                    {
                        _writer.Write($"{exactDate:d MMMM, yyyy}");
                    }
                    else
                    {
                        _writer.Write(birthDate.RawDateValue);
                    }
                }
                else
                {
                    _writer.Write("?");
                }
                _writer.Write(" - ");
                if (deathDate != null)
                {
                    DateTime? exactDate = deathDate.ExactDate1;
                    if (exactDate.HasValue)
                    {
                        _writer.Write($"{exactDate:d MMMM, yyyy}");
                    }
                    else
                    {
                        _writer.Write(deathDate.RawDateValue);
                    }
                }
                else
                {
                    _writer.Write("?");
                }
                _writer.WriteLine(")");
            }

            _writer.WriteLine();
        }

        private void WriteFooter(GedcomIndividualRecord subject)
        {
            var thisDirectory = new FileInfo(_fileNamer.GetIndividualFile(subject)).DirectoryName;
            var indexByNameFile = _fileNamer.GetByNameIndexFile(thisDirectory);
            var indexSourcesFile = _fileNamer.GetSourceIndexFile(thisDirectory);
            _writer.WriteLine();
            _writer.WriteLine("## See also");
            _writer.WriteLine();
            var associates = _associatesOrganiser.Associates;
            if (associates.Any())
            {
                _writer.WriteLine("- People Mentioned");
                foreach (var person in associates.OrderByStandardSort())
                {
                    var linkedName = _nameRenderer.RenderLinkedNameWithLifespan(person, subject);
                    _writer.WriteLine($"  - {linkedName}");
                }
            }
            _writer.WriteLine("- Indexes");
            _writer.WriteLine($"  - [By family name]({indexByNameFile})");
            if (_hasSources)
                _writer.WriteLine($"  - [Sources]({indexSourcesFile})");

            if (subject.FamilySearchId.HasContent())
                _writer.WriteLine($"- [Family Search Person Details](https://www.familysearch.org/tree/person/details/{subject.FamilySearchId})");
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _writer?.Dispose();
                _fs?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RenderIndividualAsMarkdownHandler()
        {
            Dispose(false);
        }
    }
}