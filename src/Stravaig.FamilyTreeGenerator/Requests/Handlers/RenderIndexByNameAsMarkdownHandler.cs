using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public class RenderIndexByNameAsMarkdownHandler : RequestHandler<RenderIndex>
    {
        private readonly ILogger<RenderIndexByNameAsMarkdownHandler> _logger;
        private readonly IFileNamer _fileNamer;

        public RenderIndexByNameAsMarkdownHandler(
            ILogger<RenderIndexByNameAsMarkdownHandler> logger,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _fileNamer = fileNamer;
        }
        public override RenderIndex Handle(RenderIndex command)
        {
            _logger.LogInformation("Rendering Index by name.");

            var fileName = _fileNamer.GetByNameIndexFile();
            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);
            WriteHeader(writer);
            WriteIndex(writer, command.Individuals);
            
            return base.Handle(command);
        }

        private void WriteHeader(TextWriter writer)
        {       
            writer.WriteLine("# Index - By Family Name");
        }

        private void WriteIndex(TextWriter writer, GedcomIndividualRecord[] people)
        {
            var familyGroups = people
                .GroupBy(p => p.FamilyName)
                .OrderBy(g => g.Key)
                .ToArray();

            foreach (var family in familyGroups)
            {
                writer.WriteLine();
                if (string.IsNullOrWhiteSpace(family.Key))
                    writer.WriteLine("## ???");
                else
                    writer.WriteLine($"## {family.Key}");
                writer.WriteLine();

                var familyMembers = family
                    .OrderBy(fm => fm.Name)
                    .ThenBy(fm => fm.BirthEvent?.Date?.Year)
                    .ThenBy(fm => fm.BirthEvent?.Date?.Month)
                    .ThenBy(fm => fm.BirthEvent?.Date?.Day)
                    .ToArray();
                foreach (var person in familyMembers)
                {
                    string filePath = _fileNamer.GetIndividualFile(person, true, true);
                    writer.Write($"- **[{person.NameWithoutMarker.Trim()}]({filePath})**");

                    if (person.BirthEvent == null && person.DeathEvent == null)
                    {
                        writer.WriteLine();                        
                        continue;
                    }
                    
                    if (person.BirthEvent != null)
                    {
                        int? day = person.BirthEvent.Date?.Day;
                        int? month = person.BirthEvent.Date?.Month;
                        int? year = person.BirthEvent.Date?.Year;
                        string format = "";
                        if (day.HasValue) format += "d/";
                        if (month.HasValue) format += "MMM/";
                        if (year.HasValue) format += "yyyy";
                        format = format.TrimEnd('-');
                        var date = new DateTime(year??1, month??1, day??1);
                        writer.Write(" (");
                        if (!string.IsNullOrWhiteSpace(format))
                            writer.Write(date.ToString(format));
                        writer.Write(" - ");
                    }
                    else
                    {
                        writer.Write(" ( - ");
                    }

                    if (person.DeathEvent != null)
                    {
                        int? day = person.DeathEvent.Date?.Day;
                        int? month = person.DeathEvent.Date?.Month;
                        int? year = person.DeathEvent.Date?.Year;
                        string format = "";
                        if (day.HasValue) format += "d/";
                        if (month.HasValue) format += "MMM/";
                        if (year.HasValue) format += "yyyy";
                        format = format.TrimEnd('-');
                        var date = new DateTime(year??1, month??1, day??1);
                        if (!string.IsNullOrWhiteSpace(format))
                            writer.Write(date.ToString(format));
                        writer.Write(")");
                    }
                    else
                    {
                        writer.Write(")");
                    }
                    writer.WriteLine();
                }
            }


        }
    }
}