using System;
using System.Text;

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
                .Replace('*', '-')
                .Replace(':', '-')
                .Replace('/', '-')
                .Replace('\\', '-')
                .Replace('(', '-')
                .Replace(')', '-')
                .Replace(' ', '-');

            int len;
            do
            {
                len = result.Length;
                result = result.Replace("--", "-");
            } while (result.Length < len);
            if (len > 100)
                result = result.Substring(0, 99) + ellipsis;
            return result;
        }
        
        public static string RenderLinksAsMarkdown(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return source;
            
            StringBuilder sb = new StringBuilder(source.Length);
            string[] lines = source.Split(Environment.NewLine);

            foreach (string line in lines)
            {
                string[] words = line.Split((char[])null, StringSplitOptions.None);
                bool isFirst = true;
                foreach (string word in words)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        sb.Append(" ");
                    if (word.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) ||
                        word.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                    {
                        sb.Append($"[{word}]({word})");
                    }
                    else
                    {
                        sb.Append(word);
                    }

                }

                sb.AppendLine();
            }

            sb.Remove(sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            
            return sb.ToString();
        }
    }
}