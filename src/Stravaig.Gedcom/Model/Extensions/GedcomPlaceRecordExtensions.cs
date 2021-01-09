namespace Stravaig.Gedcom.Model.Extensions
{
    public static class GedcomPlaceRecordExtensions
    {
        public static string NormalisedPlaceName(this GedcomPlaceRecord place)
        {
            if (place?.Name == null)
                return null;

            string result = place.Name.Replace(", ", ",");
            int len;
            do
            {
                len = result.Length;
                result = result.Replace(",,", ",");
            } while (result.Length < len);

            result = result.Replace(",", ", ");
            return result;
        }

        public static string NormalisedPlaceName(this IPlace recordWithPlace)
        {
            return recordWithPlace?.Place.NormalisedPlaceName();
        }
    }
}