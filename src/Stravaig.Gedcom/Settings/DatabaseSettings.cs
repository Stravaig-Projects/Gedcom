namespace Stravaig.Gedcom.Settings
{
    public class DatabaseSettings
    {
        public DateOrderingRule DateOrderingRule { get; set; }
        public int AssumedDeathAge { get; set; }

        public DatabaseSettings()
        {
            DateOrderingRule = DateOrderingRule.BeginningOfExtent;
            AssumedDeathAge = 100;
        }
    }
}