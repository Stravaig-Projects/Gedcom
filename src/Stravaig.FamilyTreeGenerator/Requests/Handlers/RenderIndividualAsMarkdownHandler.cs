using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Humanizer;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Stravaig.FamilyTree.Common.Extensions;
using Stravaig.FamilyTree.Common.Humaniser;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;
using Size = System.Drawing.Size;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // Instantiated by Paramore Brighter
    public class RenderIndividualAsMarkdownHandler : RequestHandler<RenderIndividual>, IDisposable
    {
        private const double ProfilePictureSize = 200;
        private readonly ILogger<RenderIndividualAsMarkdownHandler> _logger;
        private readonly IStaticFootnoteOrganiser _footnoteOrganiser;
        private readonly IAssociatesOrganiser _associatesOrganiser;
        private readonly ITimelineRenderer _timelineRenderer;
        private readonly IIndividualNameRenderer _nameRenderer;
        private readonly IFileNamer _fileNamer;
        private readonly IRelationshipRenderer _relationshipRenderer;
        private readonly IResidenceRenderer _residenceRenderer;
        private readonly IOccupationRenderer _occupationRenderer;
        private FileStream _fs;
        private TextWriter _writer;

        public RenderIndividualAsMarkdownHandler(
            ILogger<RenderIndividualAsMarkdownHandler> logger,
            IStaticFootnoteOrganiser footnoteOrganiser,
            IAssociatesOrganiser associatesOrganiser,
            ITimelineRenderer timelineRenderer,
            IIndividualNameRenderer nameRenderer,
            IRelationshipRenderer relationshipRenderer,
            IResidenceRenderer residenceRenderer,
            IOccupationRenderer occupationRenderer,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _footnoteOrganiser = footnoteOrganiser;
            _associatesOrganiser = associatesOrganiser;
            _timelineRenderer = timelineRenderer;
            _nameRenderer = nameRenderer;
            _fileNamer = fileNamer;
            _relationshipRenderer = relationshipRenderer;
            _residenceRenderer = residenceRenderer;
            _occupationRenderer = occupationRenderer;
        }

        public override RenderIndividual Handle(RenderIndividual command)
        {
            _logger.LogInformation(
                "Render As Markdown {IndividualCrossReferenceId} : {IndividualName}",
                command.Individual.CrossReferenceId,
                command.Individual.Name);

            InitHandler(command);
            WriteHeader(command.Individual);
            WriteNames(command.Individual);
            WriteImmediateFamily(command.Individual);
            _timelineRenderer.WriteTimeline(_writer, command.Individual, _footnoteOrganiser, _associatesOrganiser);
            _residenceRenderer.WriteResidence(_writer, command.Individual, _footnoteOrganiser);
            _occupationRenderer.WriteOccupations(_writer, command.Individual, _footnoteOrganiser);
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
                command.AddSource(record, individualRecord);
            }

            _footnoteOrganiser.InitFootnotes(AddSource, command.Individual);
            var fileName = _fileNamer.GetIndividualFile(command.Individual);
            _logger.LogInformation($"Writing to file: {fileName}");
            _fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            var utf8NoBom = new UTF8Encoding(false);
            _writer = new StreamWriter(_fs, utf8NoBom);
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
            var simpleIndividualId = subject.CrossReferenceId.ToSimpleIndividualId();

            _writer.WriteLine("---");
            _writer.WriteLine("layout: person");
            _writer.WriteLine($"subject_key: {simpleIndividualId}");
            _writer.WriteLine($"permalink: /people/{simpleIndividualId}");
            _writer.WriteLine("---");
            _writer.WriteLine();

            var name = subject.NameWithoutMarker;
            _writer.WriteLine($"# {name}");

            WriteProfilePicture(subject);

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

        private void WriteProfilePicture(GedcomIndividualRecord subject)
        {
            var profilePictureObject = subject.Objects
                .FirstOrDefault(o => o.HasLabel("Publish Image") && o.IsFileType("jpg"));
            if (profilePictureObject == null)
                return;

            var fullFilePath = _fileNamer.GetMediaFile(profilePictureObject);

            using var image = Image.Load(fullFilePath);
            var width = (double)image.Width;
            var height = (double)image.Height;
            if (width > ProfilePictureSize)
            {
                var scale = width / ProfilePictureSize;
                height /= scale;
                width = ProfilePictureSize;
            }

            if (height > ProfilePictureSize)
            {
                var scale = height / ProfilePictureSize;
                width /= scale;
                height = ProfilePictureSize;
            }

            image.Mutate(ipc => ipc.Resize((int)width, (int)height));
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, new JpegEncoder()
            {
                SkipMetadata = true,
                Quality = 75,
            });
            byte[] imageBytes = memoryStream.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);
            _writer.WriteLine($"<img alt=\"{HttpUtility.HtmlEncode(profilePictureObject.Title)}\" class=\"profile-pic\" src=\"data:image/jpg;base64,{base64String}\"/>");
        }

        private void WriteFooter(GedcomIndividualRecord subject)
        {
            if (subject.FamilySearchId.HasContent())
            {
                _writer.WriteLine();
                _writer.WriteLine("## See also");
                _writer.WriteLine();
                _writer.WriteLine($"- [Family Search Person Details](https://www.familysearch.org/tree/person/details/{subject.FamilySearchId})");
            }
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
