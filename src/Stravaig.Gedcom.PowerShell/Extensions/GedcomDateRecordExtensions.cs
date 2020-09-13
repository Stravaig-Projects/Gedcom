using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.PowerShell.Extensions
{
    public static class GedcomDateRecordExtensions
    {
        public static PSGedcomDate Wrap(this GedcomDateRecord source)
        {
            if (source == null)
                return null;
            return new PSGedcomDate(source);
        }
    }
}