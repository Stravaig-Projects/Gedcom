using System;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public class RenderIndividual : Request
    {
        public GedcomIndividualRecord Individual { get; }
        public Action<GedcomSourceRecord, GedcomIndividualRecord> AddSource { get; }

        public RenderIndividual(GedcomIndividualRecord individual, Action<GedcomSourceRecord, GedcomIndividualRecord> addSourceAction)
        {
            Individual = individual;
            AddSource = addSourceAction;
        }
    }
}