using System;
using System.Diagnostics;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    [DebuggerDisplay("{DebugDisplay}")]
    public class GedcomIndividualRecord : Record, ISubject
    {
        public static readonly GedcomTag Tag = "INDI".AsGedcomTag();
        public static readonly GedcomTag SexTag = "SEX".AsGedcomTag();
        public static readonly GedcomTag RestrictionNoticeTag = "RESN".AsGedcomTag();
        public static readonly GedcomTag FamilySearchIdTag = "_FID".AsGedcomTag();

        private readonly Lazy<GedcomNameRecord[]> _lazyNames;
        private readonly Lazy<GedcomIndividualEventRecord[]> _lazyEvents;
        private readonly Lazy<GedcomFamilyLinkRecord[]> _lazyFamilies;
        private readonly Lazy<GedcomNoteRecord[]> _lazyNotes;
        private readonly Lazy<string> _lazyFamilySearchId;
        private readonly Lazy<GedcomIndividualAttributeRecord[]> _lazyAttributes;
        private readonly Lazy<GedcomSourceRecord[]> _lazySources;
        private readonly Lazy<GedcomObjectRecord[]> _lazyObjects;

        public GedcomIndividualRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (record.Tag != Tag)
                throw new ArgumentException($"Must be an \"INDIVIDUAL_RECORD\" ({Tag}) record, but was {record.Tag}.");
            if (!record.CrossReferenceId.HasValue)
                throw new ArgumentException("An \"INDIVIDUAL_RECORD\" must have a CrossReferenceId.");

            _lazyNames = new Lazy<GedcomNameRecord[]>(
                () => _record.Children
                    .Where(r => r.Tag == GedcomNameRecord.NameTag)
                    .Select(r => new GedcomNameRecord(r, _database))
                    .ToArray());

            _lazyEvents = new Lazy<GedcomIndividualEventRecord[]>(
                () => _record.Children
                    .Where(r => GedcomIndividualEventRecord.EventTags.Contains(r.Tag))
                    .Select(r => new GedcomIndividualEventRecord(r, _database, this))
                    .ToArray());

            _lazyFamilies = new Lazy<GedcomFamilyLinkRecord[]>(
                () => _record.Children
                    .Where(r => GedcomFamilyLinkRecord.FamilyTags.Contains(r.Tag))
                    .Select(r => new GedcomFamilyLinkRecord(r, _database))
                    .ToArray());

            _lazyObjects = new Lazy<GedcomObjectRecord[]>(
                () => _record.Children
                    .Where(r => r.Tag == GedcomObjectRecord.ObjectTag && !string.IsNullOrWhiteSpace(r.Value))
                    .Select(r => _database.ObjectRecords[r.Value.AsGedcomPointer()])
                    .ToArray());

            _lazyNotes = new Lazy<GedcomNoteRecord[]>(GetNoteRecords);
            _lazyFamilySearchId = new Lazy<string>(GetFamilySearchId);
            _lazyAttributes = new Lazy<GedcomIndividualAttributeRecord[]>(GetAttributeRecords);
            _lazySources = new Lazy<GedcomSourceRecord[]>(GetSourceRecords);
        }

        #if DEBUG
        internal string DebugDisplay
        {
            get
            {
                if (_lazyNames.IsValueCreated)
                    return $"{GetType().Name}: {CrossReferenceId} {Name} (b:{BirthEvent?.Date?.RawDateValue ?? "Unknown"} - d:{DeathEvent?.Date?.RawDateValue ?? "Unknown"})";

                return $"{GetType().Name}: {_record.DebugDisplay}";
            }
        }
        #endif

        private string GetFamilySearchId()
        {
            // This is a custom tag used by Synium Software GmbH Germany (syniumsoftware.com)
            var record = _record.Children.FirstOrDefault(r => r.Tag == FamilySearchIdTag);
            return record?.Value;
        }

        private GedcomIndividualAttributeRecord[] GetAttributeRecords()
        {
            return _record.Children
                .Where(r => GedcomIndividualAttributeRecord.AttributeTags.Contains(r.Tag))
                .Select(r => new GedcomIndividualAttributeRecord(r, _database, this))
                .ToArray();
        }

        // ReSharper disable once PossibleInvalidOperationException
        // Checked in the ctor.
        public GedcomPointer CrossReferenceId => _record.CrossReferenceId.Value;

        public GedcomObjectRecord[] Objects => _lazyObjects.Value;

        public string RestrictionNotice => _record.Children
            .FirstOrDefault(r => r.Tag == RestrictionNoticeTag)
            ?.Value;

        public int AssumedDeathAge => _database.Settings.AssumedDeathAge;

        public string Name => Names.FirstOrDefault()?.Name;

        public string NameWithoutMarker => Name?.Replace("/", "").Trim();

        public string FamilyName => Name == null
            ? string.Empty
            : Name.Substring(
                Name.IndexOf("/", StringComparison.Ordinal) + 1,
                Name.LastIndexOf("/", StringComparison.Ordinal) - Name.IndexOf("/", StringComparison.Ordinal) - 1)
                .Trim();

        public string GivenName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                    return string.Empty;

                var result = NameWithoutMarker;
                var familyName = FamilyName;
                if (familyName.HasContent())
                    result = result.Replace(familyName, string.Empty).Replace("  ", " ").Trim();
                return result;
            }
        }

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
        public GedcomIndividualAttributeRecord[] Attributes => _lazyAttributes.Value;
        public GedcomSourceRecord[] Sources => _lazySources.Value;

        public GedcomIndividualEventRecord BirthEvent =>
            Events.FirstOrDefault(e => e.Tag == GedcomIndividualEventRecord.BirthTag);

        public GedcomIndividualEventRecord DeathEvent =>
            Events.FirstOrDefault(e => e.Tag == GedcomIndividualEventRecord.DeathTag);

        public GedcomNoteRecord[] Notes => _lazyNotes.Value;

        public string FamilySearchId => _lazyFamilySearchId.Value;
        public GedcomIndividualRecord Subject => this;

        public override int GetHashCode()
        {
            return CrossReferenceId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is GedcomIndividualRecord other)
                return (this == other);
            return false;
        }

        public static bool operator ==(GedcomIndividualRecord a, GedcomIndividualRecord b) =>
            ((object) a == null && (object) b == null) ||
            (
                !((object) a == null || (object) b == null) &&
                a.CrossReferenceId == b.CrossReferenceId
            );

        public static bool operator !=(GedcomIndividualRecord a, GedcomIndividualRecord b) => !(a == b);
    }
}
