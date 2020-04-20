using Stravaig.FamilyTreeGenerator.Requests.Models;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public class RenderSourceIndex : Request
    {
        public SourceEntry[] SourceEntries { get; }

        public RenderSourceIndex(SourceEntry[] sourceEntries)
        {
            SourceEntries = sourceEntries;
        }
    }
}