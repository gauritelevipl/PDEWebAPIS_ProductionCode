using AutoMapper;
using Hangfire;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Nancy;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.ContractRepo;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;
using PDEWebAPIS.Services;
using PDEWebAPIS.TokenMethods;
using PDEWebAPIS.ViewModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;

namespace PDEWebAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrievanceSystemController : ControllerBase
    {
        private readonly GrievanceService grievanceservices;
        private readonly ApplicationServices applicationServices;
        private readonly UserServices userServices;
        private readonly MutationServices mutationServices;

        private readonly TokenBlacklistForGrievanceService tokenBlacklistServiceForGrievance;
        private readonly IConfiguration configuration;
        private readonly ILogger _grievanceLogger;
        //private readonly ILogger<TestFileUpload> _logger;
        private readonly ILogger<UserServices> _loggeruserService;
        private readonly ILogger<TestFileUpload> _loggerTest;
        private readonly ICommonRepository _ICommonRepository;
        private readonly IMapper _mapper;

        public GrievanceSystemController(AppDBContext context, AppDBContextR contextR, IConfiguration configuration, ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory, ILogger<TestFileUpload> logger, ILogger<UserServices> loggeruserService, ICommonRepository IcommonRepository, IMapper mapper)//ILogger<GrievanceSystemController> logger)
        {
            applicationServices = new ApplicationServices(context, contextR, loggeruserService);
            grievanceservices = new GrievanceService(context, contextR);
            tokenBlacklistServiceForGrievance = new Services.TokenBlacklistForGrievanceService((scopeFactory));
            this.configuration = configuration;
            _loggeruserService = loggeruserService;
            _grievanceLogger = loggerFactory.CreateLogger("GrievanceLogs");
            userServices = new UserServices(context, contextR, loggeruserService);
            _ICommonRepository = IcommonRepository;
            _mapper = mapper;
            mutationServices = new MutationServices(context, contextR, loggeruserService, IcommonRepository, mapper);

        }

        //Get all Grievance Status

        //To check without encrypted data
        //[Authorize]
        //[HttpGet]
        //[Route("fetchStatus")]
        //public async Task<IActionResult> FetchGrievanceStatus()
        //{
        //    List<FetchGrievanceStatus> statuses = await grievanceservices.FetchGrievanceStatus();
        //    return Ok(statuses);
        //}


        //for sending encrypted data
        //[Authorize]
        //[HttpPost]
        //[Route("GetGrievanceStatus")]
        //public string GetGrievanceStatus()
        //{
        //    try
        //    {
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
        //        //var decrypted = Security.DeCryptData(val);
        //        //string ApplicationType = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        List<FetchGrievanceStatus> fetchGrievanceStatuses = new List<FetchGrievanceStatus>();
        //        fetchGrievanceStatuses = grievanceservices.FetchGrievanceStatus();

        //        _grievanceLogger.LogInformation("Get Grievance Status Request Data - " + fetchGrievanceStatuses);

        //        if (fetchGrievanceStatuses != null)
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Grievance Status Data Found", fetchGrievanceStatuses))));
        //        }
        //        else
        //        {
        //            _grievanceLogger.LogInformation("Get Grievance status Response Failed - " + fetchGrievanceStatuses);
        //            type = ReponseType.NotFound;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Grievance Status Data Not Found", fetchGrievanceStatuses))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _grievanceLogger.LogError("Get Grievance Data Exception - " + ex.StackTrace!.ToString());

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

        //[HttpGet]
        //[Route("GetAppIdByUserId")]
        //public IActionResult GetAppIdByUserId(int userid)
        //{
        //    userid = 34;
        //    var result = applicationServices.FetchApplicationIDS(userid);
        //    return Ok(result);
        //}

        //encrypted
        //[Authorize]
        //[Authorize]
        //[HttpPost]
        //[Route("GetAppIdByUserId")]
        //public string GetAppIdByUserId()
        //    //[FromBody] string val)
        //    //int userid)
        //{
        //    try
        //    {
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
        //        //var decrypted = Security.DeCryptData(val);
        //        //string UserId = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        _grievanceLogger.LogInformation("GetApplicationId by UserId Request Data");

        //        var result = applicationServices.FetchApplicationIDS(UserID);

        //        if (result != null)
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application ID Found", result))));
        //        }
        //        else
        //        {
        //            _grievanceLogger.LogInformation("Get Application Id Response Failed - " + result);
        //            type = ReponseType.NotFound;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application ID Not Found", result))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _grievanceLogger.LogError("Get Application Id Data Exception - " + ex.StackTrace!.ToString());

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
        //[HttpGet]
        //[Route("GetMutationDataByAppId")]
        //public string GetMutationDataByAppId([FromBody] string val)
        //    //string applicationId)
        //{
        //    try
        //    {
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
        //        var decrypted = Security.DeCryptData(val);
        //        string applicationId = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        _grievanceLogger.LogInformation("Get Mutation Types Request Data - " + decrypted);

        //        var result = grievanceservices.FetchMutationDataByApplicationID(applicationId);

        //        if (result != null)
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Data Found", result))));
        //        }
        //        else
        //        {
        //            _grievanceLogger.LogInformation("Get Mutation Data Response Failed - " + result);
        //            type = ReponseType.NotFound;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Data Not Found", result))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _grievanceLogger.LogError("Get Mutation Data Exception - " + ex.StackTrace!.ToString());

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
        //[HttpGet]
        //[Route("GetGrievanceIssueReason")]
        //public IActionResult GetGrievanceIssueReason()
        //{
        //    List<FetchGrievanceIssueReason> statuses = grievanceservices.FetchGrievanceIssueReason();
        //    return Ok(statuses);
        //}


        //[Authorize]
        //[HttpPost]
        //[Route("GetGrievanceIssueReason")]
        //public string GetGrievanceIssueReason()
        //{
        //    try
        //    {
        //        string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        ReponseType type = ReponseType.Success;
        //        MessageResponseCode responseCode = MessageResponseCode.Success;
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
        //        //var decrypted = Security.DeCryptData(val);
        //        //string ApplicationType = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        List<FetchGrievanceIssueReason> statuses = grievanceservices.FetchGrievanceIssueReason();

        //        _grievanceLogger.LogInformation("Get Grievance Issue Reasons Request Data" + statuses);


        //        if (statuses != null)
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Grievance Issue reason Found", statuses))));
        //        }
        //        else
        //        {
        //            _grievanceLogger.LogInformation("Get Grievance Issue reason Response Failed - " + statuses);
        //            type = ReponseType.NotFound;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Grievance Issue reason Data Not Found", statuses))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _grievanceLogger.LogError("Get Grievance Issue reason Data Exception - " + ex.StackTrace!.ToString());

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

        [Authorize]
        [HttpPost]
        [Route("GetGrievanceUserDashboard")]
        public string GetGrievanceUserDashboard()
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                //UserID = 268;
                //var result = grievanceservices.FetchApplicationIDS(UserID);
                var result = grievanceservices.FetchTickitIds(UserID);
                List<FetchGrievanceDashboardData> data = new List<FetchGrievanceDashboardData>();
                if (result != null)
                {
                    foreach (string tickitid in result)
                    {
                        FetchGrievanceDashboardData fechData = new FetchGrievanceDashboardData();
                        fechData = grievanceservices.FetchDashboardData(tickitid);
                        if (fechData != null)
                        {
                            data.Add(fechData);
                            //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dashboard Data Found", fechData))));
                        }
                    }
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dashboard Data Found", data))));

                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dashboaard Data Not Found", data))));
                }


            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Get Grievance User Dashboard Exception - " + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }

        [Authorize]
        [HttpPost]
        [Route("GetMutationAndApplicationIdData")]
        public string GetMutationAndApplicationIdData()
        //string applicationId)
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
                int UserID = 0;
                //int UserID = 268;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                //var decrypted = Security.DeCryptData(val);
                //string applicationId = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _grievanceLogger.LogInformation("Get Mutation Types and Application Id data Request Data - ");

                var result = grievanceservices.FetchMutationAndApplicationIdDataUserid(UserID);

                if (result != null)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Data and application id data Found", result))));
                }
                else
                {
                    _grievanceLogger.LogInformation("Get Mutation Data and application id data Response Failed - " + result);
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Data and application id data  Not Found", result))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Get Mutation Data and application id data Exception - " + ex.StackTrace!.ToString());

                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }


        //save
        [Authorize]
        [HttpPost]
        [Route("SaveGrievanceSystemIssues")]
        public string SaveGrievanceSystemIssues([FromBody] string val)
        //GrievanceInputModel grievanceInputModel)
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("SaveGrievanceSystemIssues - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GrievanceInputModel grievanceInputModel = JsonConvert.DeserializeObject<GrievanceInputModel>(decrypted!)!;
                //UserID = 268;
                _grievanceLogger.LogInformation("SaveGrievanceSystemIssues request Data - " + grievanceInputModel);// decrypted);

                grievanceInputModel.userId = UserID;

                string Response = grievanceservices.SaveGrievanceSystemIssues(grievanceInputModel);

                //string[] SuccessData = Response.Split(",");
                //"Success";
                if (Response == "Success")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Issue Raised!!", grievanceInputModel))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("SaveGrievanceSystemIssues Submission Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, grievanceInputModel))));
                }

            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("SaveGrievanceSystemIssues Data not submitted Successfully! -" + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }


        //delete

        [Authorize]
        [HttpPost]
        [Route("DeleteGrievance")]
        public string DeleteGrievance([FromBody] string val)
        //string tickitId)
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("DeleteGrievance - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string tickitId = JsonConvert.DeserializeObject<String>(decrypted!)!;
                _grievanceLogger.LogInformation("DeleteGrievance Request Data - " + decrypted);
                string Response = grievanceservices.DeleteGrievaceApplication(tickitId);

                //"Success";
                if (Response == "Success")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Issue Deleted Successfully", Response))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("DeleteGrievance Submission Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, null))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("DeleteGrievance Data not Submitted Successfully! -" + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }


        //Register Dept Grievance user
        //[Authorize]
        //[HttpPost]
        //[Route("RegisterGrievanceUser")]
        //public string RegisterGrievanceUser(//[FromBody] string val)
        //GrievanceUserMasterInputModel grievanceUserMasterInputModel)
        //{
        //    try
        //    {
        //        var CallAPIForFlag = Request.Headers["CallAPIFor"];
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        ReponseType type = ReponseType.Success;
        //        string StatusCode = string.Empty;
        //        int UserID = 0;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }

        //        UserMaster user = new UserMaster();

        //        //var decrypted = Security.DeCryptData(val);
        //        //GrievanceUserMasterInputModel grievanceUserMasterInputModel = JsonConvert.DeserializeObject<GrievanceUserMasterInputModel>(decrypted!)!;
        //        _grievanceLogger.LogInformation("Register User for Grievance Request Data - " + grievanceUserMasterInputModel);


        //        string Response = grievanceservices.SaveGrievanceUserData(grievanceUserMasterInputModel);

        //        if (Response == "Success")
        //        {
        //            //return Ok(ResponseHandler.GetAppResponse(type, "User Created Successfully", result));
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Grievance User Registered Successfully", ""))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _grievanceLogger.LogInformation("Register Grievance User Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _grievanceLogger.LogError("Register Grievance User Exception - " + ex.StackTrace!.ToString());
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


        [HttpPost]
        [Route("LoginGrievanceUser")]
        public string LoginGrievanceUser([FromBody] string val)
        //GrievanceLoginInputModel grievanceLoginInputModel)
        {
            try
            {
                JwtSecurityToken token = new JwtSecurityToken();
                string Status = "0";
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
                string StatusCode = string.Empty;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("LoginGrievanceUser - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GrievanceLoginInputModel grievanceLoginInputModel = JsonConvert.DeserializeObject<GrievanceLoginInputModel>(decrypted!)!;
                _grievanceLogger.LogInformation("Grievance Login Request Data - " + grievanceLoginInputModel);

                string Response = grievanceservices.CheckLogin(grievanceLoginInputModel);

                //string username = grievanceLoginInputModel.username!;
                //string pass = grievanceLoginInputModel.password!;
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                if (Response == "Success")
                {
                    string username = grievanceLoginInputModel.username!;
                    string pass = grievanceLoginInputModel.password!;
                    string districtCode = grievanceLoginInputModel.district_code!;
                    string regionCode = grievanceLoginInputModel.region_code!;
                    int loginType = grievanceLoginInputModel.loginType;

                    int guserid = grievanceservices.FetchUserID(username, pass, loginType, districtCode, regionCode);

                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,configuration["Jwt:Subject"]!),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim("GUserid",Security.EnCryptData( guserid.ToString())),
                        new Claim("CurrentDateTime",DateTime.Now.ToString()),
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    if (CallAPIForFlag.ToUpper() == "WEB")
                    {
                        token = new JwtSecurityToken(
                        configuration["Jwt:Issuer"],
                        configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(60),
                        signingCredentials: signIn
                        );
                    }
                    else
                    {
                        token = new JwtSecurityToken(
                       configuration["Jwt:Issuer"],
                       configuration["Jwt:Audience"],
                       claims,
                       expires: DateTime.UtcNow.AddMonths(6),
                       signingCredentials: signIn
                       );
                    }
                    string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
                    //Save Token In User Master table
                    //Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    Status = grievanceservices.UpdateToken(tokenValue, guserid, CallAPIForFlag);
                    if (Status == "Success")
                    {

                        FetchGrievanceUsers fetchGrievanceUsers = new FetchGrievanceUsers();
                        fetchGrievanceUsers = grievanceservices.FetchGrievanceUserData(guserid);
                        string usertype = string.Empty, division = string.Empty;

                        if (fetchGrievanceUsers != null)
                        {
                            usertype = fetchGrievanceUsers.usertype!;
                            division = fetchGrievanceUsers.division!;
                        }
                        keyValuePairs.Add("usertype", usertype!);
                        keyValuePairs.Add("division", division!);
                        keyValuePairs.Add("AccessToken", tokenValue);
                        keyValuePairs.Add("district_code", districtCode);
                        keyValuePairs.Add("region_code", regionCode);

                    }
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Login successful!", keyValuePairs))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("Login for Grievance User Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, keyValuePairs))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Login Grievance User Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        //[Authorize]
        //[HttpPost("logout")]
        //public string Logout()
        //{
        //    string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
        //    if (string.IsNullOrEmpty(CallAPIForFlag))
        //    {
        //        throw new HandleException("Send CallAPIFor Flag In Header");
        //    }
        //    int GUserID = 0;
        //    ReponseType type = ReponseType.Success;
        //    var authorization = Request.Headers[HeaderNames.Authorization];
        //    try
        //    {
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
        //            var expirationClaim = User.FindFirst("exp")?.Value;
        //            DateTime expirationTime = DateTime.UtcNow;

        //            if (expirationClaim != null)
        //            {
        //                DateTimeOffset utcExpirationOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim));

        //                expirationTime = utcExpirationOffset.UtcDateTime;

        //                DateTime databaseExpirationTime = DateTime.SpecifyKind(expirationTime, DateTimeKind.Utc);

        //                TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        //                DateTime istExpirationTime = TimeZoneInfo.ConvertTimeFromUtc(databaseExpirationTime, indianTimeZone);

        //            }
        //            tokenBlacklistServiceForGrievance.AddToBlacklist(Token!, expirationTime);
        //        }
        //        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Logout successful!", ""))));
        //    }
        //    catch (Exception ex)
        //    {
        //        type = ReponseType.Failure;
        //        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, ex.Message.ToString(), ""))));
        //    }
        //}


        [Authorize]
        [HttpPost]
        [Route("GetGrievanceDashboardForDept")]
        public string GetGrievanceDashboardForDept()
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header ");
                }
                ReponseType type = ReponseType.Success;
                int GUserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                //GUserID = 3;
                var result = grievanceservices.FetchTickitIDSForDeptUsers(GUserID);
                List<FetchGrievanceDashboardData> data = new List<FetchGrievanceDashboardData>();
                if (result != null)
                {
                    foreach (string tickitid in result)
                    {
                        FetchGrievanceDashboardData fechData = new FetchGrievanceDashboardData();
                        fechData = grievanceservices.FetchDashboardData(tickitid);
                        if (fechData != null)
                        {
                            data.Add(fechData);

                            //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dashboard Data Found", fechData))));
                        }
                    }
                    _grievanceLogger.LogInformation("Get Grievance Dashboard Data found:");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Grievance Dashboard Data Found", data))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    _grievanceLogger.LogError("Get Grievance Dashboard Data fetch error: ");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Grievance Dashboard Data Not Found", data))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Get Dashboard Exception - " + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }

        //edit
        //[Authorize]
        //[HttpPost]
        //[Route("GrievanceEditAssignTo")]
        //public string GrievanceEditAssignTo([FromBody] string val)
        ////GrievanceEditAssignTo grievanceEditAssignTo)
        //{
        //    try
        //    {
        //        var CallAPIForFlag = Request.Headers["CallAPIFor"];
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        ReponseType type = ReponseType.Success;

        //        int GUserID = 0;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }
        //        //GUserID = 3;
        //        //string StatusCode = string.Empty;
        //        var decrypted = Security.DeCryptData(val);
        //        GrievanceEditAssignTo grievanceEditAssignTo = JsonConvert.DeserializeObject<GrievanceEditAssignTo>(decrypted!)!;
        //        _grievanceLogger.LogInformation("Grievance Edit Assign To Request Data - " + grievanceEditAssignTo);

        //        string Response = grievanceservices.EditAssignIssueTo(grievanceEditAssignTo, GUserID);

        //        if (Response == "Success")
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Issue assigned to user Successfully", grievanceEditAssignTo))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _grievanceLogger.LogInformation("Grievance Edit Assign to Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, grievanceEditAssignTo))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _grievanceLogger.LogError("Grievance Edit Assign To Exception - " + ex.StackTrace!.ToString());
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

        [Authorize]
        [HttpPost]
        [Route("GetUserDetails")]
        public string GetUserDetails([FromBody] string val)
        //string tickitId)
        {
            try
            {
                var CallAPIForFlag = Request.Headers["CallAPIFor"];
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;

                int GUserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);

                }
                //GUserID = 1;
                //GUserID = 2;
                //GUserID = 3;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("GetUserDetails - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string tickitId = JsonConvert.DeserializeObject<string>(decrypted!)!;

                _grievanceLogger.LogInformation("Get User View Request Data - " + tickitId);
                string Response = grievanceservices.EditGrievanceUserViewStatus(tickitId, GUserID);
                FetchGrievanceDashboardData fetchGrievanceDashboardData = grievanceservices.GetViewDetails(tickitId, GUserID);

                if (fetchGrievanceDashboardData != null && Response.ToLower() == "success")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data found", fetchGrievanceDashboardData))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("Get User View Response Failed");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data not found", fetchGrievanceDashboardData))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Get User View Exception - " + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }


        //search By application id api
        //[Authorize]
        [HttpPost]
        [Route("GetApplicationDataBySearch")]
        public string GetApplicationDataBySearch([FromBody] string val)
        //string applicationId)
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
                int GUserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("GetApplicationDataBySearch - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string applicationId = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _grievanceLogger.LogInformation("Get application data by search Request Data - ");

                var result = grievanceservices.GetApplicationDataBySearch(applicationId);

                if (result != null)
                {
                    _grievanceLogger.LogInformation("Application data by search Response - " + result);
                    type = ReponseType.Success;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application data Found", result))));
                }
                else
                {
                    _grievanceLogger.LogInformation("Application data Response Failed - " + result);
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Id wrong / Application data Not Found", result))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Application data Exception - " + ex.StackTrace!.ToString());

                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }

        //show all the NIC errors
        [Authorize]
        [HttpPost]
        [Route("GetApplicationDataForInwardNoError")]
        public string GetApplicationDataForInwardNoError()
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
                int GUserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }

                _grievanceLogger.LogInformation("Get application data for inward number error Request Data - ");

                var result = grievanceservices.GetApplicationDataForInwardNoError();

                if (result != null && result.Count > 0)
                {
                    _grievanceLogger.LogInformation("Application data for inward number error Response - " + result);
                    type = ReponseType.Success;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application data Found", result))));
                }
                else
                {
                    _grievanceLogger.LogInformation("Application data Response for inward number error Failed - " + result);
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Id wrong / Application data Not Found", result))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Application data Exception - " + ex.StackTrace!.ToString());

                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }

        //Get all Application Ids
        [Authorize]
        [HttpPost]
        [Route("GetAllApplicationIds")]
        public string GetAllApplicationIds([FromBody] string val)
        //GetAllApplicationIdForReport getAllApplicationIdForReport)
        //int pageno = 1, int pagesize=10)
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
                int GUserID = 0;
                //int GUserID = 5;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }

                _grievanceLogger.LogInformation("GetAllApplicationIds Request Data - ");
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("GetAllApplicationIds - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                //Fetch all Application Data start//
                var decrypted = Security.DeCryptData(val);
                GetAllApplicationIdForReport getAllApplicationIdForReport = JsonConvert.DeserializeObject<GetAllApplicationIdForReport>(decrypted)!;

                var applicationIds = grievanceservices.FetchAllApplicationIDS(getAllApplicationIdForReport);
                List<FetchApplicationDataBySearch> data = new List<FetchApplicationDataBySearch>();

                if (applicationIds != null)
                {
                    foreach (string applicationid in applicationIds.Data)
                    {
                        FetchApplicationDataBySearch fechData = new FetchApplicationDataBySearch();
                        fechData = grievanceservices.FetchAllApplicationIdsDashboardData(applicationid);
                        if (fechData != null)
                        {
                            data.Add(fechData);

                            //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dashboard Data Found", fechData))));
                        }
                    }
                    var paginatedResponse = new PaginatedResult<FetchApplicationDataBySearch>
                    {
                        Data = data,
                        TotalRecords = applicationIds.TotalRecords,
                        TotalPages = applicationIds.TotalPages,
                        CurrentPage = applicationIds.CurrentPage
                    };

                    _grievanceLogger.LogInformation("All ApplicationIds Data Found " + JsonConvert.SerializeObject(paginatedResponse));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "All ApplicationId Data Found", paginatedResponse))));
                }
                else
                {
                    _grievanceLogger.LogInformation("ApplicationIds data Response Failed - " + data);
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "ApplicationIds data Not Found", data))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Application data Exception - " + ex.StackTrace!.ToString());

                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }

        //Takrar purtata 

        //Dept user
        [Authorize]
        [HttpPost]
        [Route("SaveUserTakrarPurtata")]
        public string SaveUserTakrarPurtata([FromBody] string val)
        //GrievanceTakrarPurtataInputModel grievanceTakrarPurtataInputModel)
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                int GUserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                //GUserID = 1;
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("SaveUserTakrarPurtata - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GrievanceTakrarPurtataInputModel grievanceTakrarPurtataInputModel = JsonConvert.DeserializeObject<GrievanceTakrarPurtataInputModel>(decrypted!)!;
                _grievanceLogger.LogInformation("SaveUserTakrarPurtata request Data - " + grievanceTakrarPurtataInputModel);// decrypted);


                string response = grievanceservices.SaveGrievanceTakrarPurtata(grievanceTakrarPurtataInputModel, GUserID.ToString());

                //string[] SuccessData = Response.Split(",");
                //"Success";
                if (response == "Success")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Takrar Purtata Saved!", grievanceTakrarPurtataInputModel))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("Save User Takrar Purtata Submission Failed - " + response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, response, grievanceTakrarPurtataInputModel))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Save User Takrar Purtata Data not submitted Successfully! -" + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }

        //Admin takrar purtata save
        [Authorize]
        [HttpPost]
        [Route("SaveAdminTakrarPurtata")]
        public string SaveAdminTakrarPurtata([FromBody] string val)
        //GrievanceTakrarPurtataInputModel grievanceTakrarPurtataInputModel)
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                int GUserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                //GUserID = 3;
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("SaveAdminTakrarPurtata - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GrievanceTakrarPurtataInputModel grievanceTakrarPurtataInputModel = JsonConvert.DeserializeObject<GrievanceTakrarPurtataInputModel>(decrypted!)!;
                _grievanceLogger.LogInformation("SaveAdminTakrarPurtata request Data - " + grievanceTakrarPurtataInputModel);// decrypted);


                string AssignResponse = grievanceservices.EditAssignIssueTo(grievanceTakrarPurtataInputModel, GUserID);
                string Response = grievanceservices.SaveGrievanceAdminTakrarPurtata(grievanceTakrarPurtataInputModel, GUserID.ToString());

                //string[] SuccessData = Response.Split(",");
                //"Success";
                if (Response == "Success" && AssignResponse == "Success")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Takrar Purtata Saved!", grievanceTakrarPurtataInputModel))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("Save Admin Takrar Purtata Submission Failed - " + Response + " " + AssignResponse);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response + " " + AssignResponse, grievanceTakrarPurtataInputModel))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Save Admin Takrar Purtata Data not submitted Successfully! -" + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }


        //Get Actual user details on panel 

        [Authorize]
        [HttpPost]
        [Route("GetActualUserDetails")]
        public string GetActualUserDetails([FromBody] string val)
        //string tickitId)
        {
            try
            {
                var CallAPIForFlag = Request.Headers["CallAPIFor"];
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;

                int UserId = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserId = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                //UserId = 268;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("GetActualUserDetails - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string tickitId = JsonConvert.DeserializeObject<string>(decrypted!)!;

                _grievanceLogger.LogInformation("Get User View Request Data - " + tickitId);
                FetchActualUserData fetchActualUserData = grievanceservices.GetActualUserViewDetails(tickitId);

                if (fetchActualUserData != null)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Actual User Data found", fetchActualUserData))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("Get Actual User View Response Failed");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Actual User Data not found", fetchActualUserData))));
                }
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Get Actual User View Exception - " + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }
        }


        //[Authorize]
        //[HttpPost]
        //[Route("exportExcel")]
        //public ActionResult<string> exportExcel([FromBody] string val)
        ////GetAllApplicationIdForReport getAllApplicationIdForReport)
        //{
        //    try
        //    {
        //        var CallAPIForFlag = Request.Headers["CallAPIFor"];
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        ReponseType type = ReponseType.Success;

        //        int GuserId = 0;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            GuserId = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);

        //        }
        //        //UserId = 268;
        //        bool check = true;
        //        check = Security.IsBase64String(val);
        //        if (!check)
        //        {
        //            type = ReponseType.Failure;
        //            _grievanceLogger.LogInformation("exportExcel - Inout String Is Not Encrypted");
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
        //        }
        //        var decrypted = Security.DeCryptData(val);
        //        GetAllApplicationIdForReport getAllApplicationIdForReport = JsonConvert.DeserializeObject<GetAllApplicationIdForReport>(decrypted!)!;

        //        _grievanceLogger.LogInformation("Export excel request Data - " + getAllApplicationIdForReport);// decrypted);
        //        DateTime createdDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        //        var excelFile = grievanceservices.ExportToExcel(getAllApplicationIdForReport);

        //        if (excelFile != null)
        //        {
        //            //return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-Applicationdata.xlsx");
        //            string file = Security.EnCryptData(JsonConvert.SerializeObject(Ok(File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-Applicationdata.xlsx"))));
        //            return file;
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _grievanceLogger.LogInformation("Excel export error");
        //            var errorResponse = ResponseHandler.GetAppResponse(type, "Error exporting Excel", "");
        //            var encryptedError = Security.EnCryptData(JsonConvert.SerializeObject(Ok(errorResponse)));
        //            return encryptedError;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _grievanceLogger.LogError("excel export failed  - " + ex.StackTrace!.ToString());
        //        if (ex.Message.ToString() == "User Not Found")
        //        {
        //            ReponseType type = ReponseType.Failure;
        //            var errorResponse = ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString());
        //            var encryptedError = Security.EnCryptData(JsonConvert.SerializeObject(Ok(errorResponse)));
        //            return encryptedError;
        //        }
        //        else
        //        {
        //            var errorResponse = ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString());
        //            var encryptedError = Security.EnCryptData(JsonConvert.SerializeObject(Ok(errorResponse)));
        //            return encryptedError;
        //        }
        //    }
        //}


        ////Get all Application Ids
        //[Authorize]
        //[HttpPost]
        //[Route("GetCountOfApplicationId")]
        //public string GetCountOfApplicationId([FromBody] string val)
        ////GetAllApplicationIdForReport getAllApplicationIdForReport)
        ////int pageno = 1, int pagesize=10)
        //{
        //    try
        //    {
        //        string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        ReponseType type = ReponseType.Success;
        //        int GUserID = 0;
        //        //int GUserID = 5;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }

        //        _grievanceLogger.LogInformation("GetAllApplicationIds count Request Data - ");
        //        bool check = true;
        //        check = Security.IsBase64String(val);
        //        if (!check)
        //        {
        //            type = ReponseType.Failure;
        //            _grievanceLogger.LogInformation("GetCountOfApplicationId - Inout String Is Not Encrypted");
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
        //        }
        //        var decrypted = Security.DeCryptData(val);
        //        GetAllApplicationIdForReport getAllApplicationIdForReport = JsonConvert.DeserializeObject<GetAllApplicationIdForReport>(decrypted)!;
        //        var res = grievanceservices.FetchCountOfApplications(getAllApplicationIdForReport);

        //        if (res != null)
        //        {
        //            _grievanceLogger.LogInformation("Get count of application id data " + JsonConvert.SerializeObject(res));
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "ApplicationIds count Found", res))));
        //        }
        //        else
        //        {
        //            _grievanceLogger.LogInformation("ApplicationIds count Response Failed - " + res);
        //            type = ReponseType.NotFound;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "ApplicationIds Count Not Found", res))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _grievanceLogger.LogError("ApplicationID count data Exception - " + ex.StackTrace!.ToString());

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


        ////get mutation wise count
        //[Authorize]
        //[HttpPost]
        //[Route("GetCountOfMutations")]
        //public string GetCountOfMutations([FromBody] string val)
        ////GetAllApplicationIdForReport getAllApplicationIdForReport)
        ////int pageno = 1, int pagesize=10)
        //{
        //    try
        //    {
        //        string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        ReponseType type = ReponseType.Success;
        //        int GUserID = 0;
        //        //int GUserID = 5;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }

        //        _grievanceLogger.LogInformation("GetAllApplicationIds count Request Data - ");
        //        bool check = true;
        //        check = Security.IsBase64String(val);
        //        if (!check)
        //        {
        //            type = ReponseType.Failure;
        //            _grievanceLogger.LogInformation("GetCountOfMutations - Inout String Is Not Encrypted");
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
        //        }
        //        var decrypted = Security.DeCryptData(val);
        //        GetAllApplicationIdForReport getAllApplicationIdForReport = JsonConvert.DeserializeObject<GetAllApplicationIdForReport>(decrypted)!;
        //        var res = grievanceservices.FetchMutationCount(getAllApplicationIdForReport);

        //        if (res != null)
        //        {
        //            _grievanceLogger.LogInformation("Get count of Mutations data " + JsonConvert.SerializeObject(res));
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation count Found", res))));
        //        }
        //        else
        //        {
        //            _grievanceLogger.LogInformation("Mutation count Response Failed - " + res);
        //            type = ReponseType.NotFound;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "mutation Count Not Found", res))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _grievanceLogger.LogError("Mutation count data Exception - " + ex.StackTrace!.ToString());

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


        [Authorize]
        [HttpPost]
        [Route("GetApplicationDataOnInwardNoClick")]
        public string GetApplicationDataOnInwardNoClick([FromBody] string val)
        //string ApplicationID)
        {
            try
            {
                ReponseType type = ReponseType.Success;
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                int GUserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GUserID = grievanceservices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _grievanceLogger.LogInformation("GetApplicationDataOnInwardNoClick - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _grievanceLogger.LogInformation("Get Application Data For Inward No Error Tab Request Data - " + ApplicationID);
                var fetchapplication = mutationServices.GetApplicationDetailsonInwardNoClick(ApplicationID);
                // return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Found", fetchapplication)));
                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Found", fetchapplication))));
            }
            catch (Exception ex)
            {
                _grievanceLogger.LogError("Get Application Data Exception - " + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString()))));
                }
                else
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
                }
            }

        }
    }
}
