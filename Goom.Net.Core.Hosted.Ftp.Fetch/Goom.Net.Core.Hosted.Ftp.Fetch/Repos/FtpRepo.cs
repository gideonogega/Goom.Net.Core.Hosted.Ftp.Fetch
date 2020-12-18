using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace Goom.Net.Core.Hosted.Ftp.Fetch.Repos
{
    public interface IFtpRepo
    {
        List<string> ListAllInputFiles();
        string ReadInputFile(string fileName);
        void ArchiveInputFile(string fileName);
    }

    public class FtpRepo : IFtpRepo
    {
        private readonly ILogger<FtpRepo> logger;
        private readonly SftpEndpointInfo endpointInfo;
        private readonly ConnectionInfo connection;

        public FtpRepo(ILogger<FtpRepo> logger, SftpEndpointInfo endpointInfo)
        {
            this.logger = logger;
            this.endpointInfo = endpointInfo;

            connection = new ConnectionInfo(
                endpointInfo.Host,
                endpointInfo.Port,
                endpointInfo.Username,
                new PasswordAuthenticationMethod(endpointInfo.Username, endpointInfo.Password));
        }

        public List<string> ListAllInputFiles()
        {
            try
            {
                using (var client = new SftpClient(connection))
                {
                    client.Connect();

                    var files = client.ListDirectory(endpointInfo.InputDirectory);
                    return files.Select(x => x.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                var msg = $"Unable to read files from {endpointInfo.InputDirectory}. Details: {ex.Message}";
                logger.LogError(msg, ex);
                return new List<string>();
            }
        }

        public string ReadInputFile(string fileName)
        {
            var path = $"{endpointInfo.InputDirectory}/{fileName}";
            try
            {
                using (var client = new SftpClient(connection))
                {
                    client.Connect();

                    return client.ReadAllText(path);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Unable to read file {path}. Details: {ex.Message}";
                logger.LogError(msg, ex);
                return null;
            }
        }

        public void ArchiveInputFile(string fileName)
        {
            var inputPath = $"{endpointInfo.InputDirectory}/{fileName}";
            var archivePath = $"{endpointInfo.ArchiveDirectory}/{fileName}";
            try
            {
                using (var client = new SftpClient(connection))
                {
                    client.Connect();
                    var file = client.Get(inputPath);

                    file.MoveTo(archivePath);
                }
            }
            catch (Exception ex)
            {
                var msg = $"Unable to archive file {inputPath}. Details: {ex.Message}";
                logger.LogError(msg, ex);
            }
        }
    }

    public class SftpEndpointInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string InputDirectory { get; set; }
        public string ArchiveDirectory { get; set; }
    }
}
