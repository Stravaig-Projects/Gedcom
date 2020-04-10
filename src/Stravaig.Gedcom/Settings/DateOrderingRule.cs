namespace Stravaig.Gedcom.Settings
{
    /// <summary>
    /// Setting used for dates, date periods or date ranges where trying to
    /// define an order with a date of a different type, or even the same type,
    /// may otherwise prove difficult.
    /// </summary>
    public enum DateOrderingRule
    {
        /// <summary>
        /// For dates without a day or month component it will assume a date of
        /// the first day and/or first month. For ranges or periods it will
        /// assume the beginning of the period. For ranges or period without a
        /// start date, will fallback to single date rules.
        /// </summary>
        BeginningOfExtent,
        
        /// <summary>
        /// For dates without a day or month component it will assume a date of
        /// the middle day of the month and/or June. For ranges or periods it
        /// will assume the beginning of the period. For ranges or period
        /// without a start date, will fallback to single date rules.
        /// </summary>
        MiddleOfExtent,

        /// <summary>
        /// For dates without a day or month component it will assume a date of
        /// the first day and/or first month. For ranges or periods it will
        /// assume the beginning of the period. For ranges or period without a
        /// start date, will fallback to single date rules.
        /// </summary>
        EndOfExtent,
    }
}