using System;
using System.Linq;
using System.Threading;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Settings;

namespace Stravaig.Gedcom.Model
{
    public class GedcomIndividualRecord
    {
        public static readonly GedcomTag Tag = "INDI".AsGedcomTag();
        public static readonly GedcomTag SexTag = "SEX".AsGedcomTag();
        
        private readonly GedcomRecord _record;
        private readonly GedcomDatabase _database;
        private readonly Lazy<GedcomNameRecord[]> _lazyNames;
        private readonly Lazy<GedcomIndividualEventRecord[]> _lazyEvents;
        private readonly Lazy<GedcomFamilyLinkRecord[]> _lazyFamilies;

        public GedcomIndividualRecord(GedcomRecord record, GedcomDatabase database)
        {
            _record = record ?? throw new ArgumentNullException(nameof(record));
            _database = database;
            if (record.Tag != Tag)
                throw new ArgumentException($"Must be an \"INDIVIDUAL_RECORD\" ({Tag}) record, but was {record.Tag}.");
            if (!record.CrossReferenceId.HasValue)
                throw new ArgumentException("An \"INDIVIDUAL_RECORD\" must have a CrossReferenceId.");
            
            _lazyNames = new Lazy<GedcomNameRecord[]>(
                () => _record.Children
                    .Where(r => r.Tag == GedcomNameRecord.NameTag)
                    .Select(r => new GedcomNameRecord(r))
                    .ToArray());

            _lazyEvents = new Lazy<GedcomIndividualEventRecord[]>(
                () => _record.Children
                    .Where(r => GedcomIndividualEventRecord.EventTags.Contains(r.Tag))
                    .Select(r => new GedcomIndividualEventRecord(r, _database))
                    .ToArray());

            _lazyFamilies = new Lazy<GedcomFamilyLinkRecord[]>(
                () => _record.Children
                    .Where(r => GedcomFamilyLinkRecord.FamilyTags.Contains(r.Tag))
                    .Select(r => new GedcomFamilyLinkRecord(r, _database))
                    .ToArray());
        }

        // ReSharper disable once PossibleInvalidOperationException
        // Checked in the ctor.
        public GedcomPointer CrossReferenceId => _record.CrossReferenceId.Value;

        public int AssumedDeathAge => _database.Settings.AssumedDeathAge;
        
        public string Name => Names.FirstOrDefault()?.Name;

        public string NameWithoutMarker => Name?.Replace("/", "");

        public string FamilyName => Name == null
            ? string.Empty
            : Name.Substring(
                Name.IndexOf("/", StringComparison.Ordinal) + 1, 
                Name.LastIndexOf("/", StringComparison.Ordinal) - Name.IndexOf("/", StringComparison.Ordinal) - 1);

        public GedcomNameRecord[] Names => _lazyNames.Value;
        
        public GedcomSex Sex => _record.Children.FirstOrDefault(r => r.Tag == SexTag)?.Value.AsGedcomSex() ?? GedcomSex.NotKnown;

        public GedcomFamilyLinkRecord[] FamilyLinks => _lazyFamilies.Value;

        public GedcomFamilyRecord[] ChildToFamilies =>
            FamilyLinks.Where(fl => fl.Type == GedcomFamilyType.ChildToFamily)
                .Select(fl => fl.Family)
                .ToArray();

        public GedcomFamilyRecord[] SpouseToFamilies =>
            FamilyLinks.Where(fl => fl.Type == GedcomFamilyType.SpouseToFamily)
                .Select(fl => fl.Family)
                .ToArray();
        
        public GedcomIndividualEventRecord[] Events => _lazyEvents.Value;

        public GedcomIndividualEventRecord BirthEvent =>
            Events.FirstOrDefault(e => e.Tag == GedcomIndividualEventRecord.BirthTag);

        public GedcomIndividualEventRecord DeathEvent =>
            Events.FirstOrDefault(e => e.Tag == GedcomIndividualEventRecord.DeathTag);
    }
}