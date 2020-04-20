using System;
using System.Diagnostics;
using System.Linq;
using Stravaig.Gedcom.Extensions;

// FAMILY_EVENT_STRUCTURE:=
// [
//   n [ ANUL | CENS | DIV | DIVF ] {1:1}
//     +1 <<FAMILY_EVENT_DETAIL>> {0:1} p.32
//   |
//   n [ ENGA | MARB | MARC ] {1:1}
//     +1 <<FAMILY_EVENT_DETAIL>> {0:1} p.32
//   |
//   n MARR  [Y|<NULL>] {1:1}
//     +1 <<FAMILY_EVENT_DETAIL>> {0:1} p.32
//   |
//   n [ MARL | MARS ] {1:1}
//     +1 <<FAMILY_EVENT_DETAIL>> {0:1} p.32
//   |
//   n RESI
//     +1 <<FAMILY_EVENT_DETAIL>> {0:1} p.32
//   |
//   n EVEN [<EVENT_DESCRIPTOR> | <NULL>] {1:1} p.48
//     +1 <<FAMILY_EVENT_DETAIL>> {0:1} p.32
// ]
//
// ANUL {ANNULMENT}:=
//   Declaring a marriage void from the beginning (never existed).
//
// CENS {CENSUS}:=
//   The event of the periodic count of the population for a designated
//   locality, such as a national or state Census.
//
// DIV {DIVORCE}:=
//   An event of dissolving a marriage through civil action.
//
// DIVF {DIVORCE_FILED}:=
//   An event of filing for a divorce by a spouse.
//
// ENGA {ENGAGEMENT}:=
//   An event of recording or announcing an agreement between two people to
//   become married.
//
// MARB {MARRIAGE_BANN}:=
//   An event of an official public notice given that two people intend to
//   marry.
//
// MARC {MARR_CONTRACT}:=
//   An event of recording a formal agreement of marriage, including the
//   prenuptial agreement in which marriage partners reach agreement about the
//   property rights of one or both, securing property to their children.
//
// MARL {MARR_LICENSE}:=
//   An event of obtaining a legal license to marry.
//
// MARR {MARRIAGE}:=
//   A legal, common-law, or customary event of creating a family unit of a man
//   and a woman as husband and wife.
//
// MARS {MARR_SETTLEMENT}:=
//   An event of creating an agreement between two people contemplating
//   marriage, at which time they agree to release or modify property rights
//   that would otherwise arise from the marriage.
//
// RESI {RESIDENCE}:=
//   An address or place of residence that a family or individual resided.
//
// EVEN {EVENT}:=
//   Pertaining to a noteworthy happening related to an individual, a group,
//   or an organization. An EVENt structure is usually qualified or classified
//   by a subordinate use of the TYPE tag.

namespace Stravaig.Gedcom.Model
{
    [DebuggerDisplay("{Tag}:{Type}")]
    public class GedcomFamilyEventRecord : EventRecord
    {
        private static readonly GedcomTag AnnulmentTag = "ANUL".AsGedcomTag();
        private static readonly GedcomTag CensusTag = "CENS".AsGedcomTag();
        private static readonly GedcomTag DivorceTag = "DIV".AsGedcomTag();
        private static readonly GedcomTag DivorceFiledTag = "DIVF".AsGedcomTag();

        private static readonly GedcomTag EngagementTag = "ENGA".AsGedcomTag();
        private static readonly GedcomTag MarriageBannTag = "MARB".AsGedcomTag();
        private static readonly GedcomTag MarriageContractTag = "MARC".AsGedcomTag();

        private static readonly GedcomTag MarriageLicenceTag = "MARL".AsGedcomTag();
        private static readonly GedcomTag MarriageTag = "MARR".AsGedcomTag();
        private static readonly GedcomTag MarriageSettlementTag = "MARS".AsGedcomTag();

        private static readonly GedcomTag ResidenceTag = "RESI".AsGedcomTag();
        private static readonly GedcomTag EventTag = "EVEN".AsGedcomTag();

        
        public static readonly GedcomTag[] FamilyEventTags =
        {
            AnnulmentTag,
            CensusTag,
            DivorceTag,
            DivorceFiledTag,
            EngagementTag,
            MarriageBannTag,
            MarriageContractTag,
            MarriageLicenceTag,
            MarriageTag,
            MarriageSettlementTag,
            ResidenceTag,
            EventTag,
        };
        
        public GedcomFamilyEventRecord(GedcomRecord record, GedcomDatabase database) 
            : base(record, database)
        {
            if (!FamilyEventTags.Contains(record.Tag))
                throw new ArgumentException($"The record must be a known family event type. One of {string.Join(", ", FamilyEventTags.Select(ft=>ft.ToString()))}.");
        }
    }
}