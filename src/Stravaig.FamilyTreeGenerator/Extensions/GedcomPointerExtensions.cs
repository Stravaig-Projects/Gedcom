using Stravaig.Gedcom;

namespace Stravaig.FamilyTreeGenerator.Extensions
{
    public static class GedcomPointerExtensions
    {
        public static string ToSimpleId(this GedcomPointer pointer)
        {
            return pointer.ToString().Trim('@').ToLowerInvariant();
        }
    }
}