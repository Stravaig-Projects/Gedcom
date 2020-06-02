using System.IO;
using System.Text;
using Humanizer;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public abstract class RenderPersonIndexBaseHandler : RequestHandler<RenderPersonIndex>
    {
        private readonly ILogger _logger;
        protected readonly IFileNamer _fileNamer;

        protected RenderPersonIndexBaseHandler(ILogger logger,
            IFileNamer fileNamer)
        {
            _logger = logger;
            _fileNamer = fileNamer;
        }
        
        public override RenderPersonIndex Handle(RenderPersonIndex command)
        {
            _logger.LogInformation("Rendering Index.");
            using FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);
            WriteHeader(writer);
            WriteIndex(writer, command.Individuals);

            return base.Handle(command);
        }
        
        protected abstract string FileName { get; }

        private void WriteHeader(TextWriter writer)
        {
            string fileName = new FileInfo(FileName).Name
                .Replace("index-", string.Empty)
                .Replace(".md",string.Empty);
            writer.WriteLine("---");
            writer.WriteLine("layout: page");
            writer.WriteLine($"permalink: /indexes/{fileName}");
            writer.WriteLine("---");
            writer.WriteLine();

            writer.WriteLine($"# Index - {fileName.Humanize(LetterCasing.Title)}");
        }
        protected abstract void WriteIndex(TextWriter writer, GedcomIndividualRecord[] individuals);
    }
}