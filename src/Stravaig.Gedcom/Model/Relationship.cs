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

    public enum Gender
     {
         Unknown = 0,
         Male,
         Female,
         NonBinary,
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
        
        public Relationship(Gender gender, int directedGenerationsRemoved)
        {
            _generationZero = directedGenerationsRemoved == 0 
                ? GenerationZeroRelationships.Sibling 
                : GenerationZeroRelationships.NotGenZero;
            Gender = gender;
            DirectedGenerationsRemoved = directedGenerationsRemoved;
        }
        
        public Relationship(Gender gender, int generationsRemoved, Direction direction)
            : this(gender, Math.Abs(generationsRemoved) * (int)direction)
        {
        }

        public Relationship(Gender gender, GenerationZeroRelationships relationship)
            : this(gender, 0)
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
        public int DirectedGenerationsRemoved { get; }

        public int GenerationsRemoved => Math.Abs(DirectedGenerationsRemoved);
        public Direction Direction => DirectedGenerationsRemoved == 0
            ? Direction.SameGeneration
            : DirectedGenerationsRemoved < 0
                ? Direction.Ancestor
                : Direction.Descendent;
    }
}