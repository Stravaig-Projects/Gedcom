using System;
using System.Diagnostics;
using System.Linq;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model.Parsers;

// INDIVIDUAL_ATTRIBUTE_STRUCTURE:=
// [
//   n CAST <CASTE_NAME> {1:1} p.43
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n DSCR <PHYSICAL_DESCRIPTION> {1:1} p.58
//     +1 [CONC | CONT ] <PHYSICAL_DESCRIPTION> {0:M} p.58
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n EDUC <SCHOLASTIC_ACHIEVEMENT> {1:1} p.61
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n IDNO <NATIONAL_ID_NUMBER> {1:1} p.56
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n NATI <NATIONAL_OR_TRIBAL_ORIGIN> {1:1} p.56
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n NCHI <COUNT_OF_CHILDREN> {1:1} p.44
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n NMR <COUNT_OF_MARRIAGES> {1:1} p.44
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n OCCU <OCCUPATION> {1:1} p.57
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n PROP <POSSESSIONS> {1:1} p.59
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n RELI <RELIGIOUS_AFFILIATION> {1:1} p.60
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n RESI /* Resides at */ {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n SSN <SOCIAL_SECURITY_NUMBER> {1:1} p.61
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n TITL <NOBILITY_TYPE_TITLE> {1:1} p.57
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n FACT <ATTRIBUTE_DESCRIPTOR> {1:1} p.43
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
// ]
//
// * Note: The usage of IDNO or the FACT tag require that a subordinate TYPE
//   tag be used to define what kind of identification number or fact
//   classification is being defined.  The TYPE tag can be used with each of
//   the above tags used in this structure.
//
// CAST {CASTE}:=
//   The name of an individual's rank or status in society which is sometimes
//   based on racial or religious differences, or differences in wealth,
//   inherited  rank, profession, occupation, etc.
//
// DSCR {PHY_DESCRIPTION}:=
//   The physical characteristics of a person, place, or thing.
//
// EDUC {EDUCATION}:=
//   Indicator of a level of education attained.
//
// IDNO {IDENT_NUMBER}:=
//   A number assigned to identify a person within some significant external
//   system.
//
// NATI {NATIONALITY}:=
//   The national heritage of an individual.
//
// NCHI {CHILDREN_COUNT}:=
//   The number of children that this person is known to be the parent of (all
//   marriages) when subordinate to an individual, or that belong to this
//   family when subordinate to a FAM_RECORD.
//
// NMR {MARRIAGE_COUNT}:=
//   The number of times this person has participated in a family as a spouse
//   or parent.
//
// OCCU {OCCUPATION}:=
//   The type of work or profession of an individual.
//
// PROP {PROPERTY}:=
//   Pertaining to possessions such as real estate or other property of
//   interest.
//
// RELI {RELIGION}:=
//   A religious denomination to which a person is affiliated or for which a record applies.
//
// RESI {RESIDENCE}:=
//   An address or place of residence that a family or individual resided.
//
// SSN {SOC_SEC_NUMBER}:=
//   A number assigned by the United States Social Security Administration.
//   Used for tax identification purposes.
//
// NOBILITY_TYPE_TITLE:= {Size=1:120}
//   The title given to or used by a person, especially of royalty or other
//   noble class within a locality.
//
// ATTRIBUTE_DESCRIPTOR:= {Size=1:90}
//   Text describing a particular characteristic or attribute assigned to an
//   individual. This attribute value is assigned to the FACT tag.  The
//   classification of this specific attribute or fact is specified by the
//   value of the subordinate TYPE tag selected from the EVENT_DETAIL
//   structure.  For example if you were classifying the skills a person had
//   obtained;
//     1 FACT Woodworking
//       2 TYPE Skills
//
// ---------------------------------------------------------------------------|

namespace Stravaig.Gedcom.Model
{
    [DebuggerDisplay("{Tag}:{Type}")]
    public class GedcomIndividualAttributeRecord : EventRecord, ISubject, IPlace
    {
        public static readonly GedcomTag CasteTag = "CAST".AsGedcomTag();
        public static readonly GedcomTag DescriptionTag = "DESC".AsGedcomTag();
        public static readonly GedcomTag EducationTag = "EDUC".AsGedcomTag();
        public static readonly GedcomTag IdentNumberTag = "IDNO".AsGedcomTag();
        public static readonly GedcomTag NationalityTag = "NATI".AsGedcomTag();
        public static readonly GedcomTag ChildrenCountTag = "NCHI".AsGedcomTag();
        public static readonly GedcomTag MarriageCountTag = "NMR".AsGedcomTag();
        public static readonly GedcomTag OccupationTag = "OCCU".AsGedcomTag();
        public static readonly GedcomTag PropertyTag = "PROP".AsGedcomTag();
        public static readonly GedcomTag ReligionTag = "RELI".AsGedcomTag();
        public static readonly GedcomTag ResidenceTag = "RESI".AsGedcomTag();
        public static readonly GedcomTag SocialSecurityNumberTag = "SSN".AsGedcomTag();
        public static readonly GedcomTag TitleTag = "TITL".AsGedcomTag();
        public static readonly GedcomTag FactTag = "FACT".AsGedcomTag();

        public static readonly GedcomTag[] AttributeTags =
        {
            CasteTag,
            DescriptionTag,
            EducationTag,
            IdentNumberTag,
            NationalityTag,
            ChildrenCountTag,
            MarriageCountTag,
            OccupationTag,
            PropertyTag,
            ReligionTag,
            ResidenceTag,
            SocialSecurityNumberTag,
            TitleTag,
            FactTag,
        };

        private readonly Lazy<string> _lazyGetText;
        
        public GedcomIndividualAttributeRecord(GedcomRecord record, GedcomDatabase database, GedcomIndividualRecord subject)
            : base(record, database)
        {
            if (!AttributeTags.Contains(record.Tag))
                throw new ArgumentException($"The record must be a known event type. One of {string.Join(", ", AttributeTags.Select(et=>et.ToString()))}.");

            Subject = subject;
            _lazyGetText = new Lazy<string>(GetText);
        }

        private string GetText()
        {
            if (_record.Tag == DescriptionTag)
                return MultilineTextParser.GetText(_record);
            return _record.Value;
        }

        public string Text => _lazyGetText.Value;
        public GedcomIndividualRecord Subject { get; }
    }
}