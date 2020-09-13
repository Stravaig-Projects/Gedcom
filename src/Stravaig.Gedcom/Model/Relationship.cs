using System;

namespace Stravaig.Gedcom.Model
{
    public readonly struct Relationship
    {
        public static readonly Relationship NotRelated = new Relationship(Model.Gender.Unknown, GenerationZeroRelationships.NotGenZero);
        private readonly GenerationZeroRelationships _generationZero;
        
        public Relationship(Gender gender, int directedGenerationsRemoved, Qualification qualification = Qualification.Biological)
        {
            _generationZero = directedGenerationsRemoved == 0 
                ? GenerationZeroRelationships.Sibling 
                : GenerationZeroRelationships.NotGenZero;
            Gender = gender;
            DirectedGenerationsRemoved = directedGenerationsRemoved;
            Qualification = qualification;
        }
        
        public Relationship(Gender gender, int generationsRemoved, Direction direction, Qualification qualification = Qualification.Biological)
            : this(gender, Math.Abs(generationsRemoved) * (int)direction, qualification)
        {
        }

        public Relationship(Gender gender, GenerationZeroRelationships relationship, Qualification qualification = Qualification.Biological)
            : this(gender, 0, qualification)
        {
            _generationZero = relationship;
        }

        public bool IsNotRelated => _generationZero == GenerationZeroRelationships.NotGenZero && DirectedGenerationsRemoved == 0;
        public bool IsSelf => DirectedGenerationsRemoved == 0 && _generationZero == GenerationZeroRelationships.Self;
        public bool IsSpouse => DirectedGenerationsRemoved == 0 && _generationZero == GenerationZeroRelationships.Spouse;
        public bool IsSibling => DirectedGenerationsRemoved == 0 && _generationZero == GenerationZeroRelationships.Sibling;
        public bool IsParent => DirectedGenerationsRemoved == -1;
        public bool IsChild => DirectedGenerationsRemoved == 1;
        public bool IsAncestor => Direction == Direction.Ancestor;
        public bool IsDescendant => Direction == Direction.Descendant;
        public bool IsSameGeneration => Direction == Direction.SameGeneration;
        
        public Gender Gender { get; }
        public Qualification Qualification { get; }
        public int DirectedGenerationsRemoved { get; }

        public int GenerationsRemoved => Math.Abs(DirectedGenerationsRemoved);
        public Direction Direction => DirectedGenerationsRemoved == 0
            ? Direction.SameGeneration
            : DirectedGenerationsRemoved < 0
                ? Direction.Ancestor
                : Direction.Descendant;

        public override int GetHashCode()
        {
            return DirectedGenerationsRemoved
                   + ((int) Gender * 32)
                   + ((int) Qualification * 256)
                   + ((int) _generationZero * 2048);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Relationship other)
                return this == other;

            return false;
        }

        public static bool operator ==(Relationship a, Relationship b) =>
            a.DirectedGenerationsRemoved == b.DirectedGenerationsRemoved
            && a.Gender == b.Gender
            && a.Qualification == b.Qualification
            && a._generationZero == b._generationZero;
        public static bool operator !=(Relationship a, Relationship b) => !(a == b);
    }
}