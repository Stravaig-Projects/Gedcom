using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Stravaig.FamilyTreeGenerator.Services;

public interface IOccupationDescriptionService
{
    string GetDescription(string topLevelOccupation);
}

public class OccupationDescriptionService : IOccupationDescriptionService
{
    private readonly ILogger<OccupationDescriptionService> _logger;

    public OccupationDescriptionService(ILogger<OccupationDescriptionService> logger)
    {
        _logger = logger;
    }
    public string GetDescription(string topLevelOccupation)
    {
        const string prefix = "__resources.Occupations.";
        string resourceName = prefix + topLevelOccupation + ".md";
        var assembly = GetType().Assembly;
        var resourceStreamName = $"{Root.Namespace}.{resourceName}";
        try
        {
            var resourceStream = assembly.GetManifestResourceStream(resourceStreamName);

            // Allow the test to break if the resource needed can't be found.
            // ReSharper disable once AssignNullToNotNullAttribute
            using var reader = new StreamReader(resourceStream, Encoding.UTF8);
            string markdown = reader.ReadToEnd();
            markdown = markdown.Trim();
            return markdown;
        }
        catch(Exception ex)
        {
            if (topLevelOccupation.Equals("Lapidary", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("Available resources: " + Environment.NewLine + string.Join(Environment.NewLine, assembly.GetManifestResourceNames().Select(s => $" * {s}")));
                Thread.Sleep(1000);
                _logger.LogWarning($"Requested Resource: {resourceStreamName}");
                Thread.Sleep(2000);
            }
            return string.Empty;
        }
    }
}