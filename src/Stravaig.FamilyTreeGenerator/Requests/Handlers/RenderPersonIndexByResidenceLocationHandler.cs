using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderPersonIndexByResidenceLocationHandler : RenderPersonIndexByMultipleLocationsBaseHandler
    {
        public RenderPersonIndexByResidenceLocationHandler(ILogger<RenderPersonIndexByNameAsMarkdownHandler> logger, IIndividualNameRenderer nameRenderer, IFileNamer fileNamer) : base(logger, nameRenderer, fileNamer)
        {
        }

        protected override string FileName => _fileNamer.GetByResidenceLocationIndexFile();
        
        protected override string[][] GetPlaceNames(GedcomIndividualRecord person)
        {
            return person.Attributes
                .Where(a => a.Tag == GedcomIndividualAttributeRecord.ResidenceTag)
                .Select(a => a.Place?.Name)
                .Where(p => p.HasContent())
                .Distinct()
                .Select(p => p.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .ToArray();
        }
    }
}