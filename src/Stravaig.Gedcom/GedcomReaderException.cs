using System;
using System.Runtime.Serialization;

namespace Stravaig.Gedcom
{
    [Serializable]
    public class GedcomReaderException : GedcomException
    {
        public GedcomReaderException(int lineNumber, string message) 
            : base(message)
        {
            LineNumber = lineNumber;
        }
        public GedcomReaderException(int lineNumber, string message, Exception inner) 
            : base(message, inner)
        {
            LineNumber = lineNumber;
        }

        public GedcomReaderException(int lineNumber, int characterPosition, string message, Exception inner) 
            : base(message, inner)
        {
            LineNumber = lineNumber;
            CharacterPosition = characterPosition;
        }

        protected GedcomReaderException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
        
        public int LineNumber { get; }
        
        public int? CharacterPosition { get; }
    }
}