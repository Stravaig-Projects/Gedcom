using System.Collections.Generic;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Models
{
    public class SourceEntry
    {
        public GedcomSourceRecord Source { get; }
        public List<GedcomIndividualRecord> ReferencedByIndividuals { get; }

        public SourceEntry(GedcomSourceRecord source, GedcomIndividualRecord individual)
        {
            Source = source;
            ReferencedByIndividuals = new List<GedcomIndividualRecord>();
            ReferencedByIndividuals.Add(individual);
        }
    }}