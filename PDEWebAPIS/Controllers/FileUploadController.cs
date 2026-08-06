using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using System.IO;

namespace PDEWebAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");
            var uploadsFolder = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS";
            //@"D:\WWW\test";
            //Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { filePath });
        }

        //[HttpPost("upload1")]
        //public async Task<IActionResult> UploadFile1(IFormFile file)
        //{
        //    string FolderPath = @"\\10.10.248.2\pde_propertycard\MUTATIONDOCS";
        //    string Host = "10.10.248.2";
        //    int Port = 22;
        //    string Username = "smbadmin";
        //    string Password = "$m6@DM!9";
        //    var uploadFile = localFolderPath + FileName;
        //    using (var client = new SftpClient(new PasswordConnectionInfo(Host, Port, Username, Password)))
        //    {
        //        client.Connect();
        //        client.ChangeDirectory(FolderPath);
        //        if (client.IsConnected)
        //        {
        //            using (var fileStream = new FileStream(uploadFile, FileMode.))
        //            {
        //                client.BufferSize = 4 * 1024;
        //                client.UploadFile(fileStream, Path.GetFileName(uploadFile));
        //            }
        //        }
        //        client.Disconnect();
        //    }

        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file selected.");
        //    var uploadsFolder = @"\\10.10.248.2\pde_propertycard";
        //    //@"D:\WWW\test";
        //    //Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        //    if (!Directory.Exists(uploadsFolder))
        //    {
        //        Directory.CreateDirectory(uploadsFolder);
        //    }
        //    var filePath = Path.Combine(uploadsFolder, file.FileName);
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }
        //    return Ok(new { filePath });
        //}

        [HttpPost("upload2")]
        public async Task<IActionResult> UploadFile2(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");
            var uploadsFolder = @"\\10.10.248.2";
            //@"D:\WWW\test";
            //Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { filePath });
        }
    }
}
