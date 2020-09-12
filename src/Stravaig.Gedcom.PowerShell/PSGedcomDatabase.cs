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
        private readonly GedcomDatabase _database;
        private readonly Lazy<PSGedcomIndividualRecord[]> _individualRecords;
        
        public PSGedcomDatabase(GedcomDatabase database)
        {
            _database = database;
            _individualRecords = new Lazy<PSGedcomIndividualRecord[]>(
                () => _database.IndividualRecords
                    .Select(kvp => kvp.Value)
                    .OrderByStandardSort()
                    .Select(ir => new PSGedcomIndividualRecord(ir))
                    .ToArray());
        }

        public PSGedcomIndividualRecord[] Individuals => _individualRecords.Value;
    }
}