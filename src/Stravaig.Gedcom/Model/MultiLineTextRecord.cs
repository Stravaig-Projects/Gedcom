using System;
using System.Linq;
using System.Text;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public abstract class MultiLineTextRecord : Record
    {
        private static readonly GedcomTag ContinuedTag = "CONT".AsGedcomTag();
        private static readonly GedcomTag ConcatenatedTag = "CONC".AsGedcomTag();
        private static readonly GedcomTag[] TextTags =
        {
            ContinuedTag, 
            ConcatenatedTag,
        };

        private readonly Lazy<string> _lazyText;
        protected MultiLineTextRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            _lazyText = new Lazy<string>(GetText);
        }

        protected string GetText()
        {
            bool isFirst = true;
            StringBuilder sb = new StringBuilder();
            if (_record.Value.HasContent())
            {
                sb.Append(_record.Value);
                isFirst = false;
            }

            var textEntries = _record.Children.Where(r => TextTags.Contains(r.Tag));
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

        public string Text => _lazyText.Value;
    }
}