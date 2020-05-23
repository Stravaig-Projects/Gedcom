using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTreeGenerator.Extensions;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
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
            public PersonModel Subject { get; set; }
            public ParentModel[] Parents { get; set; }
            public SiblingModel[] Siblings { get; set; }
            public SpouseModel[] Spouses { get; set; }
            public ChildModel[] Children { get; set; }
        }
        public class PersonModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string DateOfBirth { get; set; }
            public string DateOfDeath { get; set; }

            public PersonModel FromSubject(GedcomIndividualRecord subject, IDateRenderer dateRenderer)
            {
                Id = subject.CrossReferenceId.ToSimpleId();
                Name = RenderPersonAsJsonHandlerBase.GetName(subject);
                DateOfBirth = dateRenderer.RenderAsShortDate(subject.BirthEvent?.Date);
                DateOfDeath = dateRenderer.RenderAsShortDate(subject.DeathEvent?.Date);
                return this;
            }
        }

        public class ParentModel : PersonModel
        {
            public string Pedigree { get; set; }

            public ParentModel FromRelative(ImmediateRelative relative, IDateRenderer dateRenderer, IRelationshipRenderer relationshipRenderer)
            {
                FromSubject(relative.Relative, dateRenderer);
                Pedigree = relationshipRenderer.HumanReadable(relative.TypeOfRelationship, true);
                return this;
            }
        }

        public class SiblingModel : PersonModel
        {
            public string Pedigree { get; set; }
            public string[] ParentIds { get; set; }

            public SiblingModel FromRelative(ImmediateRelative relative,
                IDateRenderer dateRenderer, IRelationshipRenderer relationshipRenderer)
            {
                FromSubject(relative.Relative, dateRenderer);
                Pedigree = relationshipRenderer.HumanReadable(relative.TypeOfRelationship, true);
                ParentIds = relative.Subject
                    .Parents()
                    .Select(p => GedcomPointerExtensions.ToSimpleId(p.Relative.CrossReferenceId))
                    .ToArray();
                return this;
            }
        }

        public class SpouseModel : PersonModel
        {
            public string MarriageDate { get; set; }
            public string DivorceDate { get; set; }

            public SpouseModel FromRelative(ImmediateRelative relative, IDateRenderer dateRenderer,
                IRelationshipRenderer relationshipRenderer)
            {
                FromSubject(relative.Relative, dateRenderer);
                var family = relative.Subject.SpouseToFamilies
                    .First(f => f.OtherSpouse(relative.Subject) == relative.Relative);
                
                var marriage = family?.Events
                    .FirstOrDefault(fe => fe.Tag == GedcomFamilyEventRecord.MarriageTag);
                MarriageDate = dateRenderer.RenderAsShortDate(marriage?.Date);

                var divorce = family?.Events
                    .FirstOrDefault(fe => fe.Tag == GedcomFamilyEventRecord.DivorceTag);
                DivorceDate = dateRenderer.RenderAsShortDate(divorce?.Date);
                return this;
            }
        }

        public class ChildModel : PersonModel
        {
            public string Pedigree { get; set; }
            public string[] ParentIds { get; set; }

            public ChildModel FromRelative(ImmediateRelative relative, IDateRenderer dateRenderer,
                IRelationshipRenderer relationshipRenderer)
            {
                FromSubject(relative.Relative, dateRenderer);
                Pedigree = relationshipRenderer.HumanReadable(relative.TypeOfRelationship, true);
                ParentIds = relative.Subject
                    .Parents()
                    .Select(p => p.Relative.CrossReferenceId.ToSimpleId())
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
                    .Select(r=>new ParentModel().FromRelative(r, _dateRenderer, _relationshipRenderer))
                    .ToArray(),
                Siblings = relatives
                    .Where(r=>r.TypeOfRelationship.IsSibling)
                    .Select(r=>new SiblingModel().FromRelative(r, _dateRenderer, _relationshipRenderer))
                    .ToArray(),
                Spouses = relatives.Where(r=>r.TypeOfRelationship.IsSpouse)
                    .Select(r => new SpouseModel().FromRelative(r, _dateRenderer, _relationshipRenderer))
                    .ToArray(),
                Children = relatives
                    .Where(r=>r.TypeOfRelationship.IsChild)
                    .Select(r => new ChildModel().FromRelative(r, _dateRenderer, _relationshipRenderer))
                    .ToArray()
            };
            return result;
        }
    }
}