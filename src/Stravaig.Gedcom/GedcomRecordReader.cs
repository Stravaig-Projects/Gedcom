using System;
using System.IO;

namespace Stravaig.Gedcom
{
    public class GedcomRecordReader : IDisposable
    {
        private readonly GedcomLineReader _lineReader;
        public GedcomRecordReader(TextReader reader)
        {
            _lineReader = new GedcomLineReader(reader);
        }
        
        private void ReleaseUnmanagedResources()
        {
            _lineReader.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~GedcomRecordReader()
        {
            ReleaseUnmanagedResources();
        }
    }
}