using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderMarriageByNameIndexHandler : RenderMarriageIndexBaseHandler
    {
        private readonly IDateRenderer _dateRenderer;

        public class Entry
        {
            public GedcomFamilyRecord Family;
            public GedcomFamilyEventRecord Event;
            public GedcomIndividualRecord Subject;
            public GedcomIndividualRecord Partner;
            public GedcomDateRecord Date;
        }
        public RenderMarriageByNameIndexHandler(ILogger<RenderMarriageByNameIndexHandler> logger, 
            IDateRenderer dateRenderer,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer) 
            : base(logger, nameRenderer, fileNamer)
        {
            _dateRenderer = dateRenderer;
        }

        protected override string FileName => _fileNamer.GetByMarriageByNameIndexFile();
        protected override void WriteIndex(TextWriter writer, GedcomFamilyRecord[] families)
        {
            var marriages = families
                .SelectMany(f => f.Events
                    .Where(fe => MarriageTags.Contains(fe.Tag))
                    .SelectMany(fe => new[]
                    {
                        new Entry {Family = f, Event = fe, Subject = f.Spouses[0], Partner = f.Spouses[1], Date = fe.Date},
                        new Entry {Family = f, Event = fe, Subject = f.Spouses[1], Partner = f.Spouses[0], Date = fe.Date},
                    }))
                .Where(m => m.Subject.IsDead())
                .GroupBy(m => m.Subject.FamilyName)
                .OrderBy(m => m.Key)
                .ToArray();

            foreach (var group in marriages)
            {
                writer.WriteLine($"## {group.Key}");
                writer.WriteLine();

                var familyMarriages = group
                    .OrderBy(m => m.Subject.GivenName)
                    .ThenBy(m => m.Date)
                    .ToArray();

                foreach (var marriage in familyMarriages)
                {
                    RenderPartnershipLine(writer, marriage);
                }
                writer.WriteLine();
            }
        }
        private void RenderPartnershipLine(TextWriter writer, Entry entry)
        {
            writer.Write("  * ");
            writer.Write(RenderPartner(entry.Subject));
            writer.Write(" and ");
            writer.Write(RenderPartner(entry.Partner));
            
            if (entry.Date != null && entry.Date.HasCoherentDate)
            {
                writer.Write(" on ");
                writer.Write("**");                
                writer.Write(_dateRenderer.RenderAsShortDate(entry.Date));
                writer.Write("**");
            }

            if (entry.Event.Place != null)
            {
                writer.Write(" at ");
                writer.Write(GedcomPlaceRecordExtensions.NormalisedPlaceName((IPlace) entry.Event));
            }

            writer.WriteLine(".");
        }
    }
}