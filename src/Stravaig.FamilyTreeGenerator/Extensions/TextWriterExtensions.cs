using System;
using System.IO;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.FamilyTreeGenerator.Extensions
{
    public static class TextWriterExtensions
    {
        public static void WriteMarkdownBlockQuote(this TextWriter writer, string text)
        {
            if (text.HasContent())
            {
                string[] lines = text.Split(Environment.NewLine);
                foreach (string line in lines)
                {
                    string escapedLine = line.Replace("~", "\\~");
                    if (string.IsNullOrWhiteSpace(escapedLine))
                        escapedLine = "<br/>";
                    writer.Write("> ");
                    writer.WriteLine(escapedLine);
                    writer.WriteLine(">");
                }
                writer.WriteLine();
            }
        }
    }
}