using System;
using System.Runtime.Serialization;

namespace Stravaig.Gedcom
{
    [Serializable]
    public class GedcomLineParserException : GedcomException
    {
        public GedcomLineParserException(string message, int characterPosition) 
            : base(message)
        {
            CharacterPosition = characterPosition;
        }

        public GedcomLineParserException(string message, int characterPosition, Exception innerException) 
            : base(message, innerException)
        {
            CharacterPosition = characterPosition;
        }

        protected GedcomLineParserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
        
        public int CharacterPosition { get; }
    }
}