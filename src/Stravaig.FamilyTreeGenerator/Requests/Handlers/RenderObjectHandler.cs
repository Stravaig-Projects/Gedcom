using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Stravaig.FamilyTreeGenerator.Services;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers;

public class RenderObjectHandler : RequestHandler<RenderObject>
{
    private const double MaxPictureSize = 1000;
    private readonly ILogger<RenderObjectHandler> _logger;
    private readonly IFileNamer _fileNamer;

    public RenderObjectHandler(
        ILogger<RenderObjectHandler> logger,
        IFileNamer fileNamer)
    {
        _logger = logger;
        _fileNamer = fileNamer;
    }

    public override RenderObject Handle(RenderObject command)
    {
        var obj = command.Object;
        bool grayScale = obj.HasLabel(Labels.GrayscaleImage);
        var input = _fileNamer.GetSourceMediaFile(obj);
        var output = _fileNamer.GetDestinationMediaFile(obj);

        _logger.LogInformation($"Rendering {obj.Title} to {output}");

        using var image = Image.Load(input);
        var originalWidth = image.Width;
        var originalHeight = image.Height;
        var width = (double)originalWidth;
        var height = (double)originalHeight;
        if (width > MaxPictureSize)
        {
            var scale = width / MaxPictureSize;;
            height /= scale;
            width = MaxPictureSize;
        }

        if (grayScale)
            image.Mutate(ipc => ipc.Grayscale());

        image.Mutate(ipc => ipc.Resize((int)width, (int)height));
        using var fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.Read);
        image.Save(fs, new JpegEncoder
        {
            SkipMetadata = true,
            Quality = 75,
        });

        var originalFileSize = new FileInfo(input).Length;
        var newFileSize = new FileInfo(output).Length;

        _logger.LogInformation($"Rendered {obj.Title} to {output}.\n"+
                               $"  Dimensions: {originalWidth}x{originalHeight} --> {width:F0}x{height:F0}\n"+
                               $"  File Size: {originalFileSize/1024.0:F1} KiB --> {newFileSize/1024.0:F1} KiB");

        return command;
    }
}
