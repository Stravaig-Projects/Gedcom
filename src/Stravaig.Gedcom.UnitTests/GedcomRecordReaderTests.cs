using System;
using System.IO;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomRecordReaderTests
    {
        [Test]
        public void EnsureThatUnderlyingReaderIsClosedOnDispose()
        {
            using StringReader stringReader = new StringReader(string.Empty);
            using (GedcomRecordReader gedcomReader = new GedcomRecordReader(stringReader))
            {
                // Do nothing here. Just going to dispose directly.
            }
            Should.Throw<ObjectDisposedException>(() => stringReader.ReadToEnd());
        }
    }
}