using System;

namespace Stravaig.Gedcom.Model
{
    public enum GenerationZeroRelationships
    {
        NotGenZero = 0,
        Sibling,
        Self,
        Spouse,
    }

    public enum Direction
    {
        Ancestor = -1,
        SameGeneration = 0,
        Descendent = 1,
    }
    
    public readonly struct Relationship
    {
        public static readonly Relationship NotRelated = new Relationship(Model.Gender.Unknown, GenerationZeroRelationships.NotGenZero);
        private readonly GenerationZeroRelationships _generationZero;
        
        public Relationship(Gender gender, int directedGenerationsRemoved, Pedigree pedigree = Pedigree.Biological)
        {
            _generationZero = directedGenerationsRemoved == 0 
                ? GenerationZeroRelationships.Sibling 
                : GenerationZeroRelationships.NotGenZero;
            Gender = gender;
            DirectedGenerationsRemoved = directedGenerationsRemoved;
            Pedigree = pedigree;
        }
        
        public Relationship(Gender gender, int generationsRemoved, Direction direction, Pedigree pedigree = Pedigree.Biological)
            : this(gender, Math.Abs(generationsRemoved) * (int)direction, pedigree)
        {
        }

        public Relationship(Gender gender, GenerationZeroRelationships relationship, Pedigree pedigree = Pedigree.Biological)
            : this(gender, 0, pedigree)
        {
            _generationZero = relationship;
        }

        public bool IsNotRelated => _generationZero == GenerationZeroRelationships.NotGenZero && DirectedGenerationsRemoved == 0;
        public bool IsSelf => DirectedGenerationsRemoved == 0 && _generationZero == GenerationZeroRelationships.Self;
        public bool IsSpouse => DirectedGenerationsRemoved == 0 && _generationZero == GenerationZeroRelationships.Spouse;
        public bool IsSibling => DirectedGenerationsRemoved == 0 && _generationZero == GenerationZeroRelationships.Sibling;
        public bool IsParent => DirectedGenerationsRemoved == -1;
        public bool IsChild => DirectedGenerationsRemoved == 1;
        
        public Gender Gender { get; }
        public Pedigree Pedigree { get; }
        public int DirectedGenerationsRemoved { get; }

        public int GenerationsRemoved => Math.Abs(DirectedGenerationsRemoved);
        public Direction Direction => DirectedGenerationsRemoved == 0
            ? Direction.SameGeneration
            : DirectedGenerationsRemoved < 0
                ? Direction.Ancestor
                : Direction.Descendent;

        public override int GetHashCode()
        {
            return DirectedGenerationsRemoved
                   + ((int) Gender * 32)
                   + ((int) Pedigree * 256)
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
            && a.Pedigree == b.Pedigree
            && a._generationZero == b._generationZero;
        public static bool operator !=(Relationship a, Relationship b) => !(a == b);
    }
}