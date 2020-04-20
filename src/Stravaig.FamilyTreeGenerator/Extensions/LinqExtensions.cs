using System;
using System.Collections.Generic;
using System.Linq;

namespace Stravaig.FamilyTreeGenerator.Extensions
{
    public static class LinqExtensions
    {
        public static bool NotAny<T>(this IEnumerable<T> sequence)
        {
            return !sequence.Any();
        }

        public static bool NotAny<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        {
            return !sequence.Any(predicate);
        }
    }
}