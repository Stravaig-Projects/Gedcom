using System;
using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.PowerShell.Extensions
{
    public static class GedcomNameRecordExtensions
    {
        public static PSGedcomName Wrap(this GedcomNameRecord source)
        {
            if (source == null)
                return null;
            
            return new PSGedcomName(source);
        }

        public static PSGedcomName[] Wrap(this GedcomNameRecord[] source)
        {
            if (source == null)
                return Array.Empty<PSGedcomName>();

            var result = new PSGedcomName[source.Length];
            for (int i = 0; i < source.Length; i++)
                result[i] = source[i].Wrap();
            
            return result;
        }
    }
}