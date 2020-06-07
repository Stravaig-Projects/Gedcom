using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public abstract class RenderMarriageIndexBaseHandler : RenderPersonIndexBaseHandler
    {
        public RenderMarriageIndexBaseHandler(ILogger logger, IFileNamer fileNamer) 
            : base(logger, fileNamer)
        {
        }

        protected static readonly GedcomTag[] MarriageTags = new[]
        {
            GedcomFamilyEventRecord.MarriageTag,
        };
        
        protected sealed override void WriteIndex(TextWriter writer, GedcomIndividualRecord[] people)
        {
            var marriedFamilies = people.SelectMany(p => p.SpouseToFamilies)
                .Distinct()
                .Where(f => f.Events.Any(e => MarriageTags.Contains(e.Tag)))
                .ToArray();

            WriteIndex(writer, marriedFamilies);
        }

        protected abstract void WriteIndex(TextWriter writer, GedcomFamilyRecord[] families);
    }
}