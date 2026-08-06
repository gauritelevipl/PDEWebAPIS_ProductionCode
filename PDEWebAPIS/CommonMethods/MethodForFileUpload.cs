using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using PDEWebAPIS.Controllers;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.Services;
using System.IO;
using System.Text.RegularExpressions;

namespace PDEWebAPIS.CommonMethods
{
    public class MethodForFileUpload
    {
        public bool SaveImage(string ImgStr, string ImgName, string ID, string FileTypeFlag, string FolderPath)
        {
            FolderPath = FolderPath + ID + @"\";
            //HttpContext.Current.Server.MapPath("~/ImageStorage"); //Path
            //Check if directory exist
            if (!System.IO.Directory.Exists(FolderPath))
            {
                System.IO.Directory.CreateDirectory(FolderPath); //Create directory if it doesn't exist
            }
            //string imageName = ImgName + ".jpg";
            //set the image path
            if (FileTypeFlag == "PassportPhoto")
            {
                string ext = Path.GetExtension(ImgName);
                ImgName = "PassportPhoto" + ID + ext;
            }
            if (FileTypeFlag == "AddressProof")
            {
                string ext = Path.GetExtension(ImgName);
                ImgName = "AddressProof" + ID + ext;
            }
            if (FileTypeFlag == "Signature")
            {
                string ext = Path.GetExtension(ImgName);
                ImgName = "Signature" + ID + ext;
            }
            string imgPath = Path.Combine(FolderPath, ImgName);
            byte[] imageBytes = Convert.FromBase64String(ImgStr);
            File.WriteAllBytes(imgPath, imageBytes);
            return true;
        }

        public string ConvertImageToBase64(string FilePath)
        {
            try
            {
                string ext = Path.GetExtension(FilePath);
                if (System.IO.File.Exists(FilePath))
                {
                    byte[] imageArray = System.IO.File.ReadAllBytes(FilePath);
                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                    return base64ImageRepresentation;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public bool SaveImageForApplicant(string ImgStr, string ImgName, string ID, string FileTypeFlag, string FolderPath, string CurrentDateTime)
        {
            FolderPath = FolderPath + ID + @"\";
            //Check if directory exist
            if (!System.IO.Directory.Exists(FolderPath))
            {
                System.IO.Directory.CreateDirectory(FolderPath); //Create directory if it doesn't exist
            }
            //set the image path
            if (FileTypeFlag == "PassportPhoto")
            {
                string ext = Path.GetExtension(ImgName);
                ImgName = "PassportPhoto" + ID + "_" + CurrentDateTime + ext;
            }
            if (FileTypeFlag == "AddressProof")
            {
                string ext = Path.GetExtension(ImgName);
                ImgName = "AddressProof" + ID + "_" + CurrentDateTime + ext;
            }
            if (FileTypeFlag == "Signature")
            {
                string ext = Path.GetExtension(ImgName);
                ImgName = "Signature" + ID + "_" + CurrentDateTime + ext;
            }
            string imgPath = Path.Combine(FolderPath, ImgName);
            byte[] imageBytes = Convert.FromBase64String(ImgStr);
            File.WriteAllBytes(imgPath, imageBytes);
            return true;
        }

        public string UploadPDFFile(string FolderPath, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                var completeFilePath = FolderPath + file.FileName;
                if (File.Exists(completeFilePath))
                {
                    return "Same File Name Is Already Exists";
                }
                else
                {
                    using (var stream = new FileStream(completeFilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        stream.Dispose();
                        stream.Close();
                    }
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string ConvertBase64ToPdf(string FolderPath,string FileName,string base64String)
        {
            try
            {
                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                var completeFilePath = FolderPath + FileName;
                if (File.Exists(completeFilePath))
                {
                    return "Same File Name Is Already Exists";
                }
                else
                {
                    byte[] fileBytes = Convert.FromBase64String(base64String);

                    // Write bytes to file
                    File.WriteAllBytes(completeFilePath, fileBytes);
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public bool PermanatlyDeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return true;
            }
            else if (System.IO.Directory.Exists(path))
            {
                if (System.IO.Directory.EnumerateFileSystemEntries(path).Any())
                {
                    System.IO.File.Delete(path);
                    System.IO.Directory.Delete(path);
                }
                else
                {
                    System.IO.Directory.Delete(path);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

       

        //gauriw
        //For Grievance
        public int GetPdfPageCount(string file)
        {
            try
            {
                if (string.IsNullOrEmpty(file))
                {
                    throw new ArgumentException("Invalid PDF data."); ;
                }

                byte[] pdfBytes = Convert.FromBase64String(file);
                // Create a MemoryStream from the byte array
                using (MemoryStream pdfStream = new MemoryStream(pdfBytes))
                using (PdfReader reader = new PdfReader(pdfStream))
                using (PdfDocument pdfDoc = new PdfDocument(reader))
                {
                    return pdfDoc.GetNumberOfPages();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing PDF: {ex.Message}");
            }
        }


        public bool SaveImageForGrievance(string ImgStr, string ImgName, string ID, string FileTypeFlag, string FolderPath)
        {
            try
            {
                FolderPath = FolderPath + ID + @"\";

                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }
                string extension = string.Empty;
                if (FileTypeFlag.Equals("Image"))
                {
                    extension = ".jpeg";
                }

                byte[] imageBytes = Convert.FromBase64String(ImgStr);

                string filePath = Path.Combine(FolderPath, $"{ImgName}{extension}");
                File.WriteAllBytes(filePath, imageBytes);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
                return false;
            }
        }

        public bool DeleteImageForGrievance(string ImgName, string ID, string FileTypeFlag, string FolderPath)
        {
            try
            {

                string filePath = Path.Combine(FolderPath, ImgName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                else
                {
                    Console.WriteLine("File not found.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex.Message}");
                return false;
            }
        }

        public bool ContainsSpecialCharacters(string fileName)//for image
        {
            bool result = Regex.IsMatch(fileName, @"[^a-zA-Z0-9\u0900-\u097F _-]");
            return result;
        }
        public bool ContainsSpecialCharactersInName(string Name)//for english name
        {
            bool result = Regex.IsMatch(Name, @"[^a-zA-Z .]");
            return result;
        }
        public bool ContainsSpecialCharactersInMarathiName(string Name)//for marathi names
        {
            bool result = Regex.IsMatch(Name, @"[^\u0900-\u0903\u0904-\u0939\u093E-\u094C\u094D\u200D\s.]");
            return result;
            //return Regex.IsMatch(Name, @"[^\u0900-\u0903\u0904-\u0939\u093E-\u094C\u094D\s.]");

        }
        public bool CheckMobNo(string mobno)//for mobno
        {
            return !Regex.IsMatch(mobno, @"^[6-9]\d{9}$");
        }
        public bool CheckEmail(string mail)//for mail
        {
            return !Regex.IsMatch(mail, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }
        public bool CheckPinCode(string pincode)//for pincode
        {
            return !Regex.IsMatch(pincode, @"^\d{6}$");

        }
        public bool CheckIndianAddress(string address)//indian address
        {
            return !Regex.IsMatch(address, @"^[a-zA-Z0-9\u0900-\u0903\u0904-\u0939\u093E-\u094C\u094D\u200D\s,]{1,100}$");
        }
        public bool CheckForeignAddress(string address)//foreign address
        {
            return !Regex.IsMatch(address, @"^[a-zA-Z0-9\u0900-\u0903\u0904-\u0939\u093E-\u094C\u094D\u200D\s.,]*$");
        }
        public bool CheckNabhu(string nabhu)//for nabhu
        {
            return !Regex.IsMatch(nabhu, @"^[0-9]*$");
        }
        public bool CheckBuildingName(string buildingName)//for building
        {
            return !Regex.IsMatch(buildingName, @"^[a-zA-Z0-9\u0900-\u0903\u0904-\u0939\u093E-\u094C\u094D\u200D\s,.]{1,300}$");

        }
        public bool checkFloorNo(string floorNo)
        {
            return !Regex.IsMatch(floorNo, @"^\d{1,4}$");
        }
        public bool checkUnitNo(string unitNo)
        {
            return !Regex.IsMatch(unitNo, @"^[0-9.]{1,10}$");
        }
        public bool CheckKArea(string area)
        {
            return !Regex.IsMatch(area, @"^[0-9.]{1,10}$");
        }
        public bool CheckBojaAmt(string amt)
        {
            return !Regex.IsMatch(amt, @"^[0-9.]{1,11}$");
        }
        public bool CheckMobadlaAmt(string amt)
        {
            return !Regex.IsMatch(amt, @"^[0-9]{1,10}$");
        }
        public bool CheckCourtDavaTapshil(string str)
        {
            return !Regex.IsMatch(str, @"^[a-zA-Z0-9\u0900-\u0903\u0904-\u0939\u093E-\u094C\u094D\u200D\s,.]*$");
        }
        public bool CheckDastNo(string dastNo)
        {
            return !Regex.IsMatch(dastNo, @"^[0-9]{1,10}$");
        }
        public bool CheckYrLen(string yr)
        {
            return !Regex.IsMatch(yr, @"^[0-9]{4}$");
        }
       
        public bool ContainsSpecialCharactersInMrutyuDakhlaNo(string Name)
        {
            bool result = Regex.IsMatch(Name, @"[^^a-zA-Z0-9\u0900-\u0903\u0904-\u0939\u093E-\u094C\u094D\u200D ]");
            return result;
        }
        public bool ContainsSpecialCharactersInCourtCase(string fileName)
        {
            bool result = Regex.IsMatch(fileName, @"[^a-zA-Z0-9\u0900-\u097F ,.]");
            return result;
        }
        public string checkMarathiEngNameValidations()
        {
            return "true";
        }
    }
}
