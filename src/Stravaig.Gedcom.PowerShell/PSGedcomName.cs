using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.PowerShell
{
    // ReSharper disable once InconsistentNaming
    public class PSGedcomName
    {
        private readonly GedcomNameRecord _name;

        public PSGedcomName(GedcomNameRecord name)
        {
            _name = name;
        }

        public string Value => _name.Name;
        public string Type => _name.Type;

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Type))
                return Value;

            return $"({Type}) {Value}";
        }
    }
}