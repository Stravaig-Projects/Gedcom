using System;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderPersonIndexByDeathLocationHandler : RenderPersonIndexByLocationBaseHandler
    {
        public RenderPersonIndexByDeathLocationHandler(
            ILogger<RenderPersonIndexByNameAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
            : base(logger, nameRenderer, fileNamer)
        {
        }

        protected override string FileName => _fileNamer.GetByDeathLocationIndexFile();


        protected override string[] GetPlaceName(GedcomIndividualRecord p)
        {
            return p.DeathEvent?.Place?.Name
                       .Split(',', StringSplitOptions.RemoveEmptyEntries)
                   ?? Array.Empty<string>();
        }
    }
}