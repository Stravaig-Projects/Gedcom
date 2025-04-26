using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests;

public class RenderObject : Request
{
    public GedcomObjectRecord Object { get; }

    public RenderObject(GedcomObjectRecord @object)
    {
        Object = @object;
    }
}