using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.Data;
using PDEWebAPIS.ViewModel;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using NuGet.Packaging.Signing;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.InputDataModel.MrutyuPatra;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;
using System;
using System.Data;
using System.Reflection.Metadata;
using static PDEWebAPIS.InputDataModel.INameWithCodeFormatData;
using static PDEWebAPIS.InputDataModel.IUserData;
using static System.Net.Mime.MediaTypeNames;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Security.Cryptography.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;
using iText.Kernel.Geom;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
//using OfficeOpenXml;
using ClosedXML.Excel;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using DocumentFormat.OpenXml.ExtendedProperties;


namespace PDEWebAPIS.Services
{
    public class GrievanceService
    {

        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private AppDBContext _context;
        private AppDBContextR _contextR;
        private CommonFunctions commonFunctions = new CommonFunctions();

        public GrievanceService(AppDBContext context, AppDBContextR contextR)
        {
            this._context = context;
            this._contextR = contextR;
        }

        //Get
        public List<FetchGrievanceIssueReason> FetchGrievanceIssueReason()
        {
            List<FetchGrievanceIssueReason> grievanceIssueList = new List<FetchGrievanceIssueReason>();
            var GrievanceIssueList = _contextR.grievanceReasonMaster.ToList();
            if (GrievanceIssueList.Count > 0)
            {
                GrievanceIssueList.ForEach(row => grievanceIssueList.Add(new FetchGrievanceIssueReason()
                {

                    reasonName = row.reasonName,
                    reasonCode = row.reasonCode,

                }));
            }
            return grievanceIssueList;
        }

        public List<FetchGrievanceStatus> FetchGrievanceStatus()
        {
            List<FetchGrievanceStatus> grievanceList = new List<FetchGrievanceStatus>();
            var GrievanceStatusList = _contextR.grievanceStatusMaster.ToList();
            if (GrievanceStatusList.Count > 0)
            {
                GrievanceStatusList.ForEach(row => grievanceList.Add(new FetchGrievanceStatus()
                {

                    status = row.status,
                    statusCode = row.statusCode,

                }));
            }
            return grievanceList;
        }

        public List<FetchMutationTypes> FetchMutationDataByApplicationID(string applicationID)
        {
            List<FetchMutationTypes> mutationDataList = new List<FetchMutationTypes>();
            var MutationDtaList = _contextR.applicationDTL.Where(data => data.applicationid!.Equals(applicationID)).ToList();
            if (MutationDtaList.Count > 0)
            {
                MutationDtaList.ForEach(row => mutationDataList.Add(new FetchMutationTypes()
                {
                    mutationTypeName = row.mutation_type_name,
                    mutationTypeCode = row.mutation_type_code,
                }));

            }
            return mutationDataList;
        }

        //public FetchGrievanceDashboardData FetchDashboardData(string applicationid)
        //{
        //    try
        //    {
        //        GrievanceTbl grievanceTbl = _context.grievanceData.FirstOrDefault(s => s.applicationId == applicationid)!;
        //        FetchGrievanceDashboardData fetchData = new FetchGrievanceDashboardData();


        //        if (grievanceTbl != null)
        //        {
        //            FetchGrievanceStatus grievanceStatuses = new FetchGrievanceStatus();
        //            GrievanceStatusMaster statusMaster = new GrievanceStatusMaster();

        //            int statusCode = _context.grievanceStatusMaster
        //                  .Where(g => g.status == grievanceTbl.grivanceStatus)
        //                  .Select(g => g.statusCode) 
        //                  .FirstOrDefault();
        //            status s = new status();
        //            s.grievanceStatus = grievanceTbl.grivanceStatus;
        //            s.grievanceCode = statusCode;
        //            fetchData.applicationId = grievanceTbl.applicationId;
        //            fetchData.tickitId = grievanceTbl.tickitId;
        //            fetchData.issueReportDate = grievanceTbl.issueReportDate;
        //            fetchData.issueCategory = grievanceTbl.issueCategory;
        //            fetchData.mutationName = grievanceTbl.mutationName;
        //            fetchData.AssignIssueTo = grievanceTbl.AssignIssueTo;
        //            fetchData.priority = grievanceTbl.priority; 
        //        }
        //        else
        //        {
        //            fetchData = null!;
        //        }
        //        return fetchData;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}


        //fetch tickit ids
        public List<string> FetchTickitIds(int userid)
        {

            try
            {
                List<string> result = _contextR.grievanceData.Where(u => u.userId == userid && u.IsDeleted!.Equals("N")).OrderByDescending(s => s.issueReportDate)
                             .Select(s => s.tickitId).ToList()!;
                return result;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }
        public FetchGrievanceDashboardData FetchDashboardData(string tickitid)
        {
            try
            {
                GrievanceTbl grievanceTbl = _contextR.grievanceData.OrderByDescending(s => s.issueReportDate).FirstOrDefault(s => s.tickitId == tickitid && s.IsDeleted == "N")!;
                List<GrievanveIssueTransactionDtl> grievanceissueTransactionDtl = _contextR.grievanveIssueTransactionDtls.Where(x => x.tickitId == tickitid).ToList();
                FetchGrievanceDashboardData fetchData = new FetchGrievanceDashboardData();
                MethodForFileUpload methodForFileUpload = new MethodForFileUpload();
                GrievanceIssueDatesDetails grievanceIssueDatesDetails = _contextR.grievanceIssueDatesDetails.Where(x => x.tickitid == tickitid).FirstOrDefault()!;

                GrievanceReplyData data = new GrievanceReplyData();
                if (grievanceTbl != null)
                {
                    GrievanceStatusMaster statusMaster = new GrievanceStatusMaster();
                    int statusCode = _contextR.grievanceStatusMaster.Where(s => s.status!.ToLower() == grievanceTbl.grivanceStatus!.ToLower()).Select
                        (x => x.statusCode).FirstOrDefault();
                    fetchData.applicationId = grievanceTbl.applicationId;
                    fetchData.tickitId = grievanceTbl.tickitId;
                    fetchData.issueReportDate = grievanceTbl.issueReportDate;
                    fetchData.issueCategory = grievanceTbl.issueCategory;
                    fetchData.mutationName = grievanceTbl.mutationName;
                    fetchData.district_code = grievanceTbl.district_code;
                    fetchData.taluka_code = grievanceTbl.taluka_code;
                    fetchData.district_name_in_marathi = grievanceTbl.district_name_in_marathi;
                    fetchData.taluka_name = grievanceTbl.taluka_name;
                    if (grievanceTbl.ReAssignIssueTo == 0)
                    {
                        fetchData.AssignIssueTo = grievanceTbl.AssignIssueTo;
                    }
                    else
                    {
                        string ReassignIssueTo = _contextR.grievanceUserMaster.Where(x => x.guserid == grievanceTbl.ReAssignIssueTo).Select(x => x.division).FirstOrDefault()!.ToString()!;
                        fetchData.AssignIssueTo = ReassignIssueTo;
                    }


                    fetchData.priority = grievanceTbl.priority;//dropdown
                    fetchData.grivanceStatus = grievanceTbl.grivanceStatus;//dropdown/edit\
                    fetchData.grivanceStatusCode = statusCode;
                    fetchData.mobileno = grievanceTbl.mobileno;
                    fetchData.secondaryMoNo = grievanceTbl.secondaryMoNo;
                    var entity = _contextR.grievanceIssueDatesDetails.Where(x => x.tickitid == tickitid && x.status!.ToLower()=="seen").OrderByDescending(x => x.Datetime).Take(1).FirstOrDefault();
                    if (entity != null)
                    { 
                        if (entity!.status!.ToLower() == "seen")
                        {
                            fetchData.SeenStatus = "Seen";
                            fetchData.seenby = _contextR.grievanceUserMaster.Where(x => x.guserid.ToString() == entity.IssueSeenByUserId).Select(s => s.division).FirstOrDefault()!.ToString();
                        }
                        else
                        {
                            fetchData.SeenStatus = "NA";
                            fetchData.seenby = "NA";
                        }
                    }
                    
                    
                    //image
                    string fileExt = System.IO.Path.GetExtension(grievanceTbl.docPath!);
                    string encryptedDocPath = methodForFileUpload.ConvertImageToBase64(grievanceTbl.docPath!);
                    fetchData.docpath = string.IsNullOrEmpty(encryptedDocPath) ? "NA" : "data:image/" + fileExt.Replace(".", "") + ";base64," + encryptedDocPath;
                    fetchData.docname = grievanceTbl.docTitle;
                    fetchData.issueDescription = grievanceTbl.issueDesc;

                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        //public int FetchUserID(string username, string pass)
        //{
        //    try
        //    {
        //        int FetchedUserID = 0;
        //        var row = _contextR.grievanceUserMaster.Where(data => data.username! == username &&
        //        data.password! == Security.EnCryptData(pass)).FirstOrDefault();
        //        if (row != null)
        //        {
        //            FetchedUserID = row.guserid;
        //        }
        //        else
        //        {
        //            FetchedUserID = 0;
        //        }

        //        return FetchedUserID;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}

        public int FetchUserID(string username, string pass, int loginType, string districtCode, string regionCode)
        {
            try
            {

                int FetchedUserID = 0;
                if (loginType == 2)
                {
                    var row = _contextR.grievanceUserMaster.Where(data => data.district_code! == districtCode && data.region_code! == regionCode &&
                   data.password! == Security.EnCryptData(pass)).FirstOrDefault();
                    if (row != null)
                    {
                        FetchedUserID = row.guserid;
                    }
                    else
                    {
                        FetchedUserID = 0;
                    }
                }
                else
                {
                    var row = _contextR.grievanceUserMaster.Where(data => data.username! == username &&
                    data.password! == Security.EnCryptData(pass)).FirstOrDefault();
                    if (row != null)
                    {
                        FetchedUserID = row.guserid;
                    }
                    else
                    {
                        FetchedUserID = 0;
                    }
                }


                return FetchedUserID;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public string UpdateToken(string Token, int GuserId, string APIFlag)
        {
            try
            {
                if (APIFlag.Trim().ToUpper() == "WEB")
                {
                    _context.grievanceUserMaster.Where(t => t.guserid == GuserId).ExecuteUpdate(mt => mt.SetProperty(e => e.webtoken, e => Token));
                }
                if (APIFlag.Trim().ToUpper() == "MOBILE")
                {
                    _context.userMasters.Where(t => t.userid == GuserId).ExecuteUpdate(mt => mt.SetProperty(e => e.mobileno, e => Token));
                }
                _context.SaveChanges();
                return "Success";
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        //public List<string> FetchApplicationIDS(int userid)
        //{

        //    try
        //    {

        //        List<string> result = _context.grievanceData.Where(u => u.userId.Equals(userid) && u.IsDeleted.Equals("N")).Select(s => s.tickitId).ToList();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}

        //public List<string> FetchApplicationIDS(int userid)
        //{

        //    try
        //    {
        //        List<string> applicationId = _context.applicationDTL.Where(x => x.userMaster!.userid == userid && x.isDeleted == false).
        //            OrderByDescending(s => s.createddatetime).Select(u => u.applicationid).ToList()!;
        //        List<string> grievanceApplicationID = _context.grievanceData.Where(u => u.userId.Equals(userid) && u.IsDeleted!.Equals("N")).Select(a => a.applicationId).ToList()!;
        //        MethodForFileUpload methodForFileUpload = new MethodForFileUpload();


        //        for (int i = 0; i < applicationId.Count; i++)
        //        {
        //            for (int j = 0; j < grievanceApplicationID.Count; j++)
        //            {
        //                if (applicationId[i] == grievanceApplicationID[j])
        //                {
        //                    var entity = _context.grievanceData.FirstOrDefault(s => s.applicationId == grievanceApplicationID[j] && s.IsDeleted == "N")!;
        //                    entity.IsDeleted = "Y";
        //                    methodForFileUpload.PermanatlyDeleteFile(entity.docPath!);
        //                    var path = @"D:\WWW\GRIEVANCEDOCS\" + entity.tickitId;
        //                    bool isDeleted = methodForFileUpload.PermanatlyDeleteFile(path);
        //                    _context.grievanceData.Attach(entity);
        //                    _context.SaveChanges();
        //                }
        //            }

        //        }

        //        List<string> result = _context.grievanceData.Where(u => u.userId.Equals(userid) && u.IsDeleted!.Equals("N")).Select(s => s.tickitId).ToList()!;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}





        //fetch ids from grievancetbl for admins and other team
        public int FetchUserIDThroughToken(string Token, string Flag)
        {
            try
            {
                GrievanceUserMaster grievanceUserMaster = new GrievanceUserMaster();
                int FetchedGUserID = 0;

                if (Flag.Trim().ToUpper() == "WEB")
                {
                    grievanceUserMaster = _contextR.grievanceUserMaster.Where(data => data.webtoken.Equals(Token)).FirstOrDefault()!;
                }
                if (Flag.Trim().ToUpper() == "MOBILE")
                {
                    grievanceUserMaster = _contextR.grievanceUserMaster.Where(data => data.moiletoken.Equals(Token)).FirstOrDefault()!;
                }
                if (grievanceUserMaster != null)
                {
                    FetchedGUserID = grievanceUserMaster.guserid;
                }
                else
                {
                    throw new HandleException("User Not Found");
                }
                return FetchedGUserID;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }
        public int FetchUserIDThroughTokenForLogout(string Token, string Flag)
        {
            try
            {
                GrievanceUserMaster grievanceUserMaster = new GrievanceUserMaster();
                int FetchedGUserID = 0;

                if (Flag.Trim().ToUpper() == "WEB")
                {
                    grievanceUserMaster = _contextR.grievanceUserMaster.Where(data => data.webtoken.Equals(Token)).FirstOrDefault()!;
                }
                if (Flag.Trim().ToUpper() == "MOBILE")
                {
                    grievanceUserMaster = _contextR.grievanceUserMaster.Where(data => data.moiletoken.Equals(Token)).FirstOrDefault()!;
                }
                if (grievanceUserMaster != null)
                {
                    FetchedGUserID = grievanceUserMaster.guserid;
                }
                //else
                //{
                //    throw new HandleException("User Not Found");
                //}
                return FetchedGUserID;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public List<string> FetchTickitIDSForDeptUsers(int guserid)
        {
            try
            {
                //for admin
                string usertype = _contextR.grievanceUserMaster.Where(u => u.guserid == guserid).Select(x => x.usertype).FirstOrDefault()!;
                if (usertype != null)
                {
                    if (usertype.ToLower() == "admin")
                    {
                        List<string> result = _contextR.grievanceData.Where(u => u.IsDeleted!.Equals("N")).OrderByDescending(s => s.issueReportDate).
                            Select(s => s.tickitId).ToList()!;
                        return result;
                    }
                    //for other dept users
                    else
                    {
                        List<string> result = _contextR.grievanceData.Where(u => u.AssignIssueToUserId == guserid || u.ReAssignIssueTo == guserid && u.IsDeleted!.Equals("N")).OrderByDescending(s => s.IssueAssignDateTime)
                            .Select(s => s.tickitId).ToList()!;
                        return result;
                    }
                }
                else
                {
                    throw new HandleException("User Not Found!!".ToString());
                }

            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public List<FetchApplicationDTL> FetchMutationAndApplicationIdDataUserid(int userId)
        {
            //List<FetchMutationTypes> mutationDataList = new List<FetchMutationTypes>();
            List<FetchApplicationDTL> fetchApplicationDTL = new List<FetchApplicationDTL>();
            var MutationDtaList = _contextR.applicationDTL.Where(data => data.userMaster!.userid.Equals(userId) && data.isDeleted == false).OrderByDescending(s => s.createddatetime).ToList();
            if (MutationDtaList.Count > 0)
            {
                MutationDtaList.ForEach(row => fetchApplicationDTL.Add(new FetchApplicationDTL()
                {
                    applicationId = row.applicationid,
                    mutation_type = row.mutation_type_name,
                    mutation_type_code = row.mutation_type_code,
                    district_code = row.district_code,
                    district_name_in_marathi = row.district_name_in_marathi,
                    taluka_code = row.office_code,
                    taluka_name = row.office_name,
                }));
            }
            return fetchApplicationDTL;
        }

        //tickit id generate
        public string GenerateTickitID()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
            IConfiguration configuration = builder.Build();
            string ConnectionString = configuration.GetValue<string>("ConnectionStrings:PDEDBW")!;
            try
            {

                string GeneratedtickitId = string.Empty;
                string tableTickitID = string.Empty;

                NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
                connection.Open();
                NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
                npgsqlCommand.Connection = connection;
                npgsqlCommand.Parameters.Clear();
                npgsqlCommand.CommandType = CommandType.Text;
                npgsqlCommand.CommandText = "CALL GenerateAutoTickitId(NULL,NULL);\r\n";

                using (var reader = npgsqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GeneratedtickitId = reader.GetString(0);
                        tableTickitID = reader.GetString(1);
                    }
                }
                if (tableTickitID.Equals("0"))
                {
                    string num = "000001";
                    GeneratedtickitId = GeneratedtickitId + num;
                }
                else
                {
                    int Num = Convert.ToInt32(tableTickitID.Substring(tableTickitID.Length - 6));
                    Num++;
                    GeneratedtickitId = GeneratedtickitId + Num.ToString($"D{tableTickitID.Substring(tableTickitID.Length - 6).Length}");
                }
                npgsqlCommand.Dispose();
                connection.Close();
                return GeneratedtickitId!;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        //another method same as application id 
        //public string GenerateTickitID()
        //{
        //    string GeneratedtickitId = string.Empty;
        //    string tableTickitID = string.Empty;

        //    try
        //    {
        //        using var command = _context.Database.GetDbConnection().CreateCommand();
        //        command.CommandText = "CALL GenerateAutoTickitId(NULL, NULL)";
        //        command.CommandType = CommandType.Text;

        //        _context.Database.OpenConnection();

        //        using var reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            GeneratedtickitId = reader.GetString(0);
        //            tableTickitID = reader.GetString(1);
        //        }

        //        if (tableTickitID.Equals("0"))
        //        {
        //            string num = "000001";
        //            GeneratedtickitId = GeneratedtickitId + num;
        //        }
        //        else
        //        {
        //            int Num = Convert.ToInt32(tableTickitID[^6..]); // last 6 digits
        //            Num++;
        //            GeneratedtickitId += Num.ToString($"D{tableTickitID[^6..].Length}");
        //        }

        //        _context.Database.CloseConnection();
        //        return GeneratedtickitId;
        //    }
        //    catch (Exception ex)
        //    {
        //        _context.Database.CloseConnection();
        //        throw new HandleException(ex.Message);
        //    }
        //}




        //Save
        public string SaveGrievanceSystemIssues(GrievanceInputModel grievanceInputModel)//input model
        {

            using (var scope = new TransactionScope())
            {
                MethodForFileUpload methodForFile = new MethodForFileUpload();
                GrievanceTbl grievancetable = new GrievanceTbl();//table
                GrievanceModel grievanceModel = new GrievanceModel();//model
                GrievanceIssueDatesDetails grievanceDatesTbl = new GrievanceIssueDatesDetails();
                string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
                DateTime createdDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                string FolderPath = @"\\10.10.248.2\pde_propertycard\GRIEVANCEDOCS\"; 
                //string FolderPath = @"D:\www\GRIEVANCEDOCS\";
                bool checkImageSaveFlag = true;
                if (grievanceInputModel != null)
                {
                    try
                    {
                        //
                        string imageName = System.IO.Path.GetFileNameWithoutExtension(grievanceInputModel.ImageName)!;
                        if (methodForFile.ContainsSpecialCharacters(imageName))
                        {
                            return "Image name only contains (letters, _, -, numbers).";
                        }

                        //int gid = _context.grievanceData.Max(x=>x.Gid);
                        //assign input values to model
                        grievanceModel.userId = grievanceInputModel.userId;
                        grievanceModel.applicationId = grievanceInputModel.applicationId;
                        grievanceModel.mutationName = grievanceInputModel.mutationName;
                        grievanceModel.mutationCode = _context.applicationDTL
                        .Where(x => x.mutation_type_name == grievanceModel.mutationName).Select(x => x.mutation_type_code).AsEnumerable()
                        .Select(value => int.TryParse(value, out int number) ? number : 0)
                        .FirstOrDefault();
                        grievanceModel.district_code = grievanceInputModel.district_code;
                        grievanceModel.taluka_code = grievanceInputModel.taluka_code;
                        grievanceModel.district_name_in_marathi = grievanceInputModel.district_name_in_marathi;
                        grievanceModel.taluka_name = grievanceInputModel.taluka_name;
                        grievanceModel.issueCategory = grievanceInputModel.issueCategory;
                        grievanceModel.grivanceStatus = "Open";
                        grievanceModel.tickitId = GenerateTickitID();
                        grievanceModel.issueDesc = grievanceInputModel.issueDesc;
                        grievanceModel.ImageName = System.IO.Path.GetFileNameWithoutExtension(grievanceInputModel.ImageName) + "-" + grievanceModel.tickitId + "-" + CurrentDateTime + ".jpeg";
                        grievanceModel.docPath = FolderPath + grievanceModel.tickitId + @"\" + grievanceModel.ImageName;
                        grievanceModel.AssignIssueTo = "NA";
                        grievanceModel.priority = "NA";
                        grievanceModel.IssueClosedBy = "NA";
                        grievanceModel.secondaryMoNo = grievanceInputModel.secondaryMoNo;
                        grievanceModel.mobileno = _context.userMasters.Where(x => x.userid == grievanceInputModel.userId).Select(x => x.mobileno).FirstOrDefault();


                        //assign values from model to table
                        if (grievanceInputModel.issueCategory!.ToLower() == "other")
                        {
                            grievancetable.applicationId = "NA";
                            grievancetable.mutationName = "NA";
                            grievancetable.mutationCode = 0;
                            grievancetable.district_code = grievanceModel.district_code;
                            grievancetable.taluka_code = grievanceModel.taluka_code;
                            grievancetable.district_name_in_marathi = grievanceModel.district_name_in_marathi;
                            grievancetable.taluka_name = grievanceModel.taluka_name;

                        }
                        else if (grievanceInputModel.issueCategory.ToLower() == "applicationrelated")
                        {
                            grievancetable.applicationId = grievanceModel.applicationId;
                            grievancetable.mutationName = grievanceModel.mutationName;
                            grievancetable.mutationCode = grievanceModel.mutationCode;
                            grievancetable.district_code = grievanceModel.district_code;
                            grievancetable.taluka_code = grievanceModel.taluka_code;
                            grievancetable.district_name_in_marathi = grievanceModel.district_name_in_marathi;
                            grievancetable.taluka_name = grievanceModel.taluka_name;

                        }

                        grievancetable.userId = grievanceModel.userId;
                        grievancetable.issueCategory = grievanceModel.issueCategory;
                        grievancetable.grivanceStatus = grievanceModel.grivanceStatus;
                        grievancetable.issueReportDate = grievanceModel.issueReportDate;
                        grievancetable.issueDesc = grievanceModel.issueDesc;
                        grievancetable.docPath = grievanceModel.docPath;
                        grievancetable.AssignIssueTo = grievanceModel.AssignIssueTo;
                        grievancetable.priority = grievanceModel.priority;
                        grievancetable.IssueClosedBy = grievanceModel.IssueClosedBy;
                        grievancetable.tickitId = grievanceModel.tickitId;
                        grievancetable.docTitle = grievanceModel.ImageName;
                        grievancetable.docPath = grievanceModel.docPath;
                        grievancetable.IsDeleted = "N";
                        grievancetable.secondaryMoNo = grievanceModel.secondaryMoNo;
                        grievancetable.mobileno = grievanceModel.mobileno;

                        //insert into date details table
                        grievanceDatesTbl.applicationId = grievanceModel.applicationId;
                        grievanceDatesTbl.tickitid = grievanceModel.tickitId;
                        grievanceDatesTbl.status = grievanceModel.grivanceStatus;
                        grievanceDatesTbl.StatusDatetime = createdDateTime;
                        grievanceDatesTbl.PerformByUserId = grievanceModel.userId.ToString();
                        grievanceDatesTbl.IssueAssignToUserId = "NA";
                        grievanceDatesTbl.IssueReassignToUserId = "NA";
                        grievanceDatesTbl.IssueSeenByUserId = "NA";
                        grievanceDatesTbl.IssueResolvedByUserid = "NA";


                        if (!string.IsNullOrEmpty(grievanceInputModel.imagesrc) && grievanceInputModel.imagesrc != "NA")
                        {
                            string imgSrc = grievanceInputModel.imagesrc.Substring(grievanceInputModel.imagesrc.IndexOf(",") + 1);
                            checkImageSaveFlag = methodForFile.SaveImageForGrievance(imgSrc, System.IO.Path.GetFileNameWithoutExtension(grievanceModel.ImageName), grievanceModel.tickitId, "Image", FolderPath);
                        }
                        if (checkImageSaveFlag)
                        {
                            _context.grievanceIssueDatesDetails.Add(grievanceDatesTbl);
                            _context.grievanceData.Add(grievancetable);
                            _context.SaveChanges();
                            scope.Complete();
                            return "Success";
                        }
                        else
                        {
                            return "Error while uploading image!";
                        }
                    }
                    catch (Exception e)
                    {
                        methodForFile.DeleteImageForGrievance(grievanceModel.ImageName!, grievanceModel.tickitId!, "Image", FolderPath);
                        return e.Message.ToString();
                    }
                }
                else
                {
                    return "Please fill atleast one Feild!";
                }
            }
        }

        //Delete
        public string DeleteGrievaceApplication(string tickitId)
        //,GrievanceInputModel grievanceInputModel)
        {
            MethodForFileUpload methodForFile = new MethodForFileUpload();
            GrievanceTbl grievancetable = new GrievanceTbl();

            try
            {
                var entity = _context.grievanceData.FirstOrDefault(s => s.tickitId == tickitId && s.IsDeleted == "N")!;
                if (entity != null)
                {
                    entity.IsDeleted = "Y";
                    _context.grievanceData.Attach(entity);
                    _context.SaveChanges();
                    return "Success";
                }
                else
                {
                    return "Data Not Found";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        //Save Grievance Dept user 
        public string SaveGrievanceUserData(GrievanceUserMasterInputModel grievanceUserMasterInputModel)
        {
            //using (var scope = new TransactionScope())
            //{
            try
            {
                GrievanceUserMasterModel grievanceUserMasterModel = new GrievanceUserMasterModel();
                GrievanceUserMaster grievanceUserMastertbl = new GrievanceUserMaster();

                GrievanceUserMaster grievanceUserMaster = _context.grievanceUserMaster.Where(d => d.username!.ToLower().Equals(grievanceUserMasterInputModel.username!.ToLower()) && d.emailid!.Equals(grievanceUserMasterInputModel.emailid)
                && d.mobileno!.Equals(grievanceUserMasterInputModel.mobileno)).FirstOrDefault()!;

                //bool res = _context.grievanceUserMaster.Any(x => x.usertype!.ToLower() == grievanceUserMasterInputModel.usertype!.ToLower());
                //if (res)
                //{
                //    return "Only One admin login is allowed!!";
                //}
                //else
                //{
                if (grievanceUserMaster != null)
                {
                    return "User Already Registered!";
                }
                else
                {
                    //assign ip model to model
                    grievanceUserMasterModel.usertype = grievanceUserMasterInputModel.usertype;
                    grievanceUserMasterModel.fullname = grievanceUserMasterInputModel.fullname;
                    grievanceUserMasterModel.username = grievanceUserMasterInputModel.username;
                    grievanceUserMasterModel.division = grievanceUserMasterInputModel.division;
                    grievanceUserMasterModel.mobileno = grievanceUserMasterInputModel.mobileno;
                    grievanceUserMasterModel.emailid = grievanceUserMasterInputModel.emailid;
                    grievanceUserMasterModel.password = grievanceUserMasterInputModel.password;

                    //assign values to table
                    grievanceUserMastertbl.usertype = grievanceUserMasterModel.usertype;
                    grievanceUserMastertbl.fullname = grievanceUserMasterModel.fullname;
                    grievanceUserMastertbl.username = grievanceUserMasterModel.username;
                    grievanceUserMastertbl.division = grievanceUserMasterModel.division;
                    grievanceUserMastertbl.mobileno = grievanceUserMasterModel.mobileno;
                    grievanceUserMastertbl.emailid = grievanceUserMasterModel.emailid;
                    grievanceUserMastertbl.password = Security.EnCryptData(grievanceUserMasterModel.password!);
                    grievanceUserMastertbl.webtoken = "NA";
                    grievanceUserMastertbl.moiletoken = "NA";

                    _context.grievanceUserMaster.Add(grievanceUserMastertbl);
                    _context.SaveChanges();
                    return "Success";
                    //}
                }

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            //}
        }

        //login
        public string CheckLogin(GrievanceLoginInputModel grievanceLoginInputModel)
        {
            try
            {
                bool flag = _context.grievanceUserMaster.Any(x => (x.username! == grievanceLoginInputModel.username!)
                || (x.district_code == grievanceLoginInputModel.district_code && x.region_code == grievanceLoginInputModel.region_code)); //mobno);

                if (flag)
                {
                    //admin login
                    if (grievanceLoginInputModel.loginType == 1)
                    {
                        var dbValue = _context.grievanceUserMaster.Where(x => x.username! == grievanceLoginInputModel.username!
                        && x.password! == Security.EnCryptData(grievanceLoginInputModel.password!)).FirstOrDefault();
                        if (dbValue != null)
                        {
                            if (dbValue.usertype!.ToLower() == "slr" || dbValue.usertype!.ToLower() == "dyslr")
                            {
                                return "Your login is not admin login. Please login from officer's login";
                            }
                            else
                                return "Success";

                        }
                        else
                        {
                            return "Username or Password doesn't match!";
                        }
                    }
                    //officer's login
                    else
                    {
                        var dbValue = _context.grievanceUserMaster.Where(x => x.district_code! == grievanceLoginInputModel.district_code!
                        && x.region_code! == grievanceLoginInputModel.region_code! && x.password! == Security.EnCryptData(grievanceLoginInputModel.password!)).FirstOrDefault();
                        if (dbValue != null)
                        {
                            return "Success";
                        }
                        else
                        {
                            return "Username or Password doesn't match!";
                        }
                    }

                }
                else
                {
                    return "User not found!";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        //assign issue to
        //public string EditAssignIssueTo(GrievanceEditAssignTo grievanceEditAssignTo, int guserid)
        //{
        //    using (var scope = new TransactionScope())
        //    {
        //        try
        //        {
        //            GrievanceIssueDatesDetails grievanceIssueDatesDetails = new GrievanceIssueDatesDetails();
        //            DateTime DefaultDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //            DateTime createdDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        //            var entity = _context.grievanceData.FirstOrDefault(s => s.tickitId == grievanceEditAssignTo.tickitId && s.IsDeleted!.Equals("N"))!;
        //            var entityTwo = _context.grievanceIssueDatesDetails.Where(s => s.tickitid == grievanceEditAssignTo.tickitId).OrderByDescending(s=>s.Datetime).Take(1).FirstOrDefault()!;

        //            if (entity != null)
        //            {
        //                string usertype = _context.grievanceUserMaster.Where(s => s.guserid == guserid).Select(x => x.usertype).FirstOrDefault()!;
        //                if (usertype != null)
        //                {
        //                    if (usertype.ToLower() == "admin")
        //                    {
        //                        if(entityTwo.status!.ToLower() == "open")
        //                        {
        //                            entity.AssignIssueTo = _context.grievanceUserMaster.Where(x => x.guserid == grievanceEditAssignTo.AssignIssueToUserId)
        //                            .Select(u => u.division).FirstOrDefault()!;
        //                            entity.AssignIssueToUserId = grievanceEditAssignTo.AssignIssueToUserId;
        //                            entity.priority = grievanceEditAssignTo.priority;
        //                            entity.grivanceStatus = "Assigned";
        //                            if (entity.IssueAssignDateTime == DefaultDateTime)
        //                            {
        //                                entity.IssueAssignDateTime = createdDateTime;
        //                            }

        //                            //insert
        //                            grievanceIssueDatesDetails.tickitid = entity.tickitId;
        //                            grievanceIssueDatesDetails.applicationId = entity.applicationId;
        //                            grievanceIssueDatesDetails.status = "Assigned";
        //                            grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
        //                            grievanceIssueDatesDetails.StatusDatetime = createdDateTime;
        //                            grievanceIssueDatesDetails.IssueAssignToUserId = grievanceEditAssignTo.AssignIssueToUserId.ToString();
        //                            grievanceIssueDatesDetails.IssueReassignToUserId = "NA";
        //                            grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
        //                            grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
        //                            _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

        //                        }
        //                        else if(entityTwo.status!.ToLower() == "assigned")
        //                        {
        //                            grievanceIssueDatesDetails.tickitid = entity.tickitId;
        //                            grievanceIssueDatesDetails.applicationId = entity.applicationId;
        //                            grievanceIssueDatesDetails.status = "Reassign";
        //                            grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
        //                            grievanceIssueDatesDetails.IssueReassignToUserId = grievanceEditAssignTo.AssignIssueToUserId.ToString();
        //                            grievanceIssueDatesDetails.IssueAssignToUserId = "NA";
        //                            grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
        //                            grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
        //                            grievanceIssueDatesDetails.StatusDatetime = createdDateTime;
        //                            _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

        //                            entity.ReAssignIssueTo = Convert.ToInt32(grievanceEditAssignTo.AssignIssueToUserId);
        //                            entity.IssueReassignedDateTime = createdDateTime;
        //                            entity.grivanceStatus = "Reassigned";

        //                        }
        //                        else
        //                        {
        //                            if(entityTwo.status!.ToLower() == "resolved")
        //                            {
        //                                string user = _context.grievanceUserMaster.Where(x => x.guserid == Convert.ToInt32(entityTwo.IssueResolvedByUserid)).FirstOrDefault()!.ToString()!;
        //                                return "Issue resolved By user " + user;
        //                            }
        //                            else
        //                            {
        //                                grievanceIssueDatesDetails.tickitid = entity.tickitId;
        //                                grievanceIssueDatesDetails.applicationId = entity.applicationId;
        //                                grievanceIssueDatesDetails.status = "Reassign";
        //                                grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
        //                                grievanceIssueDatesDetails.IssueReassignToUserId = grievanceEditAssignTo.AssignIssueToUserId.ToString();
        //                                grievanceIssueDatesDetails.IssueAssignToUserId = "NA";
        //                                grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
        //                                grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
        //                                grievanceIssueDatesDetails.StatusDatetime = createdDateTime;
        //                                _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

        //                                entity.ReAssignIssueTo = Convert.ToInt32(grievanceEditAssignTo.AssignIssueToUserId);
        //                                entity.IssueReassignedDateTime = createdDateTime;
        //                                entity.grivanceStatus = "Reassigned";
        //                            }

        //                        }
        //                    }
        //                }
        //            }
        //            _context.grievanceData.Attach(entity!);
        //            _context.SaveChanges();
        //            scope.Complete();
        //            return "Success";
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new HandleException(ex.Message.ToString());
        //        }
        //    }
        //}

        public string EditAssignIssueTo(GrievanceTakrarPurtataInputModel grievanceTakrarPurtataInputModel, int guserid)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    GrievanceIssueDatesDetails grievanceIssueDatesDetails = new GrievanceIssueDatesDetails();
                    DateTime DefaultDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime createdDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                    var entity = _context.grievanceData.FirstOrDefault(s => s.tickitId == grievanceTakrarPurtataInputModel.tickitId && s.IsDeleted!.Equals("N"))!;
                    var entityTwo = _context.grievanceIssueDatesDetails.Where(s => s.tickitid == grievanceTakrarPurtataInputModel.tickitId).OrderByDescending(s => s.Datetime).Take(1).FirstOrDefault()!;

                    bool IsIssueResolvedorClosed = _context.grievanceIssueDatesDetails.Any(x => x.tickitid == grievanceTakrarPurtataInputModel.tickitId &&
                    x.status!.ToLower() == "resolved" || x.status!.ToLower() == "closed");

                    //self case
                    var assignUsercheck = _context.grievanceUserMaster.Where(x => x.guserid == grievanceTakrarPurtataInputModel.AssignIssueToUserId).FirstOrDefault();
                    if (entity != null)
                    {
                        if (assignUsercheck != null && assignUsercheck.usertype!.ToLower() == "admin")
                        {
                            entity.AssignIssueTo = _context.grievanceUserMaster.Where(x => x.guserid == grievanceTakrarPurtataInputModel.AssignIssueToUserId)
                                        .Select(u => u.division).FirstOrDefault()!;
                            entity.AssignIssueToUserId = grievanceTakrarPurtataInputModel.AssignIssueToUserId;
                            entity.priority = grievanceTakrarPurtataInputModel.priority;
                            entity.grivanceStatus = "Assigned";
                            if (entity.IssueAssignDateTime == DefaultDateTime)
                            {
                                entity.IssueAssignDateTime = createdDateTime;
                            }

                            //insert
                            grievanceIssueDatesDetails.tickitid = entity.tickitId;
                            grievanceIssueDatesDetails.applicationId = entity.applicationId;
                            grievanceIssueDatesDetails.status = "Assigned";
                            grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
                            grievanceIssueDatesDetails.StatusDatetime = createdDateTime;
                            grievanceIssueDatesDetails.IssueAssignToUserId = grievanceTakrarPurtataInputModel.AssignIssueToUserId.ToString();
                            grievanceIssueDatesDetails.IssueReassignToUserId = "NA";
                            grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
                            grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
                            _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);
                        }
                        else
                        {
                            string usertype = _context.grievanceUserMaster.Where(s => s.guserid == guserid).Select(x => x.usertype).FirstOrDefault()!;
                            if (usertype != null)
                            {
                                if (usertype.ToLower() == "admin")
                                {
                                    if (entityTwo.status!.ToLower() == "open")
                                    {
                                        entity.AssignIssueTo = _context.grievanceUserMaster.Where(x => x.guserid == grievanceTakrarPurtataInputModel.AssignIssueToUserId)
                                        .Select(u => u.division).FirstOrDefault()!;
                                        entity.AssignIssueToUserId = grievanceTakrarPurtataInputModel.AssignIssueToUserId;
                                        entity.priority = grievanceTakrarPurtataInputModel.priority;
                                        entity.grivanceStatus = "Assigned";
                                        if (entity.IssueAssignDateTime == DefaultDateTime)
                                        {
                                            entity.IssueAssignDateTime = createdDateTime;
                                        }

                                        //insert
                                        grievanceIssueDatesDetails.tickitid = entity.tickitId;
                                        grievanceIssueDatesDetails.applicationId = entity.applicationId;
                                        grievanceIssueDatesDetails.status = "Assigned";
                                        grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
                                        grievanceIssueDatesDetails.StatusDatetime = createdDateTime;
                                        grievanceIssueDatesDetails.IssueAssignToUserId = grievanceTakrarPurtataInputModel.AssignIssueToUserId.ToString();
                                        grievanceIssueDatesDetails.IssueReassignToUserId = "NA";
                                        grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
                                        grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
                                        _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);
                                    }
                                    else if (entityTwo.status!.ToLower() == "assigned" || entityTwo.status!.ToLower() == "seen" || entityTwo.status!.ToLower() == "reassigned")
                                    {
                                        grievanceIssueDatesDetails.tickitid = entity.tickitId;
                                        grievanceIssueDatesDetails.applicationId = entity.applicationId;
                                        grievanceIssueDatesDetails.status = "Reassigned";
                                        grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
                                        grievanceIssueDatesDetails.IssueReassignToUserId = grievanceTakrarPurtataInputModel.AssignIssueToUserId.ToString();
                                        grievanceIssueDatesDetails.IssueAssignToUserId = "NA";
                                        grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
                                        grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
                                        grievanceIssueDatesDetails.StatusDatetime = createdDateTime;
                                        _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

                                        entity.ReAssignIssueTo = Convert.ToInt32(grievanceTakrarPurtataInputModel.AssignIssueToUserId);
                                        entity.IssueReassignedDateTime = createdDateTime;
                                        entity.grivanceStatus = "Reassigned";
                                    }
                                    //else if (!IsIssueResolvedorClosed)
                                    //{
                                    //    grievanceIssueDatesDetails.tickitid = entity.tickitId;
                                    //    grievanceIssueDatesDetails.applicationId = entity.applicationId;
                                    //    grievanceIssueDatesDetails.status = "Reassigned";
                                    //    grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
                                    //    grievanceIssueDatesDetails.IssueReassignToUserId = grievanceTakrarPurtataInputModel.AssignIssueToUserId.ToString();
                                    //    grievanceIssueDatesDetails.IssueAssignToUserId = "NA";
                                    //    grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
                                    //    grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
                                    //    grievanceIssueDatesDetails.StatusDatetime = createdDateTime;
                                    //    _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

                                    //    entity.ReAssignIssueTo = Convert.ToInt32(grievanceTakrarPurtataInputModel.AssignIssueToUserId);
                                    //    entity.IssueReassignedDateTime = createdDateTime;
                                    //    entity.grivanceStatus = "Reassigned";
                                    //}

                                    //else if (entityTwo.status!.ToLower() == "inprocess" || entityTwo.status!.ToLower() == "seen")
                                    //{
                                    //    grievanceIssueDatesDetails.tickitid = entity.tickitId;
                                    //    grievanceIssueDatesDetails.applicationId = entity.applicationId;
                                    //    grievanceIssueDatesDetails.status = "Reassigned";
                                    //    grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
                                    //    grievanceIssueDatesDetails.IssueReassignToUserId = grievanceTakrarPurtataInputModel.AssignIssueToUserId.ToString();
                                    //    grievanceIssueDatesDetails.IssueAssignToUserId = "NA";
                                    //    grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
                                    //    grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
                                    //    grievanceIssueDatesDetails.StatusDatetime = createdDateTime;
                                    //    _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

                                    //    entity.ReAssignIssueTo = Convert.ToInt32(grievanceTakrarPurtataInputModel.AssignIssueToUserId);
                                    //    entity.IssueReassignedDateTime = createdDateTime;
                                    //    entity.grivanceStatus = "Reassigned";
                                    //}
                                    //else if (entityTwo.status!.ToLower() == "resolved" && grievanceTakrarPurtataInputModel.isTakrarPurtataDone!.ToLower()=="n")
                                    //{

                                    //    grievanceIssueDatesDetails.tickitid = entity.tickitId;
                                    //    grievanceIssueDatesDetails.applicationId = entity.applicationId;
                                    //    grievanceIssueDatesDetails.status = "Reassigned";
                                    //    grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
                                    //    grievanceIssueDatesDetails.IssueReassignToUserId = grievanceTakrarPurtataInputModel.AssignIssueToUserId.ToString();
                                    //    grievanceIssueDatesDetails.IssueAssignToUserId = "NA";
                                    //    grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
                                    //    grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
                                    //    grievanceIssueDatesDetails.StatusDatetime = createdDateTime;
                                    //    _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

                                    //    entity.ReAssignIssueTo = Convert.ToInt32(grievanceTakrarPurtataInputModel.AssignIssueToUserId);
                                    //    entity.IssueReassignedDateTime = createdDateTime;
                                    //    entity.grivanceStatus = "Reassigned";
                                    //}
                                }

                            }
                        }
                    }
                    else
                    {
                        return "Tickit Data not found";
                    }


                    _context.grievanceData.Attach(entity!);
                    _context.SaveChanges();
                    scope.Complete();
                    return "Success";
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }


        public FetchGrievanceUsers FetchGrievanceUserData(int guserid)
        {
            try
            {
                FetchGrievanceUsers fetchGrievanceUsers = new FetchGrievanceUsers();
                GrievanceUserMaster grievanceUserMasterData = new GrievanceUserMaster();
                //MethodForFileUpload methodForFile = new MethodForFileUpload();
                grievanceUserMasterData = _contextR.grievanceUserMaster.Where(data => data.guserid.Equals(guserid)).FirstOrDefault()!;


                fetchGrievanceUsers.division = grievanceUserMasterData.division;
                fetchGrievanceUsers.usertype = grievanceUserMasterData.usertype;

                return fetchGrievanceUsers;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }

        }


        //start GetUserDetails
        public string EditGrievanceUserViewStatus(string tickitid, int guserid)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    DateTime DefaultDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime currentDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                    var entity = _context.grievanceData.FirstOrDefault(s => s.tickitId == tickitid && s.IsDeleted!.Equals("N"))!;
                    var entityTwo = _context.grievanceIssueDatesDetails.OrderByDescending(x => x.Datetime).FirstOrDefault(s => s.tickitid == tickitid &&
                    s.status!.ToLower() == "seen" && s.IssueSeenByUserId == guserid.ToString())!;



                    GrievanceIssueDatesDetails grievanceIssueDatesDetails = new GrievanceIssueDatesDetails();
                    GrievanceUserMaster grievanceUserMaster = new GrievanceUserMaster();

                    string usertype = _context.grievanceUserMaster.Where(s => s.guserid == guserid).Select(s => s.usertype).FirstOrDefault()!;
                    if (usertype.ToLower() != "admin")
                    {
                        if (entity != null)
                        {
                            if (entityTwo != null)
                            {
                                entityTwo.IssueSeenByUserId = guserid.ToString();
                                entityTwo.StatusDatetime = currentDateTime;
                                _context.grievanceIssueDatesDetails.Attach(entityTwo);
                            }
                            else
                            {
                                grievanceIssueDatesDetails.applicationId = entity.applicationId;
                                grievanceIssueDatesDetails.tickitid = tickitid;
                                grievanceIssueDatesDetails.status = "Seen";
                                grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
                                grievanceIssueDatesDetails.IssueSeenByUserId = guserid.ToString();
                                grievanceIssueDatesDetails.StatusDatetime = currentDateTime;
                                grievanceIssueDatesDetails.IssueAssignToUserId = "NA";
                                grievanceIssueDatesDetails.IssueReassignToUserId = "NA";
                                grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
                                _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

                            }
                            _context.SaveChanges();
                            scope.Complete();
                            return "Success";
                        }
                        else
                        {
                            return "Data not found!";
                        }
                    }
                    else
                    {
                        return "Success";
                    }
                }
                catch (Exception ex)
                {
                    throw new HandleException(ex.Message.ToString());
                }
            }
        }

        public FetchGrievanceDashboardData GetViewDetails(string tickitid, int guserId)
        {
            try
            {
                GrievanceTbl grievanceTbl = _contextR.grievanceData.FirstOrDefault(s => s.tickitId == tickitid && s.IsDeleted == "N")!;
                FetchGrievanceDashboardData fetchData = new FetchGrievanceDashboardData();
                MethodForFileUpload methodForFileUpload = new MethodForFileUpload();
                GrievanceIssueDatesDetails grievanceDatesTbl = new GrievanceIssueDatesDetails();
                GrievanveIssueTransactionDtl grievanveIssueTransactionDtl = new GrievanveIssueTransactionDtl();
                if (grievanceTbl != null)
                {
                    GrievanceStatusMaster statusMaster = new GrievanceStatusMaster();
                    int statusCode = _contextR.grievanceStatusMaster.Where(s => s.status!.ToLower() == grievanceTbl.grivanceStatus!.ToLower()).Select
                        (x => x.statusCode).FirstOrDefault();
                    fetchData.applicationId = grievanceTbl.applicationId;
                    fetchData.tickitId = grievanceTbl.tickitId;
                    fetchData.issueReportDate = grievanceTbl.issueReportDate;
                    fetchData.issueCategory = grievanceTbl.issueCategory;
                    fetchData.mutationName = grievanceTbl.mutationName;
                    fetchData.district_code = grievanceTbl.district_code;
                    fetchData.taluka_code = grievanceTbl.taluka_code;
                    fetchData.district_name_in_marathi = grievanceTbl.district_name_in_marathi;
                    fetchData.taluka_name = grievanceTbl.taluka_name;
                    fetchData.AssignIssueTo = grievanceTbl.AssignIssueTo;//dropdown/edit
                    fetchData.priority = grievanceTbl.priority;//dropdown\
                    fetchData.grivanceStatus = grievanceTbl.grivanceStatus;//dropdown/edit\
                    fetchData.grivanceStatusCode = statusCode;
                    fetchData.seenStatusCode = _contextR.grievanceStatusMaster.Where(s => s.status!.ToLower() == "seen").Select(x => x.statusCode).FirstOrDefault();
                    fetchData.SeenStatus = _contextR.grievanceStatusMaster.Where(s => s.statusCode.ToString() == fetchData.seenStatusCode.ToString()).Select(x => x.status).FirstOrDefault();

                    var hasData = _contextR.grievanceIssueDatesDetails.Where(x => x.tickitid == tickitid && x.status == "seen").FirstOrDefault();
                    if (hasData != null)
                    {
                        string fetchedUserid = _contextR.grievanceIssueDatesDetails.Where(s => s.tickitid! == tickitid && s.status!.ToLower() == "seen").OrderByDescending(x => x.StatusDatetime).Take(1).
                        Select(x => x.IssueSeenByUserId).FirstOrDefault()!.ToString()!;
                        //if (fetchedUserid == null)
                        //{
                        //    fetchData.seenby = "NA";
                        //}
                        fetchData.seenby = _contextR.grievanceUserMaster.Where(x => x.guserid.ToString() == fetchedUserid).Select(x => x.division).FirstOrDefault()!;
                    }
                    fetchData.seenby = "NA";

                    string fileExt = System.IO.Path.GetExtension(grievanceTbl.docPath!);
                    string encryptedDocPath = methodForFileUpload.ConvertImageToBase64(grievanceTbl.docPath!);
                    fetchData.docpath = string.IsNullOrEmpty(encryptedDocPath) ? "NA" : "data:image/" + fileExt.Replace(".", "") + ";base64," + encryptedDocPath;
                    fetchData.docname = grievanceTbl.docTitle;
                    fetchData.issueDescription = grievanceTbl.issueDesc;

                    //fetch replies
                    //fetch reply from DB

                    var replies = _contextR.grievanveIssueTransactionDtls.Where(r => r.tickitId == tickitid)
                    .Select(r => new GrievanceReplyData
                    {
                        reply = r.reply,
                        replyFrom = _contextR.grievanceUserMaster.Where(s => s.guserid == Convert.ToInt32(r.replyUserid)).Select(s => s.division).FirstOrDefault()!.ToString(),
                        time = r.datetime.ToLocalTime()

                    })
                    .ToList();

                    fetchData.grievanceReplyData = replies;
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        //end GetUserDetails

        //search Application data by ID
        public FetchApplicationDataBySearch GetApplicationDataBySearch(string applicationId)
        {
            try
            {
                DateTime date = new DateTime(2025, 04, 04, 0, 0, 0, DateTimeKind.Utc);
                var hasInwardNoError = _context.applicationDTL.Include(u => u.userMaster)
                    .Any(s => s.applicationid == applicationId && s.isDeleted == false)!;
                int status = _context.applicationDTL.Where(x => x.applicationid == applicationId).Select(s => s.status).FirstOrDefault()!;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                NICAPIResponse nICAPIresponse = new NICAPIResponse();

                if (hasInwardNoError && status == 15)
                {
                    applicationDTL = _context.applicationDTL.Include(u => u.userMaster).
                    Where(s => s.applicationid == applicationId && s.isDeleted == false && s.status == 15).FirstOrDefault()!;

                    nICAPIresponse = _context.nICAPIResponses.Where(s => s.applicationid == applicationId).FirstOrDefault()!;

                    var fetchData = new FetchApplicationDataBySearch
                    {
                        applicationId = applicationDTL.applicationid,
                        UserName = _context.userMasters.Where(s => s.userid == applicationDTL.userMaster!.userid).
                   Select(x => x.prefix_in_marathi + " " + x.fname_in_marathi + " " + x.mname_in_marathi + " " + x.lname_in_marathi)
                   .FirstOrDefault()!.ToString(),
                        mutationName = applicationDTL.mutation_type_name,
                        district_code = applicationDTL.district_code,
                        taluka_code = applicationDTL.office_code,
                        district_name_in_marathi = applicationDTL.district_name_in_marathi,
                        taluka_name = applicationDTL.office_name,
                        mobileno = applicationDTL.userMaster?.mobileno,
                        inwardNoError = nICAPIresponse.inwardno,
                        inwardNo = applicationDTL.inwardno,
                        ApplicationCreatedDate = applicationDTL.createddatetime,
                        applicationStatus = applicationDTL.status == 10 ? "Application is submitted to EPCIS" :
                   applicationDTL.status == 11 ? "Truti Patra is generated" :
                   applicationDTL.status == 12 ? "Application is rejected" :
                   applicationDTL.status == 13 ? "Nikali Patra is generated" :
                   applicationDTL.status == 14 ? "Notice 9 is generated" :
                   applicationDTL.status == 15 ? "Inward Number Error" :
                   (applicationDTL.status >= 1 && applicationDTL.status <= 9) ? "Partially Submitted / Pending" :
                   "unknown"
                    };
                    return fetchData;
                }
                else
                {
                    applicationDTL = _context.applicationDTL.Include(u => u.userMaster).
                    Where(s => s.applicationid == applicationId && s.isDeleted == false).FirstOrDefault()!;

                    var fetchData = new FetchApplicationDataBySearch
                    {
                        applicationId = applicationDTL.applicationid,
                        UserName = _context.userMasters.Where(s => s.userid == applicationDTL.userMaster!.userid).
                   Select(x => x.prefix_in_marathi + " " + x.fname_in_marathi + " " + x.mname_in_marathi + " " + x.lname_in_marathi)
                   .FirstOrDefault()!.ToString(),
                        mutationName = applicationDTL.mutation_type_name,
                        district_code = applicationDTL.district_code,
                        taluka_code = applicationDTL.office_code,
                        district_name_in_marathi = applicationDTL.district_name_in_marathi,
                        taluka_name = applicationDTL.office_name,
                        mobileno = applicationDTL.userMaster?.mobileno,
                        inwardNoError = nICAPIresponse.inwardno,
                        inwardNo = applicationDTL.inwardno,
                        ApplicationCreatedDate = applicationDTL.createddatetime,
                        applicationStatus = applicationDTL.status == 10 ? "Application is submitted to EPCIS" :
                   applicationDTL.status == 11 ? "Truti Patra is generated" :
                   applicationDTL.status == 12 ? "Application is rejected" :
                   applicationDTL.status == 13 ? "Nikali Patra is generated" :
                   applicationDTL.status == 14 ? "Notice 9 is generated" :
                   applicationDTL.status == 15 ? "Inward Number Error" :
                   (applicationDTL.status >= 1 && applicationDTL.status <= 9) ? "Partially Submitted / Pending" :
                   "unknown"
                    };
                    return fetchData;
                }

                //ApplicationStatusHistory appStatusHistory = new ApplicationStatusHistory();
                //var fetchData = new FetchApplicationDataBySearch
                //{
                //    applicationId = applicationDTL.applicationid,
                //    UserName = _context.userMasters.Where(s => s.userid == applicationDTL.userMaster!.userid).
                //    Select(x => x.prefix_in_marathi + " " + x.fname_in_marathi + " " + x.mname_in_marathi + " " + x.lname_in_marathi)
                //    .FirstOrDefault()!.ToString(),
                //    mutationName = applicationDTL.mutation_type_name,
                //    district_code = applicationDTL.district_code,
                //    taluka_code = applicationDTL.office_code,
                //    district_name_in_marathi = applicationDTL.district_name_in_marathi,
                //    taluka_name = applicationDTL.office_name,
                //    mobileno = applicationDTL.userMaster?.mobileno,
                //    inwardNoError = nICAPIresponse.inwardno,
                //    inwardNo = applicationDTL.inwardno,
                //    ApplicationCreatedDate = applicationDTL.createddatetime.ToString("dd-MM-yyyy"),
                //    applicationStatus = applicationDTL.status == 10 ? "Application is submitted to EPCIS" :
                //    applicationDTL.status == 11 ? "Truti Patra is generated" :
                //    applicationDTL.status == 12 ? "Application is rejected" :
                //    applicationDTL.status == 13 ? "Nikali Patra is generated" :
                //    applicationDTL.status == 14 ? "Notice 9 is generated" :
                //    applicationDTL.status == 15 ? "Inward Number Error" :
                //    (applicationDTL.status >= 1 && applicationDTL.status <= 9) ? "Partially Submitted / Pending":
                //    "unknown"
                //};


            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public List<FetchApplicationDataBySearch> GetApplicationDataForInwardNoError()
        {
            try
            {
                DateTime date = new DateTime(2025, 04, 04, 0, 0, 0, DateTimeKind.Utc);


                var errorApplicationIds = _context.nICAPIResponses.Where(x => (EF.Functions.Like(x.inwardno!.Trim().ToLower(), "%error%")
                || EF.Functions.Like(x.inwardno!.Trim().ToLower(), "%timeout%")) && x.createddatetime >= date).
                //OrderByDescending(s=>s.createddatetime).
                Select(x => x.applicationid)
                    .Distinct().ToList()!;

                var applicationIds = _context.applicationDTL.Include(u => u.userMaster).
                    Where(s => s.isDeleted == false && errorApplicationIds.Contains(s.applicationid) && s.status == 15)
                    //.OrderByDescending(x => x.createddatetime)
                    .ToList()!;

                var nICAPIResponses = _context.nICAPIResponses.Where(x => errorApplicationIds.Contains(x.applicationid) &&
                     (EF.Functions.Like(x.inwardno!.Trim().ToLower(), "%error%") || EF.Functions.Like(x.inwardno!.Trim().ToLower(), "%timeout%")))
                     //.OrderByDescending(x => x.createddatetime)
                     .ToList();



                var fetchDataList = applicationIds.Select(app => new FetchApplicationDataBySearch
                {
                    UserName = $"{app.userMaster?.fname_in_marathi} {app.userMaster?.mname_in_marathi} {app.userMaster?.lname_in_marathi}".Trim(),
                    applicationId = app.applicationid,
                    mutationName = app.mutation_type_name,
                    district_code = app.district_code,
                    taluka_code = app.office_code,
                    district_name_in_marathi = app.district_name_in_marathi,
                    taluka_name = app.office_name,
                    mobileno = app.userMaster?.mobileno,
                    inwardNo = nICAPIResponses.FirstOrDefault(x => x.applicationid == app.applicationid)?.inwardno,
                    ApplicationCreatedDate = nICAPIResponses.FirstOrDefault(x => x.applicationid == app.applicationid)?.createddatetime,
                })
                .OrderByDescending(x => x.ApplicationCreatedDate)
                .ToList();





                return fetchDataList;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        //fetch all application ids
        //public List<string> FetchAllApplicationIDS(int pageno, int pagesize)
        //{
        //    try
        //    {
        //        List<string> applicationIds = _context.applicationDTL.Where(x => x.isDeleted == false).Select(u => u.applicationid).ToList()!;
        //        //List<string> result = _context.grievanceData.Where(u => u.userId.Equals(userid) && u.IsDeleted!.Equals("N")).Select(s => s.tickitId).ToList()!;
        //        return applicationIds;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}

        public PaginatedResult<string?> FetchAllApplicationIDS(GetAllApplicationIdForReport getAllApplicationIdForReport)
        {
            try
            {
                DateTime fromDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.fromDate!, DateTimeKind.Utc);
                DateTime toDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.toDate!, DateTimeKind.Utc);

                if (getAllApplicationIdForReport.pageno <= 0) getAllApplicationIdForReport.pageno = 1;
                if (getAllApplicationIdForReport.pagesize <= 0) getAllApplicationIdForReport.pagesize = 10;

                //var totalRecords = _context.applicationDTL.CountAsync().Result;
                var totalRecords = _contextR.applicationDTL.Where(x => x.isDeleted == false && x.createddatetime >= fromDate && x.createddatetime <= toDate).CountAsync().Result;
                var totalPages = (int)Math.Ceiling((double)totalRecords / getAllApplicationIdForReport.pagesize);

                var applicationIds = _contextR.applicationDTL.Where(x => x.isDeleted == false && x.createddatetime >= fromDate && x.createddatetime <= toDate).
                    OrderByDescending(x => x.createddatetime).Select(u => u.applicationid).
                    Skip((getAllApplicationIdForReport.pageno - 1) * getAllApplicationIdForReport.pagesize).Take(getAllApplicationIdForReport.pagesize).ToList()!;

                var response = new PaginatedResult<string?>
                {
                    Data = applicationIds,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    CurrentPage = getAllApplicationIdForReport.pageno
                };
                return response;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        //fetch dashboard for all application ids 
        public FetchApplicationDataBySearch FetchAllApplicationIdsDashboardData(string applicationid)
        {
            try
            {

                ApplicationDTL applicationDtl = _contextR.applicationDTL.OrderByDescending(s => s.createddatetime).FirstOrDefault(s => s.applicationid == applicationid && s.isDeleted == false)!;
                FetchApplicationDataBySearch fetchData = new FetchApplicationDataBySearch();
                if (applicationDtl != null)
                {
                    if (applicationDtl.status == 10 && applicationDtl.inwardno != "NA")
                    {
                        fetchData.applicationStatus = "Submitted to NIC";
                    }
                    if (applicationDtl.status == 1 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Application ID Generated";
                    }
                    if (applicationDtl.status == 2 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Applicants are added";
                    }
                    if (applicationDtl.status == 3 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "CTS Details are added";
                    }
                    if (applicationDtl.status == 4 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Dast Details are added";
                    }
                    if (applicationDtl.status == 5 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Mutation Details are added";
                    }
                    if (applicationDtl.status == 6 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Power Of Attroney Details are added";
                    }
                    if (applicationDtl.status == 7 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Court Claim Details are added";
                    }
                    if (applicationDtl.status == 8 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Documents are uploaded";
                    }
                    if (applicationDtl.status == 9 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Self Declaration Details are added";
                    }
                    if (applicationDtl.status == 11 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Truti Patra is generated";
                    }
                    if (applicationDtl.status == 12 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Application is rejected";
                    }
                    if (applicationDtl.status == 13 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Nikali Patra is generated";
                    }
                    if (applicationDtl.status == 14 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Notice 9 is generated";
                    }
                    if (applicationDtl.status == 15 && applicationDtl.inwardno == "NA")
                    {
                        fetchData.applicationStatus = "Inward No Error";
                    }

                    fetchData.applicationId = applicationDtl.applicationid;
                    fetchData.mutationName = applicationDtl.mutation_type_name;
                    fetchData.mutationTypeCode = applicationDtl.mutation_type_code;
                    fetchData.district_code = applicationDtl.district_code;
                    fetchData.taluka_code = applicationDtl.office_code;
                    fetchData.district_name_in_marathi = applicationDtl.district_name_in_marathi;
                    fetchData.taluka_name = applicationDtl.office_name;
                    fetchData.inwardNo = applicationDtl.inwardno;
                    fetchData.ApplicationCreatedDate = applicationDtl.createddatetime;
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        //takrar purtata save for user
        public string SaveGrievanceTakrarPurtata(GrievanceTakrarPurtataInputModel grievanceTakrarPurtataInputModel, string guserid)//input model
        {

            using (var scope = new TransactionScope())
            {
                GrievanceTbl grievancetable = new GrievanceTbl();//table
                grievanceIssueTransactionDtlModel grievanceIssueTransactionDtlmodel = new grievanceIssueTransactionDtlModel();//model
                GrievanveIssueTransactionDtl grievanveIssueTransactionDtl = new GrievanveIssueTransactionDtl();//table
                GrievanceIssueDatesDetails grievanceIssueDatesDetails = new GrievanceIssueDatesDetails();
                DateTime currentDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                DateTime DefaultDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                if (grievanceTakrarPurtataInputModel != null)
                {
                    try
                    {
                        //assign input values to model
                        grievanceIssueTransactionDtlmodel.applicationId = grievanceTakrarPurtataInputModel.applicaitonId;
                        grievanceIssueTransactionDtlmodel.tickitId = grievanceTakrarPurtataInputModel.tickitId;
                        grievanceIssueTransactionDtlmodel.issueDescByUser = grievanceTakrarPurtataInputModel.actualUserIssue;
                        grievanceIssueTransactionDtlmodel.reply = grievanceTakrarPurtataInputModel.userCompliance;
                        grievanceIssueTransactionDtlmodel.replyUserid = guserid;



                        //assign values from model to table
                        grievanveIssueTransactionDtl.applicationId = grievanceIssueTransactionDtlmodel.applicationId;
                        grievanveIssueTransactionDtl.tickitId = grievanceIssueTransactionDtlmodel.tickitId;
                        grievanveIssueTransactionDtl.issueDescByUser = grievanceIssueTransactionDtlmodel.issueDescByUser;
                        grievanveIssueTransactionDtl.reply = grievanceIssueTransactionDtlmodel.reply;
                        grievanveIssueTransactionDtl.replyUserid = grievanceIssueTransactionDtlmodel.replyUserid;
                        _context.grievanveIssueTransactionDtls.Add(grievanveIssueTransactionDtl);


                        //
                        var entity = _context.grievanceData.Where(x => x.applicationId == grievanceTakrarPurtataInputModel.applicaitonId && x.tickitId == grievanceTakrarPurtataInputModel.tickitId).FirstOrDefault()!;

                        var entityTwo = _context.grievanceIssueDatesDetails.Where(x => x.applicationId == grievanceTakrarPurtataInputModel.applicaitonId && x.tickitid == grievanceTakrarPurtataInputModel.tickitId)
                            .OrderByDescending(x => x.Datetime).Take(1).FirstOrDefault()!;
                        if(entity!=null && entityTwo!=null)
                        {
                            if (entityTwo.status!.ToLower() != "resolved")
                            {
                                if (grievanceTakrarPurtataInputModel.takrarPurtataStatusInYOrN!.ToLower() == "y")
                                {
                                    entity.grivanceStatus = "Resolved";
                                    entity.IssueResolvedDateTime = currentDateTime;
                                    _context.grievanceData.Attach(entity);

                                    //dates table
                                    grievanceIssueDatesDetails.applicationId = entity.applicationId;
                                    grievanceIssueDatesDetails.tickitid = grievanceTakrarPurtataInputModel.tickitId;
                                    grievanceIssueDatesDetails.status = "Resolved";
                                    grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
                                    grievanceIssueDatesDetails.IssueResolvedByUserid = guserid.ToString();
                                    grievanceIssueDatesDetails.IssueAssignToUserId = "NA";
                                    grievanceIssueDatesDetails.IssueReassignToUserId = "NA";
                                    grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
                                    grievanceIssueDatesDetails.StatusDatetime = currentDateTime;
                                    _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

                                }
                                else if (grievanceTakrarPurtataInputModel.takrarPurtataStatusInYOrN!.ToLower() == "n")
                                {
                                    entity.grivanceStatus = "InProcess";
                                    _context.grievanceData.Attach(entity);
                                }
                            }
                            else
                            {
                                string user = _context.grievanceUserMaster.Where(x => x.guserid == Convert.ToInt32(entityTwo.IssueResolvedByUserid)).FirstOrDefault()!.ToString()!;
                                return "Issue Already resolved by " + " " + user;
                            }
                        }
                        else
                        {
                            return "Data not found!";
                        }


                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    catch (Exception e)
                    {

                        return e.Message.ToString();
                    }
                }
                else
                {
                    return "Please fill atleast one Feild!";
                }
            }
        }


        //takrar purtata save for Admin

        public string SaveGrievanceAdminTakrarPurtata(GrievanceTakrarPurtataInputModel grievanceTakrarPurtataInputModel, string guserid)
        {
            using (var scope = new TransactionScope())
            {
                GrievanceTbl grievancetable = new GrievanceTbl();//table
                grievanceIssueTransactionDtlModel grievanceIssueTransactionDtlmodel = new grievanceIssueTransactionDtlModel();//model
                GrievanveIssueTransactionDtl grievanveIssueTransactionDtl = new GrievanveIssueTransactionDtl();//table
                GrievanceIssueDatesDetails grievanceIssueDatesDetails = new GrievanceIssueDatesDetails();
                DateTime currentDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                DateTime DefaultDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                 if (grievanceTakrarPurtataInputModel != null)
                {
                    try
                    {
                        var entity = _context.grievanceData.Where(x => x.applicationId == grievanceTakrarPurtataInputModel.applicaitonId && x.tickitId == grievanceTakrarPurtataInputModel.tickitId).FirstOrDefault()!;
                        if (entity != null)
                        {
                            //check status and assign issue to admin asel tr closed direct
                            var data = _context.grievanceUserMaster.Where(s => s.guserid == grievanceTakrarPurtataInputModel.AssignIssueToUserId).FirstOrDefault();
                            if (data != null && data.usertype!.ToLower() == "admin")
                            {
                                if (grievanceTakrarPurtataInputModel.isTakrarPurtataDone!.ToLower() == "y")
                                {
                                    entity.IssueClosedBy = guserid;
                                    entity.grivanceStatus = "Closed";
                                    _context.grievanceData.Attach(entity);

                                    grievanceIssueDatesDetails.applicationId = grievanceTakrarPurtataInputModel.applicaitonId;
                                    grievanceIssueDatesDetails.tickitid = grievanceTakrarPurtataInputModel.tickitId;
                                    grievanceIssueDatesDetails.status = "Closed";
                                    grievanceIssueDatesDetails.IssueAssignToUserId = "NA";
                                    grievanceIssueDatesDetails.IssueReassignToUserId = "NA";
                                    grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
                                    grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
                                    grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
                                    grievanceIssueDatesDetails.StatusDatetime = currentDateTime;
                                    _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

                                    //assign input values to model
                                    grievanceIssueTransactionDtlmodel.applicationId = grievanceTakrarPurtataInputModel.applicaitonId;
                                    grievanceIssueTransactionDtlmodel.tickitId = grievanceTakrarPurtataInputModel.tickitId;
                                    grievanceIssueTransactionDtlmodel.issueDescByUser = grievanceTakrarPurtataInputModel.actualUserIssue;
                                    grievanceIssueTransactionDtlmodel.reply = grievanceTakrarPurtataInputModel.adminComplience;
                                    grievanceIssueTransactionDtlmodel.replyUserid = guserid;


                                    //assign values from model to table
                                    grievanveIssueTransactionDtl.applicationId = grievanceIssueTransactionDtlmodel.applicationId;
                                    grievanveIssueTransactionDtl.tickitId = grievanceIssueTransactionDtlmodel.tickitId;
                                    grievanveIssueTransactionDtl.issueDescByUser = grievanceIssueTransactionDtlmodel.issueDescByUser;
                                    grievanveIssueTransactionDtl.reply = grievanceIssueTransactionDtlmodel.reply;
                                    grievanveIssueTransactionDtl.replyUserid = grievanceIssueTransactionDtlmodel.replyUserid;
                                    _context.grievanveIssueTransactionDtls.Add(grievanveIssueTransactionDtl);
                                }

                            }
                            else
                            {
                                if (entity.grivanceStatus!.ToLower() == "resolved")
                                {
                                    if (entity.IssueClosedBy!.ToLower() == "na" && grievanceTakrarPurtataInputModel.isTakrarPurtataDone!.ToLower() == "y")
                                    {
                                        entity.IssueClosedBy = guserid;
                                        entity.grivanceStatus = "Closed";
                                        _context.grievanceData.Attach(entity);

                                        grievanceIssueDatesDetails.applicationId = grievanceTakrarPurtataInputModel.applicaitonId;
                                        grievanceIssueDatesDetails.tickitid = grievanceTakrarPurtataInputModel.tickitId;
                                        grievanceIssueDatesDetails.status = "Closed";
                                        grievanceIssueDatesDetails.IssueAssignToUserId = "NA";
                                        grievanceIssueDatesDetails.IssueReassignToUserId = "NA";
                                        grievanceIssueDatesDetails.IssueSeenByUserId = "NA";
                                        grievanceIssueDatesDetails.IssueResolvedByUserid = "NA";
                                        grievanceIssueDatesDetails.PerformByUserId = guserid.ToString();
                                        grievanceIssueDatesDetails.StatusDatetime = currentDateTime;
                                        _context.grievanceIssueDatesDetails.Add(grievanceIssueDatesDetails);

                                        ////assign input values to model
                                        //grievanceIssueTransactionDtlmodel.applicationId = grievanceTakrarPurtataInputModel.applicaitonId;
                                        //grievanceIssueTransactionDtlmodel.tickitId = grievanceTakrarPurtataInputModel.tickitId;
                                        //grievanceIssueTransactionDtlmodel.issueDescByUser = grievanceTakrarPurtataInputModel.actualUserIssue;
                                        //grievanceIssueTransactionDtlmodel.reply = grievanceTakrarPurtataInputModel.adminComplience;
                                        //grievanceIssueTransactionDtlmodel.replyUserid = guserid;


                                        ////assign values from model to table
                                        //grievanveIssueTransactionDtl.applicationId = grievanceIssueTransactionDtlmodel.applicationId;
                                        //grievanveIssueTransactionDtl.tickitId = grievanceIssueTransactionDtlmodel.tickitId;
                                        //grievanveIssueTransactionDtl.issueDescByUser = grievanceIssueTransactionDtlmodel.issueDescByUser;
                                        //grievanveIssueTransactionDtl.reply = grievanceIssueTransactionDtlmodel.reply;
                                        //grievanveIssueTransactionDtl.replyUserid = grievanceIssueTransactionDtlmodel.replyUserid;
                                        //_context.grievanveIssueTransactionDtls.Add(grievanveIssueTransactionDtl);
                                    }
                                }
                                else if (entity.grivanceStatus.ToLower() == "closed")
                                {
                                    return "Issue is already closed!";
                                }


                                //assign input values to model
                                grievanceIssueTransactionDtlmodel.applicationId = grievanceTakrarPurtataInputModel.applicaitonId;
                                grievanceIssueTransactionDtlmodel.tickitId = grievanceTakrarPurtataInputModel.tickitId;
                                grievanceIssueTransactionDtlmodel.issueDescByUser = grievanceTakrarPurtataInputModel.actualUserIssue;
                                grievanceIssueTransactionDtlmodel.reply = grievanceTakrarPurtataInputModel.adminComplience;
                                grievanceIssueTransactionDtlmodel.replyUserid = guserid;


                                //assign values from model to table
                                grievanveIssueTransactionDtl.applicationId = grievanceIssueTransactionDtlmodel.applicationId;
                                grievanveIssueTransactionDtl.tickitId = grievanceIssueTransactionDtlmodel.tickitId;
                                grievanveIssueTransactionDtl.issueDescByUser = grievanceIssueTransactionDtlmodel.issueDescByUser;
                                grievanveIssueTransactionDtl.reply = grievanceIssueTransactionDtlmodel.reply;
                                grievanveIssueTransactionDtl.replyUserid = grievanceIssueTransactionDtlmodel.replyUserid;
                                _context.grievanveIssueTransactionDtls.Add(grievanveIssueTransactionDtl);
                            }

                        }
                        else
                        {
                            return "Data not found!";
                        }
                        _context.SaveChanges();
                        scope.Complete();
                        return "Success";
                    }
                    catch (Exception e)
                    {

                        return e.Message.ToString();
                    }
                }
                else
                {
                    return "Please fill atleast one Feild!";
                }
            }
        }


        //get Actual user data

        public FetchActualUserData GetActualUserViewDetails(string tickitid)
        {
            try
            {
                FetchActualUserData fetchData = new FetchActualUserData();
                GrievanveIssueTransactionDtl grievanveIssueTransactionDtl = new GrievanveIssueTransactionDtl();
                GrievanceUserMaster grievanceUserMaster = new GrievanceUserMaster();
                GrievanceTbl grievanceTbl = new GrievanceTbl();

                if (grievanveIssueTransactionDtl != null)
                {
                    bool isDeleted = _contextR.grievanceData.Where(x => x.IsDeleted!.ToLower() == "y" && x.tickitId == tickitid).Any();
                    var IsClosed = _contextR.grievanceData.Any(x => x.tickitId == tickitid && x.grivanceStatus!.ToLower() == "closed");


                    if (!isDeleted)
                    {
                        var latestReply = _contextR.grievanveIssueTransactionDtls.Where(x => x.tickitId == tickitid).OrderByDescending(x => x.datetime).FirstOrDefault()!;
                        if (latestReply == null)
                        {
                            fetchData.reply = "No Any Reply";
                            fetchData.replyFrom = "NA";
                            fetchData.ReplyDate = Convert.ToDateTime("1900-01-01");
                        }
                        else
                        {
                            if (IsClosed)
                            {
                                fetchData.reply = latestReply.reply;
                                fetchData.replyFrom = _contextR.grievanceUserMaster.Where(x => x.guserid == 3).Take(1).FirstOrDefault()!.ToString();
                                fetchData.ReplyDate = latestReply.datetime;
                            }
                            else
                            {
                                fetchData.reply = "No Any Reply";
                                fetchData.replyFrom = "NA";
                                fetchData.ReplyDate = Convert.ToDateTime("1900-01-01");
                            }

                        }
                    }
                    else
                    {
                        fetchData = null!;
                    }
                }
                else
                {
                    fetchData = null!;
                }
                return fetchData;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        //export to excel

        public byte[] ExportToExcel(GetAllApplicationIdForReport getAllApplicationIdForReport)
        {
            
            DateTime fromDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.fromDate!, DateTimeKind.Utc);
            DateTime toDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.toDate!, DateTimeKind.Utc);
            toDate = toDate.Date.AddDays(1).AddTicks(-1);

            var data = _contextR.applicationDTL.Where(x => x.createddatetime >= fromDate && x.createddatetime <= toDate).
            OrderByDescending(x => x.createddatetime).ToList()!;



            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Applications");

            // Headers

            worksheet.Cell(1, 1).Value = "Application ID";
            worksheet.Cell(1, 2).Value = "Inward No";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Mutation Type Code";
            worksheet.Cell(1, 5).Value = "Mutation Name";
            worksheet.Cell(1, 6).Value = "District Code";
            worksheet.Cell(1, 7).Value = "District Name In Marathi";
            worksheet.Cell(1, 8).Value = "District Name In English";
            worksheet.Cell(1, 9).Value = "Office / Talika Code";
            worksheet.Cell(1, 10).Value = "Office / Taluka Name";
            worksheet.Cell(1, 11).Value = "Application Created Date";
            worksheet.Cell(1, 12).Value = "Is Deleted";

            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.applicationid!.ToString();
                worksheet.Cell(row, 1).Style.NumberFormat.Format = "@"; // text format

                worksheet.Cell(row, 2).Value = item.inwardno;
                worksheet.Cell(row, 3).Value = GetStatusText(item.status, item.inwardno!);
                worksheet.Cell(row, 4).Value = item.mutation_type_code;
                worksheet.Cell(row, 5).Value = item.mutation_type_name;
                worksheet.Cell(row, 6).Value = item.district_code;
                worksheet.Cell(row, 7).Value = item.district_name_in_marathi;
                worksheet.Cell(row, 8).Value = item.district_name_in_english;
                worksheet.Cell(row, 9).Value = item.office_code;
                worksheet.Cell(row, 10).Value = item.office_name;
                worksheet.Cell(row, 11).Value = item.isDeleted;
                worksheet.Cell(row, 12).Value = item.createddatetime.ToString("yyyy-MM-dd");

                row++;
            }

            var headerRange = worksheet.Range("A1:L1");
            headerRange.SetAutoFilter();
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.Apricot;
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


        //private string GetStatusText(int status, string inwardNo)
        //{
        //    return (status, inwardNo) switch
        //    {
        //        (10, not "NA") => "Submitted to NIC",
        //        (1,"NA") => "Application ID Generated",
        //        (2,"NA") => "Applicants are added",
        //        (3,"NA") => "CTS Details are added",
        //        (4,"NA") => "Dast Details are added",
        //        (5,"NA") => "Mutation Details are added",
        //        (6,"NA") => "Power Of Attroney Details are added",
        //        (7,"NA") => "Court Claim Details are added",
        //        (8,"NA") => "Documents are uploaded",
        //        (9,"NA") => "Self Declaration Details are added",
        //        (11,"NA") => "Truti Patra is generated",
        //        (12,"NA") => "Application is rejected",
        //        (13,"NA") => "Nikali Patra is generated",
        //        (14,"NA") => "Notice 9 is generated",
        //        (15,"NA") => "Inward No Error",
        //        _ => "Unknown"
        //    };
        //}

        private string GetStatusText(int status, string inwardNo)
        {
            if (status >= 1 && status <= 9 && inwardNo == "NA")
            {
                return "Partially Submitted / Pending";
            }

            return (status, inwardNo) switch
            {
                (10, not "NA") => "Submitted to NIC",
                (11, "NA") => "Truti Patra is generated",
                (12, "NA") => "Application is rejected",
                (13, "NA") => "Nikali Patra is generated",
                (14, "NA") => "Notice 9 is generated",
                (15, "NA") => "Inward No Error",
                _ => "Unknown"
            };
        }

        //return only count 
        public List<FetchCountOfApplication> FetchCountOfApplications(GetAllApplicationIdForReport getAllApplicationIdForReport)
        {
            try
            {

                DateTime fromDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.fromDate!, DateTimeKind.Utc);
                DateTime toDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.toDate!, DateTimeKind.Utc);
                toDate = toDate.Date.AddDays(1).AddTicks(-1);
                //var result = _context.applicationDTL
                //.Where(s =>s.isDeleted==false &&
                //            s.createddatetime >= fromDate &&
                //            s.createddatetime <= toDate)
                //.OrderByDescending(r => r.createddatetime)
                //.GroupBy(s => s.status)
                //.Select(g => new FetchCountOfApplication
                //{
                //    applicationStatusCode = g.Key,
                //    applicationStatus = g.Key == 1 ? "Application ID Generated" :
                //                 g.Key == 2 ? "Applicants are added" :
                //                 g.Key == 3 ? "CTS Details are added" :
                //                 g.Key == 4 ? "Dast Details are added" :
                //                 g.Key == 5 ? "Mutation Details are added" :
                //                 g.Key == 6 ? "Power Of Attorney Details are added" :
                //                 g.Key == 7 ? "Court Claim Details are added" :
                //                 g.Key == 8 ? "Documents are uploaded" :
                //                 g.Key == 9 ? "Self Declaration Details are added" :
                //                 g.Key == 10 ? "Application is submitted to NIC" :
                //                 g.Key == 11 ? "Truti Patra is generated" :
                //                 g.Key == 12 ? "Application is rejected" :
                //                 g.Key == 13 ? "Nikali Patra is generated" :
                //                 g.Key == 14 ? "Notice 9 is generated" :
                //                 g.Key == 15 ? "Inward No Error" :
                //                 "Unknown",
                //    countOfApplicationId = g.Count()
                //})
                //.ToList();

                //var result = _contextR.applicationDTL
                //.Where(s => s.createddatetime >= fromDate &&
                //            s.createddatetime <= toDate)
                //.OrderByDescending(r => r.createddatetime)
                //.GroupBy(a => a.status >= 1 && a.status <= 9 ? 0 : a.status)
                //.Select(g => new FetchCountOfApplication
                //{
                //    applicationStatusCode = g.Key,
                //    applicationStatus = g.Key == 0 ? "Partially Submitted / Pending" :
                //                 //g.Key == 2 &&
                //                 //g.Key == 3 &&
                //                 //g.Key == 4 &&
                //                 //g.Key == 5 &&
                //                 //g.Key == 6 &&
                //                 //g.Key == 7 &&
                //                 //g.Key == 8 &&
                //                 //g.Key == 9 ? 
                //                 //g.Keyg.Key == 5 == 2 ? "Applicants are added" :
                //                 //g.Keyg.Key == 6 == 3 ? "CTS Details are added" :
                //                 //g.Keyg.Key == 7 == 4 ? "Dast Details are added" :
                //                 //g.Keyg.Key == 8 == 5 ? "Mutation Details are added" :
                //                 //g.Keyg.Key == 9 == 6 ? "Power Of Attorney Details are added" :
                //                 //g.Key == 7 ? "Court Claim Details are added" :
                //                 //g.Key == 8 ? "Documents are uploaded" :
                //                 //g.Key == 9 ? "Self Declaration Details are added" :
                //                 g.Key == 10 ? "Application is submitted to EPCIS" :
                //                 g.Key == 11 ? "Truti Patra is generated" :
                //                 g.Key == 12 ? "Application is rejected" :
                //                 g.Key == 13 ? "Nikali Patra is generated" :
                //                 g.Key == 14 ? "Notice 9 is generated" :
                //                 g.Key == 15 ? "Inward Number Error" :
                //                 "unknown",
                //                //"Application Processed by to EPCIS",
                //    countOfApplicationId = g.Count()
                //})
                //.ToList();

                //long grandTotal = result.Sum(x => x.countOfApplicationId);

                //// Append grand total record
                //result.Add(new FetchCountOfApplication
                //{
                //    applicationStatusCode = 0,
                //    applicationStatus = "Grand Total",
                //    countOfApplicationId = grandTotal
                //});

                var expectedStatusCodes = new Dictionary<int, string>
                {
                    { 0, "Partially Submitted / Pending" },
                    { 10, "Application is submitted to EPCIS" },
                    { 11, "Truti Patra is generated" },
                    { 12, "Application is rejected" },
                    { 13, "Nikali Patra is generated" },
                    { 14, "Notice 9 is generated" },
                    { 15, "Inward Number Error" }
                };

                // Fetch actual counts
                var actualCounts = _contextR.applicationDTL
                    .Where(s => s.createddatetime >= fromDate && s.createddatetime <= toDate)
                    .GroupBy(a => a.status >= 1 && a.status <= 9 ? 0 : a.status)
                    .Select(g => new
                    {
                        StatusCode = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                // Merge with expected statuses to fill in zeros
                var result = expectedStatusCodes.Select(kvp =>
                {
                    var count = actualCounts.FirstOrDefault(a => a.StatusCode == kvp.Key)?.Count ?? 0;
                    return new FetchCountOfApplication
                    {
                        applicationStatusCode = kvp.Key,
                        applicationStatus = kvp.Value,
                        countOfApplicationId = count
                    };
                }).ToList();

                // Append grand total
                long grandTotal = result.Sum(x => x.countOfApplicationId);
                result.Add(new FetchCountOfApplication
                {
                    applicationStatusCode = -1,
                    applicationStatus = "Grand Total",
                    countOfApplicationId = grandTotal
                });



                if (result != null)
                {
                    return result;
                }
                else
                {
                    return result = null;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        //give mutation wise count
        public List<FetchCountOfMutations> FetchMutationCount(GetAllApplicationIdForReport getAllApplicationIdForReport)
        {
            try
            {

                DateTime fromDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.fromDate!, DateTimeKind.Utc);
                DateTime toDate = DateTime.SpecifyKind((DateTime)getAllApplicationIdForReport.toDate!, DateTimeKind.Utc);
                toDate = toDate.Date.AddDays(1).AddTicks(-1);

                //var result = _context.applicationDTL
                //.Where(s => !s.isDeleted &&
                //            s.createddatetime >= fromDate &&
                //            s.createddatetime <= toDate &&
                //            (s.status == 10 || s.status == 15)) // Only submitted and error
                //.GroupBy(a => new { a.mutation_type_name, a.status })
                //.Select(g => new FetchCountOfMutations
                //{
                //    mutationName = g.Key.mutation_type_name ?? "Unknown Mutation",
                //    applicationStatusCode = g.Key.status,
                //    applicationStatus = g.Key.status == 10 ? "Application is submitted to NIC" :
                //                        g.Key.status == 15 ? "Inward Number Error" :
                //                        "Unknown",
                //    countOfmutation = g.Count()
                //})
                //.OrderBy(r => r.mutationName)
                //.ThenBy(r => r.applicationStatusCode)
                //.ToList();
                //long grandTotal = result.Sum(x => x.countOfmutation);

                //// Append grand total record
                //result.Add(new FetchCountOfMutations
                //{
                //    applicationStatusCode = 0,
                //    applicationStatus = "Grand Total",
                //    mutationName = "NA",
                //    countOfmutation = grandTotal
                //});
                var rawData = _context.applicationDTL
                .Where(s => s.createddatetime >= fromDate && s.createddatetime <= toDate)
                .Select(s => new
                {
                    MutationName = s.mutation_type_name ?? "Unknown",
                    StatusCode = s.status >= 1 && s.status <= 9 ? 0 : s.status
                })
                .ToList(); // Materialize here so you can use .NET methods below

                var result = rawData
                    .GroupBy(x => new { x.MutationName, x.StatusCode })
                    .Select(g => new
                    {
                        MutationName = GetShortMutationName(g.Key.MutationName),
                        StatusCode = g.Key.StatusCode,
                        Count = g.Count()
                    })
                    .GroupBy(x => x.MutationName)
                    .Select(g => new FetchCountOfMutations
                    {
                        MutationName = g.Key,
                        Statuses = g.Select(x => new StatusDetail
                        {
                            ApplicationStatusCode = x.StatusCode,
                            ApplicationStatus = x.StatusCode == 0 ? "Partially Submitted / Pending" :
                                                x.StatusCode == 10 ? "Application is submitted to EPCIS" :
                                                x.StatusCode == 15 ? "Inward Number Error" :
                                                "Application Processed by EPCIS",
                            CountOfMutation = x.Count
                        }).ToList()
                    })
                    .ToList();

                if (result != null)
                {
                    return result;
                }
                else
                {
                    result = null;
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        private string GetShortMutationName(string fullName)
        {
            return fullName switch
            {

                "गहाणखत / तारण / बोजा दाखल नोंद" => "गहाणखत नोंद",
                _ => fullName // default to original if no match
            };
        }
    }
}
