using System;
using System.Runtime.Serialization;

namespace Stravaig.Gedcom
{
    [Serializable]
    public class GedcomException : Exception
    {
        public GedcomException()
        {
        }

        public GedcomException(string message) : base(message)
        {
        }

        public GedcomException(string message, Exception inner) : base(message, inner)
        {
        }

        protected GedcomException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}