using System;
using System.IO;
using Stravaig.FamilyTree.Common.Footnotes;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public interface IStaticFootnoteOrganiser : IFootnoteOrganiser
    {
        void InitFootnotes(Action<GedcomSourceRecord, GedcomIndividualRecord> addSourceAction, GedcomIndividualRecord subject);
        void WriteFootnotes(TextWriter writer, string linkRelativeTo);
        void WriteFootnotes(TextWriter writer, GedcomIndividualRecord linkRelativeTo);
    }
}