using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.ContractRepo;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;
using PDEWebAPIS.Services;
using PDEWebAPIS.ViewModel;
using PDEWebAPIS.ViewModel.ModelForNICData;
using System;
using System.Net.Http.Headers;

namespace PDEWebAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NICAPISController : ControllerBase
    {
        private readonly NICService nicServices;
        private readonly UserServices userServices;
        private readonly ApplicationServices applicationServices;
        private readonly IConfiguration configuration;
        private readonly ILogger<NICAPISController> _logger;
        private readonly ILogger<TestFileUpload> _loggerTest;
        private readonly ILogger<UserServices> _loggeruserService;
        private readonly IMapper _mapper;

        public NICAPISController(AppDBContext context, IConfiguration configuration, ILogger<NICAPISController> logger, AppDBContextR contextR,  ILogger<UserServices> loggeruserservice, ICommonRepository commonRepository, IMapper mapper)
        {
            _loggeruserService = loggeruserservice;
            nicServices = new NICService(context, contextR,  loggeruserservice, commonRepository, mapper);
            userServices = new UserServices(context, contextR, loggeruserservice);
            applicationServices = new ApplicationServices(context, contextR,  loggeruserservice);
            this.configuration = configuration;
            _logger = logger;
            //_loggerTest = loggerTest;
        }

        [HttpPost]
        [Route("GetNICInwardNo")]
        public async Task<string> GetNICInwardNo([FromBody] string val)
        {
            try
            {
                //int UserID = 0;
                //ApplicationDTL applicationDTL = new ApplicationDTL();
                //applicationDTL = nicServices.FetchApplicationData(ApplicationID);
                //if (applicationDTL != null)
                //{
                //UserID = applicationDTL!.userMaster!.userid;

                ApplicationDataForNIC fetchapplication = JsonConvert.DeserializeObject<ApplicationDataForNIC>(val!)!;
                //fetchapplication = nicServices.GetApplicationDataForNIC(6, ApplicationID);
                _logger.LogInformation("Get NIC Inward No Request Application ID - " + fetchapplication.applicationdtl![0].applicationid);
                string response = string.Empty;
                for (int i = 1; i <= 3; i++)
                {
                    NICApiRes nICApiRes = new NICApiRes();
                    nICApiRes = await nicServices.CallNICInwardNoAPI(HttpMethod.Post, i, _logger, fetchapplication);
                    if (nICApiRes.statusCode == "200")
                    {
                        response = nICApiRes.inwardno!;
                        response = new string((from c in response
                                               where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                                               select c).ToArray());
                        if (long.TryParse(response, out long result))
                        {
                            break;
                        }
                    }
                    else
                    {
                        response = nICApiRes.statusCode + " " + nICApiRes.statusMsg;
                    }
                }
                _logger.LogInformation("Get NIC Inward No Response - " + response);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Inward No Exception - " + ex.StackTrace!.ToString());
                return JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString())));
                //return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        [HttpPost]
        [Route("GetApplicationDataforNIC")]
        public string GetApplicationDataforNIC(string ApplicationID)
        {
            try
            {
                int UserID = 0;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = nicServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null)
                {
                    UserID = applicationDTL!.userMaster!.userid;
                    var fetchapplication = nicServices.GetApplicationDataForNIC(UserID, ApplicationID);
                    return JsonConvert.SerializeObject(fetchapplication);
                }
                else
                {
                    string message = "Application ID Not Found";
                    //return NotFound(JsonConvert.SerializeObject(message));
                    return JsonConvert.SerializeObject(message);
                    //return JsonConvert.SerializeObject(NotFound(message));
                }
                //return Security.EnCryptDataNIC(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Found", fetchapplication))));
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Application Data For NIC Exception - " + ex.StackTrace!.ToString());
                return JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString())));
                //return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        [HttpPost]
        [Route("UpdateInwardNo")]
        public string UpdateInwardNo(UpdateInwardNoData updateInwardNoData)
        {
            try
            {
                _logger.LogInformation("Update Inward No Request Data - " + updateInwardNoData);
                //ApplicantData.userId = UserID;
                string Response = nicServices.UpdateInwardnoData(updateInwardNoData);
                if (Response == "Success")
                {
                    return JsonConvert.SerializeObject("Inward No Updated Successfully");
                }
                else
                {
                    _logger.LogInformation("Update Inward No Response Failed - " + Response);
                    return JsonConvert.SerializeObject(Response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Update Inward No Exception - " + ex.StackTrace!.ToString());
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

        [HttpPost]
        [Route("SaveNicDocUpload")]
        public IActionResult SaveNicDocUpload(NicDocUploadData nicDocUploadData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _logger.LogInformation("SaveNicDocUpload Request Data - " + nicDocUploadData);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = nicServices.FetchApplicationDataForInwardNo(nicDocUploadData.applicationid!, nicDocUploadData.inwardno!);
                if (applicationDTL != null)
                {
                    nicDocUploadData.userid = applicationDTL!.userMaster!.userid;
                    string Response = nicServices.insertDocUploadData(nicDocUploadData);
                    if (Response == "Success")
                    {
                        return Ok(JsonConvert.SerializeObject("Document Data Saved Successfully"));
                    }
                    else
                    {
                        _logger.LogInformation("SaveNicDocUpload Response Failed - " + Response);
                        return StatusCode(500, new { message = "SaveNicDocUpload Response Failed.", details = Response });
                    }
                }
                else
                {
                    _logger.LogInformation("SaveNicDocUpload Response Failed -  Application ID Or Inward No Not Found");
                    return NotFound("Application ID Or Inward No Not Found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SaveNicDocUpload Exception - " + ex.StackTrace!.ToString());
                return StatusCode(500, new { message = "An unexpected error occured.", details = ex.Message.ToString() });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CallNICTrutiPatraDTL")]
        public string CallNICTrutiPatraDTL([FromBody] string val)
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
                    _logger.LogInformation("CallNICTrutiPatraDTL - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Document Uploaded DTL Request Data - " + ApplicationID);

                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchUploadedDocumentDataForNIC> docDataList = new List<FetchUploadedDocumentDataForNIC>();
                NICTrutiPatraAPIResponse nicresponse = new NICTrutiPatraAPIResponse();
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.uploadedDocIDs))
                {
                    List<string> uploadedDocumentIDS = nicServices.FetchTrutiUploadedDocumentIDs(ApplicationID);
                    foreach (string documentID in uploadedDocumentIDS)
                    {
                        FetchUploadedDocumentDataForNIC fetchData = new FetchUploadedDocumentDataForNIC();
                        fetchData = nicServices.FetchUploadedDocumentDTLsForTrutiPatra(Convert.ToInt32(documentID));
                        docDataList.Add(fetchData);
                    }
                    if (docDataList != null)
                    {
                        NICTrutiPatraData nICTrutiPatraData = new NICTrutiPatraData();
                        nICTrutiPatraData.uploaded_documents_dtl = docDataList;
                        string response = string.Empty, saveResponseStatus = string.Empty;
                        NicApiResponseModel nicApiResponseModel = new NicApiResponseModel();
                        int attempt = 3;
                        for (int i = 1; i <= attempt; i++)
                        {
                            _logger.LogInformation("Call NIC Truti Patra API For Application ID - " + ApplicationID);
                            nicresponse = nicServices.CallNICTrutiPatraDTLAPI(HttpMethod.Post, i, _logger, nICTrutiPatraData);
                            if (nicresponse.statusCode == "200")
                            {
                                response = nicServices.UpdateApplicationStatusForTruti(ApplicationID);

                                SaveExternalAPIResponse saveExternalAPIResponse = new SaveExternalAPIResponse();
                                saveExternalAPIResponse.applicationid = ApplicationID;
                                saveExternalAPIResponse.vendorname = "NIC";
                                saveExternalAPIResponse.api = nicresponse.apiname;
                                saveExternalAPIResponse.statuscode = nicresponse.statusCode;
                                saveExternalAPIResponse.response = JsonConvert.DeserializeObject<string>(nicresponse.response!)!;
                                saveResponseStatus = nicServices.SaveNICApiResponse(saveExternalAPIResponse);
                                nicServices.SaveApplicationStatusHistory(ApplicationID, "Truti Patra Submitted To NIC");
                                break;
                            }
                            //else
                            //{
                            //    SaveExternalAPIResponse saveExternalAPIResponse = new SaveExternalAPIResponse();
                            //    saveExternalAPIResponse.applicationid = ApplicationID;
                            //    saveExternalAPIResponse.vendorname = "NIC";
                            //    saveExternalAPIResponse.api = nicresponse.apiname;
                            //    saveExternalAPIResponse.statuscode = nicresponse.statusCode;
                            //    saveExternalAPIResponse.response = nicresponse.response;
                            //    saveResponseStatus = nicServices.SaveNICApiResponse(saveExternalAPIResponse);
                            //}
                        }
                        if (response == "Success")
                        {
                            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Truti Patra Documents Submitted To NIC Successfully", response))));
                        }
                        else
                        {
                            type = ReponseType.Failure;
                            _logger.LogInformation("CallNICTrutiPatraDTL Response Failed - " + nicresponse.statusMsg);
                            if (nicresponse.statusCode == "500")
                            {
                                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, nicresponse.statusMsg!, nicresponse))));
                            }
                            else
                            {
                                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, nicresponse.response!, nicresponse))));
                            }
                        }
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Uploaded Truti Patra Document Data Not Found", ""))));
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
                _logger.LogError("Call NIC Truti Patra DTL Exception - " + ex.StackTrace!.ToString());
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
