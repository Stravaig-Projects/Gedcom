using Stravaig.Gedcom.Model;

namespace Stravaig.Gedcom.PowerShell
{
    // ReSharper disable once InconsistentNaming
    public class PSGedcomIndividualRecord
    {
        private readonly GedcomIndividualRecord _individual;

        public PSGedcomIndividualRecord(GedcomIndividualRecord individual)
        {
            _individual = individual;
        }

        public string Name => _individual.Name;
        public GedcomDateRecord DateOfBirth => _individual.BirthEvent?.Date;
        public GedcomDateRecord DateOfDeath => _individual.DeathEvent?.Date;

        public GedcomIndividualRecord Unwrapped => _individual;

        public override string ToString()
        {
            return $"{_individual.NameWithoutMarker} ({DateOfBirth?.RawDateValue ?? "?"} - {DateOfDeath?.RawDateValue ?? "?"})";
        }
    }
}