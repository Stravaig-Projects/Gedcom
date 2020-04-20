using System;
using System.IO;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public interface IFootnoteOrganiser
    {
        void InitFootnotes(Action<GedcomSourceRecord, GedcomIndividualRecord> addSourceAction, GedcomIndividualRecord subject);
        int AddFootnote(GedcomSourceRecord source);
        int AddFootnote(GedcomNoteRecord note);
        void WriteFootnotes(TextWriter writer);
    }
}