namespace Stravaig.FamilyTreeGenerator.Extensions
{
    public static class StringExtensions
    {
        private const char ellipsis = (char) 0x2026; // …
        public static string MakeFileNameSafe(this string target)
        {
            if (target == null)
                return null;
            
            if (string.IsNullOrWhiteSpace(target))
                return string.Empty;

            string result = target.Replace("?", "-")
                .Replace("*", "-")
                .Replace(":", "-")
                .Replace("/", "-")
                .Replace("\\", "-");
            if (result.Length > 100)
                result = result.Substring(0, 99) + ellipsis;
            return result;
        }
    }
}