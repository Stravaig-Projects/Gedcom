using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stravaig.FamilyTree.Common.Extensions;
using Stravaig.FamilyTree.Common.Humaniser;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class RenderPersonFamilyTreeAsJsonHandler : RenderPersonAsJsonHandlerBase
    {
        private readonly IDateRenderer _dateRenderer;
        private readonly IRelationshipRenderer _relationshipRenderer;

        public class FamilyTreeModel
        {
            [JsonProperty(Order = 1)]
            public PersonModel Subject { get; set; }
            
            [JsonProperty(Order = 2)]
            public ParentModel[] Parents { get; set; }
            
            [JsonProperty(Order = 3)]
            public SiblingModel[] Siblings { get; set; }
            
            [JsonProperty(Order = 4)]
            public SpouseModel[] Spouses { get; set; }

            [JsonProperty(Order = 5)]
            public ChildModel[] Children { get; set; }
        }
        public class PersonModel
        {
            [JsonProperty(Order = -5)]
            public string Id { get; set; }

            [JsonProperty(Order = -4)]
            public string Name { get; set; }
            
            [JsonProperty(Order = -3)]
            public string Gender { get; set; }
            
            [JsonProperty(Order = -2)]
            public string DateOfBirth { get; set; }
            
            [JsonProperty(Order = -1)]
            public string DateOfDeath { get; set; }

            public PersonModel FromSubject(GedcomIndividualRecord subject, IDateRenderer dateRenderer)
            {
                Id = subject.CrossReferenceId.ToSimpleIndividualId();
                Name = GetName(subject);
                Gender = subject.Sex.ToString();
                DateOfBirth = dateRenderer.RenderAsShortDate(subject.BirthEvent?.Date);
                DateOfDeath = dateRenderer.RenderAsShortDate(subject.DeathEvent?.Date);
                return this;
            }
        }

        public class ParentModel : PersonModel
        {
            [JsonProperty(Order = 1)]
            public string RelationToSubject { get; set; }

            public ParentModel FromRelative(ImmediateRelative relative, IDateRenderer dateRenderer, IRelationshipRenderer relationshipRenderer)
            {
                FromSubject(relative.Relative, dateRenderer);
                RelationToSubject = relationshipRenderer.HumanReadable(relative.TypeOfRelationship, true);
                return this;
            }
        }

        public class SiblingModel : PersonModel
        {
            [JsonProperty(Order = 1)]
            public string RelationToSubject { get; set; }
            
            [JsonProperty(Order = 2)]
            public string[][] ParentGroupIds { get; set; }

            public SiblingModel FromRelative(ImmediateRelative relative,
                IDateRenderer dateRenderer, IRelationshipRenderer relationshipRenderer)
            {
                FromSubject(relative.Relative, dateRenderer);
                RelationToSubject = relationshipRenderer.HumanReadable(relative.TypeOfRelationship, true);
                ParentGroupIds = relative.Relative.ChildToFamilies
                    .Select(f => f.Spouses
                        .Select(s => s.CrossReferenceId.ToSimpleIndividualId())
                        .OrderBy(id => id)
                        .ToArray())
                    .ToArray();
                return this;
            }
        }

        public class SpouseModel : PersonModel
        {
            [JsonProperty(Order = 1)]
            public string RelationToSubject { get; set; }

            [JsonProperty(Order = 2)]
            public string DateOfMarriage { get; set; }
            
            [JsonProperty(Order = 3)]
            public string DateOfDivorce { get; set; }

            public SpouseModel FromRelative(ImmediateRelative relative, IDateRenderer dateRenderer,
                IRelationshipRenderer relationshipRenderer)
            {
                FromSubject(relative.Relative, dateRenderer);
                RelationToSubject = relationshipRenderer.HumanReadable(relative.TypeOfRelationship, true);
                var family = relative.Subject.SpouseToFamilies
                    .FirstOrDefault(f => f.OtherSpouse(relative.Subject) == relative.Relative);
                
                var marriage = family?.Events
                    .FirstOrDefault(fe => fe.Tag == GedcomFamilyEventRecord.MarriageTag);
                DateOfMarriage = dateRenderer.RenderAsShortDate(marriage?.Date);

                var divorce = family?.Events
                    .FirstOrDefault(fe => fe.Tag == GedcomFamilyEventRecord.DivorceTag);
                DateOfDivorce = dateRenderer.RenderAsShortDate(divorce?.Date);
                return this;
            }
        }

        public class ChildModel : PersonModel
        {
            [JsonProperty(Order = 1)]
            public string RelationToSubject { get; set; }

            [JsonProperty(Order = 2)]
            public string[] ParentIds { get; set; }

            public ChildModel FromRelative(ImmediateRelative relative, IDateRenderer dateRenderer,
                IRelationshipRenderer relationshipRenderer)
            {
                FromSubject(relative.Relative, dateRenderer);
                RelationToSubject = relationshipRenderer.HumanReadable(relative.TypeOfRelationship, true);
                ParentIds = relative.Relative
                    .Parents()
                    .Select(p => p.Relative.CrossReferenceId.ToSimpleIndividualId())
                    .ToArray();
                return this;
            }
        }
        public RenderPersonFamilyTreeAsJsonHandler(
            ILogger<RenderPersonFamilyTreeAsJsonHandler> logger,
            IDateRenderer dateRenderer,
            IRelationshipRenderer relationshipRenderer,
            IFileNamer fileNamer) 
            : base(logger, fileNamer)
        {
            _dateRenderer = dateRenderer;
            _relationshipRenderer = relationshipRenderer;
        }

        protected override string FileType => "family-tree";
        protected override object MapIndividual(GedcomIndividualRecord subject)
        {
            var relatives = subject.GetImmediateRelatives();
            var parents = relatives
                .Where(r => r.TypeOfRelationship.IsParent)
                .ToArray();
            var result = new FamilyTreeModel
            {
                Subject = new PersonModel().FromSubject(subject, _dateRenderer),
                Parents = parents
                    .OrderBy(p=>p.Relative.BirthEvent?.Date)
                    .Select(r=>new ParentModel().FromRelative(r, _dateRenderer, _relationshipRenderer))
                    .ToArray(),
                Siblings = relatives
                    .Where(r=>r.TypeOfRelationship.IsSibling)
                    .Union(new[]
                    {
                        // Inject self into siblings to make it easier to position in sequence.
                        new ImmediateRelative(subject, subject, new Relationship(subject.Sex.ToGender(), GenerationZeroRelationships.Self))
                    })
                    .OrderBy(p=>p.Relative.BirthEvent?.Date)
                    .Select(r=>new SiblingModel().FromRelative(r, _dateRenderer, _relationshipRenderer))
                    .ToArray(),
                Spouses = relatives
                    .Where(r=>r.TypeOfRelationship.IsSpouse)
                    .OrderBy(p=>p.Relative.BirthEvent?.Date)
                    .Select(r => new SpouseModel().FromRelative(r, _dateRenderer, _relationshipRenderer))
                    .ToArray(),
                Children = relatives
                    .Where(r=>r.TypeOfRelationship.IsChild)
                    .OrderBy(p=>p.Relative.BirthEvent?.Date)
                    .Select(r => new ChildModel().FromRelative(r, _dateRenderer, _relationshipRenderer))
                    .ToArray()
            };
            return result;
        }
    }
}