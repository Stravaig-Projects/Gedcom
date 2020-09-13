using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;
using Stravaig.Gedcom.PowerShell.Extensions;

namespace Stravaig.Gedcom.PowerShell
{
    // ReSharper disable once InconsistentNaming
    public class PSGedcomIndividual
    {
        private readonly GedcomIndividualRecord _individual;

        public PSGedcomIndividual(GedcomIndividualRecord individual)
        {
            _individual = individual;
        }

        public string Id => _individual.CrossReferenceId.ToString();
        public string Name => _individual.Name;
        public PSGedcomName[] Names => _individual.Names.Wrap();
        public PSGedcomDate DateOfBirth => _individual.BirthEvent?.Date.Wrap();
        public PSGedcomDate DateOfDeath => _individual.DeathEvent?.Date.Wrap();

        public bool IsAlive => _individual.IsAlive();
        public bool IsDead => _individual.IsDead();

        public PSImmediateRelative[] ImmediateRelatives => _individual.GetImmediateRelatives().Wrap();
        

        public GedcomIndividualRecord Unwrap() => _individual;

        public override string ToString()
        {
            return $"{_individual.NameWithoutMarker} ({DateOfBirth?.ToString() ?? "?"} - {DateOfDeath?.ToString() ?? "?"})";
        }
    }
}