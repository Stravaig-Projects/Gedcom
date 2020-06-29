using System;
using System.IO;
using Stravaig.FamilyTree.Common.Footnotes;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public abstract class StaticFootnoteOrganiser : FootnoteOrganiser, IStaticFootnoteOrganiser
    {
        protected Action<GedcomSourceRecord, GedcomIndividualRecord> _addSource;
        protected GedcomIndividualRecord _subject;
        
        public void InitFootnotes(Action<GedcomSourceRecord, GedcomIndividualRecord> addSourceAction, GedcomIndividualRecord subject)
        {
            _addSource = addSourceAction;
            _subject = subject;
        }

        public abstract void WriteFootnotes(TextWriter writer, string linkRelativeTo);
        public abstract void WriteFootnotes(TextWriter writer, GedcomIndividualRecord linkRelativeTo);
    }
}