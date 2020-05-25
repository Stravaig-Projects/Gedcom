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
                baseDirectory.Delete(true);
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