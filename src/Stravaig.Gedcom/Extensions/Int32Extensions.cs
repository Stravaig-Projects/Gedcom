namespace Stravaig.Gedcom.Extensions
{
    public static class Int32Extensions
    {
        public static GedcomLevel AsGedcomLevel(this int target)
        {
            return new GedcomLevel(target);
        }
    }
}