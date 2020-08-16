using System;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderPersonIndexByBirthLocationHandler : RenderPersonIndexBySingleLocationBaseHandler
    {
        public RenderPersonIndexByBirthLocationHandler(
            ILogger<RenderPersonIndexByNameAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
            : base(logger, nameRenderer, fileNamer)
        {
        }

        protected override string FileName => _fileNamer.GetByBirthLocationIndexFile();


        protected override string[] GetPlaceName(GedcomIndividualRecord p)
        {
            return p.BirthEvent?.Place?.Name
                       .Split(',', StringSplitOptions.RemoveEmptyEntries)
                   ?? Array.Empty<string>();
        }
    }
}