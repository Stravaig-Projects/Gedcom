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
            if (entry.IndividualEvent.Tag == GedcomIndividualEventRecord.BirthTag)
                WriteBirthEvent(writer, entry);
            else if (entry.IndividualEvent.Tag == GedcomIndividualEventRecord.DeathTag)
                WriteDeathEvent(writer, entry);
            else
            {
                string type = $"{entry.IndividualEvent.Tag}" +
                              (entry.IndividualEvent.Type.HasContent()
                                  ? $":{entry.IndividualEvent.Type}"
                                  : string.Empty);
                string date = _dateRenderer.RenderAsProse(entry.IndividualEvent.Date);
                writer.Write($"* **{type}** {date}.");
            }

            var sources = entry.IndividualEvent.Sources;
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

            var notes = entry.IndividualEvent.Notes;
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

        private void WriteDeathEvent(TextWriter writer, TimelineEntry entry)
        {
            writer.Write("* **Died**");
            string date = _dateRenderer.RenderAsProse(entry.IndividualEvent.Date);
            if (date.HasContent())
                writer.Write(" "+date);
            writer.WriteLine(".");
        }

        private void WriteBirthEvent(TextWriter writer, TimelineEntry entry)
        {
            writer.Write("* **Born**");
            string date = _dateRenderer.RenderAsProse(entry.IndividualEvent.Date);
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
            writer.WriteLine(".");
        }
    }
}