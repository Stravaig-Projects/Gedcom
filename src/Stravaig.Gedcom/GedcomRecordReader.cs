using System.Threading.Tasks;

namespace Stravaig.Gedcom
{
    public class GedcomRecordReader
    {
        private readonly GedcomLineReader _reader;
        private int _lineNumber;
        private GedcomLine _peekedLine = null;
        public GedcomRecordReader(GedcomLineReader reader)
        {
            _reader = reader;
        }
        
        public GedcomRecord ReadRecord()
        {
            return ReadChildRecord(null);
        }

        private GedcomRecord ReadChildRecord(GedcomRecord parent)
        {
            var currentLine = ReadLine();
            if (currentLine == null)
                return null;
            
            var result = GedcomRecord.From(currentLine, parent);
            var nextLine = PeekLine();
            if (nextLine == null)
                return result;
            
            ThrowIfSubsequentLineLevelIncorrect(currentLine, nextLine);
            if (nextLine.Level.IsSubordinateTo(currentLine.Level))
                ReadChildRecord(result);
            
            nextLine = PeekLine();
            if (nextLine == null)
                return result;
            
            if (nextLine.Level == currentLine.Level)
                ReadChildRecord(parent);
            
            return result;
        }

        public async Task<GedcomRecord> ReadRecordAsync()
        {
            return await ReadChildRecordAsync(null);
        }
        
        private async Task<GedcomRecord> ReadChildRecordAsync(GedcomRecord parent)
        {
            var currentLine = await ReadLineAsync();
            if (currentLine == null)
                return null;
            
            var result = GedcomRecord.From(currentLine, parent);
            var nextLine = await PeekLineAsync();
            if (nextLine == null)
                return result;
            
            ThrowIfSubsequentLineLevelIncorrect(currentLine, nextLine);
            if (nextLine.Level.IsSubordinateTo(currentLine.Level))
                await ReadChildRecordAsync(result);
            
            nextLine = await PeekLineAsync();
            if (nextLine == null)
                return result;
            
            if (nextLine.Level == currentLine.Level)
                await ReadChildRecordAsync(parent);
            
            return result;
        }

        private void ThrowIfSubsequentLineLevelIncorrect(GedcomLine currentLine, GedcomLine nextLine)
        {
            if (!nextLine.Level.CanFollowFrom(currentLine.Level))
                throw new GedcomReaderException(
                    _lineNumber,
                    $"Unexpected Line Level. Got {nextLine.Level}, expected 0 to {currentLine.Level.NextLineLevelMax}");
        }
        
        private GedcomLine PeekLine()
        {
            return _peekedLine ??= _reader.ReadLine();
        }

        private async Task<GedcomLine> PeekLineAsync()
        {
            return _peekedLine ??= await _reader.ReadLineAsync();
        }

        private GedcomLine ReadLine()
        {
            if (_peekedLine != null)
                return ReadPeekedLine();

            var result = _reader.ReadLine();
            _lineNumber = _reader.LineNumber;
            return result;
        }
        
        private async Task<GedcomLine> ReadLineAsync()
        {
            if (_peekedLine != null)
                return ReadPeekedLine();

            var result = await _reader.ReadLineAsync();
            _lineNumber = _reader.LineNumber;
            return result;
        }
        
        private GedcomLine ReadPeekedLine()
        {
            GedcomLine result = _peekedLine;
            _peekedLine = null;
            _lineNumber++;
            return result;
        }
    }
}