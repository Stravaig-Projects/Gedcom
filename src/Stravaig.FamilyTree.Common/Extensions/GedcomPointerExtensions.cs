using Stravaig.Gedcom;

namespace Stravaig.FamilyTree.Common.Extensions
{
    public static class GedcomPointerExtensions
    {
        public static string ToSimpleId(this GedcomPointer pointer)
        {
            return pointer.ToString()
                .Trim('@')
                .ToLowerInvariant();
        }

        public static string ToSimpleIndividualId(this GedcomPointer pointer)
        {
            string result = pointer.ToSimpleId();
            if (!result.StartsWith("i"))
                result = $"i{result}";
            return result;
        }

        public static string ToSimpleSourceId(this GedcomPointer pointer)
        {
            string result = pointer.ToSimpleId();
            if (!result.StartsWith("s"))
                result = $"s{result}";
            return result;
        }
    }
}