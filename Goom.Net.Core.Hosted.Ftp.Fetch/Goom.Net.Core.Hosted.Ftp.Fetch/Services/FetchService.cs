using Goom.Net.Core.Hosted.Ftp.Fetch.Repos;
using Microsoft.Extensions.Logging;

namespace Goom.Net.Core.Hosted.Ftp.Fetch.Services
{
    public interface IFetchService
    {
        void Fetch();
    }

    public class FetchService : IFetchService
    {
        private readonly ILogger<FetchService> logger;
        private readonly IFtpRepo ftpRepo;

        public FetchService(ILogger<FetchService> logger, IFtpRepo ftpRepo)
        {
            this.logger = logger;
            this.ftpRepo = ftpRepo;
        }

        public void Fetch()
        {
            var files = ftpRepo.ListAllInputFiles();
            logger.LogInformation($"Found {files.Count} files");
            foreach (var file in files)
            {
                var fileContent = ftpRepo.ReadInputFile(file);
                logger.LogInformation(fileContent);
                ftpRepo.ArchiveInputFile(file);
            }
        }
    }
}
