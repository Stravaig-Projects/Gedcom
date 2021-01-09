using System;
using System.Linq;
using Stravaig.FamilyTree.Common.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.PowerShell
{
    // ReSharper disable once InconsistentNaming
    // This is the PowerShell standard naming.
    public class PSGedcomDatabase
    {
        private readonly Lazy<PSGedcomIndividual[]> _individualRecords;
        
        public PSGedcomDatabase(GedcomDatabase database)
        {
            _individualRecords = new Lazy<PSGedcomIndividual[]>(
                () => database.IndividualRecords
                    .Select(kvp => kvp.Value)
                    .OrderByStandardSort()
                    .Select(ir => new PSGedcomIndividual(ir))
                    .ToArray());
        }

        public PSGedcomIndividual[] Individuals => _individualRecords.Value;
    }
}