using Microsoft.EntityFrameworkCore;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.Data;
using Renci.SshNet;

namespace PDEWebAPIS.Services
{
    public class SftpService
    {
        private readonly string host = "10.10.248.2";
        private readonly int port = 22; // Default SFTP port
        private readonly string username = "smbadmin";
        private readonly string password = "$m6@DM!9";
        private readonly ILogger<SftpService> _logger;
        public SftpService(ILogger<SftpService> logger)
        {
            _logger = logger;
        }
        // Upload File
        public void UploadFile(Stream fileStream, string remoteFileName)
        {
            _logger.LogInformation("SFTP Connection - host - " + host + " port - " + port + " hostname - " + host + " password - " + password);
            try
            {
                using (var sftp = new SftpClient(host, port, username, password))
                {
                    sftp.Connect();
                    _logger.LogInformation("SFTP Connected Successfully - " + fileStream);
                    if (sftp.IsConnected)
                    {
                        sftp.UploadFile(fileStream, $"/upload/{remoteFileName}");
                    }
                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SFTP Error - " + ex.Message!.ToString());
            }
        }

        // Download File
        public MemoryStream DownloadFile(string remoteFileName)
        {
            using (var sftp = new SftpClient(host, port, username, password))
            {
                sftp.Connect();
                var stream = new MemoryStream();
                sftp.DownloadFile($"/upload/{remoteFileName}", stream);
                sftp.Disconnect();
                stream.Position = 0; // Reset the stream position before returning
                return stream;
            }
        }
    }
}
