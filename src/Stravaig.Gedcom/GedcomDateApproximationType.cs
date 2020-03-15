namespace Stravaig.Gedcom
{
    public enum GedcomDateApproximationType
    {
        Exact,
        
        NoValue,
        
        /// <summary>
        /// About, meaning the date is not exact.
        /// </summary>
        About,
        
        /// <summary>
        /// Calculated mathematically, for example, from an event date and age.
        /// </summary>
        Calculated,
        
        /// <summary>
        /// Estimated based on an algorithm using some other event date.
        /// </summary>
        Estimated
    }
}