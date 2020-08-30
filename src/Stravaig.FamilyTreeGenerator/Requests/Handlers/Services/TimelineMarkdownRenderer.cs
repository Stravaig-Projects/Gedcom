using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public class TimelineMarkdownRenderer : TimelineMarkdownRendererBase, ITimelineRenderer
    {
        private readonly IRelationshipRenderer _relationshipRenderer;
        private readonly IFileNamer _fileNamer;
        private IAssociatesOrganiser _associatesOrganiser;
        private readonly IIndividualNameRenderer _nameRenderer;

        public TimelineMarkdownRenderer(
            IDateRenderer dateRenderer,
            IRelationshipRenderer relationshipRenderer,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
            : base(dateRenderer)
        {
            _relationshipRenderer = relationshipRenderer;
            _fileNamer = fileNamer;
            _nameRenderer = nameRenderer;
        }

        public void WriteTimeline(TextWriter writer,
            GedcomIndividualRecord subject, 
            IStaticFootnoteOrganiser footnoteOrganiser,
            IAssociatesOrganiser associatesOrganiser)
        {
            _writer = writer;
            _footnoteOrganiser = footnoteOrganiser;
            _associatesOrganiser = associatesOrganiser;
            var timeline = subject.GetTimeline(false);
            if (timeline.NotAny())
                return;

            _writer.WriteLine("## Timeline");
            WriteTableHeader();

            foreach (var entry in timeline)
            {
                WriteTimelineEntry(entry);
            }

            _writer.WriteLine();
        }

        private void WriteTimelineEntry(TimelineEntry entry)
        {
            switch (entry.Type)
            {
                case TimelineEntry.EventType.SubjectAttribute:
                case TimelineEntry.EventType.SubjectEvent:
                    WriteSubjectTimelineEntry(entry);
                    break;
                case TimelineEntry.EventType.FamilyEvent:
                    WriteFamilyTimelineEntry(entry);
                    break;
                case TimelineEntry.EventType.FamilyMemberEvent:
                    WriteFamilyMemberTimelineEntry(entry);
                    break;
            }
        }

        private void WriteTableHeader()
        {
            _writer.WriteLine();
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


        private void WriteFamilyMemberTimelineEntry(TimelineEntry entry)
        {
            var eventRecord = entry.IndividualEvent;
            string item, description;
            if (eventRecord.Tag == GedcomIndividualEventRecord.BirthTag)
                (item, description) = WriteBirthEvent(entry);
            else if (eventRecord.Tag == GedcomIndividualEventRecord.DeathTag)
                (item, description) = WriteDeathEvent(entry);
            else
            {
                Relationship relationship = entry.Subject.GetRelationshipTo(entry.OtherFamilyMember);
                string relationshipName = _relationshipRenderer.HumanReadable(relationship, true);
                if (relationship.IsNotRelated)
                    relationshipName = "someone";
                item = $"{eventRecord.Tag}" +
                       (eventRecord.Type.HasContent()
                           ? $":{eventRecord.Type}"
                           : string.Empty) + " of " + relationshipName;
                
                description = eventRecord.RawValue;
                if (eventRecord.Place != null)
                    description += " at " + eventRecord.NormalisedPlaceName();
            }
            var sources = GetSourceFootnotes(eventRecord);
            var notes = GetNoteFootnotes(eventRecord);

            WriteTableRow(entry.Date, item, description, sources, notes);
        }

        private void WriteFamilyTimelineEntry(TimelineEntry entry)
        {
            string item, description;

            if (entry.FamilyEvent.Tag == GedcomFamilyEventRecord.MarriageTag)
                (item, description) = WriteMarriageEvent(entry);
            else if (entry.FamilyEvent.Tag == GedcomFamilyEventRecord.DivorceTag)
                (item, description) = WriteDivorceEvent(entry);
            else
            {
                item = entry.FamilyEvent.Tag.ToString();
                description = string.Empty;
            }
            var sources = GetSourceFootnotes(entry.FamilyEvent);
            var notes = GetNoteFootnotes(entry.FamilyEvent);

            WriteTableRow(entry.Date, item, description, sources, notes);
        }

        private (string, string) WriteDivorceEvent(TimelineEntry entry)
        {
            string item = "Divorced";
            var sb = new StringBuilder();
            if (entry.Family.IsSpouse(entry.Subject))
            {
                var spouse = entry.Family.OtherSpouse(entry.Subject);
                sb.Append("Divorced from ");
                if (spouse.IsAlive())
                {
                    sb.Append("X");
                }
                else
                {
                    var link = _fileNamer.GetIndividualFile(spouse, entry.Subject);
                    sb.Append($"[{spouse.NameWithoutMarker}]({link})");
                }
            }
            else if (entry.Family.IsChild(entry.Subject))
            {
                item = "Divorce of parents";
                if (entry.Family.Spouses.Length >= 1)
                {
                    var parent = entry.Family.Spouses[0];
                    if (parent.IsAlive())
                        sb.Append("X");
                    else
                    {
                        string renderedName = _nameRenderer.RenderLinkedNameWithLifespan(parent, entry.Subject);
                        sb.Append(renderedName);
                    }
                    sb.Append(" got divorced");

                    if (entry.Family.Spouses.Length == 2)
                    {
                        parent = entry.Family.Spouses[1];
                        sb.Append(" from ");
                        if (parent.IsAlive())
                            sb.Append("X");
                        else
                        {
                            string renderedName = _nameRenderer.RenderLinkedNameWithLifespan(parent, entry.Subject);
                            sb.Append(renderedName);
                        }
                    }
                }
            }
            else
            {
                sb.Append("Divorced");
            }

            sb.Append(" ");
            if (entry.FamilyEvent.Address != null)
            {
                sb.Append("at ");
                sb.Append(entry.FamilyEvent.Address.Text);
            }
            else if (entry.FamilyEvent.Place != null)
            {
                sb.Append("in ");
                sb.Append(entry.FamilyEvent.NormalisedPlaceName());
            }
            return (item, sb.ToString());
            
        }
        private (string, string) WriteMarriageEvent(TimelineEntry entry)
        {
            string item = "Marriage";
            var sb = new StringBuilder();
            if (entry.Family.Spouses.Any(s => s == entry.Subject))
            {
                RenderMarriageEventWhereSubjectIsSpouse(entry, sb);
            }
            else
            {
                item = GetMarriageEventItemWhereSubjectIsChild(entry);
                RenderMarriageEventWhereSubjectIsChild(entry, sb);
            }


            if (entry.FamilyEvent.Address != null)
            {
                sb.Append("at ");
                sb.Append(entry.FamilyEvent.Address.Text);
            }
            else if (entry.FamilyEvent.Place != null)
            {
                sb.Append("in ");
                sb.Append(entry.FamilyEvent.NormalisedPlaceName());
            }
            return (item, sb.ToString());
        }

        private void RenderMarriageEventWhereSubjectIsChild(TimelineEntry entry, StringBuilder sb)
        {
            if (entry.Family.Spouses.Length == 1)
            {
                sb.Append("Marriage of ");
                var spouse = entry.Family.Spouses.First();
                RenderSpouse(entry, spouse, sb);
                sb.Append("and unknown");
            }
            else if (entry.Family.Spouses.Length == 2)
            {
                sb.Append("Marriage of ");
                var spouse = entry.Family.Spouses.First();
                RenderSpouse(entry, spouse, sb);
                sb.Append(" and ");
                spouse = entry.Family.Spouses.Last();
                RenderSpouse(entry, spouse, sb);
            }
            else
            {
                sb.Append("Marriage of unknown parents or guardians");
            }

            sb.Append(" ");
        }

        private string GetMarriageEventItemWhereSubjectIsChild(TimelineEntry entry)
        {
            string item = "Marriage";
            Relationship[] relations = entry.Family.Spouses
                .Select(s => entry.Subject.GetRelationshipTo(s))
                .ToArray();
            if (relations.Length == 1)
            {
                var relation = _relationshipRenderer.HumanReadable(relations.First(), true);
                item += $" of {relation}";
            }
            else if (relations.Length == 2)
            {
                var relationNames = relations.OrderBy(r => r.Qualification)
                    .Select(r => _relationshipRenderer.HumanReadable(r, true))
                    .ToArray();
                item += $" between {relationNames[0]} and {relationNames[1]}";
            }
            else
            {
                item += " of unknown";
            }

            return item;
        }

        private void RenderSpouse(TimelineEntry entry, GedcomIndividualRecord spouse, StringBuilder sb)
        {
            if (spouse.IsAlive())
            {
                sb.Append("X");
            }
            else
            {
                var link = _fileNamer.GetIndividualFile(spouse, entry.Subject);
                sb.Append($"[{spouse.NameWithoutMarker}]({link})");
            }
        }

        private void RenderMarriageEventWhereSubjectIsSpouse(TimelineEntry entry, StringBuilder sb)
        {
            var spouse = entry.Family.Spouses.FirstOrDefault(s => s != entry.Subject);
            if (spouse != null)
            {
                sb.Append("Married to ");
                if (spouse.IsAlive())
                {
                    sb.Append("X");
                }
                else
                {
                    var link = _fileNamer.GetIndividualFile(spouse, entry.Subject);
                    sb.Append($"[{spouse.NameWithoutMarker}]({link})");
                }

                sb.Append(" ");
            }
        }

        private void WriteSubjectTimelineEntry(TimelineEntry entry)
        {
            EventRecord eventRecord = (EventRecord) entry.IndividualEvent ?? entry.IndividualAttribute;
            string item, description;

            var tag = eventRecord.Tag;
            if (tag == GedcomIndividualEventRecord.BirthTag)
                (item, description) = WriteBirthEvent(entry);
            else if (tag == GedcomIndividualEventRecord.DeathTag)
                (item, description) = WriteDeathEvent(entry);
            else if ((tag == GedcomIndividualAttributeRecord.ResidenceTag) || 
                     (tag == GedcomIndividualAttributeRecord.OccupationTag))
            {
                // Do nothing, this is handled elsewhere
                return;
            }
            else if (tag == GedcomIndividualEventRecord.BaptismTag)
                (item, description) = WriteBaptism(entry);
            else if (tag == GedcomIndividualEventRecord.ImmigrationTag)
                (item, description) = WriteImmigration(entry);
            else if (tag == GedcomIndividualEventRecord.NaturalisationTag)
                (item, description) = WriteNaturalisation(entry);
            else
            {
                item = $"{tag}" +
                       (eventRecord.Type.HasContent()
                           ? $":{eventRecord.Type}"
                           : string.Empty);
                description = eventRecord.RawValue;
            }

            var sources = GetSourceFootnotes(eventRecord);
            var notes = GetNoteFootnotes(eventRecord);

            WriteTableRow(entry.Date, item, description, sources, notes);
        }

        private (string item, string description) WriteImmigration(TimelineEntry entry)
        {
            string description = "Immigrated";
            var @event = entry.IndividualEvent;

            if (@event.Place != null)
                description += " to " + @event.NormalisedPlaceName();
            if (@event.Address != null)
                description += ", and then residing at " + @event.Address.Text;
            description += ".";

            if (@event.RawValue.HasContent())
                description += $" ({@event.RawValue})";

            return ("Immigrated", description);
        }
        
        private (string item, string description) WriteNaturalisation(TimelineEntry entry)
        {
            string description = "Naturalised";
            var @event = entry.IndividualEvent;

            if (@event.Place != null)
                description += " in " + @event.NormalisedPlaceName();
            description += ".";

            if (@event.RawValue.HasContent())
                description += $" ({@event.RawValue})";

            return ("Naturalised", description);
        }

        private (string, string) WriteBaptism(TimelineEntry entry)
        {
            string description = "Baptised";
            if (entry.IndividualEvent?.Address != null)
                description += " at " + entry.IndividualEvent.Address.Text;
            else if (entry.IndividualEvent?.Place != null)
                description += " in " + entry.IndividualEvent.NormalisedPlaceName();
            return ("Baptism", description);
        }

        private (string, string) WriteDeathEvent(TimelineEntry entry)
        {
            StringBuilder sb = new StringBuilder();

            var subject = entry.OtherFamilyMember ?? entry.Subject;
            if (subject == entry.Subject)
                sb.Append("Died");
            else
            {
                _associatesOrganiser.AddAssociate(subject);
                var link = _fileNamer.GetIndividualFile(entry.OtherFamilyMember, entry.Subject);
                sb.Append($"[{subject.NameWithoutMarker}]({link}) died");
            }

            if (entry.IndividualEvent.Place != null)
            {
                sb.Append($" in {entry.IndividualEvent.NormalisedPlaceName()}");
            }

            sb.Append(".");

            string item = "Died";
            if (entry.OtherFamilyMember != null)
            {
                var relation = entry.Subject.GetRelationshipTo(entry.OtherFamilyMember);
                var relationName = relation.IsNotRelated 
                    ? entry.OtherFamilyMember.NameWithoutMarker
                    : _relationshipRenderer.HumanReadable(relation, true);
                item = "Death of " + relationName;
            }
            return (item, sb.ToString());
        }

        private (string, string) WriteBirthEvent(TimelineEntry entry)
        {
            StringBuilder sb = new StringBuilder();

            var subject = entry.OtherFamilyMember ?? entry.Subject;

            if (subject == entry.Subject)
                sb.Append("Born");
            else
            {
                if (subject.IsAlive())
                    sb.Append("X born");
                else
                {
                    _associatesOrganiser.AddAssociate(subject);
                    var link = _fileNamer.GetIndividualFile(entry.OtherFamilyMember, entry.Subject);
                    sb.Append($"[{subject.NameWithoutMarker}]({link}) born");
                }
            }

            var parentFamily = subject.ChildToFamilies.FirstOrDefault(); // TODO: Fix this assumption
            if (parentFamily != null)
            {
                var parents = parentFamily.Spouses;
                if (parents.Any())
                {
                    var parent = parents[0];
                    if (parent.IsAlive())
                        sb.Append(" to X");
                    else
                    {
                        _associatesOrganiser.AddAssociate(parent);
                        var link = _fileNamer.GetIndividualFile(parents[0], subject);
                        sb.Append($" to [{parent.NameWithoutMarker}]({link})");
                    }
                    if (parents.Length > 1)
                    {
                        parent = parents[1];
                        if (parent.IsAlive())
                        {
                            sb.Append(" and X");
                        }
                        else
                        {
                            _associatesOrganiser.AddAssociate(parent);
                            var link = _fileNamer.GetIndividualFile(parents[1], subject);
                            sb.Append($" and [{parent.NameWithoutMarker}]({link})");
                        }
                    }
                }
            }

            if (entry.IndividualEvent.Place != null)
            {
                sb.Append($" in {entry.IndividualEvent.NormalisedPlaceName()}");
            }

            sb.Append(".");

            string item = "Born";
            if (entry.OtherFamilyMember != null)
            {
                var relation = entry.Subject.GetRelationshipTo(entry.OtherFamilyMember);
                var relationName = relation.IsNotRelated 
                    ? entry.OtherFamilyMember.IsAlive() 
                        ? "X"
                        : entry.OtherFamilyMember.NameWithoutMarker
                    : _relationshipRenderer.HumanReadable(relation, true);
                item = "Birth of " + relationName;
            }
            
            return (item, sb.ToString());
        }
    }
}