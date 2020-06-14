﻿using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Stravaig.Gedcom.Model.Extensions
{
    public static class GedcomDatabaseExtensions
    {
        public static void Populate(this GedcomDatabase database, Stream stream)
        {
            using TextReader streamReader = new StreamReader(stream, Encoding.UTF8);
            database.Populate(streamReader);
        }

        public static void Populate(this GedcomDatabase database, TextReader textReader)
        {
            using GedcomLineReader lineReader = new GedcomLineReader(textReader);
            GedcomRecordReader recordReader = new GedcomRecordReader(lineReader);
            database.Populate(recordReader);
        }

        public static async Task PopulateAsync(this GedcomDatabase database, Stream stream)
        {
            using TextReader streamReader = new StreamReader(stream, Encoding.UTF8);
            await database.PopulateAsync(streamReader);
        }

        public static async Task PopulateAsync(this GedcomDatabase database, TextReader textReader)
        {
            using GedcomLineReader lineReader = new GedcomLineReader(textReader);
            GedcomRecordReader recordReader = new GedcomRecordReader(lineReader);
            await database.PopulateAsync(recordReader);
        }
    }
}