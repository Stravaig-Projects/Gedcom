using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stravaig.Gedcom.UnitTests._helpers
{
    public abstract class AsyncTestBase
    {        
        public enum Read
        {
            Synchronous,
            Asynchronous
        }
        
        protected static IEnumerable<Read> ReadTypes()
        {
            yield return Read.Synchronous;
            yield return Read.Asynchronous;
        }
    }
}