using System;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomFileRecord : Record
    {
        public static readonly GedcomTag FileTag = "FILE".AsGedcomTag();
        public static readonly GedcomTag FormTag = "FORM".AsGedcomTag();

        private Lazy<string> _form;


        public GedcomFileRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            _form = new Lazy<string>(GetForm);
        }

        private string GetForm()
        {
            var formRecord = _record.Children.FirstOrDefault(r => r.Tag == GedcomFileRecord.FormTag);
            if (formRecord == null)
                return null;

            return formRecord.Value;
        }




        public string Name => _record.Value;

        public string Form => _form.Value;
    }
}