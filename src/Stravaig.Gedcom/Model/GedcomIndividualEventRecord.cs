using System;
using System.Diagnostics;
using System.Linq;
using Stravaig.Gedcom.Extensions;

// INDIVIDUAL_EVENT_STRUCTURE:=
// [
//   n [ BIRT | CHR ] [Y|<NULL>] {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//     +1 FAMC @<XREF:FAM>@ {0:1} p.24
//   |
//   n DEAT  [Y|<NULL>] {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n [ BURI | CREM ] {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n ADOP {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//     +1 FAMC @<XREF:FAM>@ {0:1} p.24
//       +2 ADOP <ADOPTED_BY_WHICH_PARENT> {0:1} p.42
//   |
//   n [ BAPM | BARM | BASM | BLES ] {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n [ CHRA | CONF | FCOM | ORDN ] {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n [ NATU | EMIG | IMMI ] {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n [ CENS | PROB | WILL] {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n [ GRAD | RETI ] {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
//   |
//   n EVEN {1:1}
//     +1 <<INDIVIDUAL_EVENT_DETAIL>> {0:1}* p.34
// ]
//
// BIRT {BIRTH}:=
//   The event of entering into life.
//
// CHR {CHRISTENING}:=
//   The religious event (not LDS) of baptizing and/or naming a child.
//
// DEAT {DEATH}:=
//   The event when mortal life terminates.
//
// BURI {BURIAL}:=
//   The event of the proper disposing of the mortal remains of a deceased
//   person.
// 
// CREM {CREMATION}:=
//   Disposal of the remains of a person's body by fire.
//
// ADOP {ADOPTION}:=
//   Pertaining to creation of a legally approved child-parent relationship
//   that does not exist biologically.
//
// BAPM {BAPTISM}:=
//   The event of baptism (not LDS), performed in infancy or later. (See also
//   BAPL, above, and CHR, page 85.)
//
// BARM {BAR_MITZVAH}:=
//   The ceremonial event held when a Jewish boy reaches age 13.
//
// BASM {BAS_MITZVAH}:=
//   The ceremonial event held when a Jewish girl reaches age 13, also known as
//   "Bat Mitzvah."
//
// BLES {BLESSING}:=
//   A religious event of bestowing divine care or intercession. Sometimes
//   given in connection with a naming ceremony.
//
// CHRA {ADULT_CHRISTENING}:=
//   The religious event (not LDS) of baptizing and/or naming an adult person.
//
// CONF {CONFIRMATION}:=
//   The religious event (not LDS) of conferring the gift of the Holy Ghost
//   and, among protestants, full church membership.
//
// FCOM {FIRST_COMMUNION}:=
//   A religious rite, the first act of sharing in the Lord's supper as part of
//   church worship.
//
// ORDN {ORDINATION}:=
//   A religious event of receiving authority to act in religious matters.
//
// NATU {NATURALIZATION}:=
//   The event of obtaining citizenship.
//
// EMIG {EMIGRATION}:=
//   An event of leaving one's homeland with the intent of residing elsewhere.
//
// IMMI {IMMIGRATION}:=
//   An event of entering into a new locality with the intent of residing
//   there.
//
// CENS {CENSUS}:=
//   The event of the periodic count of the population for a designated
//   locality, such as a national or state Census.
// 
// PROB {PROBATE}:=
//   An event of judicial determination of the validity of a will. May indicate
//   several related court activities over several dates.
//
// WILL {WILL}:=
//   A legal document treated as an event, by which a person disposes of his or
//   her estate, to take effect after death. The event date is the date the
//   will was signed while the person was alive. (See also PROBate, page 91.)
//
// GRAD {GRADUATION}:=
//   An event of awarding educational diplomas or degrees to individuals.
//
// RETI {RETIREMENT}:=
//   An event of exiting an occupational relationship with an employer after a
//   qualifying time period.
//
// EVEN {EVENT}:=
//   Pertaining to a noteworthy happening related to an individual, a group, or
//   an organization. An EVENt structure is usually qualified or classified by
//   a subordinate use of the TYPE tag.


namespace Stravaig.Gedcom.Model
{
    [DebuggerDisplay("{Tag}:{Type}")]
    public class GedcomIndividualEventRecord : EventRecord, ISubject, IPlace
    {
        public static readonly GedcomTag AgeTag = "AGE".AsGedcomTag();
        
        public static readonly GedcomTag BirthTag = "BIRT".AsGedcomTag();
        public static readonly GedcomTag ChristeningTag = "CHR".AsGedcomTag();

        public static readonly GedcomTag DeathTag = "DEAT".AsGedcomTag();

        public static readonly GedcomTag BuriedTag = "BURI".AsGedcomTag();
        public static readonly GedcomTag CrematedTag = "CREM".AsGedcomTag();

        public static readonly GedcomTag AdoptionTag = "ADOP".AsGedcomTag();

        public static readonly GedcomTag BaptismTag = "BAPM".AsGedcomTag();
        public static readonly GedcomTag BarMitzvahTag = "BARM".AsGedcomTag();
        public static readonly GedcomTag BasMitzvahTag = "BASM".AsGedcomTag();
        public static readonly GedcomTag BlessingTag = "BLES".AsGedcomTag();

        public static readonly GedcomTag AdultChristeningTag = "CHRA".AsGedcomTag();
        public static readonly GedcomTag ConfirmationTag = "CONF".AsGedcomTag();
        public static readonly GedcomTag FirstCommunionTag = "FCON".AsGedcomTag();
        public static readonly GedcomTag OrdinationTag = "ORDN".AsGedcomTag();

        public static readonly GedcomTag NaturalisationTag = "NATU".AsGedcomTag();
        public static readonly GedcomTag EmmigrationTag = "EMIG".AsGedcomTag();
        public static readonly GedcomTag ImmigrationTag = "IMMI".AsGedcomTag();

        public static readonly GedcomTag CensusTag = "CENS".AsGedcomTag();
        public static readonly GedcomTag ProbateTag = "PROB".AsGedcomTag();
        public static readonly GedcomTag WillTag = "WILL".AsGedcomTag();

        public static readonly GedcomTag GraduationTag = "GRAD".AsGedcomTag();
        public static readonly GedcomTag RetirementTag = "RETI".AsGedcomTag();
        
        public static readonly GedcomTag EventTag = "EVEN".AsGedcomTag();

        
        public static readonly GedcomTag[] EventTags =
        {
            BirthTag,
            ChristeningTag,
            DeathTag,
            BuriedTag,
            CrematedTag,
            AdoptionTag,
            BaptismTag,
            BarMitzvahTag,
            BasMitzvahTag,
            BlessingTag,
            AdultChristeningTag,
            ConfirmationTag,
            FirstCommunionTag,
            OrdinationTag,
            NaturalisationTag,
            EmmigrationTag,
            ImmigrationTag,
            CensusTag,
            ProbateTag,
            WillTag,
            GraduationTag,
            RetirementTag,
            EventTag,
        };
        
        public GedcomIndividualEventRecord(GedcomRecord record, GedcomDatabase database, GedcomIndividualRecord subject)
            : base(record, database)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
                               if (!EventTags.Contains(record.Tag))
                throw new ArgumentException($"The record must be a known event type. One of {string.Join(", ", EventTags.Select(et=>et.ToString()))}.");
        }
        
        public GedcomIndividualRecord Subject { get; }
        
        public int? Age
        {
            get
            {
                var ageRecord = _record.Children.FirstOrDefault(r => r.Tag == AgeTag);
                if (ageRecord == null || string.IsNullOrWhiteSpace(ageRecord.Value))
                    return null;
                if (int.TryParse(ageRecord.Value, out var result))
                    return result;
                return null;
            }
        }

    }
}