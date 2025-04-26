using System;
using Stravaig.FamilyTreeGenerator.Requests.Models;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests;

public class RenderSource : Request
{
    public SourceEntry SourceEntry { get; }

    public Action<GedcomObjectRecord> AddObject { get; }

    public RenderSource(SourceEntry sourceEntry, Action<GedcomObjectRecord> addObjectAction)
    {
        SourceEntry = sourceEntry;
        AddObject = addObjectAction;
    }
}
