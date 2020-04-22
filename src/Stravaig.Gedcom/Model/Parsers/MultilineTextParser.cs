using System.Linq;
using System.Text;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model.Parsers
{
    public static class MultilineTextParser
    {
        private static readonly GedcomTag ContinuedTag = "CONT".AsGedcomTag();
        private static readonly GedcomTag ConcatenatedTag = "CONC".AsGedcomTag();
        private static readonly GedcomTag[] TextTags =
        {
            ContinuedTag, 
            ConcatenatedTag,
        };
        
        public static string GetText(GedcomRecord record)
        {
            bool isFirst = true;
            StringBuilder sb = new StringBuilder();
            if (record.Value.HasContent())
            {
                sb.Append(record.Value);
                isFirst = false;
            }

            var textEntries = record.Children.Where(r => TextTags.Contains(r.Tag));
            foreach (var textEntry in textEntries)
            {
                if (isFirst)
                    isFirst = false;
                else if (textEntry.Tag == ContinuedTag)
                    sb.AppendLine();
                sb.Append(textEntry.Value);
            }

            return sb.ToString();
        }

    }
}