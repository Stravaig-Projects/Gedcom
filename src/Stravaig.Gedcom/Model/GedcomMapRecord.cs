using System;
using System.Globalization;
using System.Linq;
using Stravaig.Gedcom.Extensions;

namespace Stravaig.Gedcom.Model
{
    public class GedcomMapRecord : Record
    {
        public static readonly GedcomTag MapTag = "MAP".AsGedcomTag();
        private static readonly GedcomTag LatitudeTag = "LATI".AsGedcomTag();
        private static readonly GedcomTag LongitudeTag = "LONG".AsGedcomTag();

        private readonly Lazy<string> _lazyLatitude;
        private readonly Lazy<string> _lazyLongitude;
        
        public GedcomMapRecord(GedcomRecord record, GedcomDatabase database)
            : base(record, database)
        {
            if (record.Tag != MapTag)
                throw new ArgumentException($"Expected a record with a \"{MapTag}\", but got \"{record.Tag}\".", nameof(record));
            
            _lazyLatitude = new Lazy<string>(GetLatitudeValue);
            _lazyLongitude = new Lazy<string>(GetLongitudeValue);
        }
        
        public static GedcomMapRecord Factory(GedcomRecord record, GedcomDatabase database)
        {
            return new GedcomMapRecord(record, database);
        }

        private string GetLongitudeValue()
        {
            var record = _record.Children.FirstOrDefault(r => r.Tag == LongitudeTag);
            return record?.Value;
        }

        private string GetLatitudeValue()
        {
            var record = _record.Children.FirstOrDefault(r => r.Tag == LatitudeTag);
            return record?.Value;
        }

        public string RawLatitude => _lazyLatitude.Value;
        public string RawLongitude => _lazyLongitude.Value;

        public double Latitude => GetNumbers(RawLatitude) * GetMultiplier(RawLatitude);

        public double Longitude =>  GetNumbers(RawLongitude) * GetMultiplier(RawLongitude);

        private double GetMultiplier(string raw)
        {
            char hemisphere = raw[0];
            if (hemisphere == 'S' || hemisphere == 'W')
                return -1.0;
            return 1.0;
        }

        private double GetNumbers(string raw)
        {
            string numbers = "0";
            if (raw.Length > 1)
                numbers = raw.Substring(1, raw.Length - 1);
            return double.Parse(numbers, NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }
}