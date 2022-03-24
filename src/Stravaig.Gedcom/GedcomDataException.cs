using System;
using System.Runtime.Serialization;

namespace Stravaig.Gedcom
{
    [Serializable]
    public class GedcomDataException : GedcomException
    {
        public GedcomDataException()
        {
        }

        public GedcomDataException(string message) : base(message)
        {
        }

        public GedcomDataException(string message, Exception inner) : base(message, inner)
        {
        }

        protected GedcomDataException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}