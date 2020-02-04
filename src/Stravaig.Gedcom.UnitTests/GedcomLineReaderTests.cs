using System;
using System.IO;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Gedcom.UnitTests
{
    [TestFixture]
    public class GedcomLineReaderTests
    {
        [Test]
        public void EnsureThatUnderlyingReaderIsClosedOnDispose()
        {
            using StringReader stringReader = new StringReader(string.Empty);
            using (GedcomLineReader gedcomReader = new GedcomLineReader(stringReader))
            {
                // Do nothing here. Just going to dispose directly.
            }
            Should.Throw<ObjectDisposedException>(() => stringReader.ReadToEnd());
        }
    }
}