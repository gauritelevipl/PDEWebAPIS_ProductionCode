using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel.GahankhatNond;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Repository;
using PDEWebAPIS.Services;
using PDEWebAPIS.ViewModel;
using System.Net;
using System.Net.Http.Headers;

namespace PDEWebAPIS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            var remoteIP = HttpContext.Connection.RemoteIpAddress;
            if (remoteIP != null && remoteIP.IsIPv4MappedToIPv6) {
                remoteIP = remoteIP.MapToIPv4();
            }
            var iplist = _configuration.GetValue<string>("IPList")!.Split(";").ToList();
            var exist=iplist.Exists(ip=>remoteIP!.Equals(IPAddress.Parse(ip)));
            if (exist)
            {
                return StatusCode(403);
            }

            _logger.LogInformation("Seri Log is Working");
            return Ok( Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray());
        }



        ////Gahankhat Ghenara
        //[Authorize]
        //[HttpPost]
        //[Route("CreateGahankhatInfoForTaker")]
        //public string CreateGahankhatInfoForTaker([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        GahankhatDataForGiver gahankhatData = JsonConvert.DeserializeObject<GahankhatDataForGiver>(decrypted!)!;
        //        _logger.LogInformation("Create Gahankhat Info For Giver Request Data - " + decrypted);

        //        int UserID = 0;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        var CallAPIForFlag = Request.Headers["CallAPIFor"];
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }
        //        gahankhatData.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = mutationServices.SaveGahankhatGiver(gahankhatData);
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(gahankhatData.applicationid!);
        //            string[] mutationgiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
        //            List<FetchGahankhatForGiverData> fetchDataLits = new List<FetchGahankhatForGiverData>();
        //            for (int i = 0; i < mutationgiverIDs.Length; i++)
        //            {
        //                FetchGahankhatForGiverData fetchData = new FetchGahankhatForGiverData();
        //                fetchData = mutationServices.FetchGahankhatInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //                if (fetchData != null)
        //                {
        //                    fetchDataLits.Add(fetchData);
        //                }
        //            }
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Is Created Successfully", fetchDataLits))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Create Gahankhat Info For Giver Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Create Gahankhat Info For Giver Exception - " + ex.StackTrace!.ToString());
        //        if (ex.Message.ToString() == "User Not Found")
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
        //        }
        //        else
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
        //        }
        //    }
        //}

        //[Authorize]
        //[HttpPost]
        //[Route("EditGahankhatInfoForTaker")]
        //public string EditGahankhatInfoForTaker([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        EditGahankhatDataForGiver gahankhatData = JsonConvert.DeserializeObject<EditGahankhatDataForGiver>(decrypted!)!;
        //        _logger.LogInformation("Edit Gahankhat Info For Giver Request Data - " + decrypted);

        //        int UserID = 0;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        var CallAPIForFlag = Request.Headers["CallAPIFor"];
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }
        //        gahankhatData.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = mutationServices.EditGahankhatGiver(gahankhatData);
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(gahankhatData.applicationid!);
        //            string[] mutationgiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
        //            List<FetchGahankhatForGiverData> fetchDataLits = new List<FetchGahankhatForGiverData>();
        //            for (int i = 0; i < mutationgiverIDs.Length; i++)
        //            {
        //                FetchGahankhatForGiverData fetchData = new FetchGahankhatForGiverData();
        //                fetchData = mutationServices.FetchGahankhatInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //                if (fetchData != null)
        //                {
        //                    fetchDataLits.Add(fetchData);
        //                }
        //            }
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Updated Successfully", fetchDataLits))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Edit Gahankhat Info For Giver Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Edit Gahankhat Info For Giver Exception - " + ex.StackTrace!.ToString());
        //        if (ex.Message.ToString() == "User Not Found")
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
        //        }
        //        else
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
        //        }
        //    }
        //}

        //[Authorize]
        //[HttpPost]
        //[Route("DeleteGahankhatInfoForTaker")]
        //public string DeleteGahankhatInfoForTaker([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
        //        _logger.LogInformation("Delete Gahankhat Info For Giver Request Data - " + decrypted);

        //        int UserID = 0;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        var CallAPIForFlag = Request.Headers["CallAPIFor"];
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }
        //        //gahankhatData.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = mutationServices.DeleteMutationGiver(delete);
        //        List<FetchGahankhatForGiverData> fetchDataList = new List<FetchGahankhatForGiverData>();
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
        //            if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
        //            {
        //                string[] mutationgiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
        //                for (int i = 0; i < mutationgiverIDs.Length; i++)
        //                {
        //                    FetchGahankhatForGiverData fetchData = new FetchGahankhatForGiverData();
        //                    fetchData = mutationServices.FetchGahankhatInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //                    if (fetchData != null)
        //                    {
        //                        fetchDataList.Add(fetchData);
        //                    }
        //                }
        //            }
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Giver Data Deleted Successfully", fetchDataList))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Delete Gahankhat Info For Giver Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchDataList))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Delete Gahankhat Info For Giver Exception - " + ex.StackTrace!.ToString());
        //        if (ex.Message.ToString() == "User Not Found")
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
        //        }
        //        else
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
        //        }
        //    }
        //}


        //[Authorize]
        //[HttpPost]
        //[Route("GetGahankhatInfoForTaker")]
        //public string GetGahankhatInfoForTaker([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        _logger.LogInformation("Get Gahankhat Info For Giver Request Data - " + ApplicationID);

        //        string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        ReponseType type = ReponseType.Success;
        //        int UserID = 0;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }
        //        ApplicationDTL applicationDTL = new ApplicationDTL();
        //        applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
        //        if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
        //        {
        //            string[] mutationgiverIDs = applicationDTL.mutationgiverIDs.Split(",");
        //            List<FetchGahankhatForGiverData> fetchDataList = new List<FetchGahankhatForGiverData>();
        //            if (mutationgiverIDs.Length > 0)
        //            {
        //                for (int i = 0; i < mutationgiverIDs.Length; i++)
        //                {
        //                    FetchGahankhatForGiverData fetchData = new FetchGahankhatForGiverData();
        //                    fetchData = mutationServices.FetchGahankhatInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //                    if (fetchData != null)
        //                    {
        //                        fetchDataList.Add(fetchData);
        //                    }
        //                }
        //                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Data Found", fetchDataList))));
        //            }
        //            else
        //            {
        //                type = ReponseType.NotFound;
        //                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Data Not Found", fetchDataList))));
        //            }
        //        }
        //        else
        //        {
        //            type = ReponseType.NotFound;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", null))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Get Gahankhat Info For Giver Exception - " + ex.StackTrace!.ToString());
        //        if (ex.Message.ToString() == "User Not Found")
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
        //        }
        //        else
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
        //        }
        //    }
        //}



        //public string SaveGahankhatTaker(GahankhatDataForGiver gahankhatData)
        //{
        //    MethodForFileUpload methodForFile = new MethodForFileUpload();
        //    MutationGiverTakerDTL dbTable = new MutationGiverTakerDTL();
        //    string FolderPath = @"D:\WWW\MUTATIONDOCS\" + gahankhatData.applicationid + @"\GIVER";
        //    using (var scope = new TransactionScope())
        //    {
        //        try
        //        {
        //            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //            // Assign Values to Model
        //            GahankhatModel gahankhatModel = new GahankhatModel();
        //            UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == gahankhatData.userid)!;
        //            gahankhatModel.userMaster = userMaster;

        //            ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == gahankhatData.applicationid)!;
        //            gahankhatModel.applicationDTL = applicationDTL;

        //            //Added By Kajal
        //            PropertyTypeMaster proptype = _context.propertyTypes.FirstOrDefault(s => s.propertytypeid == 0)!;
        //            gahankhatModel.prop_type = proptype;

        //            string nabhu = System.Text.RegularExpressions.Regex.Replace(gahankhatData.userDetails!.nabhu!, @"\s*\(.*?\)", "").Trim();
        //            MutationCTSNoDTL mutation = _context.mutationCTSNoDTLs.FirstOrDefault(s => s.selected_city_servey_no == nabhu && s.applicationDTL!.applicationid == gahankhatData.applicationid)!;
        //            gahankhatModel.mutation_cts_no_id = mutation.mutation_cts_no_id;

        //            gahankhatModel.address_type = gahankhatData.address!.addressType!.Trim().ToUpper();
        //            if (gahankhatModel.address_type == "INDIA")
        //            {
        //                gahankhatModel.flatno_plotno = gahankhatData.address.indiaAddress!.plotNo;
        //                gahankhatModel.societyname = gahankhatData.address.indiaAddress.building;
        //                gahankhatModel.mainstreet = gahankhatData.address.indiaAddress.mainRoad;
        //                gahankhatModel.landmark = gahankhatData.address.indiaAddress.impSymbol;
        //                gahankhatModel.locality = gahankhatData.address.indiaAddress.area;
        //                gahankhatModel.mobileno = gahankhatData.address.indiaAddress.mobile;
        //                gahankhatModel.mobilenoverified = gahankhatData.address.indiaAddress.mobileOTP;
        //                gahankhatModel.pincode = gahankhatData.address.indiaAddress.pincode;
        //                gahankhatModel.post_office_name = gahankhatData.address.indiaAddress.postOfficeName;
        //                gahankhatModel.city = gahankhatData.address.indiaAddress.city;
        //                gahankhatModel.taluka = gahankhatData.address.indiaAddress.taluka;
        //                gahankhatModel.district = gahankhatData.address.indiaAddress.district;
        //                gahankhatModel.state = gahankhatData.address.indiaAddress.state;
        //                gahankhatModel.emailid = "NA";
        //                gahankhatModel.emailidverified = "NA";
        //                gahankhatModel.address = "NA";
        //            }
        //            else if (gahankhatModel.address_type == "FOREIGN")
        //            {
        //                gahankhatModel.address = gahankhatData.address.foreignAddress!.address;
        //                gahankhatModel.mobileno = gahankhatData.address.foreignAddress.mobile;
        //                gahankhatModel.emailid = gahankhatData.address.foreignAddress.email;
        //                gahankhatModel.emailidverified = gahankhatData.address.foreignAddress.emailOTP;

        //                gahankhatModel.state = "NA";
        //                gahankhatModel.district = "NA";
        //                gahankhatModel.taluka = "NA";
        //                gahankhatModel.city = "NA";
        //                gahankhatModel.flatno_plotno = "NA";
        //                gahankhatModel.societyname = "NA";
        //                gahankhatModel.mainstreet = "NA";
        //                gahankhatModel.landmark = "NA";
        //                gahankhatModel.locality = "NA";
        //                gahankhatModel.pincode = "NA";
        //                gahankhatModel.post_office_name = "NA";
        //            }
        //            gahankhatModel.prefixcode_marathi = gahankhatData.userDetails!.suffixcode;
        //            gahankhatModel.prefixcode_eng = gahankhatData.userDetails!.suffixCodeEng;
        //            gahankhatModel.prefix_in_eng = gahankhatData.userDetails!.suffixEng;
        //            gahankhatModel.fname_in_eng = gahankhatData.userDetails.firstNameEng;
        //            gahankhatModel.mname_in_eng = gahankhatData.userDetails.middleNameEng;
        //            gahankhatModel.lname_in_eng = gahankhatData.userDetails.lastNameEng;
        //            gahankhatModel.prefix_in_marathi = gahankhatData.userDetails.suffix;
        //            gahankhatModel.fname_in_marathi = gahankhatData.userDetails.firstName;
        //            gahankhatModel.mname_in_marathi = gahankhatData.userDetails.middleName;
        //            gahankhatModel.lname_in_marathi = gahankhatData.userDetails.lastName;
        //            gahankhatModel.alias_name = gahankhatData.userDetails.aliceName;
        //            gahankhatModel.mother_name_in_marathi = gahankhatData.userDetails.motherName;
        //            gahankhatModel.mother_name_in_eng = gahankhatData.userDetails.motherNameEng;
        //            gahankhatModel.userName = gahankhatData.userDetails.userName;
        //            gahankhatModel.city_servey_no = gahankhatData.userDetails.nabhu;
        //            gahankhatModel.lr_property_id = gahankhatData.userDetails.lrPropertyUID;
        //            gahankhatModel.milkat = gahankhatData.userDetails.milkat;
        //            gahankhatModel.namud = gahankhatData.userDetails.namud;
        //            gahankhatModel.sub_property_no = gahankhatData.userDetails.subPropNo;
        //            gahankhatModel.isFullAreaGiven = gahankhatData.areaForMutation!.isFullAreaGiven;
        //            gahankhatModel.actual_area = gahankhatData.areaForMutation.actualArea;
        //            gahankhatModel.mutation_area = gahankhatData.areaForMutation.mutationArea;
        //            gahankhatModel.holder_type = gahankhatData.userDetails.holderType;
        //            gahankhatModel.available_area = gahankhatData.areaForMutation.availableArea;
        //            gahankhatModel.dob = gahankhatData.userDetails.dob;

        //            //Assign Data to Table fields to insert new records
        //            // Set Default Values
        //            dbTable.isTaker = 0;
        //            dbTable.prefixcode_eng = "0";
        //            dbTable.prefixcode_marathi = "0";
        //            dbTable.sub_property_no = "999999";
        //            dbTable.mobileno = "NA";
        //            dbTable.mobilenoverified = "NA";
        //            dbTable.emailid = "NA";
        //            dbTable.emailidverified = "FALSE";
        //            dbTable.prefix_in_marathi = "NA";
        //            dbTable.fname_in_marathi = "NA";
        //            dbTable.mname_in_marathi = "NA";
        //            dbTable.lname_in_marathi = "NA";
        //            dbTable.prefix_in_eng = "NA";
        //            dbTable.fname_in_eng = "NA";
        //            dbTable.mname_in_eng = "NA";
        //            dbTable.lname_in_eng = "NA";
        //            dbTable.alias_name = "NA";
        //            dbTable.owner_status_code = "NA";
        //            dbTable.owner_status_description = "NA";
        //            dbTable.dob = "NA";
        //            dbTable.mother_name_in_marathi = "NA";
        //            dbTable.mother_name_in_eng = "NA";
        //            dbTable.userName = "NA";
        //            dbTable.city_servey_no = "NA";
        //            dbTable.lr_property_id = "NA";
        //            dbTable.milkat = "NA";
        //            dbTable.namud = "NA";
        //            dbTable.isFullAreaGiven = "NA";
        //            dbTable.actual_area = "NA";
        //            dbTable.mutation_area = "NA";
        //            dbTable.available_area = "NA";
        //            dbTable.address_type = "NA";
        //            dbTable.address = "NA";
        //            dbTable.flatno_plotno = "NA";
        //            dbTable.societyname = "NA";
        //            dbTable.mainstreet = "NA";
        //            dbTable.landmark = "NA";
        //            dbTable.locality = "NA";
        //            dbTable.pincode = "NA";
        //            dbTable.post_office_name = "NA";
        //            dbTable.city = "NA";
        //            dbTable.taluka = "NA";
        //            dbTable.district = "NA";
        //            dbTable.state = "NA";
        //            dbTable.address_proof_document_name = "NA";
        //            dbTable.address_proof_document_path = "NA";
        //            dbTable.signed_file_name = "NA";
        //            dbTable.signed_file_path = "NA";
        //            dbTable.user_type = "NA";
        //            dbTable.profile_pic_file_name = "NA";
        //            dbTable.profile_pic_file_path = "NA";
        //            dbTable.has_property = "NA";
        //            //dbTable.khata_type = "NA";
        //            dbTable.company_name_in_marathi = "NA";
        //            dbTable.company_name_in_eng = "NA";
        //            //dbTable.aapak_dropdown = "NA";
        //            dbTable.aapak = "NA";
        //            dbTable.land_buy_area = "NA";
        //            dbTable.mutation_area = "NA";
        //            dbTable.account_type_code = 0;
        //            dbTable.account_type_description = "NA";
        //            dbTable.apk_code = 0;
        //            dbTable.apk_description = "NA";
        //            dbTable.khata_type_code = "NA";
        //            dbTable.khata_type_name = "NA";
        //            dbTable.owner_status_code = "NA";
        //            dbTable.owner_status_description = "NA";
        //            dbTable.khatano = "NA";
        //            dbTable.ulpin = "NA";
        //            dbTable.district_code = "NA";
        //            dbTable.district_name_in_marathi = "NA";
        //            dbTable.district_name_in_eng = "NA";
        //            dbTable.ofc_code = "NA";
        //            dbTable.ofc_name = "NA";
        //            dbTable.village_code = "NA";
        //            dbTable.village_name = "NA";
        //            dbTable.actual_area = "NA";
        //            dbTable.mutation_area = "NA";
        //            dbTable.holder_type = "NA";
        //            dbTable.owner_status_code = "NA";
        //            dbTable.owner_status_description = "NA";
        //            dbTable.account_type_code = 0;
        //            dbTable.account_type_description = "NA";
        //            dbTable.varas_relation_code = 0;
        //            dbTable.varas_relation_name = "NA";
        //            dbTable.relation_code = 0;
        //            dbTable.relation_name = "NA";
        //            dbTable.holder_type = "NA";
        //            dbTable.gender_code = "NA";
        //            dbTable.gender_description = "NA";

        //            //Set Actual Values
        //            dbTable.userMaster = gahankhatModel.userMaster;
        //            dbTable.applicationDTL = gahankhatModel.applicationDTL;
        //            dbTable.prop_type = gahankhatModel.prop_type;
        //            dbTable.mutation_cts_no_id = gahankhatModel.mutation_cts_no_id;
        //            dbTable.prefixcode_marathi = gahankhatModel.prefixcode_marathi == "" || gahankhatModel.prefixcode_marathi == null ? "0" : gahankhatModel.prefixcode_marathi;
        //            dbTable.prefixcode_eng = gahankhatModel.prefixcode_eng == "" || gahankhatModel.prefixcode_eng == null ? "0" : gahankhatModel.prefixcode_eng;
        //            //dbTable.prefixcode_eng = gahankhatModel.prefixcode_eng;
        //            //dbTable.prefixcode_marathi = gahankhatModel.prefixcode_marathi!;
        //            dbTable.prefix_in_eng = gahankhatModel.prefix_in_eng;
        //            dbTable.fname_in_eng = textInfo.ToTitleCase(gahankhatModel.fname_in_eng!.Trim());
        //            dbTable.mname_in_eng = (gahankhatModel.mname_in_eng == null || gahankhatModel.mname_in_eng == "") ? "NA" : textInfo.ToTitleCase(gahankhatModel.mname_in_eng).Trim();
        //            dbTable.lname_in_eng = (gahankhatModel.lname_in_eng == null || gahankhatModel.lname_in_eng == "") ? "NA" : textInfo.ToTitleCase(gahankhatModel.lname_in_eng).Trim();
        //            dbTable.prefix_in_marathi = gahankhatModel.prefix_in_marathi;
        //            dbTable.fname_in_marathi = gahankhatModel.fname_in_marathi;
        //            dbTable.mname_in_marathi = (gahankhatModel.mname_in_marathi == null || gahankhatModel.mname_in_marathi == "") ? "NA" : gahankhatModel.mname_in_marathi.Trim();
        //            dbTable.lname_in_marathi = (gahankhatModel.lname_in_marathi == null || gahankhatModel.lname_in_marathi == "") ? "NA" : gahankhatModel.lname_in_marathi.Trim();
        //            dbTable.alias_name = gahankhatModel.alias_name;
        //            dbTable.dob = gahankhatModel.dob;
        //            dbTable.mother_name_in_marathi = gahankhatModel.mother_name_in_marathi!.Trim();
        //            dbTable.mother_name_in_eng = textInfo.ToTitleCase(gahankhatModel.mother_name_in_eng!.Trim());
        //            dbTable.userName = gahankhatModel.userName;
        //            dbTable.city_servey_no = gahankhatModel.city_servey_no;
        //            dbTable.lr_property_id = gahankhatModel.lr_property_id;
        //            dbTable.milkat = gahankhatModel.milkat;
        //            dbTable.namud = gahankhatModel.namud;
        //            dbTable.sub_property_no = gahankhatModel.sub_property_no;
        //            dbTable.isFullAreaGiven = gahankhatModel.isFullAreaGiven;
        //            dbTable.actual_area = gahankhatModel.actual_area;
        //            dbTable.mutation_area = gahankhatModel.mutation_area;
        //            dbTable.available_area = gahankhatModel.available_area;
        //            dbTable.flatno_plotno = gahankhatModel.flatno_plotno;
        //            dbTable.societyname = gahankhatModel.societyname;
        //            dbTable.mainstreet = gahankhatModel.mainstreet;
        //            dbTable.landmark = gahankhatModel.landmark;
        //            dbTable.locality = gahankhatModel.locality;
        //            dbTable.mobileno = gahankhatModel.mobileno;
        //            dbTable.mobilenoverified = string.IsNullOrEmpty(gahankhatModel.mobilenoverified) ? "NO" : gahankhatModel.mobilenoverified;
        //            dbTable.emailid = gahankhatModel.emailid;
        //            dbTable.emailidverified = gahankhatModel.emailidverified;
        //            dbTable.pincode = gahankhatModel.pincode;
        //            dbTable.post_office_name = gahankhatModel.post_office_name;
        //            dbTable.city = gahankhatModel.city;
        //            dbTable.taluka = gahankhatModel.taluka;
        //            dbTable.district = gahankhatModel.district;
        //            dbTable.state = gahankhatModel.state;
        //            dbTable.address = gahankhatModel.address;
        //            dbTable.address_type = gahankhatModel.address_type;
        //            dbTable.isTaker = 0;
        //            _context.mutationDTL.Add(dbTable);
        //            _context.SaveChanges();

        //            //Get Saved Row ID
        //            int mutation_givertaker_id = dbTable.mutation_givertaker_id;

        //            string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //            bool checkAddressFlag = true;
        //            //bool checkSignFlag = true;
        //            if (gahankhatModel.address_type == "INDIA")
        //            {
        //                if (!string.IsNullOrEmpty(gahankhatData.address.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(gahankhatData.address.indiaAddress.addressProofName))
        //                {
        //                    string[] AddressData = gahankhatData.address.indiaAddress.addressProofSrc.Split(",");
        //                    checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], gahankhatData.address.indiaAddress.addressProofName, mutation_givertaker_id.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
        //                    string AddressProofExt = Path.GetExtension(gahankhatData.address.indiaAddress.addressProofName);
        //                    gahankhatModel.address_proof_document_name = "AddressProof" + mutation_givertaker_id + "_" + CurrentDateTime + AddressProofExt;
        //                    gahankhatModel.address_proof_document_path = FolderPath + @"\" + +mutation_givertaker_id + @"\" + gahankhatModel.address_proof_document_name;

        //                    var UpdateAddressFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == mutation_givertaker_id).FirstOrDefault();
        //                    if (UpdateAddressFilePath != null)
        //                    {
        //                        dbTable.address_proof_document_name = gahankhatModel.address_proof_document_name;
        //                        dbTable.address_proof_document_path = gahankhatModel.address_proof_document_path;
        //                        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //                        _context.SaveChanges();
        //                    }
        //                }
        //                //if (!string.IsNullOrEmpty(gahankhatData.address.indiaAddress.signatureSrc))
        //                //{
        //                //    string[] signData = gahankhatData.address.indiaAddress.signatureSrc.Split(",");
        //                //    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], gahankhatData.address.indiaAddress.signatureName, mutation_givertaker_id.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
        //                //    string SignatureExt = Path.GetExtension(gahankhatData.address.indiaAddress.signatureName);
        //                //    gahankhatModel.signed_file_name = "Signature" + mutation_givertaker_id + "_" + CurrentDateTime + SignatureExt;
        //                //    gahankhatModel.signed_file_path = FolderPath + @"\" + mutation_givertaker_id + @"\" + gahankhatModel.signed_file_name;

        //                //    var UpdateSignFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == mutation_givertaker_id).FirstOrDefault();
        //                //    if (UpdateSignFilePath != null)
        //                //    {
        //                //        dbTable.signed_file_name = gahankhatModel.signed_file_name;
        //                //        dbTable.signed_file_path = gahankhatModel.signed_file_path;
        //                //        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //                //        _context.SaveChanges();
        //                //    }
        //                //}
        //            }
        //            //if (gahankhatModel.address_type == "FOREIGN")
        //            //{
        //            //    if (!string.IsNullOrEmpty(gahankhatData.address.foreignAddress.signatureSrc))
        //            //    {
        //            //        string[] signData = gahankhatData.address.foreignAddress.signatureSrc.Split(",");
        //            //        checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], gahankhatData.address.foreignAddress.signatureName, mutation_givertaker_id.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

        //            //        string SignatureExt = Path.GetExtension(gahankhatData.address.foreignAddress.signatureName);
        //            //        gahankhatModel.signed_file_name = "Signature" + mutation_givertaker_id + "_" + CurrentDateTime + SignatureExt;
        //            //        gahankhatModel.signed_file_path = FolderPath + @"\" + mutation_givertaker_id + @"\" + gahankhatModel.signed_file_name;
        //            //        var UpdateSignFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == mutation_givertaker_id).FirstOrDefault();
        //            //        if (UpdateSignFilePath != null)
        //            //        {
        //            //            dbTable.signed_file_name = gahankhatModel.signed_file_name;
        //            //            dbTable.signed_file_path = gahankhatModel.signed_file_path;
        //            //            _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //            //            _context.SaveChanges();
        //            //        }
        //            //    }
        //            //}
        //            if (checkAddressFlag)
        //            {
        //                var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(gahankhatData.applicationid)).FirstOrDefault();
        //                if (applicationDTLdata != null)
        //                {
        //                    if (!string.IsNullOrEmpty(applicationDTLdata.mutationgiverIDs) && !applicationDTLdata.mutationgiverIDs.Contains(mutation_givertaker_id.ToString()))
        //                    {
        //                        applicationDTLdata.mutationgiverIDs = applicationDTLdata.mutationgiverIDs + "," + mutation_givertaker_id.ToString();
        //                    }
        //                    else
        //                    {
        //                        applicationDTLdata.mutationgiverIDs = mutation_givertaker_id.ToString();
        //                    }
        //                    _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
        //                    _context.SaveChanges();
        //                }
        //                scope.Complete();
        //                return "Success";
        //            }
        //            else
        //            {
        //                _context.mutationDTL.Remove(dbTable);
        //                _context.SaveChanges();
        //                return "Address Proof File Is Not Uploaded";
        //            }
        //            //if (!checkAddressFlag)
        //            //{
        //            //    _context.mutationDTL.Remove(dbTable);
        //            //    _context.SaveChanges();
        //            //    return "Address Proof File Is Not Uploaded";
        //            //}
        //            //if (!checkSignFlag)
        //            //{
        //            //    _context.mutationDTL.Remove(dbTable);
        //            //    _context.SaveChanges();
        //            //    return "Signature File Is Not Uploaded";
        //            //}
        //            //else
        //            //{
        //            //    _context.mutationDTL.Remove(dbTable);
        //            //    _context.SaveChanges();
        //            //    return "Some Files Are Not Uploaded";
        //            //}
        //        }
        //        catch (Exception ex)
        //        {
        //            //_context.mutationDTL.Remove(dbTable);
        //            //_context.SaveChanges();
        //            throw new HandleException(ex.Message.ToString());
        //        }
        //    }
        //}

        //public string EditGahankhatTaker(EditGahankhatDataForGiver gahankhatData)
        //{
        //    var entity = _context.mutationDTL.FirstOrDefault(s => s.mutation_givertaker_id == gahankhatData.MutationId!)!;
        //    MethodForFileUpload methodForFile = new MethodForFileUpload();
        //    // MutationGiverTakerDTL dbTable = new MutationGiverTakerDTL();
        //    string FolderPath = @"D:\WWW\MUTATIONDOCS\" + gahankhatData.applicationid + @"\GIVER";
        //    if (entity != null)
        //    {
        //        using (var dbContextTransaction = _context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //                // Assign Values to Model
        //                GahankhatModel gahankhatModel = new GahankhatModel();
        //                UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == gahankhatData.userid)!;
        //                gahankhatModel.userMaster = userMaster;

        //                ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == gahankhatData.applicationid)!;
        //                gahankhatModel.applicationDTL = applicationDTL;

        //                gahankhatModel.address_type = gahankhatData.address!.addressType!.Trim().ToUpper();
        //                if (gahankhatModel.address_type == "INDIA")
        //                {
        //                    gahankhatModel.flatno_plotno = gahankhatData.address.indiaAddress!.plotNo;
        //                    gahankhatModel.societyname = gahankhatData.address.indiaAddress.building;
        //                    gahankhatModel.mainstreet = gahankhatData.address.indiaAddress.mainRoad;
        //                    gahankhatModel.landmark = gahankhatData.address.indiaAddress.impSymbol;
        //                    gahankhatModel.locality = gahankhatData.address.indiaAddress.area;
        //                    gahankhatModel.mobileno = gahankhatData.address.indiaAddress.mobile;
        //                    gahankhatModel.mobilenoverified = gahankhatData.address.indiaAddress.mobileOTP;
        //                    gahankhatModel.pincode = gahankhatData.address.indiaAddress.pincode;
        //                    gahankhatModel.post_office_name = gahankhatData.address.indiaAddress.postOfficeName;
        //                    gahankhatModel.city = gahankhatData.address.indiaAddress.city;
        //                    gahankhatModel.taluka = gahankhatData.address.indiaAddress.taluka;
        //                    gahankhatModel.district = gahankhatData.address.indiaAddress.district;
        //                    gahankhatModel.state = gahankhatData.address.indiaAddress.state;
        //                    gahankhatModel.emailid = "NA";
        //                    gahankhatModel.emailidverified = "NA";
        //                    gahankhatModel.address = "NA";
        //                }
        //                else if (gahankhatModel.address_type == "FOREIGN")
        //                {
        //                    gahankhatModel.address = gahankhatData.address.foreignAddress!.address;
        //                    gahankhatModel.mobileno = gahankhatData.address.foreignAddress.mobile;
        //                    gahankhatModel.emailid = gahankhatData.address.foreignAddress.email;
        //                    gahankhatModel.emailidverified = gahankhatData.address.foreignAddress.emailOTP;

        //                    gahankhatModel.state = "NA";
        //                    gahankhatModel.district = "NA";
        //                    gahankhatModel.taluka = "NA";
        //                    gahankhatModel.city = "NA";
        //                    gahankhatModel.flatno_plotno = "NA";
        //                    gahankhatModel.societyname = "NA";
        //                    gahankhatModel.mainstreet = "NA";
        //                    gahankhatModel.landmark = "NA";
        //                    gahankhatModel.locality = "NA";
        //                    gahankhatModel.pincode = "NA";
        //                    gahankhatModel.post_office_name = "NA";
        //                }
        //                gahankhatModel.prefixcode_marathi = gahankhatData.userDetails!.suffixcode;
        //                gahankhatModel.prefixcode_eng = gahankhatData.userDetails!.suffixCodeEng;
        //                gahankhatModel.prefix_in_eng = gahankhatData.userDetails!.suffixEng;
        //                gahankhatModel.fname_in_eng = gahankhatData.userDetails.firstNameEng;
        //                gahankhatModel.mname_in_eng = gahankhatData.userDetails.middleNameEng;
        //                gahankhatModel.lname_in_eng = gahankhatData.userDetails.lastNameEng;
        //                gahankhatModel.prefix_in_marathi = gahankhatData.userDetails.suffix;
        //                gahankhatModel.fname_in_marathi = gahankhatData.userDetails.firstName;
        //                gahankhatModel.mname_in_marathi = gahankhatData.userDetails.middleName;
        //                gahankhatModel.lname_in_marathi = gahankhatData.userDetails.lastName;
        //                gahankhatModel.alias_name = gahankhatData.userDetails.aliceName;
        //                gahankhatModel.mother_name_in_marathi = gahankhatData.userDetails.motherName;
        //                gahankhatModel.mother_name_in_eng = gahankhatData.userDetails.motherNameEng;
        //                gahankhatModel.userName = gahankhatData.userDetails.userName;
        //                gahankhatModel.city_servey_no = gahankhatData.userDetails.nabhu;
        //                gahankhatModel.lr_property_id = gahankhatData.userDetails.lrPropertyUID;
        //                gahankhatModel.milkat = gahankhatData.userDetails.milkat;
        //                gahankhatModel.namud = gahankhatData.userDetails.namud;
        //                gahankhatModel.sub_property_no = gahankhatData.userDetails.subPropNo;
        //                gahankhatModel.isFullAreaGiven = gahankhatData.areaForMutation!.isFullAreaGiven;
        //                gahankhatModel.actual_area = gahankhatData.areaForMutation.actualArea;
        //                gahankhatModel.mutation_area = gahankhatData.areaForMutation.mutationArea;
        //                gahankhatModel.holder_type = gahankhatData.userDetails.holderType;
        //                gahankhatModel.available_area = gahankhatData.areaForMutation.availableArea;
        //                gahankhatModel.dob = gahankhatData.userDetails.dob;

        //                //Assign Data to Table fields to insert new records
        //                // Set Default Values
        //                /*dbTable.isTaker = 0;
        //                dbTable.mobileno = "NA";
        //                dbTable.mobilenoverified = "NA";
        //                dbTable.emailid = "NA";
        //                dbTable.emailidverified = "FALSE";
        //                dbTable.prefix_in_marathi = "NA";
        //                dbTable.fname_in_marathi = "NA";
        //                dbTable.mname_in_marathi = "NA";
        //                dbTable.lname_in_marathi = "NA";
        //                dbTable.prefix_in_eng = "NA";
        //                dbTable.fname_in_eng = "NA";
        //                dbTable.mname_in_eng = "NA";
        //                dbTable.lname_in_eng = "NA";
        //                dbTable.alias_name = "NA";
        //                dbTable.owner_status_code = "NA";
        //                dbTable.owner_status_description = "NA";
        //                dbTable.dob = "NA";
        //                dbTable.mother_name_in_marathi = "NA";
        //                dbTable.mother_name_in_eng = "NA";
        //                dbTable.userName = "NA";
        //                dbTable.city_servey_no = "NA";
        //                dbTable.lr_property_id = "NA";
        //                dbTable.milkat = "NA";
        //                dbTable.namud = "NA";
        //                dbTable.isFullAreaGiven = "NA";
        //                dbTable.actual_area = "NA";
        //                dbTable.mutation_area = "NA";
        //                dbTable.available_area = "NA";
        //                dbTable.address_type = "NA";
        //                dbTable.address = "NA";
        //                dbTable.flatno_plotno = "NA";
        //                dbTable.societyname = "NA";
        //                dbTable.mainstreet = "NA";
        //                dbTable.landmark = "NA";
        //                dbTable.locality = "NA";
        //                dbTable.pincode = "NA";
        //                dbTable.post_office_name = "NA";
        //                dbTable.city = "NA";
        //                dbTable.taluka = "NA";
        //                dbTable.district = "NA";
        //                dbTable.state = "NA";
        //                dbTable.address_proof_document_name = "NA";
        //                dbTable.address_proof_document_path = "NA";
        //                dbTable.signed_file_name = "NA";
        //                dbTable.signed_file_path = "NA";
        //                dbTable.user_type = "NA";
        //                dbTable.profile_pic_file_name = "NA";
        //                dbTable.profile_pic_file_path = "NA";
        //                dbTable.has_property = "NA";
        //                dbTable.gender = "NA";
        //                //dbTable.khata_type = "NA";
        //                dbTable.company_name_in_marathi = "NA";
        //                dbTable.company_name_in_eng = "NA";
        //                //dbTable.aapak_dropdown = "NA";
        //                dbTable.aapak = "NA";
        //                dbTable.land_buy_area = "NA";
        //                dbTable.gift_area = "NA";
        //                dbTable.account_type_code = 0;
        //                dbTable.account_type_description = "NA";
        //                dbTable.apk_code = 0;
        //                dbTable.apk_description = "NA";
        //                dbTable.khata_type_code = "NA";
        //                dbTable.khata_type_name = "NA";
        //                dbTable.owner_status_code = "NA";
        //                dbTable.owner_status_description = "NA";
        //                dbTable.khatano = "NA";
        //                dbTable.ulpin = "NA";
        //                dbTable.district_code = "NA";
        //                dbTable.district_name_in_marathi = "NA";
        //                dbTable.district_name_in_eng = "NA";
        //                dbTable.ofc_code = "NA";
        //                dbTable.ofc_name = "NA";
        //                dbTable.village_code = "NA";
        //                dbTable.village_name = "NA";
        //                dbTable.actualArea = "NA";
        //                dbTable.benefitArea = "NA";
        //                dbTable.holder_type = "NA";*/

        //                //Set Actual Values
        //                entity.userMaster = gahankhatModel.userMaster;
        //                entity.applicationDTL = gahankhatModel.applicationDTL;
        //                entity.prefixcode_eng = gahankhatModel.prefixcode_eng;
        //                entity.prefixcode_marathi = gahankhatModel.prefixcode_marathi!;
        //                entity.prefix_in_eng = gahankhatModel.prefix_in_eng;
        //                entity.fname_in_eng = textInfo.ToTitleCase(gahankhatModel.fname_in_eng!.Trim());
        //                entity.mname_in_eng = (gahankhatModel.mname_in_eng == null || gahankhatModel.mname_in_eng == "") ? "NA" : textInfo.ToTitleCase(gahankhatModel.mname_in_eng).Trim();
        //                entity.lname_in_eng = (gahankhatModel.lname_in_eng == null || gahankhatModel.lname_in_eng == "") ? "NA" : textInfo.ToTitleCase(gahankhatModel.lname_in_eng).Trim();
        //                entity.prefix_in_marathi = gahankhatModel.prefix_in_marathi;
        //                entity.fname_in_marathi = gahankhatModel.fname_in_marathi;
        //                entity.mname_in_marathi = (gahankhatModel.mname_in_marathi == null || gahankhatModel.mname_in_marathi == "") ? "NA" : gahankhatModel.mname_in_marathi.Trim();
        //                entity.lname_in_marathi = (gahankhatModel.lname_in_marathi == null || gahankhatModel.lname_in_marathi == "") ? "NA" : gahankhatModel.lname_in_marathi.Trim();
        //                entity.alias_name = gahankhatModel.alias_name;
        //                entity.dob = gahankhatModel.dob;
        //                entity.mother_name_in_marathi = gahankhatModel.mother_name_in_marathi!.Trim();
        //                entity.mother_name_in_eng = textInfo.ToTitleCase(gahankhatModel.mother_name_in_eng!.Trim());
        //                entity.userName = gahankhatModel.userName;
        //                entity.city_servey_no = gahankhatModel.city_servey_no;
        //                entity.lr_property_id = gahankhatModel.lr_property_id;
        //                entity.milkat = gahankhatModel.milkat;
        //                entity.namud = gahankhatModel.namud;
        //                entity.sub_property_no = gahankhatModel.sub_property_no;
        //                entity.isFullAreaGiven = gahankhatModel.isFullAreaGiven;
        //                entity.actual_area = gahankhatModel.actual_area;
        //                entity.mutation_area = gahankhatModel.mutation_area;
        //                entity.available_area = gahankhatModel.available_area;
        //                entity.flatno_plotno = gahankhatModel.flatno_plotno;
        //                entity.societyname = gahankhatModel.societyname;
        //                entity.mainstreet = gahankhatModel.mainstreet;
        //                entity.landmark = gahankhatModel.landmark;
        //                entity.locality = gahankhatModel.locality;
        //                entity.mobileno = gahankhatModel.mobileno;
        //                entity.mobilenoverified = string.IsNullOrEmpty(gahankhatModel.mobilenoverified) ? "NO" : gahankhatModel.mobilenoverified;
        //                entity.emailid = gahankhatModel.emailid;
        //                entity.emailidverified = gahankhatModel.emailidverified;
        //                entity.pincode = gahankhatModel.pincode;
        //                entity.post_office_name = gahankhatModel.post_office_name;
        //                entity.city = gahankhatModel.city;
        //                entity.taluka = gahankhatModel.taluka;
        //                entity.district = gahankhatModel.district;
        //                entity.state = gahankhatModel.state;
        //                entity.address = gahankhatModel.address;
        //                entity.address_type = gahankhatModel.address_type;
        //                entity.isTaker = 0;
        //                _context.mutationDTL.Attach(entity);
        //                _context.SaveChanges();

        //                //Get Saved Row ID
        //                int mutation_givertaker_id = entity.mutation_givertaker_id;

        //                string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //                bool checkAddressFlag = true;
        //                // bool checkSignFlag = true;
        //                if (gahankhatModel.address_type == "INDIA")
        //                {
        //                    if (!string.IsNullOrEmpty(gahankhatData.address.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(gahankhatData.address.indiaAddress!.addressProofName))
        //                    {
        //                        string[] AddressData = gahankhatData.address.indiaAddress.addressProofSrc!.Split(",");
        //                        checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], gahankhatData.address.indiaAddress.addressProofName!, mutation_givertaker_id.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
        //                        string AddressProofExt = Path.GetExtension(gahankhatData.address.indiaAddress.addressProofName!);
        //                        gahankhatModel.address_proof_document_name = "AddressProof" + mutation_givertaker_id + "_" + CurrentDateTime + AddressProofExt;
        //                        gahankhatModel.address_proof_document_path = FolderPath + @"\" + +mutation_givertaker_id + @"\" + gahankhatModel.address_proof_document_name;

        //                        var UpdateAddressFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == mutation_givertaker_id).FirstOrDefault();
        //                        if (UpdateAddressFilePath != null)
        //                        {
        //                            entity.address_proof_document_name = gahankhatModel.address_proof_document_name;
        //                            entity.address_proof_document_path = gahankhatModel.address_proof_document_path;
        //                            _context.Entry(entity).CurrentValues.SetValues(entity);
        //                            _context.SaveChanges();
        //                        }
        //                    }
        //                    //if (!string.IsNullOrEmpty(gahankhatData.address.indiaAddress.signatureSrc))
        //                    //{
        //                    //    string[] signData = gahankhatData.address.indiaAddress.signatureSrc.Split(",");
        //                    //    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], gahankhatData.address.indiaAddress.signatureName, mutation_givertaker_id.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
        //                    //    string SignatureExt = Path.GetExtension(gahankhatData.address.indiaAddress.signatureName);
        //                    //    gahankhatModel.signed_file_name = "Signature" + mutation_givertaker_id + "_" + CurrentDateTime + SignatureExt;
        //                    //    gahankhatModel.signed_file_path = FolderPath + @"\" + mutation_givertaker_id + @"\" + gahankhatModel.signed_file_name;

        //                    //    var UpdateSignFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == mutation_givertaker_id).FirstOrDefault();
        //                    //    if (UpdateSignFilePath != null)
        //                    //    {
        //                    //        entity.signed_file_name = gahankhatModel.signed_file_name;
        //                    //        entity.signed_file_path = gahankhatModel.signed_file_path;
        //                    //        _context.Entry(entity).CurrentValues.SetValues(entity);
        //                    //        _context.SaveChanges();
        //                    //    }
        //                    //}
        //                }
        //                //if (gahankhatModel.address_type == "FOREIGN")
        //                //{
        //                //    if (!string.IsNullOrEmpty(gahankhatData.address.foreignAddress.signatureSrc))
        //                //    {
        //                //        string[] signData = gahankhatData.address.foreignAddress.signatureSrc.Split(",");
        //                //        checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], gahankhatData.address.foreignAddress.signatureName, mutation_givertaker_id.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

        //                //        string SignatureExt = Path.GetExtension(gahankhatData.address.foreignAddress.signatureName);
        //                //        gahankhatModel.signed_file_name = "Signature" + mutation_givertaker_id + "_" + CurrentDateTime + SignatureExt;
        //                //        gahankhatModel.signed_file_path = FolderPath + @"\" + mutation_givertaker_id + @"\" + gahankhatModel.signed_file_name;
        //                //        var UpdateSignFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == mutation_givertaker_id).FirstOrDefault();
        //                //        if (UpdateSignFilePath != null)
        //                //        {
        //                //            entity.signed_file_name = gahankhatModel.signed_file_name;
        //                //            entity.signed_file_path = gahankhatModel.signed_file_path;
        //                //            _context.Entry(entity).CurrentValues.SetValues(entity);
        //                //            _context.SaveChanges();
        //                //        }
        //                //    }
        //                //}
        //                dbContextTransaction.Commit();
        //                dbContextTransaction.Dispose();
        //                return "Success";
        //            }
        //            catch (Exception ex)
        //            {
        //                dbContextTransaction.Rollback();
        //                dbContextTransaction.Dispose();
        //                /*_context.mutationDTL.Remove(dbTable);
        //                _context.SaveChanges();*/
        //                throw new HandleException(ex.Message.ToString());
        //            }
        //        }
        //    }
        //    else { return "False"; }
        //}

        //public FetchGahankhatForGiverData FetchGahankhatInfoForTaker(int mutationGiverID)
        //{
        //    try
        //    {
        //        MethodForFileUpload methodForFile = new MethodForFileUpload();
        //        MutationGiverTakerDTL mutationGiverTakerDTL = new MutationGiverTakerDTL();
        //        mutationGiverTakerDTL = _context.mutationDTL.Include(i => i.userMaster).Include(app => app.applicationDTL).Where(data => data.mutation_givertaker_id.Equals(mutationGiverID) && data.isTaker == 0 && data.isDeleted == false).FirstOrDefault()!;
        //        FetchGahankhatForGiverData fetchData = new FetchGahankhatForGiverData();
        //        if (mutationGiverTakerDTL != null)
        //        {
        //            fetchData.mutation_givertaker_id = mutationGiverTakerDTL.mutation_givertaker_id;
        //            fetchData.userid = mutationGiverTakerDTL.userMaster!.userid;
        //            fetchData.applicationid = mutationGiverTakerDTL.applicationDTL!.applicationid;

        //            //fetchData.fullNameInMarathi = mutationGiverTakerDTL.fname_in_marathi!.Trim() + " " + mutationGiverTakerDTL.mname_in_marathi!.Trim() + " " + mutationGiverTakerDTL.lname_in_marathi!.Trim();
        //            //fetchData.fullNameInEng = mutationGiverTakerDTL.fname_in_eng!.Trim() + " " + mutationGiverTakerDTL.mname_in_eng!.Trim() + " " + mutationGiverTakerDTL.lname_in_eng!.Trim();
        //            fetchData.fullNameInMarathi = commonFunctions.ReplaceNA(mutationGiverTakerDTL.fname_in_marathi!.Trim()) + " " + commonFunctions.ReplaceNA(mutationGiverTakerDTL.mname_in_marathi!.Trim()) + " " + commonFunctions.ReplaceNA(mutationGiverTakerDTL.lname_in_marathi!.Trim());
        //            fetchData.fullNameInEng = commonFunctions.ReplaceNA(mutationGiverTakerDTL.fname_in_eng!.Trim()) + " " + commonFunctions.ReplaceNA(mutationGiverTakerDTL.mname_in_eng!.Trim()) + " " + commonFunctions.ReplaceNA(mutationGiverTakerDTL.lname_in_eng!.Trim());

        //            InputDataModel.GahankhatNond.UserDetailsForGahankhatGiver userDetails = new InputDataModel.GahankhatNond.UserDetailsForGahankhatGiver();
        //            userDetails.suffixcode = mutationGiverTakerDTL.prefixcode_marathi;
        //            userDetails.suffix = mutationGiverTakerDTL.prefix_in_marathi;
        //            userDetails.firstName = mutationGiverTakerDTL.fname_in_marathi;
        //            userDetails.middleName = mutationGiverTakerDTL.mname_in_marathi;
        //            userDetails.lastName = mutationGiverTakerDTL.lname_in_marathi;
        //            userDetails.suffixCodeEng = mutationGiverTakerDTL.prefixcode_eng;
        //            userDetails.suffixEng = mutationGiverTakerDTL.prefix_in_eng;
        //            userDetails.firstNameEng = mutationGiverTakerDTL.fname_in_eng;
        //            userDetails.middleNameEng = mutationGiverTakerDTL.mname_in_eng;
        //            userDetails.lastNameEng = mutationGiverTakerDTL.lname_in_eng;
        //            userDetails.nabhu = mutationGiverTakerDTL.city_servey_no;
        //            userDetails.userName = mutationGiverTakerDTL.userName;
        //            userDetails.lrPropertyUID = mutationGiverTakerDTL.lr_property_id;
        //            userDetails.aliceName = mutationGiverTakerDTL.alias_name;
        //            userDetails.holderType = mutationGiverTakerDTL.holder_type;

        //            userDetails.dob = mutationGiverTakerDTL.dob;
        //            userDetails.motherName = mutationGiverTakerDTL.mother_name_in_marathi;
        //            userDetails.motherNameEng = mutationGiverTakerDTL.mother_name_in_eng;
        //            userDetails.userName = mutationGiverTakerDTL.userName;
        //            userDetails.nabhu = mutationGiverTakerDTL.city_servey_no;
        //            userDetails.lrPropertyUID = mutationGiverTakerDTL.lr_property_id;
        //            userDetails.milkat = mutationGiverTakerDTL.milkat;
        //            userDetails.namud = mutationGiverTakerDTL.namud;
        //            userDetails.subPropNo = mutationGiverTakerDTL.sub_property_no;
        //            fetchData.userDetails = userDetails;

        //            InputDataModel.GahankhatNond.AddressDTLForGahankhatGiver addressData = new InputDataModel.GahankhatNond.AddressDTLForGahankhatGiver();
        //            addressData.addressType = mutationGiverTakerDTL.address_type;
        //            if (mutationGiverTakerDTL.address_type == "INDIA")
        //            {
        //                IndiaAddressForGahankhatGiver addressForIndia = new IndiaAddressForGahankhatGiver();
        //                addressForIndia.state = mutationGiverTakerDTL.state;
        //                addressForIndia.district = mutationGiverTakerDTL.district;
        //                addressForIndia.city = mutationGiverTakerDTL.city;
        //                addressForIndia.taluka = mutationGiverTakerDTL.taluka;
        //                addressForIndia.plotNo = mutationGiverTakerDTL.flatno_plotno;
        //                addressForIndia.building = mutationGiverTakerDTL.societyname;
        //                addressForIndia.mainRoad = mutationGiverTakerDTL.mainstreet;
        //                addressForIndia.impSymbol = mutationGiverTakerDTL.landmark;
        //                addressForIndia.area = mutationGiverTakerDTL.locality;
        //                addressForIndia.pincode = mutationGiverTakerDTL.pincode;
        //                addressForIndia.postOfficeName = mutationGiverTakerDTL.post_office_name;
        //                addressForIndia.mobile = mutationGiverTakerDTL.mobileno;
        //                addressForIndia.mobileOTP = mutationGiverTakerDTL.mobilenoverified;
        //                addressForIndia.signatureName = mutationGiverTakerDTL.signed_file_name;

        //                addressForIndia.addressProofName = mutationGiverTakerDTL.address_proof_document_name;
        //                if (mutationGiverTakerDTL.address_proof_document_path != "NA")
        //                {
        //                    string AddressProofExt = Path.GetExtension(mutationGiverTakerDTL.address_proof_document_path)!;
        //                    string AddressProof = methodForFile.ConvertImageToBase64(mutationGiverTakerDTL.address_proof_document_path!);
        //                    mutationGiverTakerDTL.address_proof_document_path = string.IsNullOrEmpty(AddressProof) ? "NA" : "data:image/" + AddressProofExt.Replace(".", "") + ";base64," + AddressProof;
        //                    addressForIndia.addressProofSrc = mutationGiverTakerDTL.address_proof_document_path;
        //                }
        //                else
        //                {
        //                    addressForIndia.addressProofSrc = mutationGiverTakerDTL.address_proof_document_path;
        //                }
        //                //string SignatureExt = Path.GetExtension(mutationGiverTakerDTL.signed_file_path);
        //                //string Signature = methodForFile.ConvertImageToBase64(mutationGiverTakerDTL.signed_file_path);
        //                //mutationGiverTakerDTL.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
        //                addressForIndia.signatureSrc = mutationGiverTakerDTL.signed_file_path;
        //                addressData.indiaAddress = addressForIndia;
        //            }
        //            else if (mutationGiverTakerDTL.address_type == "FOREIGN")
        //            {
        //                AddressForForeignForGahankhatGiver addressForForeign = new AddressForForeignForGahankhatGiver();
        //                addressForForeign.address = mutationGiverTakerDTL.address;
        //                addressForForeign.mobile = mutationGiverTakerDTL.mobileno;
        //                addressForForeign.email = mutationGiverTakerDTL.emailid;
        //                addressForForeign.emailOTP = mutationGiverTakerDTL.emailidverified;
        //                addressForForeign.signatureName = mutationGiverTakerDTL.signed_file_name;
        //                //string SignatureExt = Path.GetExtension(mutationGiverTakerDTL.signed_file_path);
        //                //string Signature = methodForFile.ConvertImageToBase64(mutationGiverTakerDTL.signed_file_path);
        //                //mutationGiverTakerDTL.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
        //                addressForForeign.signatureSrc = mutationGiverTakerDTL.signed_file_path;
        //                addressData.foreignAddress = addressForForeign;
        //            }
        //            fetchData.address = addressData;

        //            AreaForMutationForGahankhatGiver areaForMutation = new AreaForMutationForGahankhatGiver();
        //            areaForMutation.isFullAreaGiven = mutationGiverTakerDTL.isFullAreaGiven;
        //            areaForMutation.actualArea = mutationGiverTakerDTL.actual_area;
        //            areaForMutation.mutationArea = mutationGiverTakerDTL.mutation_area;
        //            areaForMutation.availableArea = mutationGiverTakerDTL.available_area;
        //            fetchData.areaForMutation = areaForMutation;
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

        //public string SaveGahankhatGiver(GahankhatDataForTaker takerData)
        //{
        //    using (var scope = new TransactionScope())
        //    {
        //        MethodForFileUpload methodForFile = new MethodForFileUpload();
        //        MutationGiverTakerDTL dbTable = new MutationGiverTakerDTL();
        //        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //        string FolderPath = @"D:\WWW\MUTATIONDOCS\" + takerData.applicationid + @"\TAKER";
        //        try
        //        {
        //            // Assign values to model
        //            GahankhatModel dataModel = new GahankhatModel();
        //            dataModel.mutationSroNo = takerData.mutationSroNo;
        //            dataModel.ownerNo = takerData.ownerNo;
        //            dataModel.address_type = takerData.address!.addressType!.Trim().ToUpper();
        //            if (takerData.address.addressType.Trim().ToUpper() == "INDIA")
        //            {
        //                dataModel.state = takerData.address.indiaAddress!.state;
        //                dataModel.district = takerData.address.indiaAddress.district;
        //                dataModel.city = takerData.address.indiaAddress.city;
        //                dataModel.taluka = takerData.address.indiaAddress.taluka;
        //                dataModel.flatno_plotno = takerData.address.indiaAddress.plotNo;
        //                dataModel.societyname = takerData.address.indiaAddress.building;
        //                dataModel.mainstreet = takerData.address.indiaAddress.mainRoad;
        //                dataModel.landmark = takerData.address.indiaAddress.impSymbol;
        //                dataModel.locality = takerData.address.indiaAddress.area;
        //                dataModel.pincode = takerData.address.indiaAddress.pincode;
        //                dataModel.post_office_name = takerData.address.indiaAddress.postOfficeName;
        //                dataModel.mobileno = takerData.address.indiaAddress.mobile;
        //                dataModel.mobilenoverified = takerData.address.indiaAddress.mobileOTP!.Trim().ToUpper();
        //                dataModel.emailid = "NA";
        //                dataModel.emailidverified = "NA";
        //                dataModel.address = "NA";
        //            }
        //            else if (takerData.address.addressType.Trim().ToUpper() == "FOREIGN")
        //            {
        //                dataModel.address = takerData.address.foreignAddress!.address;
        //                dataModel.mobileno = takerData.address.foreignAddress.mobile;
        //                dataModel.mobilenoverified = "NO";
        //                dataModel.emailid = takerData.address.foreignAddress.email;
        //                dataModel.emailidverified = takerData.address.foreignAddress.emailOTP!.Trim().ToUpper();

        //                dataModel.state = "NA";
        //                dataModel.district = "NA";
        //                dataModel.city = "NA";
        //                dataModel.taluka = "NA";
        //                dataModel.flatno_plotno = "NA";
        //                dataModel.societyname = "NA";
        //                dataModel.mainstreet = "NA";
        //                dataModel.landmark = "NA";
        //                dataModel.locality = "NA";
        //                dataModel.pincode = "NA";
        //                dataModel.post_office_name = "NA";
        //            }

        //            ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == takerData.applicationid)!;
        //            dataModel.applicationDTL = applicationDTL;

        //            UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == takerData.userid)!;
        //            dataModel.userMaster = userMaster;

        //            //Added By Kajal
        //            PropertyTypeMaster proptype = _context.propertyTypes.FirstOrDefault(s => s.propertytypeid == 0)!;
        //            dataModel.prop_type = proptype;

        //            dataModel.institute_code = takerData.userDetails!.bankDropdown!.institute_code;
        //            dataModel.institute_description = takerData.userDetails.bankDropdown.institute_description;
        //            dataModel.bank_name_in_marathi = takerData.userDetails.bankNameMar;
        //            dataModel.bank_name_in_english = takerData.userDetails.bankNameEng;
        //            dataModel.ifsc = takerData.userDetails.ifsc;
        //            dataModel.bojaArea = takerData.userDetails.bojaArea;
        //            dataModel.bojaValue = takerData.userDetails.bojaValue;
        //            dataModel.bojaDate = takerData.userDetails.bojaDate;
        //            dataModel.bojaPeriod = takerData.userDetails.bojaPeriod;


        //            // Set Default Values
        //            dbTable.isTaker = 0;
        //            dbTable.mobileno = "NA";
        //            dbTable.mobilenoverified = "NA";
        //            dbTable.emailid = "NA";
        //            dbTable.emailidverified = "FALSE";
        //            dbTable.prefix_in_marathi = "NA";
        //            dbTable.fname_in_marathi = "NA";
        //            dbTable.mname_in_marathi = "NA";
        //            dbTable.lname_in_marathi = "NA";
        //            dbTable.prefix_in_eng = "NA";
        //            dbTable.fname_in_eng = "NA";
        //            dbTable.mname_in_eng = "NA";
        //            dbTable.lname_in_eng = "NA";
        //            dbTable.alias_name = "NA";
        //            dbTable.owner_status_code = "NA";
        //            dbTable.owner_status_description = "NA";
        //            dbTable.holder_type = "NA";
        //            dbTable.dob = "NA";
        //            dbTable.mother_name_in_marathi = "NA";
        //            dbTable.mother_name_in_eng = "NA";
        //            dbTable.userName = "NA";
        //            dbTable.city_servey_no = "NA";
        //            dbTable.lr_property_id = "NA";
        //            dbTable.milkat = "NA";
        //            dbTable.namud = "NA";
        //            dbTable.isFullAreaGiven = "NA";
        //            dbTable.actual_area = "NA";
        //            dbTable.mutation_area = "NA";
        //            dbTable.available_area = "NA";
        //            dbTable.address_type = "NA";
        //            dbTable.address = "NA";
        //            dbTable.flatno_plotno = "NA";
        //            dbTable.societyname = "NA";
        //            dbTable.mainstreet = "NA";
        //            dbTable.landmark = "NA";
        //            dbTable.locality = "NA";
        //            dbTable.pincode = "NA";
        //            dbTable.post_office_name = "NA";
        //            dbTable.city = "NA";
        //            dbTable.taluka = "NA";
        //            dbTable.district = "NA";
        //            dbTable.state = "NA";
        //            dbTable.address_proof_document_name = "NA";
        //            dbTable.address_proof_document_path = "NA";
        //            dbTable.signed_file_name = "NA";
        //            dbTable.signed_file_path = "NA";
        //            dbTable.user_type = "NA";
        //            dbTable.profile_pic_file_name = "NA";
        //            dbTable.profile_pic_file_path = "NA";
        //            dbTable.has_property = "NA";
        //            //dbTable.khata_type = "NA";
        //            dbTable.company_name_in_marathi = "NA";
        //            dbTable.company_name_in_eng = "NA";
        //            //dbTable.aapak_dropdown = "NA";
        //            dbTable.aapak = "NA";
        //            dbTable.land_buy_area = "NA";
        //            dbTable.mutation_area = "NA";
        //            dbTable.account_type_code = 0;
        //            dbTable.account_type_description = "NA";
        //            dbTable.apk_code = 0;
        //            dbTable.apk_description = "NA";
        //            dbTable.khata_type_code = "NA";
        //            dbTable.khata_type_name = "NA";
        //            dbTable.owner_status_code = "NA";
        //            dbTable.owner_status_description = "NA";
        //            dbTable.khatano = "NA";
        //            dbTable.ulpin = "NA";
        //            dbTable.district_code = "NA";
        //            dbTable.district_name_in_marathi = "NA";
        //            dbTable.district_name_in_eng = "NA";
        //            dbTable.ofc_code = "NA";
        //            dbTable.ofc_name = "NA";
        //            dbTable.village_code = "NA";
        //            dbTable.village_name = "NA";
        //            dbTable.actual_area = "NA";
        //            dbTable.mutation_area = "NA";
        //            dbTable.relation_code = 0;
        //            dbTable.relation_name = "NA";
        //            dbTable.owner_status_code = "NA";
        //            dbTable.owner_status_description = "NA";
        //            dbTable.account_type_code = 0;
        //            dbTable.account_type_description = "NA";
        //            dbTable.varas_relation_code = 0;
        //            dbTable.varas_relation_name = "NA";
        //            dbTable.relation_code = 0;
        //            dbTable.relation_name = "NA";
        //            dbTable.holder_type = "NA";
        //            dbTable.gender_code = "NA";
        //            dbTable.gender_description = "NA";
        //            //dbTable.aapak_name = "NA";
        //            //dbTable.relation = "NA";
        //            //dbTable.is_address_same = false;
        //            //dbTable.institute_code = 0;
        //            //dbTable.institute_description = "NA";
        //            //dbTable.bank_name_in_marathi = "NA";
        //            //dbTable.bank_name_in_english = "NA";
        //            //dbTable.ifsc = "NA";
        //            //dbTable.boja_area = "NA";
        //            //dbTable.boja_value = "NA";
        //            //dbTable.boja_date = "NA";
        //            //dbTable.boja_period = "NA";
        //            //Assign Data to Table fields to insert new records
        //            dbTable.mobileno = dataModel.mobileno;

        //            if (dataModel.address_type == "FOREIGN")
        //            {
        //                dbTable.emailid = dataModel.emailid;
        //                dbTable.emailidverified = dataModel.emailidverified;
        //            }
        //            if (dataModel.address_type == "INDIA")
        //            {
        //                dbTable.mobilenoverified = dataModel.mobilenoverified;
        //            }

        //            dbTable.address_type = dataModel.address_type;
        //            dbTable.address = string.IsNullOrEmpty(dataModel.address) ? "NA" : dataModel.address;
        //            dbTable.state = dataModel.state;
        //            dbTable.district = dataModel.district;
        //            dbTable.taluka = dataModel.taluka;
        //            dbTable.city = dataModel.city;
        //            dbTable.flatno_plotno = dataModel.flatno_plotno;
        //            dbTable.societyname = dataModel.societyname;
        //            dbTable.mainstreet = dataModel.mainstreet;
        //            dbTable.landmark = dataModel.landmark;
        //            dbTable.locality = dataModel.locality;
        //            dbTable.pincode = dataModel.pincode;
        //            dbTable.post_office_name = dataModel.post_office_name;
        //            dbTable.district = dataModel.district;
        //            dbTable.userMaster = dataModel.userMaster;
        //            dbTable.applicationDTL = dataModel.applicationDTL;
        //            dbTable.prop_type = dataModel.prop_type;
        //            dbTable.mutation_srno = dataModel.mutationSroNo;
        //            dbTable.owner_number = dataModel.ownerNo;
        //            dbTable.isTaker = 1;
        //            dbTable.ifsc = dataModel.ifsc;

        //            dbTable.mutation_area = dataModel.bojaArea;
        //            dbTable.boja_value = dataModel.bojaValue;
        //            dbTable.boja_date = dataModel.bojaDate;
        //            dbTable.boja_period = dataModel.bojaPeriod;
        //            dbTable.institute_code = dataModel.institute_code;
        //            dbTable.institute_description = dataModel.institute_description;
        //            dbTable.bank_name_in_english = dataModel.bank_name_in_english;
        //            dbTable.bank_name_in_marathi = dataModel.bank_name_in_marathi;
        //            dbTable.user_type_code = 11;
        //            dbTable.user_type = "संस्था";

        //            _context.mutationDTL.Add(dbTable);
        //            _context.SaveChanges();

        //            //Get Saved Row ID
        //            int createdRowID = dbTable.mutation_givertaker_id;

        //            string VerificationType = string.Empty;
        //            bool checkAddressFlag = true;
        //            //bool checkSignFlag = false;
        //            string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //            if (dataModel.address_type == "INDIA")
        //            {
        //                VerificationType = "MOBILENO";
        //                if (!string.IsNullOrEmpty(takerData.address!.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(takerData.address.indiaAddress.addressProofName))
        //                {
        //                    string[] AddressData = takerData.address.indiaAddress.addressProofSrc.Split(",");
        //                    checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], takerData.address.indiaAddress.addressProofName, createdRowID.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
        //                    string AddressProofExt = Path.GetExtension(takerData.address.indiaAddress.addressProofName);
        //                    dataModel.address_proof_document_name = "AddressProof" + createdRowID + "_" + CurrentDateTime + AddressProofExt;
        //                    dataModel.address_proof_document_path = FolderPath + @"\" + +createdRowID + @"\" + dataModel.address_proof_document_name;

        //                    var UpdateAddressFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == createdRowID).FirstOrDefault();
        //                    if (UpdateAddressFilePath != null)
        //                    {
        //                        dbTable.address_proof_document_name = dataModel.address_proof_document_name;
        //                        dbTable.address_proof_document_path = dataModel.address_proof_document_path;
        //                        _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //                        _context.SaveChanges();
        //                    }
        //                }

        //                //string[] signData = takerData.address.indiaAddress.signatureSrc.Split(",");
        //                //checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], takerData.address.indiaAddress.signatureName, createdRowID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
        //                //string SignatureExt = Path.GetExtension(takerData.address.indiaAddress.signatureName);
        //                //dataModel.signed_file_path = "Signature" + createdRowID + "_" + CurrentDateTime + SignatureExt;
        //                //dataModel.signed_file_path = FolderPath + @"\" + createdRowID + @"\" + dataModel.signed_file_name;

        //                //var UpdateSignFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == createdRowID).FirstOrDefault();
        //                //if (UpdateSignFilePath != null)
        //                //{
        //                //    dbTable.signed_file_name = dataModel.signed_file_name;
        //                //    dbTable.signed_file_path = dataModel.signed_file_path;
        //                //    _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //                //    _context.SaveChanges();
        //                //}
        //            }
        //            //if (dataModel.address_type == "FOREIGN")
        //            //{
        //            //    VerificationType = "EMAILID";
        //            //    if (!string.IsNullOrEmpty(takerData.address.foreignAddress.signatureSrc))
        //            //    {
        //            //        string[] signData = takerData.address.foreignAddress.signatureSrc.Split(",");
        //            //        checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], takerData.address.foreignAddress.signatureName, createdRowID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

        //            //        string SignatureExt = Path.GetExtension(takerData.address.foreignAddress.signatureName);
        //            //        dataModel.signed_file_name = "Signature" + createdRowID + "_" + CurrentDateTime + SignatureExt;
        //            //        dataModel.signed_file_path = FolderPath + @"\" + createdRowID + @"\" + dataModel.signed_file_name;
        //            //        var UpdateSignFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == createdRowID).FirstOrDefault();
        //            //        if (UpdateSignFilePath != null)
        //            //        {
        //            //            dbTable.signed_file_name = dataModel.signed_file_name;
        //            //            dbTable.signed_file_path = dataModel.signed_file_path;
        //            //            _context.Entry(dbTable).CurrentValues.SetValues(dbTable);
        //            //            _context.SaveChanges();
        //            //        }
        //            //    }
        //            //    else
        //            //    {
        //            //        checkSignFlag = true;
        //            //    }
        //            //}
        //            if (checkAddressFlag)
        //            {
        //                var applicationDTLdata = _context.applicationDTL.Where(data => data.applicationid!.Equals(takerData.applicationid)).FirstOrDefault();
        //                if (applicationDTLdata != null)
        //                {
        //                    if (!string.IsNullOrEmpty(applicationDTLdata.mutationtakerIDs) && !applicationDTLdata.mutationtakerIDs.Contains(createdRowID.ToString()))
        //                    {
        //                        applicationDTLdata.mutationtakerIDs = applicationDTLdata.mutationtakerIDs + "," + createdRowID.ToString();
        //                    }
        //                    else
        //                    {
        //                        applicationDTLdata.mutationtakerIDs = createdRowID.ToString();
        //                    }
        //                    applicationDTLdata.status = 5;
        //                    _context.Entry(applicationDTLdata).CurrentValues.SetValues(applicationDTLdata);
        //                    _context.SaveChanges();
        //                }
        //                _context.SaveChanges();
        //                scope.Complete();
        //                return "Success";
        //            }
        //            else
        //            {
        //                //dbContextTransaction.Rollback();
        //                return "Address Proof File Is Not Uploaded";
        //            }
        //            //if (!checkAddressFlag)
        //            //{
        //            //    dbContextTransaction.Rollback();
        //            //    return "Address Proof File Is Not Uploaded";
        //            //}
        //            //if (!checkSignFlag)
        //            //{
        //            //    dbContextTransaction.Rollback();
        //            //    return "Signature File Is Not Uploaded";
        //            //}
        //            //else
        //            //{
        //            //    dbContextTransaction.Rollback();
        //            //    return "Some Files Are Not Uploaded";
        //            //}
        //        }
        //        catch (Exception ex)
        //        {
        //            //dbContextTransaction.Rollback();
        //            throw new HandleException(ex.Message.ToString());
        //        }
        //    }
        //}

        //public string EditGahankhatGiver(EditGahankhatDataForTaker takerData)
        //{
        //    using (var dbContextTransaction = _context.Database.BeginTransaction())
        //    {
        //        var entity = _context.mutationDTL.FirstOrDefault(s => s.mutation_givertaker_id == takerData.MutationId!)!;
        //        MethodForFileUpload methodForFile = new MethodForFileUpload();
        //        //MutationGiverTakerDTL dbTable = new MutationGiverTakerDTL();
        //        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //        string FolderPath = @"D:\WWW\MUTATIONDOCS\" + takerData.applicationid + @"\TAKER";
        //        if (entity != null)
        //        {
        //            try
        //            {
        //                // Assign values to model
        //                GahankhatModel dataModel = new GahankhatModel();

        //                dataModel.address_type = takerData.address!.addressType!.Trim().ToUpper();
        //                if (takerData.address.addressType.Trim().ToUpper() == "INDIA")
        //                {
        //                    dataModel.state = takerData.address.indiaAddress!.state;
        //                    dataModel.district = takerData.address.indiaAddress.district;
        //                    dataModel.city = takerData.address.indiaAddress.city;
        //                    dataModel.taluka = takerData.address.indiaAddress.taluka;
        //                    dataModel.flatno_plotno = takerData.address.indiaAddress.plotNo;
        //                    dataModel.societyname = takerData.address.indiaAddress.building;
        //                    dataModel.mainstreet = takerData.address.indiaAddress.mainRoad;
        //                    dataModel.landmark = takerData.address.indiaAddress.impSymbol;
        //                    dataModel.locality = takerData.address.indiaAddress.area;
        //                    dataModel.pincode = takerData.address.indiaAddress.pincode;
        //                    dataModel.post_office_name = takerData.address.indiaAddress.postOfficeName;
        //                    dataModel.mobileno = takerData.address.indiaAddress.mobile;
        //                    dataModel.mobilenoverified = takerData.address.indiaAddress.mobileOTP!.Trim().ToUpper();
        //                    dataModel.emailid = "NA";
        //                    dataModel.emailidverified = "NA";
        //                    dataModel.address = "NA";
        //                }
        //                else if (takerData.address.addressType.Trim().ToUpper() == "FOREIGN")
        //                {
        //                    dataModel.address = takerData.address.foreignAddress!.address;
        //                    dataModel.mobileno = takerData.address.foreignAddress.mobile;
        //                    dataModel.mobilenoverified = "NO";
        //                    dataModel.emailid = takerData.address.foreignAddress.email;
        //                    dataModel.emailidverified = takerData.address.foreignAddress.emailOTP!.Trim().ToUpper();

        //                    dataModel.state = "NA";
        //                    dataModel.district = "NA";
        //                    dataModel.city = "NA";
        //                    dataModel.taluka = "NA";
        //                    dataModel.flatno_plotno = "NA";
        //                    dataModel.societyname = "NA";
        //                    dataModel.mainstreet = "NA";
        //                    dataModel.landmark = "NA";
        //                    dataModel.locality = "NA";
        //                    dataModel.pincode = "NA";
        //                    dataModel.post_office_name = "NA";
        //                }

        //                ApplicationDTL applicationDTL = _context.applicationDTL.FirstOrDefault(s => s.applicationid == takerData.applicationid)!;
        //                dataModel.applicationDTL = applicationDTL;

        //                UserMaster userMaster = _context.userMasters.FirstOrDefault(s => s.userid == takerData.userid)!;
        //                dataModel.userMaster = userMaster;

        //                dataModel.institute_code = takerData.userDetails!.bankDropdown!.institute_code;
        //                dataModel.institute_description = takerData.userDetails.bankDropdown.institute_description;
        //                dataModel.bank_name_in_marathi = takerData.userDetails.bankNameMar;
        //                dataModel.bank_name_in_english = takerData.userDetails.bankNameEng;
        //                dataModel.ifsc = takerData.userDetails.ifsc;
        //                dataModel.bojaArea = takerData.userDetails.bojaArea;
        //                dataModel.bojaValue = takerData.userDetails.bojaValue;
        //                dataModel.bojaDate = takerData.userDetails.bojaDate;
        //                dataModel.bojaPeriod = takerData.userDetails.bojaPeriod;


        //                /* // Set Default Values
        //                 dbTable.isTaker = 0;
        //                 dbTable.mobileno = "NA";
        //                 dbTable.mobilenoverified = "NA";
        //                 dbTable.emailid = "NA";
        //                 dbTable.emailidverified = "FALSE";
        //                 dbTable.prefix_in_marathi = "NA";
        //                 dbTable.fname_in_marathi = "NA";
        //                 dbTable.mname_in_marathi = "NA";
        //                 dbTable.lname_in_marathi = "NA";
        //                 dbTable.prefix_in_eng = "NA";
        //                 dbTable.fname_in_eng = "NA";
        //                 dbTable.mname_in_eng = "NA";
        //                 dbTable.lname_in_eng = "NA";
        //                 dbTable.alias_name = "NA";
        //                 dbTable.owner_status_code = "NA";
        //                 dbTable.owner_status_description = "NA";
        //                 dbTable.holder_type = "NA";
        //                 dbTable.dob = "NA";
        //                 dbTable.mother_name_in_marathi = "NA";
        //                 dbTable.mother_name_in_eng = "NA";
        //                 dbTable.userName = "NA";
        //                 dbTable.city_servey_no = "NA";
        //                 dbTable.lr_property_id = "NA";
        //                 dbTable.milkat = "NA";
        //                 dbTable.namud = "NA";
        //                 dbTable.isFullAreaGiven = "NA";
        //                 dbTable.actual_area = "NA";
        //                 dbTable.mutation_area = "NA";
        //                 dbTable.available_area = "NA";
        //                 dbTable.address_type = "NA";
        //                 dbTable.address = "NA";
        //                 dbTable.flatno_plotno = "NA";
        //                 dbTable.societyname = "NA";
        //                 dbTable.mainstreet = "NA";
        //                 dbTable.landmark = "NA";
        //                 dbTable.locality = "NA";
        //                 dbTable.pincode = "NA";
        //                 dbTable.post_office_name = "NA";
        //                 dbTable.city = "NA";
        //                 dbTable.taluka = "NA";
        //                 dbTable.district = "NA";
        //                 dbTable.state = "NA";
        //                 dbTable.address_proof_document_name = "NA";
        //                 dbTable.address_proof_document_path = "NA";
        //                 dbTable.signed_file_name = "NA";
        //                 dbTable.signed_file_path = "NA";
        //                 dbTable.user_type = "NA";
        //                 dbTable.profile_pic_file_name = "NA";
        //                 dbTable.profile_pic_file_path = "NA";
        //                 dbTable.has_property = "NA";
        //                 dbTable.gender = "NA";
        //                 //dbTable.khata_type = "NA";
        //                 dbTable.company_name_in_marathi = "NA";
        //                 dbTable.company_name_in_eng = "NA";
        //                 //dbTable.aapak_dropdown = "NA";
        //                 dbTable.aapak = "NA";
        //                 dbTable.land_buy_area = "NA";
        //                 dbTable.gift_area = "NA";
        //                 dbTable.account_type_code = 0;
        //                 dbTable.account_type_description = "NA";
        //                 dbTable.apk_code = 0;
        //                 dbTable.apk_description = "NA";
        //                 dbTable.khata_type_code = "NA";
        //                 dbTable.khata_type_name = "NA";
        //                 dbTable.owner_status_code = "NA";
        //                 dbTable.owner_status_description = "NA";
        //                 dbTable.khatano = "NA";
        //                 dbTable.ulpin = "NA";
        //                 dbTable.district_code = "NA";
        //                 dbTable.district_name_in_marathi = "NA";
        //                 dbTable.district_name_in_eng = "NA";
        //                 dbTable.ofc_code = "NA";
        //                 dbTable.ofc_name = "NA";
        //                 dbTable.village_code = "NA";
        //                 dbTable.village_name = "NA";
        //                 dbTable.actualArea = "NA";
        //                 dbTable.benefitArea = "NA";
        //                 dbTable.relation_code = 0;
        //                 dbTable.relation_name = "NA";*/
        //                //dbTable.aapak_name = "NA";
        //                //dbTable.relation = "NA";
        //                //dbTable.is_address_same = false;
        //                //dbTable.institute_code = 0;
        //                //dbTable.institute_description = "NA";
        //                //dbTable.bank_name_in_marathi = "NA";
        //                //dbTable.bank_name_in_english = "NA";
        //                //dbTable.ifsc = "NA";
        //                //dbTable.boja_area = "NA";
        //                //dbTable.boja_value = "NA";
        //                //dbTable.boja_date = "NA";
        //                //dbTable.boja_period = "NA";
        //                //Assign Data to Table fields to insert new records
        //                entity.mobileno = dataModel.mobileno;

        //                if (dataModel.address_type == "FOREIGN")
        //                {
        //                    entity.emailid = dataModel.emailid;
        //                    entity.emailidverified = dataModel.emailidverified;
        //                }
        //                if (dataModel.address_type == "INDIA")
        //                {
        //                    entity.mobilenoverified = dataModel.mobilenoverified;
        //                }

        //                entity.address_type = dataModel.address_type;
        //                entity.address = string.IsNullOrEmpty(dataModel.address) ? "NA" : dataModel.address;
        //                entity.state = dataModel.state;
        //                entity.district = dataModel.district;
        //                entity.taluka = dataModel.taluka;
        //                entity.city = dataModel.city;
        //                entity.flatno_plotno = dataModel.flatno_plotno;
        //                entity.societyname = dataModel.societyname;
        //                entity.mainstreet = dataModel.mainstreet;
        //                entity.landmark = dataModel.landmark;
        //                entity.locality = dataModel.locality;
        //                entity.pincode = dataModel.pincode;
        //                entity.post_office_name = dataModel.post_office_name;
        //                entity.district = dataModel.district;
        //                entity.userMaster = dataModel.userMaster;
        //                entity.applicationDTL = dataModel.applicationDTL;
        //                entity.isTaker = 1;
        //                entity.ifsc = dataModel.ifsc;

        //                entity.mutation_area = dataModel.bojaArea;
        //                entity.boja_value = dataModel.bojaValue;
        //                entity.boja_date = dataModel.bojaDate;
        //                entity.boja_period = dataModel.bojaPeriod;
        //                entity.institute_code = dataModel.institute_code;
        //                entity.institute_description = dataModel.institute_description;
        //                entity.bank_name_in_english = dataModel.bank_name_in_english;
        //                entity.bank_name_in_marathi = dataModel.bank_name_in_marathi;

        //                _context.mutationDTL.Attach(entity);
        //                _context.SaveChanges();

        //                //Get Saved Row ID
        //                int createdRowID = entity.mutation_givertaker_id;

        //                string VerificationType = string.Empty;
        //                bool checkAddressFlag = true;
        //                //bool checkSignFlag = false;
        //                string CurrentDateTime = DateTime.Now.ToString("yyyy-MMM-dd-HHmmss");
        //                if (dataModel.address_type == "INDIA")
        //                {
        //                    VerificationType = "MOBILENO";
        //                    //applicantID = FetchApplicantID(applicantMasterModel.mobileno, VerificationType);
        //                    if (!string.IsNullOrEmpty(takerData.address.indiaAddress!.addressProofSrc) && !string.IsNullOrEmpty(takerData.address.indiaAddress!.addressProofName))
        //                    {
        //                        string[] AddressData = takerData.address.indiaAddress.addressProofSrc.Split(",");
        //                        checkAddressFlag = methodForFile.SaveImageForApplicant(AddressData[1], takerData.address.indiaAddress.addressProofName, createdRowID.ToString(), "AddressProof", FolderPath + @"\", CurrentDateTime);
        //                        string AddressProofExt = Path.GetExtension(takerData.address.indiaAddress.addressProofName);
        //                        dataModel.address_proof_document_name = "AddressProof" + createdRowID + "_" + CurrentDateTime + AddressProofExt;
        //                        dataModel.address_proof_document_path = FolderPath + @"\" + createdRowID + @"\" + dataModel.address_proof_document_name;

        //                        var UpdateAddressFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == createdRowID).FirstOrDefault();
        //                        if (UpdateAddressFilePath != null)
        //                        {
        //                            entity.address_proof_document_name = dataModel.address_proof_document_name;
        //                            entity.address_proof_document_path = dataModel.address_proof_document_path;
        //                            _context.Entry(entity).CurrentValues.SetValues(entity);
        //                            _context.SaveChanges();
        //                        }
        //                    }
        //                    //if (!string.IsNullOrEmpty(takerData.address.indiaAddress!.signatureSrc))
        //                    //{
        //                    //    string[] signData = takerData.address.indiaAddress.signatureSrc.Split(",");
        //                    //    checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], takerData.address.indiaAddress.signatureName, createdRowID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);
        //                    //    string SignatureExt = Path.GetExtension(takerData.address.indiaAddress.signatureName);
        //                    //    dataModel.signed_file_path = "Signature" + createdRowID + "_" + CurrentDateTime + SignatureExt;
        //                    //    dataModel.signed_file_path = FolderPath + @"\" + createdRowID + @"\" + dataModel.signed_file_name;

        //                    //    var UpdateSignFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == createdRowID).FirstOrDefault();
        //                    //    if (UpdateSignFilePath != null)
        //                    //    {
        //                    //        entity.signed_file_name = dataModel.signed_file_name;
        //                    //        entity.signed_file_path = dataModel.signed_file_path;
        //                    //        _context.Entry(entity).CurrentValues.SetValues(entity);
        //                    //        _context.SaveChanges();
        //                    //    }
        //                    //}
        //                }
        //                //if (dataModel.address_type == "FOREIGN")
        //                //{
        //                //    VerificationType = "EMAILID";
        //                //    // applicantID = FetchApplicantID(applicantMasterModel.emailid, VerificationType);
        //                //    if (!string.IsNullOrEmpty(takerData.address.foreignAddress!.signatureSrc))
        //                //    {
        //                //        string[] signData = takerData.address.foreignAddress.signatureSrc.Split(",");
        //                //        checkSignFlag = methodForFile.SaveImageForApplicant(signData[1], takerData.address.foreignAddress.signatureName!, createdRowID.ToString(), "Signature", FolderPath + @"\", CurrentDateTime);

        //                //        string SignatureExt = Path.GetExtension(takerData.address.foreignAddress.signatureName);
        //                //        dataModel.signed_file_name = "Signature" + createdRowID + "_" + CurrentDateTime + SignatureExt;
        //                //        dataModel.signed_file_path = FolderPath + @"\" + createdRowID + @"\" + dataModel.signed_file_name;
        //                //        var UpdateSignFilePath = _context.mutationDTL.Where(w => w.mutation_givertaker_id == createdRowID).FirstOrDefault();
        //                //        if (UpdateSignFilePath != null)
        //                //        {
        //                //            entity.signed_file_name = dataModel.signed_file_name;
        //                //            entity.signed_file_path = dataModel.signed_file_path;
        //                //            _context.Entry(entity).CurrentValues.SetValues(entity);
        //                //            _context.SaveChanges();
        //                //        }
        //                //    }
        //                //    else
        //                //    {
        //                //        checkSignFlag = true;
        //                //    }
        //                //}
        //                _context.SaveChanges();
        //                dbContextTransaction.Commit();
        //                dbContextTransaction.Dispose();
        //                return "Success";
        //            }
        //            catch (Exception ex)
        //            {
        //                dbContextTransaction.Rollback();
        //                dbContextTransaction.Dispose();
        //                throw new HandleException(ex.Message.ToString());
        //            }
        //        }
        //        else { return "False"; }
        //    }
        //}

        //public FetchGahankhatDataForTaker FetchGahankhatForGiverData(int mutationGiverID)
        //{
        //    try
        //    {
        //        MethodForFileUpload methodForFile = new MethodForFileUpload();
        //        MutationGiverTakerDTL mutationGiverTakerDTL = new MutationGiverTakerDTL();
        //        mutationGiverTakerDTL = _context.mutationDTL.Include(i => i.userMaster).Include(app => app.applicationDTL).Include(proptype => proptype.prop_type).Where(data => data.mutation_givertaker_id.Equals(mutationGiverID) && data.isDeleted == false).FirstOrDefault()!;

        //        FetchGahankhatDataForTaker fetchData = new FetchGahankhatDataForTaker();
        //        fetchData.mutation_givertaker_id = mutationGiverTakerDTL.mutation_givertaker_id;
        //        fetchData.userid = mutationGiverTakerDTL.userMaster!.userid;
        //        fetchData.applicationid = mutationGiverTakerDTL.applicationDTL!.applicationid;
        //        fetchData.mutationSroNo = mutationGiverTakerDTL.mutation_srno;
        //        fetchData.ownerNo = mutationGiverTakerDTL.owner_number;

        //        UserDetailsForGahankhatTaker userDetails = new UserDetailsForGahankhatTaker();

        //        BankDataForGahankhatTaker bankDropdown = new BankDataForGahankhatTaker();
        //        bankDropdown.institute_code = mutationGiverTakerDTL.institute_code;
        //        bankDropdown.institute_description = mutationGiverTakerDTL.institute_description;
        //        userDetails.bankDropdown = bankDropdown;

        //        userDetails.bankNameMar = mutationGiverTakerDTL.bank_name_in_marathi;
        //        userDetails.bankNameEng = mutationGiverTakerDTL.bank_name_in_english;
        //        userDetails.ifsc = mutationGiverTakerDTL.ifsc;
        //        userDetails.bojaArea = mutationGiverTakerDTL.mutation_area;
        //        userDetails.bojaValue = mutationGiverTakerDTL.boja_value;
        //        userDetails.bojaDate = mutationGiverTakerDTL.boja_date;
        //        userDetails.bojaPeriod = mutationGiverTakerDTL.boja_period;
        //        fetchData.userDetails = userDetails;

        //        fetchData.fullNameInMarathi = mutationGiverTakerDTL.bank_name_in_marathi;
        //        fetchData.fullNameInEng = mutationGiverTakerDTL.bank_name_in_english;

        //        //Address Details
        //        AddressDTLForGahankhatTaker addressData = new AddressDTLForGahankhatTaker();
        //        addressData.addressType = mutationGiverTakerDTL.address_type;
        //        if (mutationGiverTakerDTL.address_type == "INDIA")
        //        {
        //            IndiaAddressForGahankhatTaker addressForIndia = new IndiaAddressForGahankhatTaker();
        //            addressForIndia.state = mutationGiverTakerDTL.state;
        //            addressForIndia.district = mutationGiverTakerDTL.district;
        //            addressForIndia.city = mutationGiverTakerDTL.city;
        //            addressForIndia.taluka = mutationGiverTakerDTL.taluka;
        //            addressForIndia.plotNo = mutationGiverTakerDTL.flatno_plotno;
        //            addressForIndia.building = mutationGiverTakerDTL.societyname;
        //            addressForIndia.mainRoad = mutationGiverTakerDTL.mainstreet;
        //            addressForIndia.impSymbol = mutationGiverTakerDTL.landmark;
        //            addressForIndia.area = mutationGiverTakerDTL.locality;
        //            addressForIndia.pincode = mutationGiverTakerDTL.pincode;
        //            addressForIndia.postOfficeName = mutationGiverTakerDTL.post_office_name;
        //            addressForIndia.addressProofName = mutationGiverTakerDTL.address_proof_document_name;
        //            addressForIndia.mobile = mutationGiverTakerDTL.mobileno;
        //            addressForIndia.mobileOTP = mutationGiverTakerDTL.mobilenoverified;
        //            addressForIndia.signatureName = mutationGiverTakerDTL.signed_file_name!;

        //            if (mutationGiverTakerDTL.address_proof_document_path != "NA")
        //            {
        //                string AddressProofExt = Path.GetExtension(mutationGiverTakerDTL.address_proof_document_path)!;
        //                string AddressProof = methodForFile.ConvertImageToBase64(mutationGiverTakerDTL.address_proof_document_path!);
        //                mutationGiverTakerDTL.address_proof_document_path = string.IsNullOrEmpty(AddressProof) ? "NA" : "data:image/" + AddressProofExt.Replace(".", "") + ";base64," + AddressProof;
        //                addressForIndia.addressProofSrc = mutationGiverTakerDTL.address_proof_document_path;
        //            }
        //            else
        //            {
        //                addressForIndia.addressProofSrc = mutationGiverTakerDTL.address_proof_document_path;
        //            }
        //            //string SignatureExt = Path.GetExtension(mutationGiverTakerDTL.signed_file_path)!;
        //            //string Signature = methodForFile.ConvertImageToBase64(mutationGiverTakerDTL.signed_file_path!);
        //            //mutationGiverTakerDTL.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
        //            addressForIndia.signatureSrc = mutationGiverTakerDTL.signed_file_path!;
        //            addressData.indiaAddress = addressForIndia;
        //        }
        //        else if (mutationGiverTakerDTL.address_type == "FOREIGN")
        //        {
        //            AddressForForeignForGahankhatTaker addressForForeign = new AddressForForeignForGahankhatTaker();
        //            addressForForeign.address = mutationGiverTakerDTL.address;
        //            addressForForeign.mobile = mutationGiverTakerDTL.mobileno;
        //            addressForForeign.email = mutationGiverTakerDTL.emailid;
        //            addressForForeign.emailOTP = mutationGiverTakerDTL.emailidverified;
        //            addressForForeign.signatureName = mutationGiverTakerDTL.signed_file_name;

        //            //string SignatureExt = Path.GetExtension(mutationGiverTakerDTL.signed_file_path)!;
        //            //string Signature = methodForFile.ConvertImageToBase64(mutationGiverTakerDTL.signed_file_path!);
        //            //mutationGiverTakerDTL.signed_file_path = string.IsNullOrEmpty(Signature) ? "NA" : "data:image/" + SignatureExt.Replace(".", "") + ";base64," + Signature;
        //            addressForForeign.signatureSrc = mutationGiverTakerDTL.signed_file_path;
        //            addressData.foreignAddress = addressForForeign;
        //        }
        //        fetchData.address = addressData;
        //        return fetchData;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HandleException(ex.Message.ToString());
        //    }
        //}
    }
}
