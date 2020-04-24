using System.Diagnostics;
using System.IO;
using System.Linq;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public partial class RenderIndividualAsMarkdownHandler
    {
        private void WriteTimeline(GedcomIndividualRecord subject)
        {
            _writer.WriteLine("## Timeline");
            
            if (subject.CrossReferenceId == "@I48241984@".AsGedcomPointer() || subject.CrossReferenceId == "@I9383584@".AsGedcomPointer())
                Debugger.Break();
            
            var timeline = subject.GetTimeline(false);
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

        private void WriteFamilyMemberTimelineEntry(TextWriter writer, TimelineEntry entry)
        {
        }

        private void WriteFamilyTimelineEntry(TextWriter writer, TimelineEntry entry)
        {
        }

        private void WriteSubjectTimelineEntry(TextWriter writer, TimelineEntry entry)
        {
            EventRecord eventRecord = (EventRecord)entry.IndividualEvent ?? entry.IndividualAttribute;
            if (eventRecord.Tag == GedcomIndividualEventRecord.BirthTag)
                WriteBirthEvent(writer, entry);
            else if (eventRecord.Tag == GedcomIndividualEventRecord.DeathTag)
                WriteDeathEvent(writer, entry);
            else if (eventRecord.Tag == GedcomIndividualAttributeRecord.OccupationTag)
                WriteOccupation(writer, entry);
            else if (eventRecord.Tag == GedcomIndividualAttributeRecord.ResidenceTag)
                WriteResidence(writer, entry);
            else
            {
                string type = $"{eventRecord.Tag}" +
                              (eventRecord.Type.HasContent()
                                  ? $":{eventRecord.Type}"
                                  : string.Empty);
                string date = _dateRenderer.RenderAsProse(eventRecord.Date);
                writer.Write($"* **{type}** {date}.");
            }

            var sources = eventRecord.Sources;
            int numSources = sources.Length;
            if (numSources > 0)
            {
                string plural = numSources > 1 ? "s" : "";
                writer.Write($" See source{plural}: ");
                bool isFirst = true;
                foreach (var source in sources.OrderBy(s=>s.Title))
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        writer.Write(", ");
                    int id = _footnoteOrganiser.AddFootnote(source);
                    writer.Write($"[{id}](#{id})");
                }
            }

            var notes = eventRecord.Notes;
            var numNotes = notes.Length;
            if (numNotes > 0)
            {
                string plural = numNotes > 1 ? "s" : "";
                if (numSources > 0)
                    writer.Write($" and note{plural}: ");
                else
                    writer.Write($" See note{plural}: ");
                bool isFirst = true;
                foreach (var note in notes.OrderBy(n=>n.Text))
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        writer.Write(", ");
                    int id = _footnoteOrganiser.AddFootnote(note);
                    writer.Write($"[{id}](#{id})");
                }
            }

            if (numNotes > 0 || numSources > 0)
                writer.Write(".");

            writer.WriteLine();
        }

        private void WriteResidence(TextWriter writer, TimelineEntry entry)
        {
            var attr = entry.IndividualAttribute;
            writer.Write("* **Resided** at ");
            if ((attr.Address?.Text).HasContent())
                writer.Write(attr.Address.Text);
            else if (attr.Text.HasContent())
                writer.Write(attr.Text);
            else if ((attr.Place?.Name).HasContent())
                writer.Write(attr.Place.Name);
            else
                writer.Write("Unkown");

            if (attr.Date?.HasCoherentDate ?? false)
            {
                writer.Write(" ");
                string date = _dateRenderer.RenderAsProse(attr.Date);
                writer.Write(date);
            }
            
            writer.Write(".");
        }

        private void WriteOccupation(TextWriter writer, TimelineEntry entry)
        {
            writer.Write("* **Occupation**");
            string date = _dateRenderer.RenderAsProse(entry.Date);
            if (date.HasContent())
                writer.Write(" "+date);
            
            var occupation = entry.IndividualAttribute.Text;
            if (occupation.HasContent())
                writer.Write($" as \"{occupation}\"");
            writer.Write(".");
        }

        private void WriteDeathEvent(TextWriter writer, TimelineEntry entry)
        {
            writer.Write("* **Died**");
            string date = _dateRenderer.RenderAsProse(entry.Date);
            if (date.HasContent())
                writer.Write(" "+date);
            writer.Write(".");
        }

        private void WriteBirthEvent(TextWriter writer, TimelineEntry entry)
        {
            writer.Write("* **Born**");
            string date = _dateRenderer.RenderAsProse(entry.Date);
            if (date.HasContent())
                writer.Write(" "+date);

            var subject = entry.Subject;
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
            writer.Write(".");
        }
    }
}