using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers;

public class RenderPersonIndexAsJsonHandler : RequestHandler<RenderPersonIndex>
{
    private readonly ILogger<RenderPersonIndexAsJsonHandler> _logger;
    private readonly IIndividualNameRenderer _nameRenderer;
    private readonly IDateRenderer _dateRenderer;
    private readonly IFileNamer _fileNamer;

    public RenderPersonIndexAsJsonHandler(
        ILogger<RenderPersonIndexAsJsonHandler> logger,
        IIndividualNameRenderer nameRenderer,
        IFileNamer fileNamer,
        IDateRenderer dateRenderer)
    {
        _logger = logger;
        _nameRenderer = nameRenderer;
        _fileNamer = fileNamer;
        _dateRenderer = dateRenderer;
    }

    public override RenderPersonIndex Handle(RenderPersonIndex command)
    {
        var people = command.Individuals;

        var fileName = _fileNamer.GetSearchIndexJsonFile();
        _logger.LogInformation("Rendering Search Index.");
        using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
        using Utf8JsonWriter jsonWriter = new Utf8JsonWriter(fs, new JsonWriterOptions {Indented = true });

        jsonWriter.WriteStartArray();

        foreach (var person in people.OrderBy(p => p.CrossReferenceId))
        {
            jsonWriter.WriteStartObject();

            // Id
            WriteId(jsonWriter, person);
            WriteNameParts(jsonWriter, person);
            WriteWholeNames(jsonWriter, person);
            WriteBirthDeathEvent(jsonWriter, person.BirthEvent);
            WriteBirthDeathEvent(jsonWriter, person.DeathEvent);

            var marriages = person.SpouseToFamilies
                .SelectMany(s => s.Events.Where(e => e.Tag == GedcomFamilyEventRecord.MarriageTag)).ToArray();
            if (marriages.Length > 0)
            {
                jsonWriter.WritePropertyName("marriages");
                jsonWriter.WriteStartArray();
                foreach(var marriage in marriages)
                    WriteMarriageEvent(jsonWriter, marriage, person);
                jsonWriter.WriteEndArray();
            }

            var residences = person.Attributes
                .Where(e => e.Tag == GedcomIndividualAttributeRecord.ResidenceTag && e.Date != null && e.Place != null)
                .OrderBy(r => r.Date)
                .GroupBy(r => r.Place.Name)
                .ToArray();
            if (residences.Length > 0)
            {
                jsonWriter.WritePropertyName("residences");
                jsonWriter.WriteStartArray();
                foreach (var residenceGroup in residences)
                {
                    jsonWriter.WriteStartObject();
                    WritePlace(jsonWriter, residenceGroup.First().Place);
                    jsonWriter.WritePropertyName("dates");
                    jsonWriter.WriteStartArray();
                    foreach (var date in residenceGroup.Select(r => r.Date).Where(d => d.HasCoherentDate && d.Year1.HasValue))
                    {
                        jsonWriter.WriteNumberValue(date.Year1.Value);
                    }
                    jsonWriter.WriteEndArray();
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndArray();
            }

            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndArray();

        return base.Handle(command);
    }

    private void WriteMarriageEvent(Utf8JsonWriter jsonWriter, GedcomFamilyEventRecord marriage, GedcomIndividualRecord primaryPerson)
    {
        jsonWriter.WriteStartObject();
        var spouse = marriage.Family.Spouses.FirstOrDefault(s => s.CrossReferenceId != primaryPerson.CrossReferenceId);
        if (spouse != null)
        {
            jsonWriter.WritePropertyName("spouse");
            jsonWriter.WriteStringValue(RenderId(spouse));
        }
        WriteDate(jsonWriter, marriage.Date);
        WritePlace(jsonWriter, marriage.Place);
        jsonWriter.WriteEndObject();
    }

    private void WriteBirthDeathEvent(Utf8JsonWriter jsonWriter, GedcomIndividualEventRecord bdmEvent)
    {
        if (bdmEvent == null)
            return;
        if (bdmEvent.Tag == GedcomIndividualEventRecord.BirthTag)
            jsonWriter.WritePropertyName("birth");
        else
            jsonWriter.WritePropertyName("death");

        jsonWriter.WriteStartObject();
        var date = bdmEvent.Date;
        WriteDate(jsonWriter, date);

        var place = bdmEvent.Place;
        WritePlace(jsonWriter, place);
        jsonWriter.WriteEndObject();
    }

    private static void WritePlace(Utf8JsonWriter jsonWriter, GedcomPlaceRecord place)
    {
        if (place != null)
        {
            jsonWriter.WritePropertyName("place");
            jsonWriter.WriteStartArray();
            var parts = place.Name
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts)
            {
                jsonWriter.WriteStringValue(part.ToLowerInvariant());
            }
            jsonWriter.WriteEndArray();
        }
    }

    private void WriteDate(Utf8JsonWriter jsonWriter, GedcomDateRecord date)
    {
        if (date != null && date.HasCoherentDate)
        {
            jsonWriter.WritePropertyName("date");
            jsonWriter.WriteStringValue(_dateRenderer.RenderAsProse(date));
            if (date.ExactDate1.HasValue)
            {
                jsonWriter.WritePropertyName("year");
                jsonWriter.WriteNumberValue(date.ExactDate1!.Value.Year);
                jsonWriter.WritePropertyName("month");
                jsonWriter.WriteNumberValue(date.ExactDate1!.Value.Month);
                jsonWriter.WritePropertyName("day");
                jsonWriter.WriteNumberValue(date.ExactDate1!.Value.Day);
            }
            else
            {
                if (date.Year1.HasValue)
                {
                    jsonWriter.WritePropertyName("year");
                    jsonWriter.WriteNumberValue(date.Year1.Value);
                }

                if (date.Month1.HasValue)
                {
                    jsonWriter.WritePropertyName("month");
                    jsonWriter.WriteNumberValue(date.Month1.Value);
                }

                if (date.Day1.HasValue)
                {
                    jsonWriter.WritePropertyName("day");
                    jsonWriter.WriteNumberValue(date.Day1.Value);
                }
            }
        }
    }

    private static void WriteId(Utf8JsonWriter jsonWriter, GedcomIndividualRecord person)
    {
        jsonWriter.WritePropertyName("id");
        jsonWriter.WriteStringValue(RenderId(person));
    }

    private void WriteWholeNames(Utf8JsonWriter jsonWriter, GedcomIndividualRecord person)
    {
        jsonWriter.WritePropertyName("wholeNames");
        jsonWriter.WriteStartArray();
        foreach (var name in GetNameVariations(person))
            jsonWriter.WriteStringValue(name);
        jsonWriter.WriteEndArray();
    }

    private void WriteNameParts(Utf8JsonWriter jsonWriter, GedcomIndividualRecord person)
    {
        jsonWriter.WritePropertyName("nameParts");
        jsonWriter.WriteStartArray();
        foreach (var part in GetNameParts(person))
            jsonWriter.WriteStringValue(part);
        jsonWriter.WriteEndArray();
    }

    private IEnumerable<string> GetNameParts(GedcomIndividualRecord person)
    {
        return GetNameVariations(person)
            .SelectMany(p => p.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Distinct()
            .OrderBy(p => p);
    }

    private IEnumerable<string> GetNameVariations(GedcomIndividualRecord person)
    {
        return person.Names
            .Select(n => n.WholeName)
            .Distinct()
            .OrderBy(n => n);
    }

    private static string RenderId(GedcomIndividualRecord person)
    {
        return $"i{person.CrossReferenceId.ToString().Trim('@')}";
    }

    public class Person
    {
        public string Id { get; init; }

        public string[] Names { get; init; }
        public BdmEvent Birth { get; init; }
        public BdmEvent Death { get; init; }
        public BdmEvent Marriage { get; init; }
        public Residence[] Residences { get; init; }
        public string[] Professions { get; init; }
    }

    public class BdmEvent
    {
        public string Date { get; init; }
        public int Year { get; init; }
        public int Month { get; init; }
        public int Day { get; init; }
        public string[] Place { get; init; }
    }

    public class Residence
    {
        public string[] Place { get; init; }
        public int FromYear { get; init; }
        public int ToYear { get; init; }
    }

}
