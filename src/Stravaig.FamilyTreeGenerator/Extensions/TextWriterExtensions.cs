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
                text = text.Trim().RenderLinksAsMarkdown();
                string[] lines = text.Split(Environment.NewLine);
                bool isInTable = false;
                foreach (string line in lines)
                {
                    if (line.Contains('|'))
                    {
                        if (!isInTable)
                            writer.WriteLine();
                        isInTable = true;
                    }
                    else
                    {
                        if (isInTable)
                            writer.WriteLine();
                        isInTable = false;
                    }

                    string escapedLine = line.Replace("~", "\\~");
                    if (string.IsNullOrWhiteSpace(escapedLine))
                        escapedLine = "<br/>";

                    // Don't put tables in block quotes.
                    if (!isInTable)
                        writer.Write("> ");

                    writer.WriteLine(escapedLine);
                    
                    // If this is a table, don't put in the new line.
                    if (!isInTable)
                        writer.WriteLine(">");
                }
                writer.WriteLine();
            }
        }
    }
}
