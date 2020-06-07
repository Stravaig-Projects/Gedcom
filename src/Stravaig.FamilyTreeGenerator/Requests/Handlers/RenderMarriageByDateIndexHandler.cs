using System.IO;
using System.Linq;
using Humanizer;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderMarriageByDateIndexHandler : RenderMarriageIndexBaseHandler
    {
        public class Entry
        {
            public GedcomFamilyRecord Family;
            public GedcomFamilyEventRecord Event;
            public GedcomDateRecord Date;
        }

        private readonly IDateRenderer _dateRenderer;

        public RenderMarriageByDateIndexHandler(ILogger<RenderMarriageByDateIndexHandler> logger, 
            IIndividualNameRenderer nameRenderer,
            IDateRenderer dateRenderer,
            IFileNamer fileNamer) 
            : base(logger, nameRenderer, fileNamer)
        {
            _dateRenderer = dateRenderer;
        }

        protected override string FileName => _fileNamer.GetByMarriageByDateIndexFile();
        protected override void WriteIndex(TextWriter writer, GedcomFamilyRecord[] families)
        {
            writer.WriteLine();
            writer.WriteLine("A list of marriages, civil partnerships and civil unions by date.");
            writer.WriteLine("");
            
            var knowns = families
                .SelectMany(f => f.Events
                    .Where(fe => MarriageTags.Contains(fe.Tag) && (fe.Date?.HasCoherentDate ?? false))
                    .Select(fe => new Entry{Family = f, Event = fe, Date = fe.Date}))
                .Where(m => m.Date.BeginningOfExtent.HasValue)
                .GroupBy(m => m.Date.BeginningOfExtent.Value.Year);

            var byDecade = knowns.GroupBy(g => g.Key / 10);
            var byCentury = byDecade.GroupBy(g => g.Key / 10);

            foreach (var century in byCentury.OrderBy(g=>g.Key))
            {
                writer.WriteLine($"## {(century.Key+1).Ordinalize()} Century");
                writer.WriteLine();
                foreach (var decade in century.OrderBy(g=>g.Key))
                {
                    writer.WriteLine($"### {decade.Key}0s");
                    writer.WriteLine();
                    foreach (var year in decade.OrderBy(g => g.Key))
                    {
                        writer.WriteLine($"* **{year.Key}**");
                        foreach (var entry in year.OrderBy(d => d.Date))
                        {
                            RenderPartnershipLine(writer, entry);
                        }
                    }
                    writer.WriteLine();
                }
            }
            
            WriteUnknownDateMarriages(writer, families);

        }

        private void RenderPartnershipLine(TextWriter writer, Entry entry)
        {
            var partners = entry.Family.Spouses;
            writer.Write("  * ");
            if (entry.Date != null && entry.Date.HasCoherentDate)
            {
                writer.Write("**");
                writer.Write(_dateRenderer.RenderAsShortDate(entry.Date));
                writer.Write("** : ");
            }
            writer.Write(RenderPartner(partners[0]));
            writer.Write(" and ");
            writer.Write(RenderPartner(partners[1]));
            if (entry.Event.Place != null)
            {
                writer.Write(" at ");
                writer.Write(GedcomPlaceRecordExtensions.NormalisedPlaceName((IPlace) entry.Event));
            }

            writer.WriteLine(".");
        }

        private void WriteUnknownDateMarriages(TextWriter writer, GedcomFamilyRecord[] families)
        {
            var unknowns = families
                .SelectMany(f => f.Events
                    .Where(fe => MarriageTags.Contains(fe.Tag) && !(fe.Date?.HasCoherentDate ?? false))
                    .Select(fe => new Entry{Family = f, Event = fe}));
            
            writer.WriteLine("## Unknown Date");
            writer.WriteLine();
            foreach (var entry in unknowns)
            {
                RenderPartnershipLine(writer, entry);
            }
        }
    }
}