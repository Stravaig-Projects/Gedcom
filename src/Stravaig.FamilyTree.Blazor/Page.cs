using System;
using Stravaig.Gedcom;

namespace Stravaig.FamilyTree.Blazor
{
    public class Page
    {
        public PageType Type { get; private set; }
        public GedcomPointer? Id { get; private set; }

        public Page()
        {
            Type = PageType.IndexPersonByName;
            Id = null;
        }

        public Page SetIndex(PageType type)
        {
            if (type < PageType.MinIndex || type > PageType.MaxIndex)
                throw new ArgumentOutOfRangeException(nameof(type), "Expected PageType value to be in the range of index pages.");
            Type = type;
            Id = null;
            return this;
        }

        public Page SetPerson(GedcomPointer id)
        {
            Type = PageType.Person;
            Id = id;
            return this;
        }

        public Page SetSource(GedcomPointer id)
        {
            Type = PageType.Source;
            Id = id;
            return this;
        }

        public override string ToString()
        {
            if (Id.HasValue)
                return $"{Type}:{Id}";
            return Type.ToString();
        }
    }
}