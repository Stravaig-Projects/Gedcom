using System.Collections.Generic;
using System.Linq;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTree.Standardiser.Extensions
{
    public static class EventOrderingExtensions
    {
        private static readonly GedcomTag ChangeTag = "CHAN".AsGedcomTag();
        private static readonly GedcomTag MultimediaTag = "OBJE".AsGedcomTag();
        public static IEnumerable<GedcomRecord> OrderIndividualEventChildren(this GedcomRecord eventRecord)
        {
            var age = eventRecord.Children.FirstOrDefault(r => r.Tag == GedcomIndividualEventRecord.AgeTag);
            if (age != null)
                yield return age;

            foreach (var gedcomRecord in OrderEventDetailRecords(eventRecord)) 
                yield return gedcomRecord;
        }

        private static IEnumerable<GedcomRecord> OrderEventDetailRecords(GedcomRecord eventRecord)
        {
            var type = eventRecord.Children.FirstOrDefault(r => r.Tag == EventRecord.TypeTag);
            if (type != null)
                yield return type;

            var date = eventRecord.Children.FirstOrDefault(r => r.Tag == GedcomDateRecord.DateTag);
            if (date != null)
                yield return date;

            var place = eventRecord.Children.FirstOrDefault(r => r.Tag == GedcomPlaceRecord.PlaceTag);
            if (place != null)
                yield return place;

            var address = eventRecord.Children.FirstOrDefault(r => r.Tag == GedcomAddressRecord.AddressTag);
            if (address != null)
                yield return address;

            // TODO: Responsible Agency

            // TODO: Religious Affiliation

            // TODO: Cause of Event

            var restrictionNotice =
                eventRecord.Children.FirstOrDefault(r => r.Tag == GedcomIndividualRecord.RestrictionNoticeTag);
            if (restrictionNotice != null)
                yield return restrictionNotice;

            var notes = eventRecord.Children.Where(r => r.Tag == GedcomNoteRecord.NoteTag)
                .OrderBy(r => r.Value);
            foreach (var note in notes)
                yield return note;

            var sources = eventRecord.Children.Where(r => r.Tag == GedcomSourceRecord.SourceTag)
                .OrderBy(r => r.Value);
            foreach (var source in sources)
                yield return source;

            var multimediaLinks = eventRecord.Children.Where(r => r.Tag == MultimediaTag)
                .OrderBy(r => r.Value);
            foreach (var link in multimediaLinks)
                yield return link;

            var childToFamily = eventRecord.Children.FirstOrDefault(r => r.Tag == GedcomFamilyLinkRecord.ChildToFamilyTag);
            if (childToFamily != null)
                yield return childToFamily;

            var changeDate = eventRecord.Children.FirstOrDefault(r => r.Tag == ChangeTag);
            if (changeDate != null)
                yield return changeDate;
        }

        public static IEnumerable<GedcomRecord> OrderFamilyEventChildren(this GedcomRecord eventRecord)
        {
            var husband = eventRecord.Children.FirstOrDefault(r => r.Tag == GedcomFamilyRecord.HusbandTag);
            if (husband != null)
                yield return husband;

            var wife = eventRecord.Children.FirstOrDefault(r => r.Tag == GedcomFamilyRecord.WifeTag);
            if (wife != null)
                yield return wife;

            foreach (var gedcomRecord in OrderEventDetailRecords(eventRecord)) 
                yield return gedcomRecord;
        }

    }
}