namespace Stravaig.Gedcom.Model
{
    public interface ISubject
    {
        GedcomIndividualRecord Subject { get; }
    }

    public interface ISubjects
    {
        GedcomIndividualRecord[] Subjects { get; }
    }
}