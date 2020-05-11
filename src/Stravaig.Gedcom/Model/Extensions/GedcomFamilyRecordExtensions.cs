using System.Linq;

namespace Stravaig.Gedcom.Model.Extensions
{
    public static class GedcomFamilyRecordExtensions
    {
        public static bool IsSpouse(this GedcomFamilyRecord family, GedcomIndividualRecord subject)
        {
            return family.Spouses.Any(s => s == subject);
        }

        public static bool IsChild(this GedcomFamilyRecord family, GedcomIndividualRecord subject)
        {
            return family.Children.Any(c => c == subject);
        }

        public static GedcomIndividualRecord OtherSpouse(this GedcomFamilyRecord family, GedcomIndividualRecord subject)
        {
            if (family.IsSpouse(subject))
                return family.Spouses.First(s => s != subject);
            return null;
        }
    }
}