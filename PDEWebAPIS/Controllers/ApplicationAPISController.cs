using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Nancy;
using Newtonsoft.Json;
using Npgsql;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.ContractRepo;
using PDEWebAPIS.Data;
using PDEWebAPIS.EncryptDecrypt;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;
using PDEWebAPIS.Services;
using PDEWebAPIS.ViewModel;
using PDEWebAPIS.ViewModel.ModelForNICData;
using Serilog;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Policy;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Net.WebRequestMethods;

namespace PDEWebAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationAPISController : ControllerBase
    {
        private readonly DBHelper dbHelper;
        private readonly UserServices userServices;
        private readonly ApplicationServices applicationServices;
        private readonly NICService nICService;
        private readonly IConfiguration configuration;
        private readonly ILogger<ApplicationAPISController> _logger;
        //private readonly ILogger<TestFileUpload> _loggerTest;
        private readonly ILogger<UserServices> _loggeruserService;
        private readonly SMSService sMSService;
        private readonly EPCISAPIService lgdapiServices;
        private readonly IMapper _mapper;
        public ApplicationAPISController(AppDBContext context, IConfiguration configuration, ILogger<ApplicationAPISController> logger, AppDBContextR contextR, ILogger<UserServices> loggeruserService, ILogger<EPCISAPIService> loggers, IOptions<EPCISConfig> config, IMapper mapper, ICommonRepository commonRepository)
        {
            _loggeruserService = loggeruserService;
            dbHelper = new DBHelper(context);
            userServices = new UserServices(context, contextR, _loggeruserService);
            _mapper = mapper;
            nICService = new NICService(context, contextR, _loggeruserService, commonRepository, mapper);
            this.configuration = configuration;
            _logger = logger;
            //_loggerTest = loggerTest;
            applicationServices = new ApplicationServices(context, contextR, _loggeruserService);
            sMSService = new SMSService(context);
            lgdapiServices = new EPCISAPIService(contextR, config, loggers);
        }

        [Authorize]
        [HttpPost]
        [Route("GetMutationTypes")]
        public string GetMutationTypes([FromBody] string val)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Get Mutation Types - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationType = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Mutation Types Request Data - " + decrypted);

                List<FetchMutationTypes> fetchMutationTypes = new List<FetchMutationTypes>();
                fetchMutationTypes = applicationServices.FetchMutationType(ApplicationType);
                if (fetchMutationTypes != null)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Type Data Found", fetchMutationTypes))));
                }
                else
                {
                    _logger.LogInformation("Get Mutation Types Response Failed - " + fetchMutationTypes);
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchMutationTypes))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Mutation Types Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetDocumentTypeByMutation")]
        public string GetDocumentTypeByMutation([FromBody] string val)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Get Document Type By Mutation - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string MutationCode = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Document Type By Mutation Request Data - " + MutationCode);
                FetchDocumentTypeData fetchDocumentTypeData = new FetchDocumentTypeData();
                fetchDocumentTypeData = applicationServices.FetchDocumentTypeByMutation(MutationCode);
                if (fetchDocumentTypeData.documentTypeDataList!.Count > 0)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Document Type Data Found", fetchDocumentTypeData))));
                }
                else
                {
                    _logger.LogInformation("Get Document Type By Mutation Response Failed - " + fetchDocumentTypeData);
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchDocumentTypeData))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Document Type By Mutation Exception - " + ex.StackTrace!.ToString());
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
        [Route("CreateApplication")]
        public string CreateApplication([FromBody] string val)
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
                    _logger.LogInformation("Create Application - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                CreateApplicationData applicationData = JsonConvert.DeserializeObject<CreateApplicationData>(decrypted!)!;
                _logger.LogInformation("Create Application Request Data - " + decrypted);
                applicationData.userId = UserID.ToString();
                string StatusCode = string.Empty;
                //var key = "pdeappfornic12345678910111213145";
                //AesOperation AesOperation = new AesOperation();
                //Start Encrypt data
                //var encryptedString = AesOperation.EncryptObjectToBytes<CreateApplicationData>(applicationData, key);
                //var decryptedString = AesOperation.DecryptString(key, encryptedString);
                //var result = JsonConvert.DeserializeObject<CreateApplicationData>(decryptedString);
                //End

                string Response = applicationServices.SaveApplicationData(applicationData);
                string[] SuccessData = Response.Split(",");
                //"Success";
                if (SuccessData[0] == "Success")
                {
                    FetchApplicationID fetchApplicationID = new FetchApplicationID();
                    fetchApplicationID.ApplicationID = SuccessData[1];
                    applicationServices.SaveApplicationDataSubmittedHistory(SuccessData[1], "Create Application Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Created Successfully", fetchApplicationID))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Application Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Application Exception -" + ex.StackTrace!.ToString());
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
        [Route("DeleteApplication")]
        public string DeleteApplication([FromBody] string val)
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
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Application - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Deleted Application Request Data - " + decrypted);
                string Response = applicationServices.DeleteApplication(ApplicationID);
                FetchApplicantsData applicant = new FetchApplicantsData();
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(ApplicationID, "Delete Application Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Deleted Successfully", ""))));
                    //applicant = applicationServices.FetchApplicantData(ApplicantData.applicantid);
                    //if (applicant != null)
                    //{
                    //    return Ok(ResponseHandler.GetAppResponse(type, "Applicant Data Deleted Successfully", applicant));
                    //}
                    //else
                    //{
                    //    type = ReponseType.NotFound;
                    //    return Ok(ResponseHandler.GetAppResponse(type, "Applicant Data Not Found", applicant));
                    //}
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Application Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Application Exception - " + ex.StackTrace!.ToString());
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

        // Applicant 
        [Authorize]
        [HttpPost]
        [Route("CreateApplicant")]
        public string CreateApplicant([FromBody] string val)
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
                    _logger.LogInformation("Create Applicant - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                CreateApplicantData createApplicantData = JsonConvert.DeserializeObject<CreateApplicantData>(decrypted!)!;
                _logger.LogInformation("Create Applicant Request Data - " + decrypted);
                createApplicantData.userId = UserID;
                //var key1 = "b14ca5898a4e4133bbce2ea2315a1916";
                //var key = "pdeappfornic12345678910111213145";
                //AesOperation AesOperation = new AesOperation();
                //var encryptedString = AesOperation.EncryptObjectToBytes<CreateApplicantData>(createApplicantData, key);
                //var decryptedString = AesOperation.DecryptString(key, encryptedString);
                //var result = JsonConvert.DeserializeObject<CreateApplicantData>(decryptedString);
                string Response = applicationServices.SaveApplicantData(createApplicantData, "APPLICANT");

                if (Response == "Success")
                {
                    //return Ok(ResponseHandler.GetAppResponse(type, "User Created Successfully", result));
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(createApplicantData.applicationId!);
                    //string[] applicantIDS = applicationDTL.applicantIDs!.Split(",");
                    //List<FetchApplicantsData> applicantsList = new List<FetchApplicantsData>();
                    //for (int i = 0; i < applicantIDS.Length; i++)
                    //{
                    //    FetchApplicantsData applicant = new FetchApplicantsData();
                    //    applicant = applicationServices.FetchApplicantData(Convert.ToInt32(applicantIDS[i]));
                    //    if (applicant != null)
                    //    {
                    //        applicantsList.Add(applicant);
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(createApplicantData.applicationId!, "Create Applicant Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Applicant Created Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Applicant Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Applicant Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditApplicant")]
        public string EditApplicant([FromBody] string val)
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
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("EditApplicant - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditApplicantData ApplicantData = JsonConvert.DeserializeObject<EditApplicantData>(decrypted!)!;
                _logger.LogInformation("Edit Applicant Request Data - " + decrypted);
                ApplicantData.userId = UserID;
                string Response = applicationServices.EditApplicantData(ApplicantData);
                FetchApplicantsData applicant = new FetchApplicantsData();
                if (Response == "Success")
                {
                    applicant = applicationServices.FetchApplicantData(ApplicantData.applicantid);
                    if (applicant != null)
                    {
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Applicant Data Updated Successfully", applicant))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Applicant Data Not Found", applicant))));
                    }
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Applicant Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Applicant Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteApplicant")]
        public string DeleteApplicant([FromBody] string val)
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
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteApplicant - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteApplicant ApplicantData = JsonConvert.DeserializeObject<DeleteApplicant>(decrypted!)!;
                _logger.LogInformation("Deleted Applicant Request Data - " + decrypted);
                string Response = applicationServices.DeleteApplicant(ApplicantData);
                FetchApplicantsData applicant = new FetchApplicantsData();
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(ApplicantData.applicationid!, "Delete Applicant Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Applicant Data Deleted Successfully", ""))));
                    //applicant = applicationServices.FetchApplicantData(ApplicantData.applicantid);
                    //if (applicant != null)
                    //{
                    //    return Ok(ResponseHandler.GetAppResponse(type, "Applicant Data Deleted Successfully", applicant));
                    //}
                    //else
                    //{
                    //    type = ReponseType.NotFound;
                    //    return Ok(ResponseHandler.GetAppResponse(type, "Applicant Data Not Found", applicant));
                    //}
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Applicant Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Applicant Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetApplicantData")]
        public string GetApplicantData([FromBody] string val)
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
                ApplicationDTL applicationDTL = new ApplicationDTL();
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetApplicantData - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Applicant Request Data - " + ApplicationID);
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.applicantIDs))
                {
                    string[] applicantIDS = applicationDTL.applicantIDs.Split(",");
                    List<FetchApplicantsData> applicantsList = new List<FetchApplicantsData>();
                    if (applicantIDS.Length > 0)
                    {
                        for (int i = 0; i < applicantIDS.Length; i++)
                        {
                            FetchApplicantsData applicant = new FetchApplicantsData();
                            applicant = applicationServices.FetchApplicantData(Convert.ToInt32(applicantIDS[i]));
                            if (applicant != null)
                            {
                                applicantsList.Add(applicant);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Applicants Data Found", applicantsList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Applicants Data Not Found", applicantsList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Applicants Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Applicant Exception - " + ex.StackTrace!.ToString());
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

        // Mutation CTS
        [Authorize]
        [HttpPost]
        [Route("CreateMutationCTS")]
        public string CreateMutationCTS([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("CreateMutationCTS - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                MutationCTSNoData mutationCTSNoData = JsonConvert.DeserializeObject<MutationCTSNoData>(decrypted!)!;
                _logger.LogInformation("Create Mutation CTS Request Data - " + decrypted);
                mutationCTSNoData.userid = UserID;
                //var key = "pdeappfornic12345678910111213145";
                //AesOperation AesOperation = new AesOperation();
                //Start Encrypt data
                //var encryptedString = AesOperation.EncryptObjectToBytes<CreateApplicationData>(applicationData, key);
                //var decryptedString = AesOperation.DecryptString(key, encryptedString);
                //var result = JsonConvert.DeserializeObject<CreateApplicationData>(decryptedString);
                //End

                string Response = applicationServices.SaveMutationCTSNoData(mutationCTSNoData);
                //"Success";
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(mutationCTSNoData.applicationid!);
                    string[] mutationCTSNos = applicationDTL.mutation_cts_nos!.Split(",");
                    List<FetchMutationCTSNoData> fetchMutationCTSNoList = new List<FetchMutationCTSNoData>();
                    for (int i = 0; i < mutationCTSNos.Length; i++)
                    {
                        FetchMutationCTSNoData fetchMutationCTSNoData = new FetchMutationCTSNoData();
                        fetchMutationCTSNoData = applicationServices.FetchMutationCTSData(Convert.ToInt32(mutationCTSNos[i]));
                        if (fetchMutationCTSNoData != null)
                        {
                            fetchMutationCTSNoList.Add(fetchMutationCTSNoData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(mutationCTSNoData.applicationid!, "Create Mutation CTS Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation CTS No Created Successfully", fetchMutationCTSNoList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Mutation CTS Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Mutation CTS Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditMutationCTS")]
        public string EditMutationCTS([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("EditMutationCTS - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditMutationCTSNoData mutationCTSNoData = JsonConvert.DeserializeObject<EditMutationCTSNoData>(decrypted!)!;
                _logger.LogInformation("Edit Mutation CTS Request Data - " + decrypted);
                mutationCTSNoData.userid = UserID;
                string Response = applicationServices.EditMutationCTSNoData(mutationCTSNoData);
                if (Response == "Success")
                {
                    FetchMutationCTSNoData fetchMutationCTSNoData = new FetchMutationCTSNoData();
                    fetchMutationCTSNoData = applicationServices.FetchMutationCTSData(mutationCTSNoData.mutation_cts_no_id);
                    if (fetchMutationCTSNoData != null)
                    {
                        applicationServices.SaveApplicationDataSubmittedHistory(mutationCTSNoData.applicationid!, "Edit Mutation CTS Form", CallAPIForFlag);
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation CTS No Data Updated Successfully", fetchMutationCTSNoData))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation CTS No Data Not Found", fetchMutationCTSNoData))));
                    }
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Mutation CTS Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Mutation CTS Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteMutationCTS")]
        public string DeleteMutationCTS([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteMutationCTS - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutationCTSNoData mutationCTSNoData = JsonConvert.DeserializeObject<DeleteMutationCTSNoData>(decrypted!)!;
                _logger.LogInformation("Delete Mutation CTS Request Data - " + decrypted);
                string Response = applicationServices.DeleteMutationCTSNoData(mutationCTSNoData);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(mutationCTSNoData.applicationid!, "Delete Mutation CTS Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation CTS No Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Mutation CTS Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Mutation CTS Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetMutationCTS")]
        public string GetMutationCTS([FromBody] string val)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetMutationCTS - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Mutation CTS Request Data - " + decrypted);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutation_cts_nos))
                {
                    string[] mutationCTSNoIDs = applicationDTL.mutation_cts_nos.Split(",");
                    List<FetchMutationCTSNoData> mutationCTSNoList = new List<FetchMutationCTSNoData>();
                    if (mutationCTSNoIDs.Length > 0)
                    {
                        for (int i = 0; i < mutationCTSNoIDs.Length; i++)
                        {
                            FetchMutationCTSNoData mutationCTSNoData = new FetchMutationCTSNoData();
                            mutationCTSNoData = applicationServices.FetchMutationCTSData(Convert.ToInt32(mutationCTSNoIDs[i]));
                            if (mutationCTSNoData != null)
                            {
                                mutationCTSNoList.Add(mutationCTSNoData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation CTSNo Data Found", mutationCTSNoList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation CTSNo Data Not Found", mutationCTSNoList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Mutation CTS Exception - " + ex.StackTrace!.ToString());
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
        [Route("CreateDast")]
        public string CreateDast([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("CreateDast - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DastInformationData dastInformationData = JsonConvert.DeserializeObject<DastInformationData>(decrypted!)!;
                _logger.LogInformation("Create Dast Request Data - " + decrypted);
                dastInformationData.userid = UserID;
                //var key = "pdeappfornic12345678910111213145";
                //AesOperation AesOperation = new AesOperation();
                //Start Encrypt data
                //var encryptedString = AesOperation.EncryptObjectToBytes<CreateApplicationData>(applicationData, key);
                //var decryptedString = AesOperation.DecryptString(key, encryptedString);
                //var result = JsonConvert.DeserializeObject<CreateApplicationData>(decryptedString);
                //End

                string Response = applicationServices.SaveDastInformation(dastInformationData);
                //"Success";
                if (Response == "Success")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(dastInformationData.applicationid!);
                    //string[] dastIDs = applicationDTL.dastIDs!.Split(",");
                    //List<FetchDastInformationData> fetchDastInformationList = new List<FetchDastInformationData>();
                    //for (int i = 0; i < dastIDs.Length; i++)
                    //{
                    //    FetchDastInformationData fetchDastInformationData = new FetchDastInformationData();
                    //    fetchDastInformationData = applicationServices.FetchDastInformationData(Convert.ToInt32(dastIDs[i]));
                    //    if (fetchDastInformationData != null)
                    //    {
                    //        fetchDastInformationList.Add(fetchDastInformationData);
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(dastInformationData.applicationid!, "Create Dast Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dast Created Successfully", null))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Dast Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Dast Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditDast")]
        public string EditDast([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("EditDast - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditDastInformationData dastInformationData = JsonConvert.DeserializeObject<EditDastInformationData>(decrypted!)!;
                dastInformationData.userid = UserID;
                _logger.LogInformation("Edit Dast Request Data - " + decrypted);
                string Response = applicationServices.EditDastInformation(dastInformationData);
                if (Response == "Success")
                {
                    FetchDastInformationData fetchDastInformationData = new FetchDastInformationData();
                    fetchDastInformationData = applicationServices.FetchDastInformationData(dastInformationData.dastid);
                    if (fetchDastInformationData != null)
                    {
                        applicationServices.SaveApplicationDataSubmittedHistory(dastInformationData.applicationid!, "Edit Dast Form", CallAPIForFlag);
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dast Data Updated Successfully", fetchDastInformationData))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dast Data Not Found", fetchDastInformationData))));
                    }
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Dast Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Dast Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteDast")]
        public string DeleteDast([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteDast - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteDastInformationData dastInformationData = JsonConvert.DeserializeObject<DeleteDastInformationData>(decrypted!)!;
                _logger.LogInformation("Delete Dast Request Data - " + decrypted);
                string Response = applicationServices.DeleteDastInformationData(dastInformationData);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(dastInformationData.applicationid!, "Delete Dast Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dast Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Dast Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Dast Exception - " + ex.StackTrace!.ToString());
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

        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //[ProducesResponseTypeAttribute(200)]
        //[ProducesResponseTypeAttribute(400)]
        //[ProducesResponseTypeAttribute(500)]
        [Authorize]
        [HttpPost]
        [Route("GetDastInfo")]
        public string GetDastInfo([FromBody] string val)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetDastInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Dast Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.dastIDs))
                {
                    string[] dastIDs = applicationDTL.dastIDs.Split(",");
                    List<FetchDastInformationData> fetchDastInformationList = new List<FetchDastInformationData>();
                    if (dastIDs.Length > 0)
                    {
                        for (int i = 0; i < dastIDs.Length; i++)
                        {
                            FetchDastInformationData fetchDastInformationData = new FetchDastInformationData();
                            fetchDastInformationData = applicationServices.FetchDastInformationData(Convert.ToInt32(dastIDs[i]));
                            if (fetchDastInformationData != null)
                            {
                                fetchDastInformationList.Add(fetchDastInformationData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dast Information Data Found", fetchDastInformationList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dast Information Data Not Found", fetchDastInformationList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Dast Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("CreateCourtClaimInfo")]
        public string CreateCourtClaimInfo([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("CreateCourtClaimInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                CourtClaimInformationData courtClaimInformationData = JsonConvert.DeserializeObject<CourtClaimInformationData>(decrypted!)!;
                _logger.LogInformation("Create Court Claim Info Request Data - " + decrypted);
                courtClaimInformationData.userid = UserID;
                //var key = "pdeappfornic12345678910111213145";
                //AesOperation AesOperation = new AesOperation();
                //Start Encrypt data
                //var encryptedString = AesOperation.EncryptObjectToBytes<CreateApplicationData>(applicationData, key);
                //var decryptedString = AesOperation.DecryptString(key, encryptedString);
                //var result = JsonConvert.DeserializeObject<CreateApplicationData>(decryptedString);
                //End

                string Response = applicationServices.SaveCourtClaimInformation(courtClaimInformationData);
                //"Success";
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(courtClaimInformationData.applicationid!);
                    string[] courtClaimIDs = applicationDTL.courtClaimIDs!.Split(",");
                    List<FetchCourtClaimInformationData> fetchCourtClaimInformationList = new List<FetchCourtClaimInformationData>();
                    for (int i = 0; i < courtClaimIDs.Length; i++)
                    {
                        FetchCourtClaimInformationData fetchCourtClaimInformationData = new FetchCourtClaimInformationData();
                        fetchCourtClaimInformationData = applicationServices.FetchCourtClaimInformation(Convert.ToInt32(courtClaimIDs[i]));
                        if (fetchCourtClaimInformationData != null)
                        {
                            fetchCourtClaimInformationList.Add(fetchCourtClaimInformationData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(courtClaimInformationData.applicationid!, "Create Court Claim Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Court Claim Created Successfully", fetchCourtClaimInformationList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Court Claim Info Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Court CLaim Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditCourtClaimInfo")]
        public string EditCourtClaimInfo([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("EditCourtClaimInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditCourtClaimInformationData courtClaimInformationData = JsonConvert.DeserializeObject<EditCourtClaimInformationData>(decrypted!)!;
                _logger.LogInformation("Edit Court Claim Info Request Data - " + decrypted);
                courtClaimInformationData.userid = UserID;
                string Response = applicationServices.EditCourtClaimInformation(courtClaimInformationData);
                if (Response == "Success")
                {
                    FetchCourtClaimInformationData fetchCourtClaimInformationData = new FetchCourtClaimInformationData();
                    fetchCourtClaimInformationData = applicationServices.FetchCourtClaimInformation(courtClaimInformationData.court_claim_id);
                    if (fetchCourtClaimInformationData != null)
                    {
                        applicationServices.SaveApplicationDataSubmittedHistory(courtClaimInformationData.applicationid!, "Edit Court Claim Form", CallAPIForFlag);
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Court Claim Data Updated Successfully", fetchCourtClaimInformationData))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Court Claim Data Not Found", fetchCourtClaimInformationData))));
                    }
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Court Claim Info Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Court Claim Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteCourtClaimInfo")]
        public string DeleteCourtClaimInfo([FromBody] string val)
        //DeleteCourtClaimInformationData courtClaimInfomationData)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteCourtClaimInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteCourtClaimInformationData courtClaimInfomationData = JsonConvert.DeserializeObject<DeleteCourtClaimInformationData>(decrypted!)!;
                _logger.LogInformation("Delete Court Claim Info Request Data - " + decrypted);
                string Response = applicationServices.DeleteCourtClaimInformationData(courtClaimInfomationData);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(courtClaimInfomationData.applicationid!, "Delete Court Claim Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Court Claim Data Is Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Court Claim Info Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Court Claim Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetCourtClaimInfo")]
        public string GetCourtClaimInfo([FromBody] string val)
        //string ApplicationID)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetCourtClaimInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Court Claim Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.courtClaimIDs))
                {
                    string[] courtClaimIDS = applicationDTL.courtClaimIDs.Split(",");
                    List<FetchCourtClaimInformationData> fetchCourtClaimInformationList = new List<FetchCourtClaimInformationData>();
                    if (courtClaimIDS.Length > 0)
                    {
                        for (int i = 0; i < courtClaimIDS.Length; i++)
                        {
                            FetchCourtClaimInformationData fetchCourtClaimInformationData = new FetchCourtClaimInformationData();
                            fetchCourtClaimInformationData = applicationServices.FetchCourtClaimInformation(Convert.ToInt32(courtClaimIDS[i]));
                            if (fetchCourtClaimInformationData != null)
                            {
                                fetchCourtClaimInformationList.Add(fetchCourtClaimInformationData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Court Claim Data Found", fetchCourtClaimInformationList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Court Claim Data Not Found", fetchCourtClaimInformationList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Court Claim Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Court Claim Info - " + ex.StackTrace!.ToString());
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
        //[Route("AttorneyInfo")]
        //public IActionResult PowerOfAttorneyInfo(PowerOfAttorneyInformationData powerOfAttorney)
        //{
        //    try
        //    {
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
        //            UserID = userServices.FetchUserIDThroughToken(Token, CallAPIForFlag);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }
        //        powerOfAttorney.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        //var key = "pdeappfornic12345678910111213145";
        //        //AesOperation AesOperation = new AesOperation();
        //        //Start Encrypt data
        //        //var encryptedString = AesOperation.EncryptObjectToBytes<CreateApplicationData>(applicationData, key);
        //        //var decryptedString = AesOperation.DecryptString(key, encryptedString);
        //        //var result = JsonConvert.DeserializeObject<CreateApplicationData>(decryptedString);
        //        //End

        //        string Response = applicationServices.SavePowerOfAttorneyInformation(powerOfAttorney);
        //        //"Success";
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(powerOfAttorney.applicationid);
        //            string[] powerofAttorneyIDS = applicationDTL.powerOfAttorneyIDs.Split(",");
        //            List<FetchPowerOfAttorneyInformationData> fetchPowerOfAttorneyInformationList = new List<FetchPowerOfAttorneyInformationData>();
        //            for (int i = 0; i < powerofAttorneyIDS.Length; i++)
        //            {
        //                FetchPowerOfAttorneyInformationData fetchPowerOfAttorneyInformationData = new FetchPowerOfAttorneyInformationData();
        //                fetchPowerOfAttorneyInformationData = applicationServices.FetchPowerOfAttorneyInformationData(Convert.ToInt32(powerofAttorneyIDS[i]));
        //                if (fetchPowerOfAttorneyInformationData != null)
        //                {
        //                    fetchPowerOfAttorneyInformationList.Add(fetchPowerOfAttorneyInformationData);
        //                }
        //            }
        //            return Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Is Created Successfully", fetchPowerOfAttorneyInformationList));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            return Ok(ResponseHandler.GetAppResponse(type, Response, ""));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()));
        //    }
        //}

        [Authorize]
        [HttpPost]
        [Route("CreateAttorneyInfoForGiver")]
        public string CreateAttorneyInfoForGiver([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("CreateAttorneyInfoForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                PowerOfAttorneyInformationDataForGiver powerOfAttorneyInformationData = JsonConvert.DeserializeObject<PowerOfAttorneyInformationDataForGiver>(decrypted!)!;
                _logger.LogInformation("Create Attorney Info For Giver Request Data - " + decrypted);
                powerOfAttorneyInformationData.userid = UserID;
                string Response = applicationServices.SavePowerOfAttorneyGiver(powerOfAttorneyInformationData);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(powerOfAttorneyInformationData.applicationid!);
                    string[] powerofAttorneyIDS = applicationDTL.powerOfAttorneyIDs!.Split(",");
                    List<FetchPOAForGiverData> fetchPowerOfAttorneyInformationList = new List<FetchPOAForGiverData>();
                    for (int i = 0; i < powerofAttorneyIDS.Length; i++)
                    {
                        FetchPOAForGiverData fetchPowerOfAttorneyInformationData = new FetchPOAForGiverData();
                        fetchPowerOfAttorneyInformationData = applicationServices.FetchPowerOfAttorneyInfoForGiver(Convert.ToInt32(powerofAttorneyIDS[i]));
                        if (fetchPowerOfAttorneyInformationData != null)
                        {
                            fetchPowerOfAttorneyInformationList.Add(fetchPowerOfAttorneyInformationData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(powerOfAttorneyInformationData.applicationid!, "Create POA Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Is Created Successfully", fetchPowerOfAttorneyInformationList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Attorney Info For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Attorney Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditPOAForGiver")]
        public string EditPOAForGiver([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                var CallAPIForFlag = Request.Headers["CallAPIFor"];
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("EditPOAForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditPowerOfAttorneyInformationDataForGiver powerOfAttorneyInformationData = JsonConvert.DeserializeObject<EditPowerOfAttorneyInformationDataForGiver>(decrypted!)!;
                _logger.LogInformation("Edit POA For Giver Request Data - " + decrypted);
                powerOfAttorneyInformationData.userid = UserID;
                string Response = applicationServices.EditPowerOfAttorneyGiver(powerOfAttorneyInformationData);
                if (Response == "Success")
                {
                    FetchPOAForGiverData fetchPowerOfAttorneyInformationData = new FetchPOAForGiverData();
                    fetchPowerOfAttorneyInformationData = applicationServices.FetchPowerOfAttorneyInfoForGiver(powerOfAttorneyInformationData.power_of_attorney_id);
                    if (fetchPowerOfAttorneyInformationData != null)
                    {
                        applicationServices.SaveApplicationDataSubmittedHistory(powerOfAttorneyInformationData.applicationid!, "Edit POA Giver Form", CallAPIForFlag);
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Data Is Updated Successfully", fetchPowerOfAttorneyInformationData))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Data Not Found", fetchPowerOfAttorneyInformationData))));
                    }
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit POA For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit POA For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeletePOAForGiver")]
        public string DeletePOAForGiver([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeletePOAForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeletePowerOfAttorneyInformationDataForGiver powerOfAttorneyInformationData = JsonConvert.DeserializeObject<DeletePowerOfAttorneyInformationDataForGiver>(decrypted!)!;
                _logger.LogInformation("Delete POA For Giver Request Data - " + decrypted);
                string Response = applicationServices.DeletePowerOfAttorneyGiver(powerOfAttorneyInformationData);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(powerOfAttorneyInformationData.applicationid!, "Delete POA Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Data Is Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete POA For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete POA For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetPowerOfAttorneyInfoForGiver")]
        public string GetPowerOfAttorneyInfoForGiver([FromBody] string val)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetPowerOfAttorneyInfoForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Power Of Attorney Info For Giver Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.powerOfAttorneyIDs) && applicationDTL.powerOfAttorneyIDs != null)
                {
                    string[] powerOfAttorneyIDS = applicationDTL.powerOfAttorneyIDs.Split(",");
                    List<FetchPOAForGiverData> fetchPowerOfAttorneyInformationList = new List<FetchPOAForGiverData>();
                    if (powerOfAttorneyIDS.Length > 0)
                    {
                        for (int i = 0; i < powerOfAttorneyIDS.Length; i++)
                        {
                            FetchPOAForGiverData fetchPowerOfAttorneyInformationData = new FetchPOAForGiverData();
                            fetchPowerOfAttorneyInformationData = applicationServices.FetchPowerOfAttorneyInfoForGiver(Convert.ToInt32(powerOfAttorneyIDS[i]));
                            if (fetchPowerOfAttorneyInformationData != null)
                            {
                                fetchPowerOfAttorneyInformationList.Add(fetchPowerOfAttorneyInformationData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Found", fetchPowerOfAttorneyInformationList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Not Found", fetchPowerOfAttorneyInformationList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Power Of Attorney Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        //[Route("CreateAttorneyInfoForTaker")]
        //public string CreateAttorneyInfoForTaker(
        //    //PowerOfAttorneyInformationDataForTaker powerOfAttorneyInformationData)
        //    [FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        PowerOfAttorneyInformationDataForTaker powerOfAttorneyInformationData = JsonConvert.DeserializeObject<PowerOfAttorneyInformationDataForTaker>(decrypted!)!;
        //        _logger.LogInformation("Create Attorney Info For Taker Request Data - " + decrypted);
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
        //        powerOfAttorneyInformationData.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = applicationServices.SavePowerOfAttorneyTaker(powerOfAttorneyInformationData);
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(powerOfAttorneyInformationData.applicationid!);
        //            string[] powerofAttorneyIDS = applicationDTL.powerOfAttorneyIDs!.Split(",");
        //            List<FetchPOAForTakerData> fetchPowerOfAttorneyInformationList = new List<FetchPOAForTakerData>();
        //            FetchGiverData fetchGiverData = new FetchGiverData();
        //            fetchGiverData = applicationServices.FetchPOAGiverData(powerOfAttorneyInformationData.applicationid!);
        //            for (int i = 0; i < powerofAttorneyIDS.Length; i++)
        //            {
        //                FetchPOAForTakerData fetchPowerOfAttorneyInformationData = new FetchPOAForTakerData();
        //                fetchPowerOfAttorneyInformationData = applicationServices.FetchPowerOfAttorneyInfoForTaker(Convert.ToInt32(powerofAttorneyIDS[i]));
        //                if (fetchPowerOfAttorneyInformationData != null)
        //                {
        //                    if (fetchGiverData != null)
        //                    {
        //                        for (int j = 0; j < fetchGiverData.giver_names_in_marathi!.Count; j++)
        //                        {
        //                            if (j == 0)
        //                            {
        //                                fetchPowerOfAttorneyInformationData.giver_name_in_marathi = fetchGiverData.giver_names_in_marathi[j];
        //                                fetchPowerOfAttorneyInformationData.giver_name_in_english = fetchGiverData.giver_names_in_english![j];
        //                            }
        //                            else
        //                            {
        //                                fetchPowerOfAttorneyInformationData.giver_name_in_marathi = fetchPowerOfAttorneyInformationData.giver_name_in_marathi + ", " + fetchGiverData.giver_names_in_marathi[j];
        //                                fetchPowerOfAttorneyInformationData.giver_name_in_english = fetchPowerOfAttorneyInformationData.giver_name_in_english + ", " + fetchGiverData.giver_names_in_english![j];
        //                            }
        //                        }
        //                    }
        //                    fetchPowerOfAttorneyInformationList.Add(fetchPowerOfAttorneyInformationData);
        //                }
        //            }
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Is Created Successfully", fetchPowerOfAttorneyInformationList))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Create Attorney Info For Taker Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Create Attorney Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("CreateAttorneyInfoForTaker")]
        public string CreateAttorneyInfoForTaker(
        //PowerOfAttorneyInformationDataForTaker powerOfAttorneyInformationData)
        [FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("CreateAttorneyInfoForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                PowerOfAttorneyInformationDataForTaker powerOfAttorneyInformationData = JsonConvert.DeserializeObject<PowerOfAttorneyInformationDataForTaker>(decrypted!)!;
                _logger.LogInformation("Create Attorney Info For Taker Request Data - " + decrypted);
                powerOfAttorneyInformationData.userid = UserID;
                string Response = applicationServices.SavePowerOfAttorneyTaker(powerOfAttorneyInformationData);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationDataForPOA(powerOfAttorneyInformationData.applicationid!);
                    string[] powerofAttorneyIDS = applicationDTL.powerOfAttorneyIDs!.Split(",");
                    List<FetchPOAForTakerData> fetchPowerOfAttorneyInformationList = new List<FetchPOAForTakerData>();
                    for (int i = 0; i < powerofAttorneyIDS.Length; i++)
                    {
                        FetchPOAForTakerData fetchPowerOfAttorneyInformationData = new FetchPOAForTakerData();
                        fetchPowerOfAttorneyInformationData = applicationServices.FetchPowerOfAttorneyInfoForTaker(Convert.ToInt32(powerofAttorneyIDS[i]));
                        if (fetchPowerOfAttorneyInformationData != null && !string.IsNullOrEmpty(fetchPowerOfAttorneyInformationData.poa_giver_ids))
                        {
                            string[] poa_giver_ids = fetchPowerOfAttorneyInformationData.poa_giver_ids!.Split(",");
                            List<FetchGiverDataForPOA> giverDataList = new List<FetchGiverDataForPOA>();
                            if (!string.IsNullOrEmpty(fetchPowerOfAttorneyInformationData.poa_giver_ids))
                            {
                                for (int k = 0; k < poa_giver_ids.Length; k++)
                                {
                                    FetchGiverDataForPOA fetchGiverData = new FetchGiverDataForPOA();
                                    fetchGiverData = applicationServices.FetchPOAGiverDataByID(Convert.ToInt32(poa_giver_ids[k]));
                                    if (fetchGiverData != null)
                                    {
                                        giverDataList.Add(fetchGiverData);
                                    }
                                }
                                for (int j = 0; j < giverDataList.Count; j++)
                                {
                                    if (j == 0)
                                    {
                                        fetchPowerOfAttorneyInformationData.giver_name_in_marathi = giverDataList[j].giver_names_in_marathi;
                                        fetchPowerOfAttorneyInformationData.giver_name_in_english = giverDataList[j].giver_names_in_english!;
                                    }
                                    else
                                    {
                                        fetchPowerOfAttorneyInformationData.giver_name_in_marathi = fetchPowerOfAttorneyInformationData.giver_name_in_marathi + ", " + giverDataList[j].giver_names_in_marathi;
                                        fetchPowerOfAttorneyInformationData.giver_name_in_english = fetchPowerOfAttorneyInformationData.giver_name_in_english + ", " + giverDataList[j].giver_names_in_english!;
                                    }
                                }
                                fetchPowerOfAttorneyInformationList.Add(fetchPowerOfAttorneyInformationData);
                            }
                        }
                        //else
                        //{
                        //    type = ReponseType.NotFound;
                        //    return Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Not Found", fetchPowerOfAttorneyInformationList));
                        //}
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(powerOfAttorneyInformationData.applicationid!, "Create POA Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Is Created Successfully", fetchPowerOfAttorneyInformationList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Attorney Info For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Attorney Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditPOAForTaker")]
        public string EditPOAForTaker([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("EditPOAForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditPowerOfAttorneyInformationDataForTaker powerOfAttorneyInformationData = JsonConvert.DeserializeObject<EditPowerOfAttorneyInformationDataForTaker>(decrypted!)!;
                _logger.LogInformation("Edit POA For Taker Request Data - " + decrypted);
                powerOfAttorneyInformationData.userid = UserID;
                string Response = applicationServices.EditPowerOfAttorneyTaker(powerOfAttorneyInformationData);
                if (Response == "Success")
                {
                    FetchGiverData fetchGiverData = new FetchGiverData();
                    fetchGiverData = applicationServices.FetchPOAGiverData(powerOfAttorneyInformationData.applicationid!);
                    FetchPOAForTakerData fetchPowerOfAttorneyInformationData = new FetchPOAForTakerData();
                    fetchPowerOfAttorneyInformationData = applicationServices.FetchPowerOfAttorneyInfoForTaker(powerOfAttorneyInformationData.power_of_attorney_id);
                    if (fetchPowerOfAttorneyInformationData != null)
                    {
                        if (fetchGiverData != null)
                        {
                            for (int j = 0; j < fetchGiverData.giver_names_in_marathi!.Count; j++)
                            {
                                if (j == 0)
                                {
                                    fetchPowerOfAttorneyInformationData.giver_name_in_marathi = fetchGiverData.giver_names_in_marathi[j];
                                    fetchPowerOfAttorneyInformationData.giver_name_in_english = fetchGiverData.giver_names_in_english![j];
                                }
                                else
                                {
                                    fetchPowerOfAttorneyInformationData.giver_name_in_marathi = fetchPowerOfAttorneyInformationData.giver_name_in_marathi + ", " + fetchGiverData.giver_names_in_marathi[j];
                                    fetchPowerOfAttorneyInformationData.giver_name_in_english = fetchPowerOfAttorneyInformationData.giver_name_in_english + ", " + fetchGiverData.giver_names_in_english![j];
                                }
                            }
                        }
                        applicationServices.SaveApplicationDataSubmittedHistory(powerOfAttorneyInformationData.applicationid!, "Edit POA Taker Form", CallAPIForFlag);
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Data Is Updated Successfully", fetchPowerOfAttorneyInformationData))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Data Not Found", fetchPowerOfAttorneyInformationData))));
                    }
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit POA For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit POA For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeletePOAForTaker")]
        public string DeletePOAForTaker(
            //DeletePowerOfAttorneyInformationDataForTaker powerOfAttorneyInformationData)
            [FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeletePOAForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeletePowerOfAttorneyInformationDataForTaker powerOfAttorneyInformationData = JsonConvert.DeserializeObject<DeletePowerOfAttorneyInformationDataForTaker>(decrypted!)!;
                _logger.LogInformation("Delete POA For Taker Request Data - " + decrypted);
                string Response = applicationServices.DeletePowerOfAttorneyTaker(powerOfAttorneyInformationData);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(powerOfAttorneyInformationData.applicationid!, "Delete POA Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Data Is Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete POA For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete POA For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetPowerOfAttorneyInfoForTaker")]
        public string GetPowerOfAttorneyInfoForTaker([FromBody] string val)
        //string ApplicationID)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetPowerOfAttorneyInfoForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Power Of Attorney Info For Taker Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationDataForPOA(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.powerOfAttorneyIDs))
                {
                    string[] powerOfAttorneyIDS = applicationDTL.powerOfAttorneyIDs.Split(",");
                    List<FetchPOAForTakerData> fetchPowerOfAttorneyInformationList = new List<FetchPOAForTakerData>();
                    if (powerOfAttorneyIDS.Length > 0)
                    {
                        for (int i = 0; i < powerOfAttorneyIDS.Length; i++)
                        {
                            FetchPOAForTakerData fetchPowerOfAttorneyInformationData = new FetchPOAForTakerData();
                            fetchPowerOfAttorneyInformationData = applicationServices.FetchPowerOfAttorneyInfoForTaker(Convert.ToInt32(powerOfAttorneyIDS[i]));
                            if (fetchPowerOfAttorneyInformationData != null && !string.IsNullOrEmpty(fetchPowerOfAttorneyInformationData.poa_giver_ids))
                            {
                                string[] poa_giver_ids = fetchPowerOfAttorneyInformationData.poa_giver_ids!.Split(",");
                                List<FetchGiverDataForPOA> giverDataList = new List<FetchGiverDataForPOA>();
                                List<GiverTakerInfoData> selectedOptions = new List<GiverTakerInfoData>();
                                if (!string.IsNullOrEmpty(fetchPowerOfAttorneyInformationData.poa_giver_ids))
                                {
                                    for (int k = 0; k < poa_giver_ids.Length; k++)
                                    {
                                        FetchGiverDataForPOA fetchGiverData = new FetchGiverDataForPOA();
                                        fetchGiverData = applicationServices.FetchPOAGiverDataByID(Convert.ToInt32(poa_giver_ids[k]));
                                        if (fetchGiverData != null)
                                        {
                                            giverDataList.Add(fetchGiverData);
                                        }
                                    }
                                    for (int j = 0; j < giverDataList.Count; j++)
                                    {
                                        if (j == 0)
                                        {
                                            fetchPowerOfAttorneyInformationData.giver_name_in_marathi = giverDataList[j].giver_names_in_marathi;
                                            fetchPowerOfAttorneyInformationData.giver_name_in_english = giverDataList[j].giver_names_in_english!;
                                        }
                                        else
                                        {
                                            fetchPowerOfAttorneyInformationData.giver_name_in_marathi = fetchPowerOfAttorneyInformationData.giver_name_in_marathi + ", " + giverDataList[j].giver_names_in_marathi;
                                            fetchPowerOfAttorneyInformationData.giver_name_in_english = fetchPowerOfAttorneyInformationData.giver_name_in_english + ", " + giverDataList[j].giver_names_in_english!;
                                        }
                                    }
                                    fetchPowerOfAttorneyInformationList.Add(fetchPowerOfAttorneyInformationData);
                                }
                            }
                            //else
                            //{
                            //    type = ReponseType.NotFound;
                            //    return Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Not Found", fetchPowerOfAttorneyInformationList));
                            //}
                        }
                        if (fetchPowerOfAttorneyInformationList.Count > 0)
                        {
                            //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Found", fetchPowerOfAttorneyInformationList)));
                            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Found", fetchPowerOfAttorneyInformationList))));
                        }
                        else
                        {
                            type = ReponseType.NotFound;
                            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Not Found", fetchPowerOfAttorneyInformationList))));
                        }
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Not Found", fetchPowerOfAttorneyInformationList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Power Of Attorney Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        //[Route("GetPowerOfAttorneyInfoForTaker")]
        //public string GetPowerOfAttorneyInfoForTaker(string ApplicationID)
        ////[FromBody] string val)
        //{
        //    try
        //    {
        //        //var decrypted = Security.DeCryptData(val);
        //        //string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        _logger.LogInformation("Get Power Of Attorney Info For Taker Request Data - " + ApplicationID);
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
        //        if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.powerOfAttorneyIDs))
        //        {
        //            string[] powerOfAttorneyIDS = applicationDTL.powerOfAttorneyIDs.Split(",");
        //            List<FetchPOAForTakerData> fetchPowerOfAttorneyInformationList = new List<FetchPOAForTakerData>();
        //            if (powerOfAttorneyIDS.Length > 0)
        //            {
        //                FetchGiverData fetchGiverData = new FetchGiverData();
        //                fetchGiverData = applicationServices.FetchPOAGiverData(ApplicationID);
        //                for (int i = 0; i < powerOfAttorneyIDS.Length; i++)
        //                {
        //                    FetchPOAForTakerData fetchPowerOfAttorneyInformationData = new FetchPOAForTakerData();
        //                    fetchPowerOfAttorneyInformationData = applicationServices.FetchPowerOfAttorneyInfoForTaker(Convert.ToInt32(powerOfAttorneyIDS[i]));
        //                    if (fetchPowerOfAttorneyInformationData != null)
        //                    {
        //                        if (fetchGiverData != null)
        //                        {
        //                            for (int j = 0; j < fetchGiverData.giver_names_in_marathi!.Count; j++)
        //                            {
        //                                if (j == 0)
        //                                {
        //                                    fetchPowerOfAttorneyInformationData.giver_name_in_marathi = fetchGiverData.giver_names_in_marathi[j];
        //                                    fetchPowerOfAttorneyInformationData.giver_name_in_english = fetchGiverData.giver_names_in_english![j];
        //                                }
        //                                else
        //                                {
        //                                    fetchPowerOfAttorneyInformationData.giver_name_in_marathi = fetchPowerOfAttorneyInformationData.giver_name_in_marathi + ", " + fetchGiverData.giver_names_in_marathi[j];
        //                                    fetchPowerOfAttorneyInformationData.giver_name_in_english = fetchPowerOfAttorneyInformationData.giver_name_in_english + ", " + fetchGiverData.giver_names_in_english![j];
        //                                }
        //                            }
        //                        }
        //                        fetchPowerOfAttorneyInformationList.Add(fetchPowerOfAttorneyInformationData);
        //                    }
        //                    //else
        //                    //{
        //                    //    type = ReponseType.NotFound;
        //                    //    return Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Not Found", fetchPowerOfAttorneyInformationList));
        //                    //}
        //                }
        //                return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Found", fetchPowerOfAttorneyInformationList)));
        //                //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Found", fetchPowerOfAttorneyInformationList))));
        //            }
        //            else
        //            {
        //                type = ReponseType.NotFound;
        //                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Power Of Attorney Information Data Not Found", fetchPowerOfAttorneyInformationList))));
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
        //        _logger.LogError("Get Power Of Attorney Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetDocumentTypeByApplicationID")]
        public string GetDocumentTypeByApplicationID([FromBody] string val)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetDocumentTypeByApplicationID - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Document Type By ApplicationID Request Data - " + ApplicationID);
                FetchDocumentTypeData fetchDocumentTypeData = new FetchDocumentTypeData();
                fetchDocumentTypeData = applicationServices.FetchDocumentType(ApplicationID);
                if (fetchDocumentTypeData != null)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Document Type Data Found", fetchDocumentTypeData))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchDocumentTypeData))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Document Type By ApplicationID Exception - " + ex.StackTrace!.ToString());
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
        [Route("SaveDocumentUpload")]
        public string SaveDocumentUpload([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("SaveDocumentUpload - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                AddUploadedDocumentsDTLData addUploadedDocuments = JsonConvert.DeserializeObject<AddUploadedDocumentsDTLData>(decrypted!)!;
                _logger.LogInformation("Save Document Upload Request Data - " + decrypted);
                addUploadedDocuments.userid = UserID;
                string Response = applicationServices.SaveDocUploadedData(addUploadedDocuments);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(addUploadedDocuments.applicationid!);
                    string[] uploadedDocIDS = applicationDTL.uploadedDocIDs!.Split(",");
                    List<FetchUploadedDocumentsData> fetchUploadedDocumentsList = new List<FetchUploadedDocumentsData>();
                    for (int i = 0; i < uploadedDocIDS.Length; i++)
                    {
                        FetchUploadedDocumentsData fetchUploadedDocumentsData = new FetchUploadedDocumentsData();
                        fetchUploadedDocumentsData = applicationServices.FetchUploadedDocumentDTL(Convert.ToInt32(uploadedDocIDS[i]));
                        if (fetchUploadedDocumentsData != null)
                        {
                            fetchUploadedDocumentsList.Add(fetchUploadedDocumentsData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(addUploadedDocuments.applicationid!, "Document Uploads Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Document Is Uploaded Successfully", fetchUploadedDocumentsList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Save Document Upload Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Save Document Upload Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditDocumentUpload")]
        public string EditDocumentUpload([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                var CallAPIForFlag = Request.Headers["CallAPIFor"];
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("EditDocumentUpload - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditAddUploadedDocumentsDTLData addUploadedDocuments = JsonConvert.DeserializeObject<EditAddUploadedDocumentsDTLData>(decrypted!)!;
                _logger.LogInformation("Edit Document Upload Request Data - " + decrypted);
                addUploadedDocuments.userid = UserID;
                string Response = applicationServices.EditDocUploadedData(addUploadedDocuments);
                if (Response == "Success")
                {
                    FetchUploadedDocumentsData fetchUploadedDocumentsData = new FetchUploadedDocumentsData();
                    fetchUploadedDocumentsData = applicationServices.FetchUploadedDocumentDTL(addUploadedDocuments.uploaded_doc_id);
                    if (fetchUploadedDocumentsData != null)
                    {
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Document Is Updated Successfully", fetchUploadedDocumentsData))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Document Data Not Found", fetchUploadedDocumentsData))));
                    }
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Document Upload Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Document Upload Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteDocumentUpload")]
        public string DeleteDocumentUpload([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                var CallAPIForFlag = Request.Headers["CallAPIFor"];
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteDocumentUpload - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteAddUploadedDocumentsDTLData addUploadedDocuments = JsonConvert.DeserializeObject<DeleteAddUploadedDocumentsDTLData>(decrypted!)!;
                _logger.LogInformation("Delete Document Upload Request Data - " + decrypted);
                string Response = applicationServices.DeleteDocUploadedData(addUploadedDocuments);
                if (Response == "Success")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Document Is Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Document Upload Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Document Upload Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetDocumentUploadedDTL")]
        public string GetDocumentUploadedDTL([FromBody] string val)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetDocumentUploadedDTL - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Document Uploaded DTL Request Data - " + ApplicationID);

                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.uploadedDocIDs))
                {
                    string[] uploadedDocIDs = applicationDTL.uploadedDocIDs.Split(",");
                    List<FetchUploadedDocumentsData> fetchUploadedDocumentsList = new List<FetchUploadedDocumentsData>();
                    if (uploadedDocIDs.Length > 0)
                    {
                        for (int i = 0; i < uploadedDocIDs.Length; i++)
                        {
                            FetchUploadedDocumentsData fetchUploadedDocumentsData = new FetchUploadedDocumentsData();
                            fetchUploadedDocumentsData = applicationServices.FetchUploadedDocumentDTL(Convert.ToInt32(uploadedDocIDs[i]));
                            if (fetchUploadedDocumentsData != null)
                            {
                                fetchUploadedDocumentsList.Add(fetchUploadedDocumentsData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Uploaded Document Data Found", fetchUploadedDocumentsList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Uploaded Document Data Not Found", fetchUploadedDocumentsList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Document Uploaded DTL Exception - " + ex.StackTrace!.ToString());
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
        [HttpPost]
        [Route("DownloadUploadedFile")]
        public string DownloadUploadedFile([FromBody] string val)
        {
            try
            {
                var decrypted = Security.DeCryptData(val);
                Dictionary<string, object> p_dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(decrypted!)!;
                string ApplicationID = p_dict["ApplicationID"].ToString()!;
                string FileName = p_dict["FileName"].ToString()!;
                string FolderPath = @"D:\WWW\MUTATIONDOCS\" + ApplicationID + @"\DOCUMENTS\" + FileName;
                _logger.LogInformation("Download Uploaded File Request Data - " + p_dict);
                ReponseType type = ReponseType.Success;
                if (System.IO.File.Exists(FolderPath))
                {
                    var provider = new FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(FolderPath, out var contentType))
                    {
                        contentType = "application/octet-stream";
                    }
                    byte[] bytes = System.IO.File.ReadAllBytes(FolderPath);
                    return Security.EnCryptData(JsonConvert.SerializeObject(File(bytes, "application/pdf", Path.GetFileName(FolderPath))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "File Not Found", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Download Uploaded File Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteUploadedFile")]
        public string DeleteUploadedFile([FromBody] string val)
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
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteUploadedFile - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteUploadedDocumentData deleteUploadedDocument = JsonConvert.DeserializeObject<DeleteUploadedDocumentData>(decrypted!)!;
                _logger.LogInformation("Delete Uploaded File Request Data - " + deleteUploadedDocument);
                string Response = applicationServices.DeleteUploadedDocument(deleteUploadedDocument);
                if (Response == "Success")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Document Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Uploaded File Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Uploaded File Exception - " + ex.StackTrace!.ToString());
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

        //Working
        [HttpPost]
        [Route("UploadFile")]
        public string UploadFile(IFormFile file)
        {
            var fileName = DateTime.Now.ToString("yyyymmddhhmmss") + "_" + file.FileName;
            try
            {
                var extention = Path.GetExtension(fileName);
                //var extentionAddress = Path.GetExtension(addresProofFile.FileName);
                var filePath = @"D:\WWW\TESTFILEUPLOAD\";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var completeFilePath = filePath + fileName;
                // var completeFilePathForAddress = filePath + addresProofFile.FileName;
                using (var stream = new FileStream(completeFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Dispose();
                    stream.Close();
                }
                //using (var stream = new FileStream(completeFilePathForAddress, FileMode.Create))
                //{
                //    addresProofFile.CopyTo(stream);
                //    stream.Dispose();
                //    stream.Close();
                //}
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(fileName)));
        }

        //End Working

        //Working
        [HttpPost]
        [Route("DownloadFile")]
        public string DownloadFile([FromBody] string val)
        {
            try
            {
                var decrypted = Security.DeCryptData(val);
                string fileName = JsonConvert.DeserializeObject<string>(decrypted!)!;

                var filePath = @"D:\WWW\TESTFILEUPLOAD\" + fileName;
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }
                byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                return Security.EnCryptData(JsonConvert.SerializeObject(File(bytes, contentType, Path.GetFileName(filePath))));

            }
            catch (Exception ex)
            {
                _logger.LogError("Download File Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        //End Working


        [Authorize]
        [HttpPost]
        [Route("GetApplicationData")]
        public string GetApplicationData([FromBody] string val)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetApplicationData - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Application Data Request Data - " + ApplicationID);

                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null)
                {
                    FetchApplicationDTL fetchApplicationDTL = new FetchApplicationDTL();
                    fetchApplicationDTL.district_code = applicationDTL.district_code;
                    fetchApplicationDTL.district_name_in_marathi = applicationDTL.district_name_in_marathi;
                    fetchApplicationDTL.district_name_in_english = applicationDTL.district_name_in_english;
                    fetchApplicationDTL.taluka_code = applicationDTL.office_code;
                    fetchApplicationDTL.taluka_name = applicationDTL.office_name;
                    fetchApplicationDTL.application_type_code = applicationDTL.applicationTypeMaster!.applicationtypeid.ToString();
                    fetchApplicationDTL.application_type_in_marathi = applicationDTL.applicationTypeMaster.application_type_name_in_marathi;
                    fetchApplicationDTL.application_type_in_english = applicationDTL.applicationTypeMaster.application_type_name_in_eng;
                    fetchApplicationDTL.mutation_type_code = applicationDTL.mutation_type_code;
                    fetchApplicationDTL.mutation_type = applicationDTL.mutation_type_name;
                    if (!string.IsNullOrEmpty(applicationDTL.mutation_cts_nos))
                    {
                        string[] mutationCTSNoIDs = applicationDTL.mutation_cts_nos.Split(",");
                        if (mutationCTSNoIDs.Length > 0)
                        {
                            List<NabhuDTLFetchApplicationDTL> nabhuDTLList = new List<NabhuDTLFetchApplicationDTL>();
                            for (int i = 0; i < mutationCTSNoIDs.Length; i++)
                            {
                                FetchMutationCTSNoData mutationCTSNoData = new FetchMutationCTSNoData();
                                NabhuDTLFetchApplicationDTL nabhuData = new NabhuDTLFetchApplicationDTL();
                                mutationCTSNoData = applicationServices.FetchMutationCTSData(Convert.ToInt32(mutationCTSNoIDs[i]));
                                if (mutationCTSNoData != null)
                                {
                                    fetchApplicationDTL.village_code = mutationCTSNoData.villageCode;
                                    fetchApplicationDTL.village_name = mutationCTSNoData.villageName;

                                    if (mutationCTSNoData.milkat == "FLAT" || mutationCTSNoData.namud!.Trim().ToUpper() == "OTHER")
                                    {
                                        nabhuData.naBhu = mutationCTSNoData.naBhu + " (" + mutationCTSNoData.namud + ")";
                                        nabhuData.cityServeyAreaInSqm = mutationCTSNoData.flatBuiltUpArea;
                                    }
                                    else
                                    {
                                        nabhuData.naBhu = mutationCTSNoData.naBhu;
                                        nabhuData.cityServeyAreaInSqm = mutationCTSNoData.cityServeyAreaInSqm;
                                    }
                                    nabhuData.actual_cts_no = mutationCTSNoData.naBhu;
                                    nabhuData.milkat = mutationCTSNoData.milkat!.ToLower();
                                    nabhuData.lrPropertyUID = mutationCTSNoData.lrPropertyUID;
                                    nabhuData.namud = mutationCTSNoData.namud;
                                    nabhuData.sub_property_no = mutationCTSNoData.subPropNo;
                                    nabhuDTLList.Add(nabhuData);
                                }
                            }
                            fetchApplicationDTL.nabhDTL = nabhuDTLList;
                        }
                    }
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Found", fetchApplicationDTL))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Application Data Exception - " + ex.StackTrace!.ToString());
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
        //[Route("SaveSelfDeclaration")]
        //public async Task<string> SaveSelfDeclaration([FromBody] string val)
        ////SelfDeclarationData selfDeclarationData)
        //{
        //    try
        //    {
        //        int UserID = 0;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
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
        //        ReponseType type = ReponseType.Success;
        //        bool check = true;
        //        check = Security.IsBase64String(val);
        //        if (!check)
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("SaveSelfDeclaration - Inout String Is Not Encrypted");
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
        //        }
        //        var decrypted = Security.DeCryptData(val);
        //        SelfDeclarationData selfDeclarationData = JsonConvert.DeserializeObject<SelfDeclarationData>(decrypted!)!;
        //        _logger.LogInformation("Save Self Declaration Request Data - " + selfDeclarationData);

        //        selfDeclarationData.userid = UserID;
        //        string Response = applicationServices.SelfDeclaration(selfDeclarationData);
        //        if (Response == "Success")
        //        {
        //            ApplicationDataForNIC fetchapplication = nICService.GetApplicationDataForNIC(UserID, selfDeclarationData.applicationid!);
        //            //string response = await nICService.callNICAPI(fetchapplication, _logger);
        //            //response = new string((from c in response
        //            //                       where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
        //            //                       select c).ToArray());
        //            //if (long.TryParse(response, out long result))
        //            //{
        //            //    UpdateInwardNoData updateInwardNoData = new UpdateInwardNoData();
        //            //    updateInwardNoData.applicationid = selfDeclarationData.applicationid;
        //            //    updateInwardNoData.inwardno = response;
        //            //    string updateInwardNo = nICService.UpdateInwardnoData(updateInwardNoData);
        //            //}
        //            string response = string.Empty, saveNICResponse = string.Empty;
        //            NicApiResponseModel nicApiResponseModel = new NicApiResponseModel();
        //            for (int i = 1; i <= 3; i++)
        //            {
        //                NICApiRes nICApiRes = new NICApiRes();
        //                nICApiRes = await nICService.CallNICInwardNoAPI(HttpMethod.Post, i, _logger, fetchapplication);
        //                if (nICApiRes.statusCode == "200")
        //                {
        //                    response = nICApiRes.inwardno!;
        //                    response = new string((from c in response
        //                                           where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
        //                                           select c).ToArray());
        //                    if (long.TryParse(response, out long result))
        //                    {
        //                        UpdateInwardNoData updateInwardNoData = new UpdateInwardNoData();
        //                        updateInwardNoData.applicationid = selfDeclarationData.applicationid;
        //                        updateInwardNoData.inwardno = response;
        //                        string updateInwardNoRes = nICService.UpdateInwardnoData(updateInwardNoData);

        //                        nicApiResponseModel.applicationid = selfDeclarationData.applicationid;
        //                        nicApiResponseModel.statuscode = nICApiRes.statusCode;
        //                        nicApiResponseModel.inwardno = updateInwardNoData.inwardno;
        //                        nicApiResponseModel.response = nICApiRes.statusMsg;
        //                        nicApiResponseModel.inwardno_generated = true;
        //                        saveNICResponse = nICService.SaveNICApiResponse(nicApiResponseModel);
        //                        string SMSresponse = sMSService.sendUnicodeSMSForInwardNo(fetchapplication!.usermaster![0].mobileno!, nicApiResponseModel.inwardno);
        //                        _logger.LogInformation("Inward No SMS Response => " + SMSresponse);
        //                        if (SMSresponse == "Success")
        //                        {
        //                            var fileName = $"C:/inetpub/wwwroot/PDE Application/logs/InwardNoSMSLogs/cdacapi-{DateTime.Now:yyyy}-{(DateTime.Now.Month <= 6 ? "H1" : "H2")}.log";
        //                            var _inwardNoSMSLogger = new LoggerConfiguration()
        //                            .WriteTo.File(
        //                            path: fileName,
        //                            rollingInterval: RollingInterval.Infinite
        //                            )
        //                            .CreateLogger();
        //                            _inwardNoSMSLogger.Information("Inward No SMS has sent successfully to " + fetchapplication!.usermaster![0].mobileno! + " this mobile number for Inward No " + nicApiResponseModel.inwardno);
        //                        }
        //                        nICService.SaveApplicationStatusHistory(nicApiResponseModel.applicationid!, "NIC Inward No Generated");
        //                        applicationServices.SaveApplicationDataSubmittedHistory(selfDeclarationData.applicationid!, "NIC Inward No Generated", CallAPIForFlag);
        //                    }
        //                    else
        //                    {
        //                        nicApiResponseModel.applicationid = selfDeclarationData.applicationid;
        //                        nicApiResponseModel.statuscode = nICApiRes.statusCode;
        //                        nicApiResponseModel.inwardno = JsonConvert.DeserializeObject<string>(nICApiRes.inwardno!)!;
        //                        nicApiResponseModel.response = nICApiRes.statusMsg;
        //                        nicApiResponseModel.inwardno_generated = false;
        //                        saveNICResponse = nICService.SaveNICApiResponse(nicApiResponseModel);
        //                    }
        //                    break;
        //                }
        //                //if (nICApiRes.statusCode == "200")
        //                //{
        //                //    response = nICApiRes.inwardno!;
        //                //    response = new string((from c in response
        //                //                           where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
        //                //                           select c).ToArray());
        //                //    if (long.TryParse(response, out long result))
        //                //    {
        //                //        UpdateInwardNoData updateInwardNoData = new UpdateInwardNoData();
        //                //        updateInwardNoData.applicationid = selfDeclarationData.applicationid;
        //                //        updateInwardNoData.inwardno = response;
        //                //        string updateInwardNo = nICService.UpdateInwardnoData(updateInwardNoData);
        //                //        break;
        //                //    }
        //                //}
        //            }
        //            applicationServices.SaveApplicationDataSubmittedHistory(selfDeclarationData.applicationid!, "Self Declaration Form", CallAPIForFlag);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Self Declaration Document Is Uploaded Successfully", ""))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Save Self Declaration Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Save Self Declaration Exception - " + ex.StackTrace!.ToString());
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
        [Route("SaveSelfDeclaration")]
        public async Task<string> SaveSelfDeclaration([FromBody] string val)
        //SelfDeclarationData selfDeclarationData)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("SaveSelfDeclaration - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                SelfDeclarationData selfDeclarationData = JsonConvert.DeserializeObject<SelfDeclarationData>(decrypted!)!;
                _logger.LogInformation("Save Self Declaration Request Data - " + selfDeclarationData);

                selfDeclarationData.userid = UserID;
                //Below code commented on 21 Jan 2026
                //bool validateMutationFlag = true;
                //var fetchdata = await applicationServices.GetMutationApplicationDataForValidate(selfDeclarationData.applicationid!);
                //foreach (ApplicationResultDto item in fetchdata)
                //{
                //    RequestvalidateMultipleMutationApplications requestData = new RequestvalidateMultipleMutationApplications();
                //    requestData.district_code = item.DistrictCode;
                //    requestData.office_code = item.OfficeCode;
                //    requestData.village_code = item.VillageOrPethCode;
                //    requestData.cts_no = item.CityServeyNo;
                //    requestData.subprop_no = item.SubPropertyNo;
                //    requestData.mutation_srno = item.MutationSrNo;
                //    requestData.owner_no = item.OwnerNumber;
                //    var response = await lgdapiServices.validateMultipleMutationApplications(requestData, _logger);
                //    if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                //    {
                //        validateMutationFlag = false;
                //        break;
                //    }
                //}
                //if (validateMutationFlag)
                //{
                    string Response = applicationServices.SelfDeclaration(selfDeclarationData);
                    if (Response == "Success")
                    {
                        ApplicationDataForNIC fetchapplication = nICService.GetApplicationDataForNIC(UserID, selfDeclarationData.applicationid!);
                        string response = string.Empty, saveNICResponse = string.Empty;
                        NicApiResponseModel nicApiResponseModel = new NicApiResponseModel();
                        for (int i = 1; i <= 3; i++)
                        {
                            NICApiRes nICApiRes = new NICApiRes();
                            nICApiRes = await nICService.CallNICInwardNoAPI(HttpMethod.Post, i, _logger, fetchapplication);
                            if (nICApiRes.statusCode == "200")
                            {
                                response = nICApiRes.inwardno!;
                                response = new string((from c in response
                                                       where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                                                       select c).ToArray());
                                if (long.TryParse(response, out long result))
                                {
                                    UpdateInwardNoData updateInwardNoData = new UpdateInwardNoData();
                                    updateInwardNoData.applicationid = selfDeclarationData.applicationid;
                                    updateInwardNoData.inwardno = response;
                                    string updateInwardNoRes = nICService.UpdateInwardnoData(updateInwardNoData);

                                    nicApiResponseModel.applicationid = selfDeclarationData.applicationid;
                                    nicApiResponseModel.statuscode = nICApiRes.statusCode;
                                    nicApiResponseModel.inwardno = updateInwardNoData.inwardno;
                                    nicApiResponseModel.response = nICApiRes.statusMsg;
                                    nicApiResponseModel.inwardno_generated = true;
                                    saveNICResponse = nICService.SaveNICApiResponse(nicApiResponseModel);
                                    string SMSresponse = sMSService.sendUnicodeSMSForInwardNo(fetchapplication!.usermaster![0].mobileno!, nicApiResponseModel.inwardno);
                                    _logger.LogInformation("Inward No SMS Response => " + SMSresponse);
                                    if (SMSresponse == "Success")
                                    {
                                        var fileName = $"C:/inetpub/wwwroot/PDE Application/logs/InwardNoSMSLogs/cdacapi-{DateTime.Now:yyyy}-{(DateTime.Now.Month <= 6 ? "H1" : "H2")}.log";
                                        var _inwardNoSMSLogger = new LoggerConfiguration()
                                        .WriteTo.File(
                                        path: fileName,
                                        rollingInterval: RollingInterval.Infinite
                                        )
                                        .CreateLogger();
                                        _inwardNoSMSLogger.Information("Inward No SMS has sent successfully to " + fetchapplication!.usermaster![0].mobileno! + " this mobile number for Inward No " + nicApiResponseModel.inwardno);
                                    }
                                    nICService.SaveApplicationStatusHistory(nicApiResponseModel.applicationid!, "NIC Inward No Generated");
                                    applicationServices.SaveApplicationDataSubmittedHistory(selfDeclarationData.applicationid!, "NIC Inward No Generated", CallAPIForFlag);
                                }
                                else
                                {
                                    nicApiResponseModel.applicationid = selfDeclarationData.applicationid;
                                    nicApiResponseModel.statuscode = nICApiRes.statusCode;
                                    nicApiResponseModel.inwardno = JsonConvert.DeserializeObject<string>(nICApiRes.inwardno!)!;
                                    nicApiResponseModel.response = nICApiRes.statusMsg;
                                    nicApiResponseModel.inwardno_generated = false;
                                    saveNICResponse = nICService.SaveNICApiResponse(nicApiResponseModel);
                                }
                                break;
                            }
                        }
                        applicationServices.SaveApplicationDataSubmittedHistory(selfDeclarationData.applicationid!, "Self Declaration Form", CallAPIForFlag);
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Self Declaration Document Is Uploaded Successfully", ""))));
                    }
                    else
                    {
                        type = ReponseType.Failure;
                        _logger.LogInformation("Save Self Declaration Response Failed - " + Response);
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                    }
                //}
                //else
                //{
                //    type = ReponseType.Failure;
                //    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "सदर न. भू. क्र. वरील धारकाची या पूर्वी दाखल केलेली फेरफार प्रक्रिया चालू आहे, चालू फेरफारची प्रक्रिया पूर्ण झाल्यानंतरच नवीन अर्ज दाखल करावा.", ""))));
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError("Save Self Declaration Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetSelfDeclaration")]
        public string GetSelfDeclaration([FromBody] string val)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetSelfDeclaration - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Self Declaration Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Submitted Successfully", ""))));
                    //FetchDashboardData fechData = new FetchDashboardData();
                    //fechData = applicationServices.FetchSelfDeclaration(ApplicationID);
                    //if (fechData != null) {

                    //}
                    //else
                    //{
                    //    type = ReponseType.NotFound;
                    //    return Ok(ResponseHandler.GetAppResponse(type, "Self Declaration Data Not Found", ""));
                    //}
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Self Declaration Exception - " + ex.StackTrace!.ToString());
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
        [Route("ReSubmitApplication")]
        public async Task<string> ReSubmitApplication([FromBody] string val)
        //string ApplicationID)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("ReSubmitApplication - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("ReSubmit Request Data - " + ApplicationID);
                string updateInwardNoRes = string.Empty;
                NICApiRes nICApiRes = new NICApiRes();
                ApplicationDataForNIC fetchapplication = nICService.GetApplicationDataForNIC(UserID, ApplicationID);
                string response = string.Empty, saveNICResponse = string.Empty;
                NicApiResponseModel nicApiResponseModel = new NicApiResponseModel();
                if (fetchapplication != null)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        nICApiRes = await nICService.CallNICInwardNoAPI(HttpMethod.Post, i, _logger, fetchapplication);
                        if (nICApiRes.statusCode == "200")
                        {
                            response = nICApiRes.inwardno!;
                            response = new string((from c in response
                                                   where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                                                   select c).ToArray());
                            if (long.TryParse(response, out long result))
                            {
                                UpdateInwardNoData updateInwardNoData = new UpdateInwardNoData();
                                updateInwardNoData.applicationid = ApplicationID;
                                updateInwardNoData.inwardno = response;
                                updateInwardNoRes = nICService.UpdateInwardnoData(updateInwardNoData);

                                nicApiResponseModel.applicationid = ApplicationID;
                                nicApiResponseModel.statuscode = nICApiRes.statusCode;
                                nicApiResponseModel.inwardno = updateInwardNoData.inwardno;
                                nicApiResponseModel.response = nICApiRes.statusMsg;
                                nicApiResponseModel.inwardno_generated = true;
                                saveNICResponse = nICService.SaveNICApiResponse(nicApiResponseModel);
                                string SMSresponse = sMSService.sendUnicodeSMSForInwardNo(fetchapplication!.usermaster![0].mobileno!, nicApiResponseModel.inwardno);
                                _logger.LogInformation("Inward No SMS Response => " + SMSresponse);
                                if (SMSresponse == "Success")
                                {
                                    var fileName = $"C:/inetpub/wwwroot/PDE Application/logs/InwardNoSMSLogs/cdacapi-{DateTime.Now:yyyy}-{(DateTime.Now.Month <= 6 ? "H1" : "H2")}.log";
                                    var _inwardNoSMSLogger = new LoggerConfiguration()
                                    .WriteTo.File(
                                    path: fileName,
                                    rollingInterval: RollingInterval.Infinite
                                    )
                                    .CreateLogger();
                                    _inwardNoSMSLogger.Information("Inward No SMS has sent successfully to " + fetchapplication!.usermaster![0].mobileno! + " this mobile number for Inward No " + nicApiResponseModel.inwardno);
                                }
                                nICService.SaveApplicationStatusHistory(nicApiResponseModel.applicationid!, "NIC Inward No Generated");
                                applicationServices.SaveApplicationDataSubmittedHistory(nicApiResponseModel.applicationid!, "NIC Inward No Generated", CallAPIForFlag);
                            }
                            else
                            {
                                nicApiResponseModel.applicationid = ApplicationID;
                                nicApiResponseModel.statuscode = nICApiRes.statusCode;
                                nicApiResponseModel.inwardno = JsonConvert.DeserializeObject<string>(nICApiRes.inwardno!)!;
                                nicApiResponseModel.response = nICApiRes.statusMsg;
                                nicApiResponseModel.inwardno_generated = false;
                                saveNICResponse = nICService.SaveNICApiResponse(nicApiResponseModel);
                            }
                            break;
                        }
                        //if (nICApiRes.statusCode == "200")
                        //{
                        //    response = nICApiRes.inwardno!;
                        //    response = new string((from c in response
                        //                           where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                        //                           select c).ToArray());
                        //    if (long.TryParse(response, out long result))
                        //    {
                        //        UpdateInwardNoData updateInwardNoData = new UpdateInwardNoData();
                        //        updateInwardNoData.applicationid = ApplicationID;
                        //        updateInwardNoData.inwardno = response;
                        //        updateInwardNoRes = nICService.UpdateInwardnoData(updateInwardNoData);
                        //        break;
                        //    }
                        //}
                    }
                    if (updateInwardNoRes == "Success")
                    {
                        applicationServices.SaveApplicationDataSubmittedHistory(ApplicationID!, "Resumbit Application For Inward No", CallAPIForFlag);
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inward No Updated Successfully", ""))));
                    }
                    else
                    {
                        type = ReponseType.Failure;
                        _logger.LogInformation("ReSubmit Response Failed - " + nICApiRes.statusMsg);
                        if (nICApiRes.statusCode == "500")
                        {
                            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, nICApiRes.statusMsg!, nICApiRes))));
                        }
                        else
                        {
                            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, nICApiRes.inwardno!, nICApiRes))));
                        }
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ReSubmit Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetDashboard")]
        public string GetDashboard()
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

                ApplicationDTL applicationDTL = new ApplicationDTL();
                List<string> result = applicationServices.FetchApplicationIDS(UserID);
                List<FetchDashboardData> data = new List<FetchDashboardData>();
                if (result!.Count > 0)
                {
                    foreach (string applicationid in result)
                    {
                        FetchDashboardData fechData = new FetchDashboardData();
                        fechData = applicationServices.FetchDashboard(applicationid);
                        if (fechData != null)
                        {
                            data.Add(fechData);
                            // return Ok(ResponseHandler.GetAppResponse(type, "Self Declaration Data Found", fechData));
                        }
                    }
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Dashboard Data Found", data))));
                    //FetchDashboardData fechData = new FetchDashboardData();
                    //fechData = applicationServices.FetchDashboard(ApplicationID);
                    //if (fechData != null)
                    //{
                    //    return Ok(ResponseHandler.GetAppResponse(type, "Self Declaration Data Found", fechData));
                    //}
                    //else
                    //{
                    //    type = ReponseType.NotFound;
                    //    return Ok(ResponseHandler.GetAppResponse(type, "Self Declaration Data Not Found", ""));
                    //}
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Dashboard Exception - " + ex.StackTrace!.ToString());
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
        //[Route("RequestOTPForApp")]
        //public string RequestOTPForApp([FromBody] string val)
        //{
        //    try
        //    {
        //        string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        int UserID = 0;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
        //        }
        //        ReponseType type = ReponseType.Success;
        //        string NUM = string.Empty;
        //        NUM = userServices.GenerateOTP();
        //        long OTP = Convert.ToInt64("123456");
        //        var decrypted = Security.DeCryptData(val);
        //        VerificationData verificationData = JsonConvert.DeserializeObject<VerificationData>(decrypted!)!;
        //        _logger.LogInformation("Request OTP For App Request Data - " + verificationData);

        //        // string Response = applicationServices.RequestOTPForApp(verificationData, OTP);
        //        if (verificationData.VERIFICATIONTYPE.Trim().ToUpper() == "MOBILENO")
        //        {
        //            HttpContext.Session.SetString("CheckMobileOTP", OTP.ToString());
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Sent Successfully", ""))));
        //        }
        //        else if (verificationData.VERIFICATIONTYPE.Trim().ToUpper() == "EMAILID")
        //        {
        //            HttpContext.Session.SetString("CheckEmailOTP", OTP.ToString());
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Sent Successfully", ""))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Is Not Send", ""))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Request OTP For App Exception - " + ex.StackTrace!.ToString());
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
        //[Route("VerifyOTPForApp")]
        //public string VerifyOTPForApp([FromBody] string val)
        //{
        //    try
        //    {
        //        string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        int UserID = 0;
        //        var authorization = Request.Headers[HeaderNames.Authorization];
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
        //        }
        //        ReponseType type = ReponseType.Success;
        //        var decrypted = Security.DeCryptData(val);
        //        VerifyOTPData verifyOTPData = JsonConvert.DeserializeObject<VerifyOTPData>(decrypted!)!;
        //        _logger.LogInformation("Verify OTP For App Request Data - " + verifyOTPData);

        //        if (verifyOTPData.VERIFICATIONTYPE!.Trim().ToUpper() != "MOBILENO" && verifyOTPData.VERIFICATIONTYPE.Trim().ToUpper() != "EMAILID")
        //        {
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Verification Type Should Be MOBILENO Or EMAILID", ""))));
        //        }
        //        string OTP = (verifyOTPData.VERIFICATIONTYPE!.Trim().ToUpper() == "MOBILENO") ? HttpContext.Session.GetString("CheckMobileOTP")! : HttpContext.Session.GetString("CheckEmailOTP")!;
        //        if (verifyOTPData.VERIFICATIONTYPE.Trim().ToUpper() == "MOBILENO" && OTP == verifyOTPData.OTP.ToString())
        //        {
        //            HttpContext.Session.Clear();
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Verified Successfully", ""))));
        //        }
        //        else if (verifyOTPData.VERIFICATIONTYPE.Trim().ToUpper() == "EMAILID" && OTP == verifyOTPData.OTP.ToString())
        //        {
        //            HttpContext.Session.Clear();
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Verified Successfully", ""))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Invalid OTP", ""))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Verify OTP For App Exception - " + ex.StackTrace!.ToString());
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

        //// Created New APIS
        [Authorize]
        [HttpPost]
        [Route("GetDocumentUploadedDTLDocTypeWise")]
        public string GetDocumentUploadedDTLDocTypeWise([FromBody] string val)
        //string ApplicationID)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetDocumentUploadedDTLDocTypeWise - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Document Uploaded DTL Request Data - " + ApplicationID);

                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.uploadedDocIDs))
                {
                    List<string> documentTypeIDS = applicationServices.FetchDocumentTypeIDs(ApplicationID);
                    List<FetchUploadedDocDataDocTypeWise> fetchUploadedDocumentsList = new List<FetchUploadedDocDataDocTypeWise>();
                    if (documentTypeIDS.Count > 0)
                    {
                        for (int i = 0; i < documentTypeIDS.Count; i++)
                        {
                            FetchUploadedDocDataDocTypeWise fetchUploadedDocumentsData = new FetchUploadedDocDataDocTypeWise();
                            fetchUploadedDocumentsData = applicationServices.FetchDocTypeWise(documentTypeIDS[i], ApplicationID);
                            if (fetchUploadedDocumentsData != null)
                            {
                                fetchUploadedDocumentsList.Add(fetchUploadedDocumentsData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Uploaded Document Data Found", fetchUploadedDocumentsList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Uploaded Document Data Not Found", fetchUploadedDocumentsList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Document Uploaded DTL Exception - " + ex.StackTrace!.ToString());
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

        //[HttpPost]
        //[Route("UploadFileOnProduction")]
        //public string UploadFileOnProduction([FromBody] string val)
        //{
        //    try
        //    {
        //        ReponseType type = ReponseType.Success;
        //        string Response = applicationServices.UploadFileOnProduction(val);
        //        if (Response == "Success")
        //        {
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "File Uploaded Successfully", "")));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, "")));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.StackTrace!.ToString());
        //        return JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString())));
        //    }
        //}

        //[HttpPost]
        //[Route("UploadFileOnProduction1")]
        //public string UploadFileOnProduction1([FromBody] string val)
        //{
        //    try
        //    {
        //        ReponseType type = ReponseType.Success;
        //        string Response = applicationServices.UploadFileOnProduction1(val);
        //        if (Response == "Success")
        //        {
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "File Uploaded Successfully", "")));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, "")));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.StackTrace!.ToString());
        //        return JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString())));
        //    }
        //}

        //[HttpPost]
        //[Route("UploadFileOnProduction3")]
        //public string UploadFileOnProduction3([FromBody] string val)
        //{
        //    try
        //    {
        //        ReponseType type = ReponseType.Success;
        //        string Response = applicationServices.UploadFileOnProduction3(val);
        //        if (Response == "Success")
        //        {
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "File Uploaded Successfully", "")));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, "")));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.StackTrace!.ToString());
        //        return JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString())));
        //    }
        //}

        //[HttpPost]
        //[Route("UploadFileOnProduction4")]
        //public string UploadFileOnProduction4([FromBody] string val)
        //{
        //    try
        //    {
        //        ReponseType type = ReponseType.Success;
        //        string Response = applicationServices.UploadFileOnProduction4(val);
        //        if (Response == "Success")
        //        {
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "File Uploaded Successfully", "")));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, "")));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.StackTrace!.ToString());
        //        return JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString())));
        //    }
        //}

        //[HttpPost]
        //[Route("UploadFileOnProduction5")]
        //public string UploadFileOnProduction5([FromBody] string val)
        //{
        //    try
        //    {
        //        ReponseType type = ReponseType.Success;
        //        string Response = applicationServices.UploadFileOnProduction5(val);
        //        if (Response == "Success")
        //        {
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "File Uploaded Successfully", "")));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, "")));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("File Upload Exception - " + ex.StackTrace!.ToString());
        //        return JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString())));
        //    }
        //}

        //[HttpPost]
        //[Route("CheckFileExists")]
        //public string CheckFileExists([FromBody] string val)
        //{
        //    try
        //    {
        //        ReponseType type = ReponseType.Success;
        //        string Response = applicationServices.CheckFileExists(val);
        //        if (Response == "Success")
        //        {
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Check File Exists", "")));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, "")));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Check File Exists Exception - " + ex.StackTrace!.ToString());
        //        return JsonConvert.SerializeObject(Unauthorized(ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString())));
        //    }
        //}

        //async Task<string>
        [HttpPost]
        [Route("RequestOTPForApp")]
        public string RequestOTPForApp([FromBody] string val)
        //string mobileno)
        {
            try
            {
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("RequestOTPForApp - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                string OTP = userServices.GenerateOTP();
                var decrypted = Security.DeCryptData(val);
                string mobileno = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Request OTP For App Request Data - " + mobileno);

                string Response = sMSService.RequestOTPForApp(mobileno, Convert.ToInt64(OTP), _logger);
                if (Response == "Success")
                {
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Sent Successfully", "")));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Sent Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Request OTP For App Response Failed - " + Response);
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, "")));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Request OTP For App Exception - " + ex.StackTrace!.ToString());
                //return JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString())));
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        [HttpPost]
        [Route("VerifyOTPForApp")]
        public string VerifyOTPForApp([FromBody] string val)
        //VerifyMobileOTP verifyMobileOTP)
        {
            try
            {
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("VerifyOTPForApp - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                VerifyMobileOTP verifyMobileOTP = JsonConvert.DeserializeObject<VerifyMobileOTP>(decrypted!)!;
                _logger.LogInformation("Verify OTP For App Request Data - " + verifyMobileOTP.mobileno);
                string Response = sMSService.VerifyOTPForApp(verifyMobileOTP.mobileno!, verifyMobileOTP.otp);
                if (Response == "Success")
                {
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Verified Successfully", "")));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Verified Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Verify OTP For App Response Failed - " + Response);
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetFailure("Wrong OTP")));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetFailure("Wrong OTP"))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Verify OTP For App Exception - " + ex.StackTrace!.ToString());
                //return JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString())));
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("SaveTrutiDocumentUpload")]
        public string SaveTrutiDocumentUpload([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    // UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("SaveTrutiDocumentUpload - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                AddTrutiUploadedDocumentsDTLData addUploadedDocuments = JsonConvert.DeserializeObject<AddTrutiUploadedDocumentsDTLData>(decrypted!)!;
                _logger.LogInformation("Save Truti Patra Document Upload Request Data - " + decrypted);
                addUploadedDocuments.userid = UserID;
                List<FetchTrutiUploadedDocumentsData> fetchDocDataList = new List<FetchTrutiUploadedDocumentsData>();
                string Response = applicationServices.SaveTrutiDocUploadedData(addUploadedDocuments);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(addUploadedDocuments.applicationid!);
                    string[] uploadedDocIDS = applicationDTL.uploadedDocIDs!.Split(",");
                    if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.uploadedDocIDs))
                    {
                        fetchDocDataList = applicationServices.FetchTrutiUploadedDocumentIDs(applicationDTL.applicationid!);
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(addUploadedDocuments.applicationid!, "Save Truti Patra Document Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Truti Patra Document Is Uploaded Successfully", fetchDocDataList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Save Truti Document Upload Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Save Truti Document Upload Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetTrutiDocumentUploadedDTL")]
        public string GetTrutiDocumentUploadedDTL([FromBody] string val)
        //string ApplicationID)
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
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetTrutiDocumentUploadedDTL - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Truti Patra Document Uploaded DTL Request Data - " + ApplicationID);

                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.uploadedDocIDs))
                {
                    List<FetchTrutiUploadedDocumentsData> filenames = applicationServices.FetchTrutiUploadedDocumentIDs(ApplicationID);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Uploaded Document Data Found", filenames))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Document Uploaded DTL Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteTrutiPatraDocumentUpload")]
        public string DeleteTrutiPatraDocumentUpload([FromBody] string val)
        {
            try
            {
                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteTrutiPatraDocumentUpload - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteAddUploadedDocumentsDTLData addUploadedDocuments = JsonConvert.DeserializeObject<DeleteAddUploadedDocumentsDTLData>(decrypted!)!;
                _logger.LogInformation("Delete Truti Patra Document Upload Request Data - " + decrypted);
                string Response = applicationServices.DeleteDocUploadedData(addUploadedDocuments);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(addUploadedDocuments.applicationid!, "Delete Truti Patra Document Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Truti Patra Document Is Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Truti Patra Document Upload Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Truti Patra Document Upload Exception - " + ex.StackTrace!.ToString());
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
