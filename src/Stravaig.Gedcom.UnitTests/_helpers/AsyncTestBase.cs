using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Shouldly;

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

        private readonly Dictionary<string, object> _syncAsyncCorrelation = new Dictionary<string, object>();

        protected void VerifySyncAsyncCorrelate(object value, Read type, string key = null, [CallerMemberName]string callerMemberName = default, [CallerLineNumber]int callerLineNumber = default)
        {
            var otherType = Other(type);
            string otherFullKey = $"{callerMemberName}-{callerLineNumber}-{otherType}-{key}";
            if (_syncAsyncCorrelation.TryGetValue(otherFullKey, out object otherValue))
            {
                value.ShouldBe(otherValue, $"Mismatch between {type} (Value = {value}) and {otherType} (Value = {otherValue}) at {callerMemberName}({callerLineNumber}).");
                return;
            }
            
            string fullKey = $"{callerMemberName}-{callerLineNumber}-{type}-{key}";
            _syncAsyncCorrelation.Add(fullKey, value);
        }

        private Read Other(Read type) => type == Read.Synchronous ? Read.Asynchronous : Read.Synchronous;
    }
}