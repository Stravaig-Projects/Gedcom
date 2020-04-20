using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public abstract class FootnoteOrganiser : IFootnoteOrganiser
    {
                public class Footnote
        {
            public int Id { get; set; }
            public GedcomSourceRecord Source { get; set; }
            public GedcomNoteRecord Note { get; set; }

            public static Footnote Null => new Footnote();
        }

        protected readonly List<Footnote> _footnotes;
        protected Action<GedcomSourceRecord, GedcomIndividualRecord> _addSource;
        protected GedcomIndividualRecord _subject;
        
        protected FootnoteOrganiser()
        {
            _footnotes = new List<Footnote>();
        }
        
        public void InitFootnotes(Action<GedcomSourceRecord, GedcomIndividualRecord> addSourceAction, GedcomIndividualRecord subject)
        {
            _addSource = addSourceAction;
            _subject = subject;
        }
        
        public int AddFootnote(GedcomSourceRecord source)
        {
            var existing = _footnotes.FirstOrDefault(f => f.Source != null &&
                                                          f.Source.CrossReferenceId == source.CrossReferenceId);
            if (existing != null)
                return existing.Id;

            int newId = _footnotes
                .Where(f => f.Source != null)
                .DefaultIfEmpty(Footnote.Null)
                .Max(f => f.Id) + 1;

            var footnote = new Footnote
            {
                Id = newId,
                Source = source,
            };
            _footnotes.Add(footnote);
            return newId;
        }

        public int AddFootnote(GedcomNoteRecord note)
        {
            var existing = _footnotes.Where(f => f.Note != null)
                .FirstOrDefault(f => note.CrossReferenceId.HasValue
                    ? f.Note.CrossReferenceId == note.CrossReferenceId
                    : f.Note.Text == note.Text);
            
            if (existing != null)
                return existing.Id;
            
            int newId = _footnotes
                .Where(f => f.Note != null)
                .DefaultIfEmpty(Footnote.Null)
                .Max(f => f.Id) + 1;

            var footnote = new Footnote
            {
                Id = newId,
                Note = note,
            };
            _footnotes.Add(footnote);
            return newId;
        }

        public abstract void WriteFootnotes(TextWriter writer);
    }
    public class MarkdownFootnoteOrganiser : FootnoteOrganiser
    {
        public override void WriteFootnotes(TextWriter writer)
        {
            if (_footnotes.Any())
            {
                writer.WriteLine("## Footnotes");
                writer.WriteLine();
                foreach (var footnote in _footnotes.OrderBy(f=>f.Id))
                {
                    writer.WriteLine($"### {footnote.Id}");
                    writer.WriteLine();
                    if (footnote.Note != null)
                        writer.WriteMarkdownBlockQuote(footnote.Note.Text);
                    else if (footnote.Source != null)
                    {
                        _addSource(footnote.Source, _subject);
                        if (footnote.Source.Title.HasContent())
                            writer.WriteLine($"**{footnote.Source.Title}**");
                        
                        if (footnote.Source.Text.HasContent())
                            writer.WriteMarkdownBlockQuote(footnote.Source.Text);
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}