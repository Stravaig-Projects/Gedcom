using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.Policies.Attributes;
using Stravaig.FamilyTreeGenerator.Services;

namespace Stravaig.FamilyTreeGenerator.Requests.Handlers
{
    public class InitFileSystemForMarkdownHandler : RequestHandler<InitFileSystem>
    {
        public const string InitFileStstemRetryPolicy = nameof(InitFileStstemRetryPolicy);
        private readonly IFileNamer _namer;
        private readonly ILogger<InitFileSystemForMarkdownHandler> _logger;

        public InitFileSystemForMarkdownHandler(IFileNamer namer, ILogger<InitFileSystemForMarkdownHandler> logger)
        {
            _namer = namer;
            _logger = logger;
        }

        [UsePolicy(policy:InitFileStstemRetryPolicy, step: 1)]
        public override InitFileSystem Handle(InitFileSystem command)
        {
            ResetFileSystem();
            CreateRequiredDirectories();

            return base.Handle(command);
        }

        private void ResetFileSystem()
        {
            var baseDirectory = _namer.BaseDirectory();
            if (baseDirectory.Exists)
            {
                int currentAttempt = 0;
                int attemptsRemaining = 5;
                while (attemptsRemaining > 0)
                {
                    try
                    {
                        currentAttempt++;
                        baseDirectory.Delete(true);
                        attemptsRemaining = 0;
                    }
                    catch (IOException ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete temporary directory on attempt {CurrentAttempt} {TryAgain}. {ExMessage}",
                            currentAttempt,
                            attemptsRemaining > 0 ? "Will try again." : "Won't try again. Operation failed.",
                            ex.Message);
                        Thread.Sleep(1000);
                    }
                    finally
                    {
                        attemptsRemaining--;
                    }
                }
            }
        }

        private void CreateRequiredDirectories()
        {
            var directories = _namer.RequiredDirectories();
            foreach (var dir in directories)
            {
                if (!dir.Exists)
                {
                    _logger.LogInformation("Creating directory {dir}", dir);
                    dir.Create();
                }
            }
        }
    }
}
