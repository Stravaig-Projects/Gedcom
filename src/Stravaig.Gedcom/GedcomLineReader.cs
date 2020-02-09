using System;
using System.IO;
using System.Threading.Tasks;

namespace Stravaig.Gedcom
{
    public class GedcomLineReader : IDisposable
    {
        private readonly GedcomLineParser _parser;
        private readonly TextReader _reader;
        private int _lineNumber;
        private bool _isAtEnd = false;

        public GedcomLineReader(TextReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _lineNumber = 0;
            _parser = new GedcomLineParser();
        }

        public bool IsAtEnd => _isAtEnd;
        public int LineNumber => _lineNumber + 1;

        public GedcomLine ReadLine()
        {
            if (_isAtEnd)
                return null;
            
            try
            {
                bool doLoop;
                GedcomLine result;
                do
                {
                    _lineNumber++;
                    string line = _reader.ReadLine();
                    (doLoop, result) = ProcessLine(line);
                } while (doLoop);

                return result;
            }
            catch (Exception ex)
            {
                throw BuildGedcomReaderException(ex);
            }
        }

        public async Task<GedcomLine> ReadLineAsync()
        {
            if (_isAtEnd)
                return null;
            
            try
            {
                bool doLoop;
                GedcomLine result;
                do
                {
                    _lineNumber++;
                    string line = await _reader.ReadLineAsync();
                    (doLoop, result) = ProcessLine(line);
                } while (doLoop);

                return result;
            }
            catch (Exception ex)
            {
                throw BuildGedcomReaderException(ex);
            }
        }
        
        private GedcomReaderException BuildGedcomReaderException(Exception ex)
        {
            string msg;
            if (ex is GedcomLineParserException glpEx)
            {
                msg = $"An error occurred at line {_lineNumber}, character {glpEx.CharacterPosition} of the GEDCOM file." +
                      " See inner exception for more details.";
                return new GedcomReaderException(_lineNumber, glpEx.CharacterPosition, msg, ex);
            }
            msg = $"An error occurred when processing line {_lineNumber} of the GEDCOM file." +
                  " See inner exception for more details.";
            return new GedcomReaderException(_lineNumber, msg, ex);
        }
        
        private (bool, GedcomLine) ProcessLine(string line)
        {
            if (line == null)
            {
                _isAtEnd = true;
                _lineNumber--;
                return (false, null);
            }

            if (string.IsNullOrWhiteSpace(line))
                return (true, null);

            GedcomLine result = _parser.Parse(line);
            return (false, result);
        }

        private void ReleaseUnmanagedResources()
        {
            _reader?.Close();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~GedcomLineReader()
        {
            ReleaseUnmanagedResources();
        }
    }
}