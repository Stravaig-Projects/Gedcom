namespace Stravaig.Gedcom.Constants
{
    internal static class Order
    {
        // For IComparable interface implementations
        public const int ThisPrecedesOther = -1;
        public const int ThisOccursInTheSamePositionAsOther = 0;
        public const int ThisFollowsOther = 1;

        // For IComparer interface implementations.
        public const int XIsLessThanY = -1;
        public const int XEqualsY = 0;
        public const int XIsGreaterThanY = 1;
    }
}