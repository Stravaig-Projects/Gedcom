using System;

namespace Stravaig.Gedcom
{
    public class GedcomLineBuilder
    {
        private GedcomLevel? _level;
        private GedcomPointer? _crossReferenceId;
        private GedcomTag? _tag;
        private string _value;

        public GedcomLine Build()
        {
            if (!_level.HasValue)
                throw new InvalidOperationException($"Cannot build a valid {nameof(GedcomLine)} because a level is required.");
            
            if (!_tag.HasValue)
                throw new InvalidOperationException($"Cannot build a valid {nameof(GedcomLine)} because a tag is required.");
            
            return new GedcomLine(_level.Value, _crossReferenceId, _tag.Value, _value);
        }

        public void Reset()
        {
            _level = null;
            _crossReferenceId = null;
            _tag = null;
            _value = null;
        }

        public GedcomLineBuilder SetLevel(int level)
        {
            return SetLevel(new GedcomLevel(level));
        }

        public GedcomLineBuilder SetLevel(GedcomLevel level)
        {
            _level = level;
            return this;
        }

        public GedcomLineBuilder SetCrossReferenceId(string crossReferenceId)
        {
            return crossReferenceId == null 
                ? SetCrossReferenceId((GedcomPointer?) null) 
                : SetCrossReferenceId(new GedcomPointer(crossReferenceId));
        }

        public GedcomLineBuilder SetCrossReferenceId(GedcomPointer? crossReferenceId)
        {
            _crossReferenceId = crossReferenceId;
            return this;
        }

        public GedcomLineBuilder SetTag(string tag)
        {
            return SetTag(new GedcomTag(tag));
        }

        public GedcomLineBuilder SetTag(GedcomTag tag)
        {
            _tag = tag;
            return this;
        }

        public GedcomLineBuilder SetValue(string value)
        {
            _value = value;
            return this;
        }
    }
}