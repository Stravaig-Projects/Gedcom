using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public class TimelineMarkdownRenderer : ITimelineRenderer
    {
        private readonly IDateRenderer _dateRenderer;
        private readonly IFileNamer _fileNamer;
        private TextWriter _writer;
        private IFootnoteOrganiser _footnoteOrganiser;

        public TimelineMarkdownRenderer(
            IDateRenderer dateRenderer,
            IFileNamer fileNamer)
        {
            _dateRenderer = dateRenderer;
            _fileNamer = fileNamer;
        }

        public void WriteTimeline(TextWriter writer, IFootnoteOrganiser footnoteOrganiser,
            GedcomIndividualRecord subject)
        {
            _writer = writer;
            _footnoteOrganiser = footnoteOrganiser;
            var timeline = subject.GetTimeline(false);
            if (timeline.NotAny())
                return;

            _writer.WriteLine("## Timeline");
            WriteTableHeader();

            if (subject.CrossReferenceId == "@I48241984@".AsGedcomPointer() ||
                subject.CrossReferenceId == "@I9383584@".AsGedcomPointer())
                Debugger.Break();


            foreach (var entry in timeline)
            {
                WriteTimelineEntry(_writer, entry);
            }

            _writer.WriteLine();
        }

        private void WriteTimelineEntry(TextWriter writer, TimelineEntry entry)
        {
            switch (entry.Type)
            {
                case TimelineEntry.EventType.SubjectAttribute:
                case TimelineEntry.EventType.SubjectEvent:
                    WriteSubjectTimelineEntry(writer, entry);
                    break;
                case TimelineEntry.EventType.FamilyEvent:
                    WriteFamilyTimelineEntry(writer, entry);
                    break;
                case TimelineEntry.EventType.FamilyMemberEvent:
                    WriteFamilyMemberTimelineEntry(writer, entry);
                    break;
            }
        }

        private void WriteTableHeader()
        {
            _writer.WriteLine("Date | Item | Description | Sources | Notes");
            _writer.WriteLine("---|---|---|---|---");
        }

        private void WriteTableRow(GedcomDateRecord date, string item, string description, IEnumerable<int> sources,
            IEnumerable<int> notes)
        {
            _writer.Write(_dateRenderer.RenderAsShortDate(date));
            _writer.Write(" | ");
            _writer.Write(item);
            _writer.Write(" | ");
            _writer.Write(description);
            _writer.Write(" | ");
            WriteFootnoteLinks(sources);
            _writer.Write(" | ");
            WriteFootnoteLinks(notes);
            _writer.WriteLine();
        }

        private void WriteFootnoteLinks(IEnumerable<int> footnoteIds)
        {
            bool isFirst = true;
            foreach (int id in footnoteIds)
            {
                if (isFirst)
                    isFirst = false;
                else
                    _writer.Write(", ");
                _writer.Write($"[{id}](#{id})");
            }
        }

        private IEnumerable<int> GetSourceFootnotes(EventRecord eventRecord)
        {
            return eventRecord.Sources
                .OrderBy(s => s.Title)
                .Select(s => _footnoteOrganiser.AddFootnote(s));
        }

        private IEnumerable<int> GetNoteFootnotes(EventRecord eventRecord)
        {
            return eventRecord.Notes
                .OrderBy(n => n.Text)
                .Select(n => _footnoteOrganiser.AddFootnote(n));
        }

        private void WriteFamilyMemberTimelineEntry(TextWriter writer, TimelineEntry entry)
        {
        }

        private void WriteFamilyTimelineEntry(TextWriter writer, TimelineEntry entry)
        {
            string item, description;

            if (entry.FamilyEvent.Tag == GedcomFamilyEventRecord.MarriageTag)
                (item, description) = WriteMarriageEvent(writer, entry);
            else
            {
                item = entry.FamilyEvent.Tag.ToString();
                description = string.Empty;
            }
            var sources = GetSourceFootnotes(entry.FamilyEvent);
            var notes = GetNoteFootnotes(entry.FamilyEvent);

            WriteTableRow(entry.Date, item, description, sources, notes);
        }

        private (string, string) WriteMarriageEvent(TextWriter writer, TimelineEntry entry)
        {
            var spouse = entry.Family.Spouses.FirstOrDefault(s => s != entry.Subject);
            var sb = new StringBuilder();
            if (spouse != null)
            {
                var link = _fileNamer.GetIndividualFile(spouse);
                sb.Append($"Married to [{spouse.NameWithoutMarker}]({link}) ");
            }

            if (entry.FamilyEvent.Address != null)
            {
                sb.Append("at ");
                sb.Append(entry.FamilyEvent.Address.Text);
            }
            else if (entry.FamilyEvent.Place != null)
            {
                sb.Append("in ");
                sb.Append(entry.FamilyEvent.Place.Name);
            }

                
            var @event = entry.FamilyEvent;
            return ("Married", sb.ToString());
        }

        private void WriteSubjectTimelineEntry(TextWriter writer, TimelineEntry entry)
        {
            EventRecord eventRecord = (EventRecord) entry.IndividualEvent ?? entry.IndividualAttribute;
            string item, description;

            if (eventRecord.Tag == GedcomIndividualEventRecord.BirthTag)
                (item, description) = WriteBirthEvent(entry);
            else if (eventRecord.Tag == GedcomIndividualEventRecord.DeathTag)
                (item, description) = WriteDeathEvent(entry);
            else if (eventRecord.Tag == GedcomIndividualAttributeRecord.OccupationTag)
                (item, description) = WriteOccupation(entry);
            else if (eventRecord.Tag == GedcomIndividualAttributeRecord.ResidenceTag)
                (item, description) = WriteResidence(entry);
            else
            {
                item = $"{eventRecord.Tag}" +
                       (eventRecord.Type.HasContent()
                           ? $":{eventRecord.Type}"
                           : string.Empty);
                description = eventRecord.RawValue;
            }

            var sources = GetSourceFootnotes(eventRecord);
            var notes = GetNoteFootnotes(eventRecord);

            WriteTableRow(entry.Date, item, description, sources, notes);
        }

        private (string, string) WriteResidence(TimelineEntry entry)
        {
            string description;
            var attr = entry.IndividualAttribute;
            if ((attr.Address?.Text).HasContent())
                description = attr.Address.Text;
            else if (attr.Text.HasContent())
                description = attr.Text;
            else if ((attr.Place?.Name).HasContent())
                description = attr.Place.Name;
            else
                description = "Unkown";

            return ("Residence", description);
        }

        private (string, string) WriteOccupation(TimelineEntry entry)
        {
            return ("Occupation", entry.IndividualAttribute.Text);
        }

        private (string, string) WriteDeathEvent(TimelineEntry entry)
        {
            return ("Died", entry.IndividualEvent.Place?.Name);
        }

        private (string, string) WriteBirthEvent(TimelineEntry entry)
        {
            StringBuilder sb = new StringBuilder();

            var subject = entry.Subject;
            var parentFamily = subject.ChildToFamilies.FirstOrDefault(); // TODO: Fix this assumption
            if (parentFamily != null)
            {
                var parents = parentFamily.Spouses;
                if (parents.Any())
                {
                    var link = _fileNamer.GetIndividualFile(parents[0], subject);
                    sb.Append($"Born to [{parents[0].NameWithoutMarker}]({link})");
                    if (parents.Length > 1)
                    {
                        link = _fileNamer.GetIndividualFile(parents[1], subject);
                        sb.Append($" and [{parents[1].NameWithoutMarker}]({link})");
                    }
                }
            }

            return ("Born", sb.ToString());
        }
    }
}