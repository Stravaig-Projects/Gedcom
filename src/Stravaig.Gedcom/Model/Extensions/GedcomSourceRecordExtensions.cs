using System;
using System.Linq;

namespace Stravaig.Gedcom.Model.Extensions
{
    public static class GedcomSourceRecordExtensions
    {
        public static bool IsReferencedByLivingPerson(this GedcomSourceRecord source)
        {
            var referred = source.ReferencedBy;
            foreach (var item in referred)
            {
                switch (item)
                {
                    case ISubject relatedToSubject:
                    {
                        var subject = relatedToSubject.Subject;
                        if (subject.IsAlive())
                            return true;
                        break;
                    }
                    case ISubjects relatedToSubjects:
                    {
                        var subjects = relatedToSubjects.Subjects;
                        if (subjects.Any(s => s.IsAlive()))
                            return true;
                        break;
                    }
                    default:
                        throw new InvalidOperationException($"item ({item.GetType().FullName}) was expected to be cast to an ISubject or ISubjects interface, but it does not implement either.");
                }
            }

            return false;
        }
    }
}