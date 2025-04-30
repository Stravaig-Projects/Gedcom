using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;
using Stravaig.FamilyTreeGenerator.Services;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers;

public class RenderOnThisDayJsonHandler : RequestHandler<RenderPersonIndex>
{
    private readonly IIndividualNameRenderer _nameRenderer;
    private readonly IFileNamer _fileNamer;
    private readonly ILogger<RenderOnThisDayJsonHandler> _logger;

    public RenderOnThisDayJsonHandler(IIndividualNameRenderer nameRenderer, IFileNamer fileNamer, ILogger<RenderOnThisDayJsonHandler> logger)
    {
        _nameRenderer = nameRenderer;
        _fileNamer = fileNamer;
        _logger = logger;
    }

    private string FileName => _fileNamer.GetOnThisDayJsonFile();

    public override RenderPersonIndex Handle(RenderPersonIndex command)
    {
        _logger.LogInformation("Rendering On This Day Json file.");
        
        
        
        using FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
        using TextWriter writer = new StreamWriter(fs, Encoding.UTF8);
        
        return base.Handle(command);
    }
}