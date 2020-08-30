using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers.Services
{
    public abstract class TimelineMarkdownRendererBase
    {
        protected TextWriter _writer;
        protected IStaticFootnoteOrganiser _footnoteOrganiser;
        
        protected void WriteFootnoteLinks(IEnumerable<int> footnoteIds)
        {
            bool isFirst = true;
            foreach (int id in footnoteIds)
            {
                if (isFirst)
                    isFirst = false;
                else
                    _writer.Write(", ");
                _writer.Write($"[{id}](#{id})");
            }
        }

        protected IEnumerable<int> GetSourceFootnotes(EventRecord eventRecord)
        {
            return eventRecord.Sources
                .OrderBy(s => s.Title)
                .Select(s => _footnoteOrganiser.AddFootnote(s));
        }

        protected IEnumerable<int> GetNoteFootnotes(EventRecord eventRecord)
        {
            var subjects = eventRecord switch
            {
                ISubject single => new[] {single.Subject},
                ISubjects multiple => multiple.Subjects,
                _ => Array.Empty<GedcomIndividualRecord>(),
            };
            if (subjects.Any(s => s.IsAlive()))
                return Array.Empty<int>();
            
            return eventRecord.Notes
                .OrderBy(n => n.Text)
                .Select(n => _footnoteOrganiser.AddFootnote(n));
        }

    }
}