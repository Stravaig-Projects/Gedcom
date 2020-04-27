namespace Stravaig.Gedcom.Model.Extensions
{
    public static class GedcomSexExtensions
    {
        public static Gender ToGender(this GedcomSex sex)
        {
            switch (sex)
            {
                case GedcomSex.Female:
                    return Gender.Female;
                case GedcomSex.Male:
                    return Gender.Male;
            }
            return Gender.Unknown;        }
    }
}