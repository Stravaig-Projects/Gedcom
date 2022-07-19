namespace Stravaig.Gedcom.Model
{
    public enum DateType
    {
        Unknown,
        Date,
        Period,
        Range,
        ApproximatedAbout,
        ApproximatedCalculated,
        ApproximatedEstimated,
        Interpreted,
        Phrase,
    }

    public static class DateTypeExtensions
    {
        public static bool IsVague(this DateType type)
        {
            if (type == DateType.Date)
                return false;
            
            return true;
        }
    }
}