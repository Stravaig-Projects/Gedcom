using System;
using System.IO;

namespace Stravaig.Gedcom
{
    public class GedcomLineReader : IDisposable
    {
        private readonly TextReader _reader;

        public GedcomLineReader(TextReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
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