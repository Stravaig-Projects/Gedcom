using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.PowerShell.Extensions
{
    public static class GedcomIndividualRecordExtension
    {
        public static PSGedcomIndividual Wrap(this GedcomIndividualRecord source)
        {
            if (source == null)
                return null;
            
            return new PSGedcomIndividual(source);
        }
    }
}