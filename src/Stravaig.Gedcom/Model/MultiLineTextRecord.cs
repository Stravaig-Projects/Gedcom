using System;
using System.Linq;
using System.Text;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model.Parsers;

namespace Stravaig.Gedcom.Model
{
    public abstract class MultiLineTextRecord : Record
    {
        private readonly Lazy<string> _lazyText;
        protected MultiLineTextRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            _lazyText = new Lazy<string>(GetText);
        }

        protected string GetText()
        {
            return MultilineTextParser.GetText(_record);
        }

        public string Text => _lazyText.Value;
    }
}