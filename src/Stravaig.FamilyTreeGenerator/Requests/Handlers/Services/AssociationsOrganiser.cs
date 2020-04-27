using System.Collections.Generic;
using System.Linq;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public class AssociatesOrganiser : IAssociatesOrganiser
    {
        private List<GedcomIndividualRecord> _associates;

        public AssociatesOrganiser()
        {
            _associates = new List<GedcomIndividualRecord>();
        }

        public void AddAssociate(GedcomIndividualRecord associate)
        {
            if (_associates.Any(a => a.CrossReferenceId == associate.CrossReferenceId))
                return;

            _associates.Add(associate);
        }

        public GedcomIndividualRecord[] Associates => _associates.ToArray();
    }
}