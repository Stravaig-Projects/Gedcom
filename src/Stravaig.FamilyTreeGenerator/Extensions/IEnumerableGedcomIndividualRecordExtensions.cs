using System.Collections.Generic;
using System.Linq;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Extensions
{
    public static class IEnumerableGedcomIndividualRecordExtensions
    {
        public static IEnumerable<GedcomIndividualRecord> OrderByStandardSort(
            this IEnumerable<GedcomIndividualRecord> sequence)
        {
            return sequence.OrderBy(s => s.FamilyName)
                .ThenBy(s => s.NameWithoutMarker)
                .ThenBy(s => s.BirthEvent?.Date);
        }
    }
}