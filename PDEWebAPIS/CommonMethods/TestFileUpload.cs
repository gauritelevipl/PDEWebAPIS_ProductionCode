using Renci.SshNet;

namespace PDEWebAPIS.CommonMethods
{
    public class TestFileUpload
    {
        private readonly ILogger<TestFileUpload> _logger;
        public TestFileUpload(ILogger<TestFileUpload> logger)
        {
            _logger = logger;
        }

        public bool TestFile(string ImgStr, string ImgName, string FolderPath, string CurrentDateTime)
        {
            //FolderPath = FolderPath + @"\";
            //Check if directory exist
            if (!System.IO.Directory.Exists(FolderPath))
            {
                System.IO.Directory.CreateDirectory(FolderPath); //Create directory if it doesn't exist
            }
            //set the image path
            _logger.LogInformation("Test File Upload Folder Path - " + FolderPath);
            string ext = Path.GetExtension(ImgName);
            ImgName = "Test" + "_" + CurrentDateTime + ext;
            string imgPath = Path.Combine(FolderPath, ImgName);
            byte[] imageBytes = Convert.FromBase64String(ImgStr);
            File.WriteAllBytes(imgPath, imageBytes);
            _logger.LogInformation("Test File Upload Image Path - " + imgPath);
            _logger.LogInformation("Test File Upload Data - " + ImgStr);
            return true;
        }
        public bool TestFile1(string ImgStr, string ImgName, string FolderPath, string CurrentDateTime)
        {
            // string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS";
            string Host = "10.10.248.2";
            int Port = 22;
            string Username = "smbadmin";
            string Password = "$m6@DM!9";
            //FolderPath = FolderPath + @"\";
            //Check if directory exist
            if (!System.IO.Directory.Exists(FolderPath))
            {
                System.IO.Directory.CreateDirectory(FolderPath); //Create directory if it doesn't exist
            }
            //set the image path
            _logger.LogInformation("Test File Upload Folder Path - " + FolderPath);
            string ext = Path.GetExtension(ImgName);
            ImgName = "Test" + "_" + CurrentDateTime + ext;
            string imgPath = Path.Combine(FolderPath, ImgName);
            //byte[] imageBytes = Convert.FromBase64String(ImgStr);
            //File.WriteAllBytes(imgPath, imageBytes);
            using (var client = new SftpClient(new PasswordConnectionInfo(Host, Port, Username, Password)))
            {
                client.Connect();
                client.ChangeDirectory(FolderPath);
                if (client.IsConnected)
                {
                    _logger.LogInformation("SFTP is connected Successfully- " + imgPath);
                    using (var fileStream = new FileStream(ImgStr, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024;
                        //byte[] imageBytes = Convert.FromBase64String(ImgStr);
                        //File.WriteAllBytes(imgPath, imageBytes);
                        //client.UploadFile(imageBytes, Path.GetFileName(imgPath));
                        client.UploadFile(fileStream, Path.GetFileName(imgPath));
                    }
                }
                client.Disconnect();
            }
            _logger.LogInformation("Test File Upload Image Path - " + imgPath);
            _logger.LogInformation("Test File Upload Data - " + ImgStr);
            return true;
        }
        public bool CheckFileExists(string FolderPath)
        {
            if (!System.IO.Directory.Exists(FolderPath))
            {
                System.IO.Directory.CreateDirectory(FolderPath); //Create directory if it doesn't exist
                _logger.LogInformation("Check File Exists - " + FolderPath);
            }
            //set the image path
            return true;
        }

    }
}
