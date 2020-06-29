using Microsoft.AspNetCore.Components;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom;

namespace Stravaig.FamilyTree.Blazor.Shared.Timeline
{
    public abstract class EventBase : ComponentBase
    {
        [Parameter]
        public TimelineEntry Entry { get; set; }

        protected abstract EventRecord Event { get; }
        protected GedcomTag Tag => Event.Tag;
    }
}