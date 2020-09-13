using System;
using System.Collections.Generic;
using System.Linq;
using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.PowerShell.Extensions
{
    public static class ImmediateRelativeExtensions
    {
        public static PSImmediateRelative Wrap(this ImmediateRelative source)
        {
            if (source == null)
                return null;
            
            return new PSImmediateRelative(source);
        }

        public static IEnumerable<PSImmediateRelative> Wrap(this IEnumerable<ImmediateRelative> source)
        {
            if (source == null)
                return Array.Empty<PSImmediateRelative>();

            return source.Select(t => t.Wrap());
        }

        public static PSImmediateRelative[] Wrap(this ImmediateRelative[] source)
        {
            if (source == null)
                return Array.Empty<PSImmediateRelative>();
            
            var result = new PSImmediateRelative[source.Length];
            for (int i = 0; i < source.Length; i++)
                result[i] = source[i].Wrap();
            return result;
        }
    }
}