using System;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public interface IIndividualNameRenderer
    {
        string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, string linkLocationInRelationTo = null, bool boldName = false);
        string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, GedcomSourceRecord inRelationToSource, bool boldName = false);
        string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, GedcomIndividualRecord inRelationToPerson, bool boldName = false);
    }
}