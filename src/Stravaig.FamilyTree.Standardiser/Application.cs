using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Stravaig.FamilyTree.Standardiser.Extensions;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Extensions;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTree.Standardiser
{
    public class Application
    {
        private static readonly GedcomTag HeadTag = "HEAD".AsGedcomTag();
        
        private readonly ILogger<Application> _logger;
        private readonly CommandLineOptions _options;
        private readonly GedcomDatabase _database;
        private readonly FileNamer _fileName;

        public Application(
            ILogger<Application> logger, 
            CommandLineOptions options,
            FileNamer fileNamer,
            GedcomDatabase database)
        {
            _logger = logger;
            _options = options;
            _fileName = fileNamer;
            _database = database;
        }

        public void Run()
        {
            _logger.LogInformation("Creating standardised GEDCOM file from {source} and writing it to {destination}.",
                _options.SourceFile, _options.DestinationFile);

            RenameDestinationFileIfExists();

            using var fs = new FileStream(_options.DestinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(fs, Encoding.UTF8);
            var orderedRecords = _database.StandardisedTopLevelRecordOrder();
            for (int i = 0; i < orderedRecords.Length; i++)
            {
                var record = orderedRecords[i];
                WriteTopLevelRecord(writer, record);
            }
        }

        private void WriteTopLevelRecord(StreamWriter writer, GedcomRecord record)
        {
            writer.WriteLine(record.Line.ToString());
            foreach (var child in _database.OrderChildren(record))
            {
                if (GedcomIndividualEventRecord.EventTags.Contains(child.Tag) || 
                    GedcomIndividualAttributeRecord.AttributeTags.Contains(child.Tag))
                    WriteIndividualEventChildren(writer, child);
                else if (GedcomFamilyEventRecord.FamilyEventTags.Contains(child.Tag))
                    WriteFamilyEventChildren(writer, child);
                else
                    WriteChildRecord(writer, child);
            }
        }

        private void WriteIndividualEventChildren(StreamWriter writer, GedcomRecord record)
        {
            writer.WriteLine(record.Line.ToString());
            foreach (var child in record.OrderIndividualEventChildren())
                WriteChildRecord(writer, child);
        }

        private void WriteFamilyEventChildren(StreamWriter writer, GedcomRecord record)
        {
            writer.WriteLine(record.Line.ToString());
            foreach (var child in record.OrderFamilyEventChildren())
                WriteChildRecord(writer, child);
        }

        private void WriteChildRecord(StreamWriter writer, GedcomRecord record)
        {
            if (record.Tag.IsUserDefined)
                return;
            
            if (record.Tag == GedcomPlaceRecord.PlaceTag)
                WriteNormalisedPlaceRecord(writer, record);
            else
                WriteGeneralChildRecord(writer, record);
        }

        private void WriteGeneralChildRecord(StreamWriter writer, GedcomRecord record)
        {
            writer.WriteLine(record.Line.ToString());
            foreach (var child in _database.OrderChildren(record))
                WriteChildRecord(writer, child);
        }

        private void WriteNormalisedPlaceRecord(StreamWriter writer, GedcomRecord @record)
        {
            var level = @record.Level;
            var tag = @record.Tag;
            var value = string.Join(',', (@record.Value ?? string.Empty).Split(',', StringSplitOptions.TrimEntries));
            writer.WriteLine($"{level} {tag} {value}".Trim());
            foreach (var child in _database.OrderChildren(record))
            {
                if (child.Tag == GedcomPlaceRecord.FormTag && child.Parent?.Parent?.Tag != HeadTag)
                    continue;
                WriteChildRecord(writer, child);
            }
        }

        private void RenameDestinationFileIfExists()
        {
            if (File.Exists(_options.DestinationFile))
            {
                var newFileName = _fileName.RenameFile(_options.DestinationFile);
                _logger.LogInformation("Renaming \"{destinationFile}\" to \"{renamedFile}\".",
                    _options.DestinationFile, newFileName);
                File.Move(_options.DestinationFile, newFileName);
            }
        }
    }
}