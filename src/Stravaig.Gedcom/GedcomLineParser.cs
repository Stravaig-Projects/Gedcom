using System;
using System.Text;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom
{
    public class GedcomLineParser
    {
        private const char Delim = (char) 0x20;
        enum State
        {
            AtStart,
            ProcessedLevel,
            ProcessedCrossReferenceId,
            ProcessedTag,
            ProcessingLineValue
        }
        private readonly GedcomLineBuilder _builder = new GedcomLineBuilder();

        public GedcomLine Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentException("Line cannot be null or whitespace.", nameof(line));
            if (line.Length > 255)
                throw new GedcomLineParserException("Line cannot be longer than 255 characters.", 0);
            
            _builder.Reset();
            GedcomLine result = Build(line);
            return result;
        }

        private GedcomLine Build(string line)
        {
            string newLine = line.TrimStart();
            int position = line.Length - newLine.Length;
            State state = State.AtStart;
            string[] tokens = newLine.Split(Delim);
            StringBuilder valueSb = null;
            foreach (string token in tokens)
            {
                switch (state)
                {
                    case State.AtStart:
                        ProcessLevel(token, position);
                        state = State.ProcessedLevel;
                        break;
                    case State.ProcessedLevel:
                        state = ProcessCrossReferenceIdOrTag(token, position);
                        break;
                    case State.ProcessedCrossReferenceId:
                        ProcessTag(token, position);
                        state = State.ProcessedTag; 
                        break;
                    case State.ProcessedTag:
                        valueSb = new StringBuilder(token);
                        state = State.ProcessingLineValue;
                        break;
                    case State.ProcessingLineValue:
                        valueSb.Append(Delim);
                        valueSb.Append(token);
                        break;
                }

                position += token.Length;
                position++; // move past the delimiter
            }

            if (valueSb != null)
                _builder.SetValue(valueSb.ToString());
            
            return _builder.Build();
        }

        private State ProcessCrossReferenceIdOrTag(string token, int position)
        {
            if (token.IsGedcomPointer())
            {
                ProcessCrossReferenceId(token, position);
                return State.ProcessedCrossReferenceId;
            }
            if (token.IsGedcomTag())
            {
                ProcessTag(token, position);
                return State.ProcessedTag;
            }
            throw new GedcomLineParserException(
                $"Expected a CrossReferenceId or Tag value, at character position {position}.",
                position);
        }

        private void ProcessLevel(string token, int position)
        {
            try
            {
                var level = new GedcomLevel(token);
                _builder.SetLevel(level);
            }
            catch (Exception ex)
            {
                throw new GedcomLineParserException(
                    $"Expected a valid level value, at character position {position}.",
                    position,
                    ex);
            }
        }

        private void ProcessCrossReferenceId(string token, int position)
        {
            try
            {
                var pointer = new GedcomPointer(token);
                _builder.SetCrossReferenceId(pointer);
            }
            catch (Exception ex)
            {
                throw new GedcomLineParserException(
                    $"Expected a pointer value, at character position {position}.",
                    position,
                    ex);
            }
        }
        
        private void ProcessTag(string token, int position)
        {
            try
            {
                var tag = new GedcomTag(token);
                _builder.SetTag(tag);
            }
            catch (Exception ex)
            {
                throw new GedcomLineParserException(
                    $"Expected a tag value, at character position {position}.",
                    position,
                    ex);
            }
        }
        
    }
}