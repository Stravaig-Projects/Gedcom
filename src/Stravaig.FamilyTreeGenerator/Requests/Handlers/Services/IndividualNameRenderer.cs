using System;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public interface IIndividualNameRenderer
    {
        string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, string linkLocationInRelationTo = null);
        string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, GedcomSourceRecord inRelationToSource);
        string RenderLinkedNameWithLifespan(GedcomIndividualRecord subject, GedcomIndividualRecord inRelationToPerson);
    }
}