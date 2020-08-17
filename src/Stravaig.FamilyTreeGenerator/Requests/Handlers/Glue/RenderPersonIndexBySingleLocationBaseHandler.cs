using System;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public abstract class RenderPersonIndexBySingleLocationBaseHandler : RenderPersonIndexByMultipleLocationsBaseHandler
    {
        protected RenderPersonIndexBySingleLocationBaseHandler(
            ILogger<RenderPersonIndexByNameAsMarkdownHandler> logger,
            IIndividualNameRenderer nameRenderer,
            IFileNamer fileNamer)
            : base(logger, nameRenderer, fileNamer)
        {
        }

        protected override string[][] GetPlaceNames(GedcomIndividualRecord person)
        {
            var placeName = GetPlaceName(person);
            if (placeName == null || placeName.NotAny())
                return new[]
                {
                    Array.Empty<string>()
                };
            
            return new[]
            {
                placeName
            };
        }

        protected abstract string[] GetPlaceName(GedcomIndividualRecord person);
    }
}