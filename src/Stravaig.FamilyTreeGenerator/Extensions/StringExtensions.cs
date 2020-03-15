using System;

namespace Stravaig.FamilyTreeGenerator.Extensions
{
    public static class StringExtensions
    {
        public static string MakeFileNameSafe(this string target)
        {
            if (target == null)
                return null;
            
            if (string.IsNullOrWhiteSpace(target))
                return string.Empty;

            return target.Replace("?", "-")
                .Replace(":", "-")
                .Replace("/", "-")
                .Replace("\\", "-");
        }
    }
}