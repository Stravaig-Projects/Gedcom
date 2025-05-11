using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers;

public class RenderOnThisDayJsonHandler : RequestHandler<RenderPersonIndex>
{
    private static GedcomTag[] InterestingEvents =
    [
        GedcomIndividualEventRecord.BirthTag,
        GedcomIndividualEventRecord.DeathTag,
        GedcomFamilyEventRecord.MarriageTag,
    ];
    private readonly IIndividualNameRenderer _nameRenderer;
    private readonly IFileNamer _fileNamer;
    private readonly ILogger<RenderOnThisDayJsonHandler> _logger;
    private readonly IDateRenderer _dateRenderer;

    public RenderOnThisDayJsonHandler(IIndividualNameRenderer nameRenderer, IFileNamer fileNamer, ILogger<RenderOnThisDayJsonHandler> logger, IDateRenderer dateRenderer)
    {
        _nameRenderer = nameRenderer;
        _fileNamer = fileNamer;
        _logger = logger;
        _dateRenderer = dateRenderer;
    }

    private string FileName => _fileNamer.GetOnThisDayJsonFile();

    public override RenderPersonIndex Handle(RenderPersonIndex command)
    {
        _logger.LogInformation("Rendering On This Day Json file.");

        var significantEvents = command.Individuals
            .SelectMany(i => i.GetTimeline(false))
            .Where(te => InterestingEvents.Contains(te.Tag) &&
                         te.Type != TimelineEntry.EventType.FamilyMemberEvent &&
                         !te.Date.Type.IsVague() &&
                         te.Date.ExactDate1.HasValue)
            .Select(GenerateNewEvent);

        JsonDto dto = new JsonDto();
        foreach (var (month, day, eventDto) in significantEvents)
        {
            dto.Months[month - 1].Days[day - 1].Events.Add(eventDto);
        }

        using FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
        using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);
        using JsonTextWriter jsonWriter = new JsonTextWriter(writer);
        jsonWriter.Formatting = Formatting.Indented;
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(jsonWriter, dto);

        return base.Handle(command);
    }

    private (int Month, int Day, EventDto Dto) GenerateNewEvent(TimelineEntry te)
    {
        //Console.WriteLine($"Generating {te.Tag} event for {te.Subject.NameWithoutMarker} on {te.Date.ExactDate1}. Type = {te.Type}.");
        var month = te.Date.ExactDate1!.Value.Month;
        var day = te.Date.ExactDate1!.Value.Day;

        if (te.Type == TimelineEntry.EventType.FamilyEvent)
            return (month, day, GenerateFamilyEvent(te));
        return (month, day, GenerateIndividualEvent(te));
    }

    private EventDto GenerateIndividualEvent(TimelineEntry te)
    {
        string description;
        if (te.Tag == GedcomIndividualEventRecord.BirthTag)
            description = GenerateBirthDescription(te);
        else if (te.Tag == GedcomIndividualEventRecord.DeathTag)
            description = GenerateDeathDescription(te);
        else
            description= string.Empty;


        return new EventDto()
        {
            //Id1 = te.Subject.CrossReferenceId.ToString(),
            Date = te.Date.ExactDate1!.Value.ToString("yyyy-MM-dd"),
            Description = description,
        };
    }

    private string GenerateBirthDescription(TimelineEntry te)
    {
        if (te.OtherFamilyMember != null)
            throw new InvalidOperationException($"Birth events should not have a family member. Subject = {te.Subject.NameWithoutMarker}; Other Family Member = {te.OtherFamilyMember.NameWithoutMarker}; Type={te.Type}");

        StringBuilder sb = new StringBuilder();
        var subject = te.Subject;
        sb.Append("[");
        sb.Append(subject.NameWithoutMarker);
        sb.Append("](/family-tree/people/i");
        sb.Append(subject.CrossReferenceId.ToString().Replace("@", string.Empty));
        sb.Append(")");
        sb.Append(" was born ");
        sb.Append(_dateRenderer.RenderAsProse(te.Date));
        var parents = subject.Parents();
        if (parents.Length > 0)
        {
            sb.Append(" to [");
            sb.Append(parents[0].Relative.NameWithoutMarker);
            sb.Append("](/family-tree/people/i");
            sb.Append(parents[0].Relative.CrossReferenceId.ToString().Replace("@", string.Empty));
            sb.Append(")");
            if (parents.Length > 1)
            {
                sb.Append(" and [");
                sb.Append(parents[1].Relative.NameWithoutMarker);
                sb.Append("](/family-tree/people/i");
                sb.Append(parents[1].Relative.CrossReferenceId.ToString().Replace("@", string.Empty));
                sb.Append(")");
            }
        }

        if (te.IndividualEvent.Address != null)
        {
            sb.Append(" at ");
            sb.Append(te.IndividualEvent.Address.Text);
        }
        else if (te.IndividualEvent.Place != null)
        {
            sb.Append(" in ");
            sb.Append(te.IndividualEvent.Place.NormalisedPlaceName());
        }
        sb.Append('.');
        return sb.ToString();
    }

    private string GenerateDeathDescription(TimelineEntry te)
    {
        StringBuilder sb = new StringBuilder();
        var subject = te.Subject;
        sb.Append("[");
        sb.Append(subject.NameWithoutMarker);
        sb.Append("](/family-tree/people/i");
        sb.Append(subject.CrossReferenceId.ToString().Replace("@", string.Empty));
        sb.Append(") died ");
        sb.Append(_dateRenderer.RenderAsProse(te.Date));

        if (te.IndividualEvent.Address != null)
        {
            sb.Append(" at ");
            sb.Append(te.IndividualEvent.Address.Text);
        }
        else if (te.IndividualEvent.Place != null)
        {
            sb.Append(" in ");
            sb.Append(te.IndividualEvent.Place.NormalisedPlaceName());
        }
        sb.Append('.');
        return sb.ToString();
    }

    private EventDto GenerateFamilyEvent(TimelineEntry te)
    {
        string description = "";
        if (te.Tag == GedcomFamilyEventRecord.MarriageTag)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(te.Subject.NameWithoutMarker);
            sb.Append("](/family-tree/people/i");
            sb.Append(te.Subject.CrossReferenceId.ToString().Replace("@", string.Empty));
            sb.Append(") got married");
            var spouse = te.Family.Spouses.FirstOrDefault(s => s != te.Subject);
            if (spouse?.IsDead() ?? false)
            {
                sb.Append(" to [");
                sb.Append(spouse.NameWithoutMarker);
                sb.Append("](/family-tree/people/i");
                sb.Append(spouse.CrossReferenceId.ToString().Replace("@", string.Empty));
                sb.Append(")");
            }
            sb.Append(' ');
            sb.Append(_dateRenderer.RenderAsProse(te.Date));
            if (te.FamilyEvent.Address != null)
            {
                sb.Append(" at ");
                sb.Append(te.FamilyEvent.Address.Text);
            }
            else if (te.FamilyEvent.Place != null)
            {
                sb.Append(" in ");
                sb.Append(te.FamilyEvent.Place.NormalisedPlaceName());
            }
            sb.Append('.');
            description = sb.ToString();

            return new EventDto()
            {
                //Id1 = te.Subject.CrossReferenceId.ToString(),
                //Id2 = spouse?.CrossReferenceId.ToString(),
                Date = te.Date.ExactDate1!.Value.ToString("yyyy-MM-dd"),
                Description = description,
            };
        }

        throw new NotImplementedException();
    }

    public class JsonDto
    {
        public MonthDto[] Months { get; }
            = Enumerable.Range(1, 12)
                .Select(i => new MonthDto(i) { Month = i })
                .ToArray();
    }

    public class MonthDto
    {
        public MonthDto(int month)
        {
            Month = month;
            Days = Enumerable.Range(1, DateTime.DaysInMonth(2020, month))
                .Select(i => new DayDto { Day = i })
                .ToArray();
        }
        public int Month { get; init; }
        public DayDto[] Days { get; }
    }

    public class DayDto
    {
        public int Day { get; init; }
        public List<EventDto> Events { get; init; } = [];
    }

    public class EventDto
    {
        public string Date { get; init; }
        public string Description { get; init; }
    }
}
