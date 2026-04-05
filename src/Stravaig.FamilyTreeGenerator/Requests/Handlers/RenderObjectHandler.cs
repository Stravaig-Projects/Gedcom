using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Stravaig.FamilyTreeGenerator.Services;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers;

public class RenderObjectHandler : RequestHandler<RenderObject>
{
    private const double MaxPictureSize = 1000;
    private const double ThumbnailSize = 32;
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
        var input = _fileNamer.GetSourceMediaFile(obj);
        var output = _fileNamer.GetDestinationMediaFile(obj);

        RenderObject(obj, input, output, MaxPictureSize, true);

        var thumbnail = _fileNamer.GetDestinationThumbnailFile(obj);
        RenderObject(obj, input, thumbnail, ThumbnailSize, false);

        return command;
    }

    private void RenderObject(GedcomObjectRecord obj, string input, string output, double newSize, bool fixWidth)
    {
        _logger.LogInformation($"Rendering {obj.Title} to {output}");

        using var image = Image.Load(input);
        bool grayScale = obj.HasLabel(Labels.GrayscaleImage);
        if (grayScale)
            image.Mutate(ipc => ipc.Grayscale());

        var originalWidth = image.Width;
        var originalHeight = image.Height;
        var width = (double)originalWidth;
        var height = (double)originalHeight;
        if (fixWidth)
        {
            if (width > newSize)
            {
                var scale = width / newSize;;
                height /= scale;
                width = newSize;
            }
        }
        else
        {
            if (height > newSize)
            {
                var scale = height / newSize;;
                width /= scale;
                height = newSize;
            }
        }
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
    }
}
