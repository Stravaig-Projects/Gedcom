using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Stravaig.Gedcom;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTree.Standardiser
{
    public class Application
    {
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
            foreach(var child in record.Children)
                WriteChildRecord(writer, child);
        }

        private void WriteChildRecord(StreamWriter writer, GedcomRecord record)
        {
            writer.WriteLine(record.Line.ToString());
            foreach(var child in record.Children)
                WriteChildRecord(writer, child);
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