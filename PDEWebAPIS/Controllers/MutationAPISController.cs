using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.ContractRepo;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.InputDataModel.GahankhatNond;
using PDEWebAPIS.InputDataModel.MrutyuPatra;
using PDEWebAPIS.Repository;
using PDEWebAPIS.Services;
using PDEWebAPIS.ViewModel;
using System.Net.Http.Headers;

namespace PDEWebAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MutationAPISController : ControllerBase
    {
        private readonly DBHelper dbHelper;
        private readonly UserServices userServices;
        private readonly MutationServices mutationServices;
        private readonly ApplicationServices applicationServices;
        private readonly NICService nicServices;
        private readonly IConfiguration configuration;
        private readonly ILogger<MutationAPISController> _logger;
        private readonly ILogger<TestFileUpload> _loggerTest;
        private readonly ILogger<UserServices> _loggeruserService;
        //private readonly ICommonRepository _ICommonRepository;
        //private readonly IMapper _mapper;

        public MutationAPISController(AppDBContext context, IConfiguration configuration, ILogger<MutationAPISController> logger, AppDBContextR contextR, ILogger<UserServices> loggeruserService, ICommonRepository commonRepository, IMapper mapper)
        {
            _loggeruserService = loggeruserService;
            dbHelper = new DBHelper(context);
            userServices = new UserServices(context, contextR, loggeruserService);
            mutationServices = new MutationServices(context, contextR, loggeruserService, commonRepository, mapper);
            applicationServices = new ApplicationServices(context, contextR, loggeruserService);
            nicServices = new NICService(context, contextR, loggeruserService, commonRepository, mapper);
            this.configuration = configuration;
            _logger = logger;
            //_loggerTest = loggerTest;
        }

        //Add Encryption Decryption Logic

        /*[HttpPost]
        [Route("Getname")]
        public List<FetchMutationName> FetchMutationName(string applicationid) {
            var list = mutationServices.GetMutationName(applicationid);
            return list;    
        }*/

        [HttpPost]
        [Route("DecryptData")]
        public string DecryptData([FromBody] string val)
        {
            var decrypted = Security.DeCryptData(val);
            return JsonConvert.SerializeObject(decrypted);
        }

        [HttpPost]
        [Route("EncryptData")]
        public string EncryptData([FromBody] string val)
        {
            var decrypted = Security.EnCryptData(val);
            return JsonConvert.SerializeObject(decrypted);
        }

        //[HttpPost]
        //[Route("EncryptData")]
        //public string EncryptData([FromBody] string val)
        //{
        //    return JsonConvert.SerializeObject(Security.EnCryptData(val));
        //}

        [Authorize]
        [HttpPost]
        [Route("GetMutationName")]
        public string GetMutationName([FromBody] string val)
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
                    _logger.LogInformation("GetMutationName - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Mutation Name Data Request Data - " + ApplicationID);

                var fetchapplication = mutationServices.GetMutationName(ApplicationID);
                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation name Data Found", fetchapplication))));
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Mutation Name Data Exception - " + ex.StackTrace!.ToString());
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
        [Route("CreateKharediNondForGiver")]
        public string CreateKharediNondForGiver([FromBody] string val)
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
                    _logger.LogInformation("CreateKharediNondForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                KharediNondDataForGiver kharediNondDataForGiver = JsonConvert.DeserializeObject<KharediNondDataForGiver>(decrypted!)!;
                _logger.LogInformation("Create Kharedi Nond For Giver Request Data - " + decrypted);

                kharediNondDataForGiver.userid = UserID;
                string Response = mutationServices.SaveKharediNondsGiver(kharediNondDataForGiver);
                if (Response == "Success")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(kharediNondDataForGiver.applicationid!);
                    //string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    //List<FetchKharediNondDataForGiver> fetchKharediNondList = new List<FetchKharediNondDataForGiver>();
                    //for (int i = 0; i < kharediNondIDs.Length; i++)
                    //{
                    //    FetchKharediNondDataForGiver fetchkhardinondData = new FetchKharediNondDataForGiver();
                    //    fetchkhardinondData = mutationServices.FetchKhrediNondInformationDataForGiver(Convert.ToInt32(kharediNondIDs[i]));
                    //    if (fetchkhardinondData != null)
                    //    {
                    //        fetchKharediNondList.Add(fetchkhardinondData);
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(kharediNondDataForGiver.applicationid!, "Create Kharedi Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Kharedi Nond Is Created Successfully", ""))));
                }
                else if (Response == "Update")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(kharediNondDataForGiver.applicationid!);
                    //string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    //List<FetchKharediNondDataForGiver> fetchKharediNondList = new List<FetchKharediNondDataForGiver>();
                    //for (int i = 0; i < kharediNondIDs.Length; i++)
                    //{
                    //    FetchKharediNondDataForGiver fetchkhardinondData = new FetchKharediNondDataForGiver();
                    //    fetchkhardinondData = mutationServices.FetchKhrediNondInformationDataForGiver(Convert.ToInt32(kharediNondIDs[i]));
                    //    if (fetchkhardinondData != null)
                    //    {
                    //        fetchKharediNondList.Add(fetchkhardinondData);
                    //    }
                    //}
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Kharedi Nond Is Updated Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Kharedi Nond For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Kharedi Nond For Giver Exception - " + ex.StackTrace!.ToString());
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

        /* [Authorize]
         [HttpPost]
         [Route("CreateKharediNondForGiver")]
         public IActionResult CreateKharediNondForGiver(KharediNondDataForGiver kharediNondDataForGiver)
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
                     // we have a valid AuthenticationHeaderValue that has the following details:
                     var scheme = headerValue.Scheme;
                     var Token = headerValue.Parameter;
                     UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                     // scheme will be "Bearer"
                     // parmameter will be the token itself.
                 }
                 kharediNondDataForGiver.userid = UserID;
                 ReponseType type = ReponseType.Success;
                 string Response = mutationServices.SaveKharediNondsGiver(kharediNondDataForGiver);
                 if (Response == "Success")
                 {
                     ApplicationDTL applicationDTL = new ApplicationDTL();
                     applicationDTL = applicationServices.FetchApplicationData(kharediNondDataForGiver.applicationid!);
                     string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                     List<FetchKharediNondDataForGiver> fetchKharediNondList = new List<FetchKharediNondDataForGiver>();
                     for (int i = 0; i < kharediNondIDs.Length; i++)
                     {
                         FetchKharediNondDataForGiver fetchkhardinondData = new FetchKharediNondDataForGiver();
                         fetchkhardinondData = mutationServices.FetchKhrediNondInformationDataForGiver(Convert.ToInt32(kharediNondIDs[i]));
                         if (fetchkhardinondData != null)
                         {
                             fetchKharediNondList.Add(fetchkhardinondData);
                         }
                     }
                     return Ok(ResponseHandler.GetAppResponse(type, "Kharedi Nond Is Created Successfully", fetchKharediNondList));
                 }
                 else
                 {
                     type = ReponseType.Failure;
                     return Ok(ResponseHandler.GetAppResponse(type, Response, ""));
                 }
             }
             catch (Exception ex)
             {
                 return BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()));
             }
         }
 */
        [Authorize]
        [HttpPost]
        [Route("EditKharediNondForGiver")]
        public string EditKharediNondForGiver([FromBody] string val)
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
                    _logger.LogInformation("EditKharediNondForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditKharediNondDataForGiver kharediNondDataForGiver = JsonConvert.DeserializeObject<EditKharediNondDataForGiver>(decrypted!)!;
                _logger.LogInformation("Edit Kharedi Nond For Giver Request Data - " + decrypted);
                kharediNondDataForGiver.userid = UserID;
                string Response = mutationServices.EditKharediNondsGiver(kharediNondDataForGiver);
                if (Response == "Success")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(kharediNondDataForGiver.applicationid!);
                    //string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    //List<FetchKharediNondDataForGiver> fetchKharediNondList = new List<FetchKharediNondDataForGiver>();
                    //for (int i = 0; i < kharediNondIDs.Length; i++)
                    //{
                    //    FetchKharediNondDataForGiver fetchkhardinondData = new FetchKharediNondDataForGiver();
                    //    fetchkhardinondData = mutationServices.FetchKhrediNondInformationDataForGiver(Convert.ToInt32(kharediNondIDs[i]));
                    //    if (fetchkhardinondData != null)
                    //    {
                    //        fetchKharediNondList.Add(fetchkhardinondData);
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(kharediNondDataForGiver.applicationid!, "Edit Kharedi Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Kharedi Nond Giver is Updated Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Kharedi Nond For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Kharedi Nond For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteKharediNondForGiver")]
        public string DeleteKharediNondForGiver([FromBody] string val)
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
                    _logger.LogInformation("DeleteKharediNondForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Kharedi Nond For Giver Request Data - " + decrypted);
                string Response = string.Empty;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    if (kharediNondIDs.Length > 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                    }
                    if (kharediNondIDs.Length == 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                        if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                        {
                            string[] takerids = applicationDTL.mutationtakerIDs!.Split(",");
                            if (takerids.Length > 0)
                            {
                                for (int i = 0; i < takerids.Length; i++)
                                {
                                    delete.MutationId = Convert.ToInt32(takerids[i]);
                                    Response = mutationServices.DeleteMutationTaker(delete);
                                }
                            }
                        }
                    }
                }
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Kharedi Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Kharedi Nond Giver Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Kharedi Nond For Giver Exception - " + ex.Source!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }


        [Authorize]
        [HttpPost]
        [Route("CreateKharediNondForTaker")]
        public string CreateKharediNondForTaker(
        //KharediNondDataForTaker kharediNondDataForTaker)
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
                    _logger.LogInformation("CreateKharediNondForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                KharediNondDataForTaker kharediNondDataForTaker = JsonConvert.DeserializeObject<KharediNondDataForTaker>(decrypted!)!;
                _logger.LogInformation("Create Kharedi Nond For Taker Request Data - " + decrypted);
                kharediNondDataForTaker.userid = UserID;
                string Response = mutationServices.SaveKharediNondTaker(kharediNondDataForTaker);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(kharediNondDataForTaker.applicationid!);
                    string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    List<FetchKharediNondDataForTaker> fetchKharediNondList = new List<FetchKharediNondDataForTaker>();
                    for (int i = 0; i < kharediNondIDs.Length; i++)
                    {
                        FetchKharediNondDataForTaker fetchkhardinondData = new FetchKharediNondDataForTaker();
                        fetchkhardinondData = mutationServices.FetchKhrediNondInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
                        if (fetchkhardinondData != null)
                        {
                            fetchKharediNondList.Add(fetchkhardinondData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(kharediNondDataForTaker.applicationid!, "Create Kharedi Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Kharedi Nond Is Created Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Kharedi Nond For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Kharedi Nond For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditKharediNondForTaker")]
        public string EditKharediNondForTaker([FromBody] string val)
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
                    _logger.LogInformation("EditKharediNondForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditKharediNondDataForTaker kharediNondDataForTaker = JsonConvert.DeserializeObject<EditKharediNondDataForTaker>(decrypted!)!;
                _logger.LogInformation("Edit Kharedi Nond For Taker Request Data - " + decrypted);
                kharediNondDataForTaker.userid = UserID;
                string Response = mutationServices.EditKharediNondTaker(kharediNondDataForTaker);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(kharediNondDataForTaker.applicationid!);
                    string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    List<FetchKharediNondDataForTaker> fetchKharediNondList = new List<FetchKharediNondDataForTaker>();
                    for (int i = 0; i < kharediNondIDs.Length; i++)
                    {
                        FetchKharediNondDataForTaker fetchkhardinondData = new FetchKharediNondDataForTaker();
                        fetchkhardinondData = mutationServices.FetchKhrediNondInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
                        if (fetchkhardinondData != null)
                        {
                            fetchKharediNondList.Add(fetchkhardinondData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(kharediNondDataForTaker.applicationid!, "Edit Kharedi Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Kharedi Nond Taker Data Updated Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Kharedi Nond For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Kharedi Nond For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteKharediNondForTaker")]
        public string DeleteKharediNondForTaker([FromBody] string val)
        //DeleteMutation delete)
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
                    _logger.LogInformation("DeleteKharediNondForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Kharedi Nond For Taker Request Data - " + decrypted);
                string Response = mutationServices.DeleteMutationTaker(delete);
                List<FetchKharediNondDataForTaker> fetchKharediNondList = new List<FetchKharediNondDataForTaker>();
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                    if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                    {
                        string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchKharediNondDataForTaker fetchkhardinondData = new FetchKharediNondDataForTaker();
                            fetchkhardinondData = mutationServices.FetchKhrediNondInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchkhardinondData != null)
                            {
                                fetchKharediNondList.Add(fetchkhardinondData);
                            }
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Kharedi Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Kharedi Nond Taker Data Deleted Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Kharedi Nond For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchKharediNondList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Kharedi Nond For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetKharediNondGiverInfo")]
        public string GetKharediNondGiverInfo([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetKharediNondGiverInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Kharedi Nond Giver Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchKharediNondDataForGiver> fetchKharediNondList = new List<FetchKharediNondDataForGiver>();
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mutationgiverIDs.Split(",");
                    if (kharediNondIDs.Length > 0)
                    {
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchKharediNondDataForGiver fetchKharediNondGiverInformationData = new FetchKharediNondDataForGiver();
                            fetchKharediNondGiverInformationData = mutationServices.FetchKhrediNondInformationDataForGiver(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchKharediNondGiverInformationData != null)
                            {
                                fetchKharediNondList.Add(fetchKharediNondGiverInformationData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Kharedi Nond Giver Information Data Found", fetchKharediNondList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchKharediNondList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Giver ID Is Not Exists For The Given Application ID", fetchKharediNondList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Kharedi Nond Giver Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetKharediNondTakerInfo")]
        public string GetKharediNondTakerInfo([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetKharediNondTakerInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Kharedi Nond Taker Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchKharediNondDataForTaker> fetchKharediNondList = new List<FetchKharediNondDataForTaker>();
                if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    if (kharediNondIDs.Length > 0)
                    {
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchKharediNondDataForTaker fetchKharediNondGiverInformationData = new FetchKharediNondDataForTaker();
                            fetchKharediNondGiverInformationData = mutationServices.FetchKhrediNondInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchKharediNondGiverInformationData != null)
                            {
                                fetchKharediNondList.Add(fetchKharediNondGiverInformationData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Kharedi Nond Taker Information Data Found", fetchKharediNondList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchKharediNondList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Taker ID Is Not Exists For The Given Application ID", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Kharedi Nond Taker Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("CreateBakshishPatraForGiver")]
        public string CreateBakshishPatraForGiver([FromBody] string val)
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
                    _logger.LogInformation("CreateBakshishPatraForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                BakshishPatraGiverData bakshishpatraDataForGiver = JsonConvert.DeserializeObject<BakshishPatraGiverData>(decrypted!)!;
                _logger.LogInformation("Create Bakshish Patra For Giver Request Data - " + decrypted);
                bakshishpatraDataForGiver.userid = UserID;
                string Response = mutationServices.SaveBakshishPatraGiver(bakshishpatraDataForGiver);
                if (Response == "Success")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(bakshishpatraDataForGiver.applicationid!);
                    //string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    //List<FetchBakshishPatraDataForGiver> fetchKharediNondList = new List<FetchBakshishPatraDataForGiver>();
                    //for (int i = 0; i < kharediNondIDs.Length; i++)
                    //{
                    //    FetchBakshishPatraDataForGiver fetchkhardinondData = new FetchBakshishPatraDataForGiver();
                    //    fetchkhardinondData = mutationServices.FetchBakshishPatraInformationDataForGiver(Convert.ToInt32(kharediNondIDs[i]));
                    //    if (fetchkhardinondData != null)
                    //    {
                    //        fetchKharediNondList.Add(fetchkhardinondData);
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(bakshishpatraDataForGiver.applicationid!, "Create Bakshish Patra Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bakshish Patra Is Created Successfully", ""))));
                }
                else if (Response == "Update")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(bakshishpatraDataForGiver.applicationid!);
                    //string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    //List<FetchBakshishPatraDataForGiver> fetchKharediNondList = new List<FetchBakshishPatraDataForGiver>();
                    //for (int i = 0; i < kharediNondIDs.Length; i++)
                    //{
                    //    FetchBakshishPatraDataForGiver fetchkhardinondData = new FetchBakshishPatraDataForGiver();
                    //    fetchkhardinondData = mutationServices.FetchBakshishPatraInformationDataForGiver(Convert.ToInt32(kharediNondIDs[i]));
                    //    if (fetchkhardinondData != null)
                    //    {
                    //        fetchKharediNondList.Add(fetchkhardinondData);
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(bakshishpatraDataForGiver.applicationid!, "Create Bakshish Patra Giver Data Updated Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bakshish Patra Is Updated Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Bakshish Patra For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Bakshish Patra For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditBakshishPatraForGiver")]
        public string EditBakshishPatraForGiver([FromBody] string val)
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
                    _logger.LogInformation("EditBakshishPatraForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditBakshishPatraGiverData bakshishpatraDataForGiver = JsonConvert.DeserializeObject<EditBakshishPatraGiverData>(decrypted!)!;
                _logger.LogInformation("Edit Bakshish Patra For Giver Request Data - " + decrypted);
                bakshishpatraDataForGiver.userid = UserID;
                string Response = mutationServices.EditBakshishPatraGiver(bakshishpatraDataForGiver);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(bakshishpatraDataForGiver.applicationid!);
                    string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    List<FetchBakshishPatraDataForGiver> fetchKharediNondList = new List<FetchBakshishPatraDataForGiver>();
                    for (int i = 0; i < kharediNondIDs.Length; i++)
                    {
                        FetchBakshishPatraDataForGiver fetchkhardinondData = new FetchBakshishPatraDataForGiver();
                        fetchkhardinondData = mutationServices.FetchBakshishPatraInformationDataForGiver(Convert.ToInt32(kharediNondIDs[i]));
                        if (fetchkhardinondData != null)
                        {
                            fetchKharediNondList.Add(fetchkhardinondData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(bakshishpatraDataForGiver.applicationid!, "Edit Bakshish Patra Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bakshish Patra Giver Updated Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Bakshish Patra For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Bakshish Patra For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteBakshishPatraForGiver")]
        public string DeleteBakshishPatraForGiver([FromBody] string val)
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
                // bakshishpatraDataForGiver.userid = UserID;
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteBakshishPatraForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Bakshish Patra For Giver Request Data - " + decrypted);
                string Response = string.Empty;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    if (kharediNondIDs.Length > 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                    }
                    if (kharediNondIDs.Length == 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                        if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                        {
                            string[] takerids = applicationDTL.mutationtakerIDs!.Split(",");
                            if (takerids.Length > 0)
                            {
                                for (int i = 0; i < takerids.Length; i++)
                                {
                                    delete.MutationId = Convert.ToInt32(takerids[i]);
                                    Response = mutationServices.DeleteMutationTaker(delete);
                                }
                            }
                        }
                    }
                }
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Bakshish Patra Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bakshish Patra Giver Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Bakshish Patra For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Bakshish Patra For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetBakshishPatraGiverInfo")]
        public string GetBakshishPatraGiverInfo([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetBakshishPatraGiverInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Bakshish Patra Giver Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchBakshishPatraDataForGiver> fetchKharediNondList = new List<FetchBakshishPatraDataForGiver>();
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] mutationgiverIDs = applicationDTL.mutationgiverIDs.Split(",");
                    if (mutationgiverIDs.Length > 0)
                    {
                        for (int i = 0; i < mutationgiverIDs.Length; i++)
                        {
                            FetchBakshishPatraDataForGiver fetchKharediNondGiverInformationData = new FetchBakshishPatraDataForGiver();
                            fetchKharediNondGiverInformationData = mutationServices.FetchBakshishPatraInformationDataForGiver(Convert.ToInt32(mutationgiverIDs[i]));
                            if (fetchKharediNondGiverInformationData != null)
                            {
                                fetchKharediNondList.Add(fetchKharediNondGiverInformationData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bakshish Patra Giver Information Data Found", fetchKharediNondList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchKharediNondList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Giver ID Is Not Exists For The Given Application ID", fetchKharediNondList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Bakshish Patra Giver Info Exception - " + ex.StackTrace!.ToString());
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
        //[Route("CreateBakshishPatraForTaker")]
        //public IActionResult CreateBakshishPatraForTaker(BakshishPatraTakerData bakshishPatraDataForTaker)
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
        //            UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }
        //        bakshishPatraDataForTaker.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = mutationServices.SaveBakshishPatraTaker(bakshishPatraDataForTaker);
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(bakshishPatraDataForTaker.applicationid!);
        //            string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
        //            List<FetchBakshishPatraDataForTaker> fetchKharediNondList = new List<FetchBakshishPatraDataForTaker>();
        //            for (int i = 0; i < kharediNondIDs.Length; i++)
        //            {
        //                FetchBakshishPatraDataForTaker fetchkhardinondData = new FetchBakshishPatraDataForTaker();
        //                fetchkhardinondData = mutationServices.FetchBakshishPatraInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
        //                if (fetchkhardinondData != null)
        //                {
        //                    fetchKharediNondList.Add(fetchkhardinondData);
        //                }
        //            }
        //            return Ok(ResponseHandler.GetAppResponse(type, "Bakshish Patra Taker Is Created Successfully", fetchKharediNondList));
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
        [Route("CreateBakshishPatraForTaker")]
        public string CreateBakshishPatraForTaker([FromBody] string val)
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
                    _logger.LogInformation("CreateBakshishPatraForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                BakshishPatraTakerData bakshishPatraDataForTaker = JsonConvert.DeserializeObject<BakshishPatraTakerData>(decrypted!)!;
                _logger.LogInformation("Create Bakshish Patra For Taker Request Data - " + decrypted);
                bakshishPatraDataForTaker.userid = UserID;

                string Response = mutationServices.SaveBakshishPatraTaker(bakshishPatraDataForTaker);
                if (Response == "Success")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(bakshishPatraDataForTaker.applicationid!);
                    //string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    //List<FetchBakshishPatraDataForTaker> fetchKharediNondList = new List<FetchBakshishPatraDataForTaker>();
                    //for (int i = 0; i < kharediNondIDs.Length; i++)
                    //{
                    //    FetchBakshishPatraDataForTaker fetchkhardinondData = new FetchBakshishPatraDataForTaker();
                    //    fetchkhardinondData = mutationServices.FetchBakshishPatraInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
                    //    if (fetchkhardinondData != null)
                    //    {
                    //        fetchKharediNondList.Add(fetchkhardinondData);
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(bakshishPatraDataForTaker.applicationid!, "Create Bakshish Patra Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bakshish Patra Taker Is Created Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Bakshish Patra For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Bakshish Patra For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditBakshishPatraForTaker")]
        public string EditBakshishPatraForTaker([FromBody] string val)
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
                    _logger.LogInformation("EditBakshishPatraForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditBakshishPatraTakerData bakshishPatraDataForTaker = JsonConvert.DeserializeObject<EditBakshishPatraTakerData>(decrypted!)!;
                _logger.LogInformation("Edit Bakshish Patra For Taker Request Data - " + decrypted);
                bakshishPatraDataForTaker.userid = UserID;
                string Response = mutationServices.EditBakshishPatraTaker(bakshishPatraDataForTaker);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(bakshishPatraDataForTaker.applicationid!);
                    string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    List<FetchBakshishPatraDataForTaker> fetchKharediNondList = new List<FetchBakshishPatraDataForTaker>();
                    for (int i = 0; i < kharediNondIDs.Length; i++)
                    {
                        FetchBakshishPatraDataForTaker fetchkhardinondData = new FetchBakshishPatraDataForTaker();
                        fetchkhardinondData = mutationServices.FetchBakshishPatraInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
                        if (fetchkhardinondData != null)
                        {
                            fetchKharediNondList.Add(fetchkhardinondData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(bakshishPatraDataForTaker.applicationid!, "Edit Bakshish Patra Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bakshish Patra Taker Updated Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Bakshish Patra For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Bakshish Patra For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteBakshishPatraForTaker")]
        public string DeleteBakshishPatraForTaker([FromBody] string val)
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
                //bakshishPatraDataForTaker.userid = UserID;
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteBakshishPatraForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Bakshish Patra For Taker Request Data - " + decrypted);
                string Response = mutationServices.DeleteMutationTaker(delete);
                List<FetchBakshishPatraDataForTaker> fetchKharediNondList = new List<FetchBakshishPatraDataForTaker>();
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                    if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                    {
                        string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchBakshishPatraDataForTaker fetchkhardinondData = new FetchBakshishPatraDataForTaker();
                            fetchkhardinondData = mutationServices.FetchBakshishPatraInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchkhardinondData != null)
                            {
                                fetchKharediNondList.Add(fetchkhardinondData);
                            }
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Bakshish Patra Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bakshish Patra Taker Data Deleted Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Bakshish Patra For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchKharediNondList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Bakshish Patra For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetBakshishPatraTakerInfo")]
        public string GetBakshishPatraTakerInfo([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetBakshishPatraTakerInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Bakshish Patra Taker Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchBakshishPatraDataForTaker> fetchKharediNondList = new List<FetchBakshishPatraDataForTaker>();
                if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");

                    if (kharediNondIDs.Length > 0)
                    {
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchBakshishPatraDataForTaker fetchKharediNondGiverInformationData = new FetchBakshishPatraDataForTaker();
                            fetchKharediNondGiverInformationData = mutationServices.FetchBakshishPatraInformationDataForTaker(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchKharediNondGiverInformationData != null)
                            {
                                fetchKharediNondList.Add(fetchKharediNondGiverInformationData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bakshish Patra Taker Information Data Found", fetchKharediNondList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchKharediNondList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Taker ID Is Not Exists For The Given Application ID", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Bakshish Patra Taker Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("CreateMayatDharak")]
        public string CreateMayatDharak(//MayatDetails mayatDetails)
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
                    _logger.LogInformation("CreateMayatDharak - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                MayatDetails mayatDetails = JsonConvert.DeserializeObject<MayatDetails>(decrypted!)!;
                _logger.LogInformation("Create Mayat Dharak Request Data - " + decrypted);
                mayatDetails.userid = UserID;
                string Response = mutationServices.SaveMayatDetails(mayatDetails);
                List<FetchMayatDetailsData> fetchKharediNondList = new List<FetchMayatDetailsData>();
                if (Response == "Success")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(mayatDetails.applicationid!);
                    //string[] mayatIDs = applicationDTL.mayatIDs!.Split(",");

                    //for (int i = 0; i < mayatIDs.Length; i++)
                    //{
                    //    FetchMayatDetailsData fetchkhardinondData = new FetchMayatDetailsData();
                    //    fetchkhardinondData = mutationServices.FetchMayatDetails(Convert.ToInt32(mayatIDs[i]));
                    //    if (fetchkhardinondData != null)
                    //    {
                    //        fetchKharediNondList.Add(fetchkhardinondData);
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(mayatDetails.applicationid!, "Create Mayat Dharak Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mayat Dharak Is Created Successfully", null))));
                }
                if (Response == "Update")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(mayatDetails.applicationid!, "Create Mayat Dharak Data Updated Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mayat Dharak Is Updated Successfully", null))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Mayat Dharak Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Mayat Dharak Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditMayatDharak")]
        public string EditMayatDharak([FromBody] string val)
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
                    _logger.LogInformation("EditMayatDharak - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditMayatDetails mayatDetails = JsonConvert.DeserializeObject<EditMayatDetails>(decrypted!)!;
                _logger.LogInformation("Edit Mayat Dharak Request Data - " + decrypted);
                mayatDetails.userid = UserID;

                string Response = mutationServices.EditMayatDetails(mayatDetails);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(mayatDetails.applicationid!);
                    string[] mayatIDs = applicationDTL.mayatIDs!.Split(",");
                    List<FetchMayatDetailsData> fetchKharediNondList = new List<FetchMayatDetailsData>();
                    for (int i = 0; i < mayatIDs.Length; i++)
                    {
                        FetchMayatDetailsData fetchkhardinondData = new FetchMayatDetailsData();
                        fetchkhardinondData = mutationServices.FetchMayatDetails(Convert.ToInt32(mayatIDs[i]));
                        if (fetchkhardinondData != null)
                        {
                            fetchKharediNondList.Add(fetchkhardinondData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(mayatDetails.applicationid!, "Edit Mayat Dharak Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mayat Dharak Updated Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Mayat Dharak Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Mayat Dharak Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteMayatDharak")]
        public string DeleteMayatDharak([FromBody] string val)
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
                //mayatDetails.userid = UserID;
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteMayatDharak - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMayat mayatDetails = JsonConvert.DeserializeObject<DeleteMayat>(decrypted!)!;
                _logger.LogInformation("Delete Mayat Dharak Request Data - " + decrypted);
                string Response = string.Empty;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(mayatDetails.applicationid!);
                if (!string.IsNullOrEmpty(applicationDTL.mayatIDs))
                {
                    string[] mayatIDs = applicationDTL.mayatIDs!.Split(",");
                    Response = mutationServices.DeleteMayat(mayatDetails);
                    if (!string.IsNullOrEmpty(applicationDTL.varasIDS))
                    {
                        string[] takerids = applicationDTL.varasIDS!.Split(",");
                        for (int i = 0; i < takerids.Length; i++)
                        {
                            DeleteMutation deleteMutation = new DeleteMutation();
                            deleteMutation.applicationid = mayatDetails.applicationid;
                            deleteMutation.MutationId = Convert.ToInt32(takerids[i]);
                            Response = mutationServices.DeleteVaras(deleteMutation);
                        }
                    }
                }
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(mayatDetails.applicationid!, "Delete Mayat Dharak Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mayat Dharak Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Mayat Dharak Response Failed -" + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Mayat Dharak Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetMayatDharakInfo")]
        public string GetMayatDharkInfo([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetMayatDharkInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Mayat Dhark Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (!string.IsNullOrEmpty(applicationDTL.mayatIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mayatIDs!.Split(",");
                    List<FetchMayatDetailsData> fetchKharediNondList = new List<FetchMayatDetailsData>();
                    if (kharediNondIDs.Length > 0)
                    {
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchMayatDetailsData fetchKharediNondGiverInformationData = new FetchMayatDetailsData();
                            fetchKharediNondGiverInformationData = mutationServices.FetchMayatDetails(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchKharediNondGiverInformationData != null)
                            {
                                fetchKharediNondList.Add(fetchKharediNondGiverInformationData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mayat Dharak Information Data Found", fetchKharediNondList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchKharediNondList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Taker ID Is Not Exists For The Given Application ID", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Mayat Dhark Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("SaveMrutyuCertificate")]
        public string SaveMrutyuCertificate([FromBody] string val)
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
                    _logger.LogInformation("SaveMrutyuCertificate - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                MrutuDakhalaDetails mayatDetails = JsonConvert.DeserializeObject<MrutuDakhalaDetails>(decrypted!)!;
                _logger.LogInformation("Save Mrutyu Certificate Request Data - " + decrypted);
                mayatDetails.userid = UserID;
                string Response = mutationServices.SaveMrutuDakhlaDetails(mayatDetails);
                if (Response == "Success")
                {
                    /* ApplicationDTL applicationDTL = new ApplicationDTL();
                     applicationDTL = applicationServices.FetchApplicationData(mayatDetails.applicationid!);
                     string[] mayatIDs = applicationDTL.mayatIDs!.Split(",");*/
                    List<FetchMrutuDakhalaDetailsData> fetchKharediNondList = new List<FetchMrutuDakhalaDetailsData>();
                    /* for (int i = 0; i < mayatIDs.Length; i++)
                     {*/
                    FetchMrutuDakhalaDetailsData fetchkhardinondData = new FetchMrutuDakhalaDetailsData();
                    fetchkhardinondData = mutationServices.FetchMrutuDakhalaDetails(Convert.ToInt32(mayatDetails.mayat_id));
                    if (fetchkhardinondData != null)
                    {
                        fetchKharediNondList.Add(fetchkhardinondData);
                    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(mayatDetails.applicationid!, "Save Mayat Dharak Death Certificate Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mayat Dharak Death Certificate is Created Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Save Mrutyu Certificate Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Save Mrutyu Certificate Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditMrutyuCertificate")]
        public string EditMrutyuCertificate([FromBody] string val)
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
                    _logger.LogInformation("EditMrutyuCertificate - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                MrutuDakhalaDetails mayatDetails = JsonConvert.DeserializeObject<MrutuDakhalaDetails>(decrypted!)!;
                _logger.LogInformation("Edit Mrutyu Certificate Request Data - " + decrypted);
                mayatDetails.userid = UserID;
                string Response = mutationServices.EditMrutuDakhlaDetails(mayatDetails);
                if (Response == "Success")
                {
                    /* ApplicationDTL applicationDTL = new ApplicationDTL();
                     applicationDTL = applicationServices.FetchApplicationData(mayatDetails.applicationid!);
                     string[] mayatIDs = applicationDTL.mayatIDs!.Split(",");*/
                    List<FetchMrutuDakhalaDetailsData> fetchKharediNondList = new List<FetchMrutuDakhalaDetailsData>();
                    /* for (int i = 0; i < mayatIDs.Length; i++)
                     {*/
                    FetchMrutuDakhalaDetailsData fetchkhardinondData = new FetchMrutuDakhalaDetailsData();
                    fetchkhardinondData = mutationServices.FetchMrutuDakhalaDetails(Convert.ToInt32(mayatDetails.mayat_id));
                    if (fetchkhardinondData != null)
                    {
                        fetchKharediNondList.Add(fetchkhardinondData);
                    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(mayatDetails.applicationid!, "Edit Mayat Dharak Death Certificate Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mayat Dharak Death Certificate Updated Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Mrutyu Certificate Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Mrutyu Certificate Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetMrutuDharakInfo")]
        public string GetMrutuDharakInfo(
        //string ApplicationID)
        [FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetMrutuDharakInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Mrutu Dharak Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (!string.IsNullOrEmpty(applicationDTL.mayatIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mayatIDs!.Split(",");
                    List<FetchMrutuDakhalaDetailsData> fetchKharediNondList = new List<FetchMrutuDakhalaDetailsData>();
                    if (kharediNondIDs.Length > 0)
                    {
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchMrutuDakhalaDetailsData fetchKharediNondGiverInformationData = new FetchMrutuDakhalaDetailsData();
                            fetchKharediNondGiverInformationData = mutationServices.FetchMrutuDakhalaDetails(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchKharediNondGiverInformationData != null)
                            {
                                if (fetchKharediNondGiverInformationData.userDetails!.dateOfDeathCertificate != "NA")
                                {
                                    fetchKharediNondList.Add(fetchKharediNondGiverInformationData);
                                }
                            }
                        }
                        if (fetchKharediNondList.Count > 0)
                        {
                            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mayat Dharak Death Certificate Information Data Found", fetchKharediNondList))));
                        }
                        else
                        {
                            type = ReponseType.NotFound;
                            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchKharediNondList))));
                        }
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchKharediNondList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Taker ID Is Not Exists For The Given Application ID", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Mrutu Dharak Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("CreateVarasNond")]
        public string CreateVarasNond([FromBody] string val)
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
                    _logger.LogInformation("CreateVarasNond - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                VarasDetails varasnond = JsonConvert.DeserializeObject<VarasDetails>(decrypted!)!;
                _logger.LogInformation("Create Varas Nond Request Data - " + decrypted);
                varasnond.userid = UserID;
                string Response = mutationServices.SaveVarasDetails(varasnond);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(varasnond.applicationid!);
                    string[] kharediNondIDs = applicationDTL.varasIDS!.Split(",");
                    List<FetchVarasNondDetailsData> fetchKharediNondList = new List<FetchVarasNondDetailsData>();
                    for (int i = 0; i < kharediNondIDs.Length; i++)
                    {
                        FetchVarasNondDetailsData fetchkhardinondData = new FetchVarasNondDetailsData();
                        fetchkhardinondData = mutationServices.FetchVarasData(Convert.ToInt32(kharediNondIDs[i]));
                        if (fetchkhardinondData != null)
                        {
                            fetchKharediNondList.Add(fetchkhardinondData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(varasnond.applicationid!, "Create Varas Nond Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Varas Nond Is Created Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Varas Nond Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Varas Nond Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditVarasNond")]
        public string EditVarasNond([FromBody] string val)
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
                    _logger.LogInformation("EditVarasNond - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditVarasDetails varasnond = JsonConvert.DeserializeObject<EditVarasDetails>(decrypted!)!;
                _logger.LogInformation("Edit Varas Nond Request Data - " + decrypted);
                varasnond.userid = UserID;
                string Response = mutationServices.EditVarasDetails(varasnond);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(varasnond.applicationid!);
                    string[] kharediNondIDs = applicationDTL.varasIDS!.Split(",");
                    List<FetchVarasNondDetailsData> fetchKharediNondList = new List<FetchVarasNondDetailsData>();
                    for (int i = 0; i < kharediNondIDs.Length; i++)
                    {
                        FetchVarasNondDetailsData fetchkhardinondData = new FetchVarasNondDetailsData();
                        fetchkhardinondData = mutationServices.FetchVarasData(Convert.ToInt32(kharediNondIDs[i]));
                        if (fetchkhardinondData != null)
                        {
                            fetchKharediNondList.Add(fetchkhardinondData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(varasnond.applicationid!, "Edit Varas Nond Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Varas Nond Updated Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Varas Nond Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Varas Nond Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteVarasNond")]
        public string DeleteVarasNond([FromBody] string val)
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
                //varasnond.userid = UserID;
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteVarasNond - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation varasnond = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Varas Nond Request Data - " + decrypted);
                string Response = mutationServices.DeleteVaras(varasnond);
                List<FetchVarasNondDetailsData> fetchKharediNondList = new List<FetchVarasNondDetailsData>();
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(varasnond.applicationid!);
                    if (!string.IsNullOrEmpty(applicationDTL.varasIDS))
                    {
                        string[] kharediNondIDs = applicationDTL.varasIDS!.Split(",");
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchVarasNondDetailsData fetchkhardinondData = new FetchVarasNondDetailsData();
                            fetchkhardinondData = mutationServices.FetchVarasData(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchkhardinondData != null)
                            {
                                fetchKharediNondList.Add(fetchkhardinondData);
                            }
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(varasnond.applicationid!, "Delete Varas Nond Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Varas Nond Data Deleted Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Varas Nond Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchKharediNondList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Varas Nond Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetVarasNondInfo")]
        public string GetVarasNondInfo([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetVarasNondInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Varas Nond Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (!string.IsNullOrEmpty(applicationDTL.varasIDS))
                {
                    string[] kharediNondIDs = applicationDTL.varasIDS.Split(",");
                    List<FetchVarasNondDetailsData> fetchKharediNondList = new List<FetchVarasNondDetailsData>();
                    if (kharediNondIDs.Length > 0)
                    {
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchVarasNondDetailsData fetchKharediNondGiverInformationData = new FetchVarasNondDetailsData();
                            fetchKharediNondGiverInformationData = mutationServices.FetchVarasData(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchKharediNondGiverInformationData != null)
                            {
                                fetchKharediNondList.Add(fetchKharediNondGiverInformationData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Varas Nond Information Data Found", fetchKharediNondList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchKharediNondList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Giver ID Is Not Exists For The Given Application ID", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Varas Nond Info Exception - " + ex.StackTrace!.ToString());
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

        // Hakkasod Denara
        [Authorize]
        [HttpPost]
        [Route("CreateHakkasodInfoForGiver")]
        public string CreateHakkasodInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("CreateHakkasodInfoForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                HakkasodDataForGiver hakkasodData = JsonConvert.DeserializeObject<HakkasodDataForGiver>(decrypted!)!;
                _logger.LogInformation("Create Hakkasod Info For Giver Request Data - " + decrypted);
                hakkasodData.userid = UserID;
                string Response = mutationServices.SaveHakkaSodGiver(hakkasodData);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(hakkasodData.applicationid!);
                    string[] mutationgiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    List<FetchHakkasodForGiverData> fetchDataLits = new List<FetchHakkasodForGiverData>();
                    for (int i = 0; i < mutationgiverIDs.Length; i++)
                    {
                        FetchHakkasodForGiverData fetchData = new();
                        fetchData = mutationServices.FetchHakkasodInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
                        if (fetchData != null)
                        {
                            fetchDataLits.Add(fetchData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(hakkasodData.applicationid!, "Create Hakkasod Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakkasod Is Created Successfully", fetchDataLits))));
                }
                else if (Response == "Update")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(hakkasodData.applicationid!);
                    string[] mutationgiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    List<FetchHakkasodForGiverData> fetchDataLits = new List<FetchHakkasodForGiverData>();
                    for (int i = 0; i < mutationgiverIDs.Length; i++)
                    {
                        FetchHakkasodForGiverData fetchData = new();
                        fetchData = mutationServices.FetchHakkasodInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
                        if (fetchData != null)
                        {
                            fetchDataLits.Add(fetchData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(hakkasodData.applicationid!, "Create Hakkasod Nond Giver Data Updated Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakkasod Is Updated Successfully", fetchDataLits))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Hakkasod Info For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Hakkasod Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditHakkasodInfoForGiver")]
        public string EditHakkasodInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("EditHakkasodInfoForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditHakkasodDataForGiver hakkasodData = JsonConvert.DeserializeObject<EditHakkasodDataForGiver>(decrypted!)!;
                _logger.LogInformation("Edit Hakkasod Info For Giver Request Data - " + decrypted);
                hakkasodData.userid = UserID;
                string Response = mutationServices.EditHakkaSodGiver(hakkasodData);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(hakkasodData.applicationid!);
                    string[] mutationgiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    List<FetchHakkasodForGiverData> fetchDataLits = new List<FetchHakkasodForGiverData>();
                    for (int i = 0; i < mutationgiverIDs.Length; i++)
                    {
                        FetchHakkasodForGiverData fetchData = new FetchHakkasodForGiverData();
                        fetchData = mutationServices.FetchHakkasodInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
                        if (fetchData != null)
                        {
                            fetchDataLits.Add(fetchData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(hakkasodData.applicationid!, "Edit Hakkasod Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakkasod Giver Updated Successfully", fetchDataLits))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Hakkasod Info For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Hakkasod Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteHakkasodInfoForGiver")]
        public string DeleteHakkasodInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("DeleteHakkasodInfoForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Hakkasod Info For Giver Request Data - " + decrypted);
                string Response = string.Empty;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    if (kharediNondIDs.Length > 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                    }
                    if (kharediNondIDs.Length == 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                        if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                        {
                            string[] takerids = applicationDTL.mutationtakerIDs!.Split(",");
                            if (takerids.Length > 0)
                            {
                                for (int i = 0; i < takerids.Length; i++)
                                {
                                    delete.MutationId = Convert.ToInt32(takerids[i]);
                                    Response = mutationServices.DeleteMutationTaker(delete);
                                }
                            }
                        }
                    }
                }
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Hakkasod Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakkasod Giver Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Hakkasod Info For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Hakkasod Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetHakkasodInfoForGiver")]
        public string GetHakkasodInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("GetHakkasodInfoForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Hakkasod Info For Giver Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] mutationgiverIDs = applicationDTL.mutationgiverIDs.Split(",");
                    List<FetchHakkasodForGiverData> fetchDataList = new List<FetchHakkasodForGiverData>();
                    if (mutationgiverIDs.Length > 0)
                    {
                        for (int i = 0; i < mutationgiverIDs.Length; i++)
                        {
                            FetchHakkasodForGiverData fetchData = new FetchHakkasodForGiverData();
                            fetchData = mutationServices.FetchHakkasodInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
                            if (fetchData != null)
                            {
                                fetchDataList.Add(fetchData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakkasod Data Found", fetchDataList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakkasod Data Not Found", fetchDataList))));
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
                _logger.LogError("Get Hakkasod Info For Giver Exception - " + ex.StackTrace!.ToString());
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

        // HakkaSod Taker

        [Authorize]
        [HttpPost]
        [Route("CreateHakkaSodForTaker")]
        public string CreateHakkaSodForTaker([FromBody] string val)
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
                    _logger.LogInformation("CreateHakkaSodForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                HakkaSodTakerData hakkasodDataForTaker = JsonConvert.DeserializeObject<HakkaSodTakerData>(decrypted!)!;
                _logger.LogInformation("Create Hakka Sod For Taker Request Data - " + decrypted);
                hakkasodDataForTaker.userid = UserID;
                string Response = mutationServices.SaveHakkaSodTaker(hakkasodDataForTaker);
                List<FetchHakkaSodTaker> fetchKharediNondList = new List<FetchHakkaSodTaker>();

                if (Response == "Success")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(hakkasodDataForTaker.applicationid!);
                    //string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    //for (int i = 0; i < kharediNondIDs.Length; i++)
                    //{
                    //    FetchHakkaSodTaker fetchkhardinondData = new FetchHakkaSodTaker();
                    //    fetchkhardinondData = mutationServices.FetchHakkaSodInfoForTaker(Convert.ToInt32(kharediNondIDs[i]));
                    //    if (fetchkhardinondData != null)
                    //    {
                    //        fetchKharediNondList.Add(fetchkhardinondData);
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(hakkasodDataForTaker.applicationid!, "Create Hakkasod Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakka Sod Taker Is Created Successfully", null))));
                }
                else if (Response == "Update")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(hakkasodDataForTaker.applicationid!);
                    //string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    //for (int i = 0; i < kharediNondIDs.Length; i++)
                    //{
                    //    FetchHakkaSodTaker fetchkhardinondData = new FetchHakkaSodTaker();
                    //    fetchkhardinondData = mutationServices.FetchHakkaSodInfoForTaker(Convert.ToInt32(kharediNondIDs[i]));
                    //    if (fetchkhardinondData != null)
                    //    {
                    //        fetchKharediNondList.Add(fetchkhardinondData);
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(hakkasodDataForTaker.applicationid!, "Create Hakkasod Nond Taker Data Updated Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakka Sod Taker Is Updated Successfully", null))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Hakka Sod For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchKharediNondList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Hakka Sod For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditHakkaSodForTaker")]
        public string EditHakkaSodForTaker([FromBody] string val)
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
                    _logger.LogInformation("EditHakkaSodForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditHakkaSodTakerData hakkasodDataForTaker = JsonConvert.DeserializeObject<EditHakkaSodTakerData>(decrypted!)!;
                _logger.LogInformation("Edit Hakka Sod For Taker Request Data - " + decrypted);
                hakkasodDataForTaker.userid = UserID;
                string Response = mutationServices.EditHakkaSodTaker(hakkasodDataForTaker);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(hakkasodDataForTaker.applicationid!);
                    string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    List<FetchHakkaSodTaker> fetchKharediNondList = new List<FetchHakkaSodTaker>();
                    for (int i = 0; i < kharediNondIDs.Length; i++)
                    {
                        FetchHakkaSodTaker fetchkhardinondData = new FetchHakkaSodTaker();
                        fetchkhardinondData = mutationServices.FetchHakkaSodInfoForTaker(Convert.ToInt32(kharediNondIDs[i]));
                        if (fetchkhardinondData != null)
                        {
                            fetchKharediNondList.Add(fetchkhardinondData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(hakkasodDataForTaker.applicationid!, "Edit Hakkasod Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakka Sod Taker Updated Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Hakka Sod For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Hakka Sod For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteHakkaSodForTaker")]
        public string DeleteHakkaSodForTaker([FromBody] string val)
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
                // hakkasodDataForTaker.userid = UserID;
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("DeleteHakkaSodForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Hakka Sod For Taker Request Data - " + decrypted);
                string Response = mutationServices.DeleteMutationTaker(delete);
                List<FetchHakkaSodTaker> fetchKharediNondList = new List<FetchHakkaSodTaker>();
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                    if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                    {
                        string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchHakkaSodTaker fetchkhardinondData = new FetchHakkaSodTaker();
                            fetchkhardinondData = mutationServices.FetchHakkaSodInfoForTaker(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchkhardinondData != null)
                            {
                                fetchKharediNondList.Add(fetchkhardinondData);
                            }
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Hakkasod Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakka Sod Taker Data Deleted Successfully", fetchKharediNondList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Hakka Sod For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchKharediNondList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Hakka Sod For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetHakkaSodTakerInfo")]
        public string GetHakkaSodTakerInfo([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetHakkaSodTakerInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Hakka Sod Taker Info Request Data - " + decrypted);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    List<FetchHakkaSodTaker> fetchKharediNondList = new List<FetchHakkaSodTaker>();
                    if (kharediNondIDs.Length > 0)
                    {
                        for (int i = 0; i < kharediNondIDs.Length; i++)
                        {
                            FetchHakkaSodTaker fetchKharediNondGiverInformationData = new FetchHakkaSodTaker();
                            fetchKharediNondGiverInformationData = mutationServices.FetchHakkaSodInfoForTaker(Convert.ToInt32(kharediNondIDs[i]));
                            if (fetchKharediNondGiverInformationData != null)
                            {
                                fetchKharediNondList.Add(fetchKharediNondGiverInformationData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Hakka Sod Taker Information Data Found", fetchKharediNondList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", fetchKharediNondList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Taker ID Is Not Exists For The Given Application ID", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Hakka Sod Taker Info Exception - " + ex.StackTrace!.ToString());
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
        //[Route("EditMrutyuPatraInfoForGiver")]
        //public string EditMrutyuPatraInfoForGiver([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        EditMrutyuPatraDataForGiver mrutyuPatraData = JsonConvert.DeserializeObject<EditMrutyuPatraDataForGiver>(decrypted!)!;
        //        _logger.LogInformation("Edit Mrutyu Patra Info For Giver Request Data - " + decrypted);

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
        //        mrutyuPatraData.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = mutationServices.EditMrutyuPatraGiver(mrutyuPatraData);
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(mrutyuPatraData.applicationid!);
        //            string[] mutationgiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
        //            List<FetchMrutyuPatraForGiverData> fetchDataLits = new List<FetchMrutyuPatraForGiverData>();
        //            for (int i = 0; i < mutationgiverIDs.Length; i++)
        //            {
        //                FetchMrutyuPatraForGiverData fetchData = new FetchMrutyuPatraForGiverData();
        //                fetchData = mutationServices.FetchMrutyuPatraInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //                if (fetchData != null)
        //                {
        //                    fetchDataLits.Add(fetchData);
        //                }
        //            }
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Updated Successfully", fetchDataLits))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Edit Mrutyu Patra Info For Giver Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogInformation("Edit Mrutyu Patra Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        //[Route("DeleteMrutyuPatraInfoForGiver")]
        //public string DeleteMrutyuPatraInfoForGiver([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
        //        _logger.LogInformation("Delete Mrutyu Patra Info For Giver Request Data - " + decrypted);

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
        //        //mrutyuPatraData.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = mutationServices.DeleteMutationGiver(delete);
        //        List<FetchMrutyuPatraForGiverData> fetchDataList = new List<FetchMrutyuPatraForGiverData>();
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
        //            if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
        //            {
        //                string[] mutationgiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
        //                for (int i = 0; i < mutationgiverIDs.Length; i++)
        //                {
        //                    FetchMrutyuPatraForGiverData fetchData = new FetchMrutyuPatraForGiverData();
        //                    fetchData = mutationServices.FetchMrutyuPatraInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //                    if (fetchData != null)
        //                    {
        //                        fetchDataList.Add(fetchData);
        //                    }
        //                }
        //            }
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Giver Data Deleted Successfully", fetchDataList))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Delete Mrutyu Patra Info For Giver Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchDataList))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Delete Mrutyu Patra Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        //[Route("GetMrutyuPatraInfoForGiver")]
        //public string GetMrutyuPatraInfoForGiver([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        _logger.LogInformation("Get Mrutyu Patra Info For Giver Request Data - " + ApplicationID);

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
        //            List<FetchMrutyuPatraForGiverData> fetchDataList = new List<FetchMrutyuPatraForGiverData>();
        //            if (mutationgiverIDs.Length > 0)
        //            {
        //                for (int i = 0; i < mutationgiverIDs.Length; i++)
        //                {
        //                    FetchMrutyuPatraForGiverData fetchData = new FetchMrutyuPatraForGiverData();
        //                    fetchData = mutationServices.FetchMrutyuPatraInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
        //                    if (fetchData != null)
        //                    {
        //                        fetchDataList.Add(fetchData);
        //                    }
        //                }
        //                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Data Found", fetchDataList))));
        //            }
        //            else
        //            {
        //                type = ReponseType.NotFound;
        //                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Data Not Found", fetchDataList))));
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
        //        _logger.LogError("Get Mrutyu Patra Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        //[Route("CreateMrutyuPatraInfoForTaker")]
        //public string CreateMrutyuPatraInfoForTaker([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        MrutyuPatraDataForTaker mrutyuPatraDataForTaker = JsonConvert.DeserializeObject<MrutyuPatraDataForTaker>(decrypted!)!;
        //        _logger.LogInformation("Create Mrutyu Patra Info For Taker Request Data - " + decrypted);
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
        //        mrutyuPatraDataForTaker.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = mutationServices.SaveMrutyuPatraTaker(mrutyuPatraDataForTaker);
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(mrutyuPatraDataForTaker.applicationid!);
        //            string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
        //            List<FetchMrutyuPatraForTakerData> fetchDataList = new List<FetchMrutyuPatraForTakerData>();
        //            for (int i = 0; i < kharediNondIDs.Length; i++)
        //            {
        //                FetchMrutyuPatraForTakerData fetchData = new FetchMrutyuPatraForTakerData();
        //                fetchData = mutationServices.FetchMrutyuPatraForTakerData(Convert.ToInt32(kharediNondIDs[i]));
        //                if (fetchData != null)
        //                {
        //                    fetchDataList.Add(fetchData);
        //                }
        //            }
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Is Created Successfully", fetchDataList))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Create Mrutyu Patra Info For Taker Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Create Mrutyu Patra Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        //[Route("EditMrutyuPatraInfoForTaker")]
        //public string EditMrutyuPatraInfoForTaker([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        EditMrutyuPatraDataForTaker mrutyuPatraDataForTaker = JsonConvert.DeserializeObject<EditMrutyuPatraDataForTaker>(decrypted!)!;
        //        _logger.LogInformation("Edit Mrutyu Patra Info For Taker Request Data - " + decrypted);

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
        //        mrutyuPatraDataForTaker.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = mutationServices.EditMrutyuPatraTaker(mrutyuPatraDataForTaker);
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(mrutyuPatraDataForTaker.applicationid!);
        //            string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
        //            List<FetchMrutyuPatraForTakerData> fetchDataList = new List<FetchMrutyuPatraForTakerData>();
        //            for (int i = 0; i < kharediNondIDs.Length; i++)
        //            {
        //                FetchMrutyuPatraForTakerData fetchData = new FetchMrutyuPatraForTakerData();
        //                fetchData = mutationServices.FetchMrutyuPatraForTakerData(Convert.ToInt32(kharediNondIDs[i]));
        //                if (fetchData != null)
        //                {
        //                    fetchDataList.Add(fetchData);
        //                }
        //            }
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Updated Successfully", fetchDataList))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Edit Mrutyu Patra Info For Taker Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Edit Mrutyu Patra Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        //[Route("DeleteMrutyuPatraInfoForTaker")]
        //public string DeleteMrutyuPatraInfoForTaker([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
        //        _logger.LogInformation("Delete Mrutyu Patra Info For Taker Request Data - " + decrypted);

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
        //        //mrutyuPatraDataForTaker.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = mutationServices.DeleteMutationTaker(delete);
        //        List<FetchMrutyuPatraForTakerData> fetchDataList = new List<FetchMrutyuPatraForTakerData>();
        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
        //            if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
        //            {
        //                string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
        //                for (int i = 0; i < kharediNondIDs.Length; i++)
        //                {
        //                    FetchMrutyuPatraForTakerData fetchData = new FetchMrutyuPatraForTakerData();
        //                    fetchData = mutationServices.FetchMrutyuPatraForTakerData(Convert.ToInt32(kharediNondIDs[i]));
        //                    if (fetchData != null)
        //                    {
        //                        fetchDataList.Add(fetchData);
        //                    }
        //                }
        //            }
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Taker Data Deleted Successfully", fetchDataList))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Delete Mrutyu Patra Info For Taker Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchDataList))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Delete Mrutyu Patra Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        //[Route("GetMrutyuPatraInfoForTaker")]
        //public string GetMrutyuPatraInfoForTaker([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        _logger.LogInformation("Get Mrutyu Patra Info For Taker Request Data - " + ApplicationID);

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
        //        if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
        //        {
        //            string[] mutationtakerIDs = applicationDTL.mutationtakerIDs.Split(",");
        //            List<FetchMrutyuPatraForTakerData> fetchDataList = new List<FetchMrutyuPatraForTakerData>();
        //            if (mutationtakerIDs.Length > 0)
        //            {
        //                for (int i = 0; i < mutationtakerIDs.Length; i++)
        //                {
        //                    FetchMrutyuPatraForTakerData fetchData = new FetchMrutyuPatraForTakerData();
        //                    fetchData = mutationServices.FetchMrutyuPatraForTakerData(Convert.ToInt32(mutationtakerIDs[i]));
        //                    if (fetchData != null)
        //                    {
        //                        fetchDataList.Add(fetchData);
        //                    }
        //                }
        //                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Data Found", fetchDataList))));
        //            }
        //            else
        //            {
        //                type = ReponseType.NotFound;
        //                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Data Not Found", fetchDataList))));
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
        //        _logger.LogError("Get Mrutyu Patra Info For Taker Exception - " + ex.StackTrace!.ToString());
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

        // Gahankhat Denara
        [Authorize]
        [HttpPost]
        [Route("CreateGahankhatInfoForGiver")]
        public string CreateGahankhatInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("CreateGahankhatInfoForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GahankhatDataForGiver takerData = JsonConvert.DeserializeObject<GahankhatDataForGiver>(decrypted!)!;
                _logger.LogInformation("Create Gahankhat Info For Giver Request Data - " + decrypted);
                takerData.userid = UserID;
                string Response = mutationServices.SaveGahankhatGiver(takerData);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(takerData.applicationid!);
                    string[] mutationGiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    List<FetchGahankhatDataForGiver> fetchDataList = new List<FetchGahankhatDataForGiver>();
                    for (int i = 0; i < mutationGiverIDs.Length; i++)
                    {
                        FetchGahankhatDataForGiver fetchData = new FetchGahankhatDataForGiver();
                        fetchData = mutationServices.FetchGahankhatForGiverData(Convert.ToInt32(mutationGiverIDs[i]));
                        if (fetchData != null)
                        {
                            fetchDataList.Add(fetchData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(takerData.applicationid!, "Create Gahankhat Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Is Created Successfully", fetchDataList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Gahankhat Info For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Gahankhat Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditGahankhatInfoForGiver")]
        public string EditGahankhatInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("EditGahankhatInfoForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditGahankhatDataForGiver takerData = JsonConvert.DeserializeObject<EditGahankhatDataForGiver>(decrypted!)!;
                _logger.LogInformation("Edit Gahankhat Info For Giver Request Data - " + decrypted);
                takerData.userid = UserID;
                string Response = mutationServices.EditGahankhatGiver(takerData);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(takerData.applicationid!);
                    string[] mutationGiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    List<FetchGahankhatDataForGiver> fetchDataList = new List<FetchGahankhatDataForGiver>();
                    for (int i = 0; i < mutationGiverIDs.Length; i++)
                    {
                        FetchGahankhatDataForGiver fetchData = new FetchGahankhatDataForGiver();
                        fetchData = mutationServices.FetchGahankhatForGiverData(Convert.ToInt32(mutationGiverIDs[i]));
                        if (fetchData != null)
                        {
                            fetchDataList.Add(fetchData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(takerData.applicationid!, "Edit Gahankhat Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Giver Updated Successfully", fetchDataList))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Gahankhat Info For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Gahankhat Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteGahankhatInfoForGiver")]
        public string DeleteGahankhatInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("DeleteGahankhatInfoForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Gahankhat Info For Giver Request Data - " + decrypted);
                string Response = string.Empty;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    if (kharediNondIDs.Length > 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                    }
                    if (kharediNondIDs.Length == 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                        //if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                        //{
                        //    string[] takerids = applicationDTL.mutationtakerIDs!.Split(",");
                        //    if (takerids.Length > 0)
                        //    {
                        //        for (int i = 0; i < takerids.Length; i++)
                        //        {
                        //            delete.MutationId = Convert.ToInt32(takerids[i]);
                        //            Response = mutationServices.DeleteMutationTaker(delete);
                        //        }
                        //    }
                        //}
                    }
                }
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Gahankhat Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Giver Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Gahankhat Info For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Gahankhat Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetGahankhatInfoForGiver")]
        public string GetGahankhatInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("GetGahankhatInfoForGiver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Gahankhat Info For Giver Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] mutationGiverIDs = applicationDTL.mutationgiverIDs.Split(",");
                    List<FetchGahankhatDataForGiver> fetchDataList = new List<FetchGahankhatDataForGiver>();
                    if (mutationGiverIDs.Length > 0)
                    {
                        for (int i = 0; i < mutationGiverIDs.Length; i++)
                        {
                            FetchGahankhatDataForGiver fetchData = new FetchGahankhatDataForGiver();
                            fetchData = mutationServices.FetchGahankhatForGiverData(Convert.ToInt32(mutationGiverIDs[i]));
                            if (fetchData != null)
                            {
                                fetchDataList.Add(fetchData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Data Found", fetchDataList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Data Not Found", fetchDataList))));
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
                _logger.LogError("Get Gahankhat Info For Giver Exception - " + ex.StackTrace!.ToString());
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

        //Gahankhat Ghenara
        [Authorize]
        [HttpPost]
        [Route("CreateGahankhatInfoForTaker")]
        public string CreateGahankhatInfoForTaker([FromBody] string val)
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
                    _logger.LogInformation("CreateGahankhatInfoForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GahankhatDataForTaker gahankhatData = JsonConvert.DeserializeObject<GahankhatDataForTaker>(decrypted!)!;
                _logger.LogInformation("Create Gahankhat Info For Taker Request Data - " + decrypted);
                gahankhatData.userid = UserID;
                string Response = mutationServices.SaveGahankhatTaker(gahankhatData);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(gahankhatData.applicationid!);
                    string[] mutationtakerIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    List<FetchGahankhatForTakerData> fetchDataLits = new List<FetchGahankhatForTakerData>();
                    for (int i = 0; i < mutationtakerIDs.Length; i++)
                    {
                        FetchGahankhatForTakerData fetchData = new FetchGahankhatForTakerData();
                        fetchData = mutationServices.FetchGahankhatInfoForTaker(Convert.ToInt32(mutationtakerIDs[i]));
                        if (fetchData != null)
                        {
                            fetchDataLits.Add(fetchData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(gahankhatData.applicationid!, "Create Gahankhat Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Is Created Successfully", fetchDataLits))));
                }
                else if (Response == "Update")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(gahankhatData.applicationid!);
                    string[] mutationtakerIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    List<FetchGahankhatForTakerData> fetchDataLits = new List<FetchGahankhatForTakerData>();
                    for (int i = 0; i < mutationtakerIDs.Length; i++)
                    {
                        FetchGahankhatForTakerData fetchData = new FetchGahankhatForTakerData();
                        fetchData = mutationServices.FetchGahankhatInfoForTaker(Convert.ToInt32(mutationtakerIDs[i]));
                        if (fetchData != null)
                        {
                            fetchDataLits.Add(fetchData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(gahankhatData.applicationid!, "Create Gahankhat Nond Taker Data Updated Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Is Updated Successfully", fetchDataLits))));
                }
                else if (Response == "Update")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(gahankhatData.applicationid!);
                    string[] mutationtakerIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    List<FetchGahankhatForTakerData> fetchDataLits = new List<FetchGahankhatForTakerData>();
                    for (int i = 0; i < mutationtakerIDs.Length; i++)
                    {
                        FetchGahankhatForTakerData fetchData = new FetchGahankhatForTakerData();
                        fetchData = mutationServices.FetchGahankhatInfoForTaker(Convert.ToInt32(mutationtakerIDs[i]));
                        if (fetchData != null)
                        {
                            fetchDataLits.Add(fetchData);
                        }
                    }
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Is Created Successfully", fetchDataLits))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Gahankhat Info For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Gahankhat Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditGahankhatInfoForTaker")]
        public string EditGahankhatInfoForTaker([FromBody] string val)
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
                    _logger.LogInformation("EditGahankhatInfoForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditGahankhatDataForTaker gahankhatData = JsonConvert.DeserializeObject<EditGahankhatDataForTaker>(decrypted!)!;
                _logger.LogInformation("Edit Gahankhat Info For Taker Request Data - " + decrypted);
                gahankhatData.userid = UserID;
                string Response = mutationServices.EditGahankhatTaker(gahankhatData);
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(gahankhatData.applicationid!);
                    string[] mutationtakerIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    List<FetchGahankhatForTakerData> fetchDataLits = new List<FetchGahankhatForTakerData>();
                    for (int i = 0; i < mutationtakerIDs.Length; i++)
                    {
                        FetchGahankhatForTakerData fetchData = new FetchGahankhatForTakerData();
                        fetchData = mutationServices.FetchGahankhatInfoForTaker(Convert.ToInt32(mutationtakerIDs[i]));
                        if (fetchData != null)
                        {
                            fetchDataLits.Add(fetchData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(gahankhatData.applicationid!, "Edit Gahankhat Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Updated Successfully", fetchDataLits))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Gahankhat Info For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Edit Gahankhat Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteGahankhatInfoForTaker")]
        public string DeleteGahankhatInfoForTaker([FromBody] string val)
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
                    _logger.LogInformation("DeleteGahankhatInfoForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Gahankhat Info For Taker Request Data - " + decrypted);
                string Response = string.Empty;

                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                {
                    string[] mutationtakerIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    if (mutationtakerIDs.Length > 1)
                    {
                        Response = mutationServices.DeleteMutationTaker(delete);
                    }
                    if (mutationtakerIDs.Length == 1)
                    {
                        Response = mutationServices.DeleteMutationTaker(delete);
                        if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                        {
                            string[] takerids = applicationDTL.mutationgiverIDs!.Split(",");
                            if (takerids.Length > 0)
                            {
                                for (int i = 0; i < takerids.Length; i++)
                                {
                                    delete.MutationId = Convert.ToInt32(takerids[i]);
                                    Response = mutationServices.DeleteMutationGiver(delete);
                                }
                            }
                        }
                    }
                }
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Gahankhat Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Taker Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Gahankhat Info For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Gahankhat Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetGahankhatInfoForTaker")]
        public string GetGahankhatInfoForTaker([FromBody] string val)
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
                    _logger.LogInformation("GetGahankhatInfoForTaker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Gahankhat Info For Taker Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                if (applicationDTL != null && !string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                {
                    string[] mutationtakerIDs = applicationDTL.mutationtakerIDs.Split(",");
                    List<FetchGahankhatForTakerData> fetchDataList = new List<FetchGahankhatForTakerData>();
                    if (mutationtakerIDs.Length > 0)
                    {
                        for (int i = 0; i < mutationtakerIDs.Length; i++)
                        {
                            FetchGahankhatForTakerData fetchData = new FetchGahankhatForTakerData();
                            fetchData = mutationServices.FetchGahankhatInfoForTaker(Convert.ToInt32(mutationtakerIDs[i]));
                            if (fetchData != null)
                            {
                                fetchDataList.Add(fetchData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Data Found", fetchDataList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Gahankhat Data Not Found", fetchDataList))));
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
                _logger.LogError("Get Gahankhat Info For Taker Exception - " + ex.StackTrace!.ToString());
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

        //End

        //[Authorize]
        //[HttpPost]
        //[Route("GetApplicationData")]
        //public string GetApplicationData([FromBody] string val)
        ////string ApplicationID)
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
        //        bool check = true;
        //        check = Security.IsBase64String(val);
        //        if (!check)
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("GetApplicationData - Inout String Is Not Encrypted");
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
        //        }
        //        var decrypted = Security.DeCryptData(val);
        //        string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        _logger.LogInformation("Get Application Data Request Data - " + ApplicationID);
        //        var fetchapplication = mutationServices.GetApplicationDetails(UserID, ApplicationID);
        //        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Found", fetchapplication))));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Get Application Data Exception - " + ex.StackTrace!.ToString());
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

        //Swara
        //[Authorize]
        //[HttpPost]
        //[Route("GetApplicationData")]
        //public string GetApplicationData([FromBody] string val)
        ////string ApplicationID)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        _logger.LogInformation("Get Application Data Request Data - " + ApplicationID);

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
        //        //UserID = 370;
        //        var fetchapplication = mutationServices.GetApplicationDetails(UserID, ApplicationID);


        //        if (fetchapplication.mutationCTSNoData == null || fetchapplication.mutationCTSNoData.Count <= 0)
        //        {
        //            type = ReponseType.Failure;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया माहिती फेरफार नोंदी मध्ये परिणाम झालेले न.भू.क्र. भरा.", fetchapplication.mutationCTSNoData))));
        //        }
        //        if (fetchapplication.applicationDtl!.applicationType == "नोंदणी कृत")
        //        {
        //            if (fetchapplication.dastInformation == null || fetchapplication.dastInformation.Count <= 0)
        //            {
        //                type = ReponseType.Failure;
        //                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया दस्त माहिती भरा.", fetchapplication.dastInformation))));
        //            }

        //            //if (fetchapplication.Mutation is IEnumerable<dynamic> mutationList)
        //            //{
        //            //    foreach (var mutation in mutationList)
        //            //    {
        //            //        if (mutation.value is not IEnumerable<dynamic> values || !values.Any())
        //            //        {
        //            //            type = ReponseType.Failure;
        //            //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया फेरफार तपशील भरा.", fetchapplication.Mutation))));
        //            //        }
        //            //    }
        //            //}
        //        }
        //        if (fetchapplication.Mutation is IEnumerable<dynamic> mutationList)
        //        {
        //            foreach (var mutation in mutationList)
        //            {
        //                if (mutation.value is IEnumerable<dynamic> values)
        //                {
        //                    if (!values.Any())
        //                    {
        //                        type = ReponseType.Failure;
        //                        return Security.EnCryptData(JsonConvert.SerializeObject(
        //                            Ok(ResponseHandler.GetAppResponse(type, "कृपया फेरफार तपशील भरा.", fetchapplication.Mutation))
        //                        ));
        //                    }
        //                }
        //                else if (mutation.value == null) //for bhadepatta
        //                {
        //                    type = ReponseType.Failure;
        //                    return Security.EnCryptData(JsonConvert.SerializeObject(
        //                        Ok(ResponseHandler.GetAppResponse(type, "कृपया फेरफार तपशील भरा.", fetchapplication.Mutation))
        //                    ));
        //                }
        //                // case 3: value is single object → allow it
        //            }
        //        }
        //        if (fetchapplication.applicationDtl!.do_you_have_power_of_attorney == true)
        //        {
        //            if (fetchapplication.poa == null || fetchapplication.poa.Count <= 0)
        //            {
        //                type = ReponseType.Failure;
        //                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया मुखत्यारपत्र माहिती भरा.", fetchapplication.poa))));
        //            }
        //        }

        //        if (fetchapplication.applicationDtl!.Is_the_claim_pending_before_the_court == true)
        //        {
        //            if (fetchapplication.courtClaimInformation == null || fetchapplication.courtClaimInformation.Count <= 0)
        //            {
        //                type = ReponseType.Failure;
        //                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया कोर्ट दावा माहिती भरा.", fetchapplication.courtClaimInformation))));
        //            }
        //        }

        //        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Found", fetchapplication))));

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Get Application Data Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetApplicationData")]
        public string GetApplicationData([FromBody] string val)
        //string ApplicationID)
        {
            try
            {
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Application Data Request Data - " + ApplicationID);

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
                var fetchapplication = mutationServices.GetApplicationDetails(UserID, ApplicationID);
                if (fetchapplication.mutationCTSNoData == null || fetchapplication.mutationCTSNoData.Count <= 0)
                {
                    type = ReponseType.Failure;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया माहिती फेरफार नोंदी मध्ये परिणाम झालेले न.भू.क्र. भरा.", fetchapplication.mutationCTSNoData))));
                }
                if (fetchapplication.applicationDtl!.applicationType == "नोंदणी कृत")
                {
                    if (fetchapplication.dastInformation == null || fetchapplication.dastInformation.Count <= 0)
                    {
                        type = ReponseType.Failure;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया दस्त माहिती भरा.", fetchapplication.dastInformation))));
                    }
                }
                //if (fetchapplication.Mutation == null || fetchapplication.Mutation!.Count <= 0)
                //var mutationList = fetchapplication.Mutation as IEnumerable<dynamic>;

                //if (mutationList == null || !mutationList.Any(m => m.value != null && ((IEnumerable<object>)m.value).Any()))
                //{
                //    //return "Please fill Dast information";
                //    type = ReponseType.Failure;
                //    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया फेरफार तपशील भरा.", fetchapplication.Mutation))));
                //}
                if (fetchapplication.Mutation is IEnumerable<dynamic> mutationList)
                {
                    foreach (var mutation in mutationList)
                    {
                        if (mutation.value is IEnumerable<dynamic> values)
                        {
                            if (!values.Any())
                            {
                                type = ReponseType.Failure;
                                return Security.EnCryptData(JsonConvert.SerializeObject(
                                    Ok(ResponseHandler.GetAppResponse(type, "कृपया फेरफार तपशील भरा.", fetchapplication.Mutation))
                                ));
                            }
                        }
                        else if (mutation.value == null) //for bhadepatta
                        {
                            type = ReponseType.Failure;
                            return Security.EnCryptData(JsonConvert.SerializeObject(
                                Ok(ResponseHandler.GetAppResponse(type, "कृपया फेरफार तपशील भरा.", fetchapplication.Mutation))
                            ));
                        }
                        // case 3: value is single object → allow it
                    }
                }
                //if (fetchapplication.Mutation is IEnumerable<dynamic> mutationList)
                //{
                //    foreach (var mutation in mutationList)
                //    {
                //        if (mutation.value is not IEnumerable<dynamic> values || !values.Any())
                //        {
                //            type = ReponseType.Failure;
                //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया फेरफार तपशील भरा.", fetchapplication.Mutation))));
                //        }
                //    }
                //}
                if (fetchapplication.applicationDtl!.do_you_have_power_of_attorney == true)
                {
                    if (fetchapplication.poa == null || fetchapplication.poa.Count <= 0)
                    {
                        type = ReponseType.Failure;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया मुखत्यारपत्र माहिती भरा.", fetchapplication.poa))));
                    }
                }

                if (fetchapplication.applicationDtl!.Is_the_claim_pending_before_the_court == true)
                {
                    if (fetchapplication.courtClaimInformation == null || fetchapplication.courtClaimInformation.Count <= 0)
                    {
                        type = ReponseType.Failure;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "कृपया कोर्ट दावा माहिती भरा.", fetchapplication.courtClaimInformation))));
                    }
                }
                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Found", fetchapplication))));
                //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Found", fetchapplication)));
                //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Found", fetchapplication))));
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
        [Authorize]
        [HttpPost]
        [Route("GetApplicationDataforNIC")]
        public string GetApplicationDataforNIC(string ApplicationID)
        //[FromBody] string val)
        {
            try
            {
                //var decrypted = Security.DeCryptDataNIC(val);
                //string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Application Data Request Data - " + ApplicationID);

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
                var fetchapplication = nicServices.GetApplicationDataForNIC(UserID, ApplicationID);
                //ApiResponseForNIC response = new ApiResponseForNIC();
                //response.ResponseData = fetchapplication;
                return JsonConvert.SerializeObject(fetchapplication);
                //return Security.EnCryptDataNIC(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data Found", fetchapplication))));
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

        // Created New APIS
        [Authorize]
        [HttpPost]
        [Route("GetGiverTakerInfo")]
        public string GetGiverTakerInfo([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetGiverTakerInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Giver And Taker Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchGiverTakerInfoData> mutationNameInMarathi = new List<FetchGiverTakerInfoData>();
                List<FetchGiverTakerInfoData> mutationNameInEng = new List<FetchGiverTakerInfoData>();

                //List<dynamic> mutationNameInMarathi = new List<dynamic>();
                //List<dynamic> mutationNameInEng = new List<dynamic>();
                if (applicationDTL != null)
                {
                    List<dynamic> mutation = new List<dynamic>();
                    //List<FetchMayatDetailsData> fetchmayatinfoList = new List<FetchMayatDetailsData>();
                    //List<FetchMrutuDakhalaDetailsData> fetchMrutyuDetails = new List<FetchMrutuDakhalaDetailsData>();
                    List<FetchVarasNondDetailsData> fetchvarasNondList = new List<FetchVarasNondDetailsData>();
                    if (applicationDTL!.mutation_type_code == "01")
                    {
                        //if (!string.IsNullOrEmpty(applicationDTL.mayatIDs))
                        //{
                        //    string[] kharediNondIDs = applicationDTL.mayatIDs!.Split(",");

                        //    if (kharediNondIDs.Length > 0)
                        //    {
                        //        for (int i = 0; i < kharediNondIDs.Length; i++)
                        //        {
                        //            FetchMayatDetailsData fetchKharediNondGiverInformationData = new FetchMayatDetailsData();
                        //            fetchKharediNondGiverInformationData = mutationServices.FetchMayatDetails(Convert.ToInt32(kharediNondIDs[i]));
                        //            if (fetchKharediNondGiverInformationData != null)
                        //            {
                        //                fetchmayatinfoList.Add(fetchKharediNondGiverInformationData);
                        //            }
                        //            FetchMrutuDakhalaDetailsData fetchKharediNondGiverInformationData1 = new FetchMrutuDakhalaDetailsData();
                        //            fetchKharediNondGiverInformationData1 = mutationServices.FetchMrutuDakhalaDetails(Convert.ToInt32(kharediNondIDs[i]));
                        //            //if (fetchKharediNondGiverInformationData1 != null)
                        //            //{
                        //            //    fetchMrutyuDetails.Add(fetchKharediNondGiverInformationData1);
                        //            //}
                        //        }
                        //    }
                        //}
                        if (!string.IsNullOrEmpty(applicationDTL.varasIDS))
                        {
                            string[] kharediNondIDs = applicationDTL.varasIDS.Split(",");

                            if (kharediNondIDs.Length > 0)
                            {
                                for (int i = 0; i < kharediNondIDs.Length; i++)
                                {
                                    FetchVarasNondDetailsData fetchdata = new FetchVarasNondDetailsData();
                                    fetchdata = mutationServices.FetchVarasData(Convert.ToInt32(kharediNondIDs[i]));
                                    if (fetchdata != null)
                                    {
                                        fetchvarasNondList.Add(fetchdata);
                                    }
                                }
                            }
                        }

                        //foreach (dynamic Data in fetchmayatinfoList)
                        //{
                        //    FetchGiverTakerInfoData fetchMarathiData = new FetchGiverTakerInfoData();
                        //    fetchMarathiData.code = Data.mayat_id;
                        //    fetchMarathiData.name = Data.fullNameInMarathi + " ( मयत )";
                        //    mutationNameInMarathi.Add(fetchMarathiData);

                        //    FetchGiverTakerInfoData fetchEngData = new FetchGiverTakerInfoData();
                        //    fetchEngData.code = Data.mayat_id;
                        //    fetchEngData.name = Data.fullNameInEng + " ( Mayat )";
                        //    mutationNameInEng.Add(fetchEngData);
                        //}
                        foreach (dynamic takerData in fetchvarasNondList)
                        {
                            FetchGiverTakerInfoData fetchMarathiData = new FetchGiverTakerInfoData();
                            fetchMarathiData.code = takerData.mutation_dtl_id;
                            fetchMarathiData.name = takerData.fullNameInMarathi + " ( वारस )";
                            mutationNameInMarathi.Add(fetchMarathiData);

                            FetchGiverTakerInfoData fetchEngData = new FetchGiverTakerInfoData();
                            fetchEngData.code = takerData.mutation_dtl_id;
                            fetchEngData.name = takerData.fullNameInEng + " ( Varas )";
                            mutationNameInEng.Add(fetchEngData);
                        }
                        mutation.Add(mutationNameInMarathi);
                        mutation.Add(mutationNameInEng);
                    }
                    else
                    {
                        List<dynamic> giver = mutationServices.mutationgiverData(applicationDTL);
                        //Mutation Taker
                        List<dynamic> taker = mutationServices.mutationtakerData(applicationDTL);

                        if (applicationDTL.mutation_type_code == "06")
                        {
                            //Below code is commented because In Gahankhat there should be only Taker data to show.
                            foreach (dynamic giverData in giver)
                            {
                                FetchGiverTakerInfoData fetchMarathiData = new FetchGiverTakerInfoData();
                                fetchMarathiData.code = giverData.mutation_givertaker_id;
                                fetchMarathiData.name = giverData.fullNameInMarathi + " ( देणारा )";
                                //fetchMarathiData.name = giverData.fullNameInMarathi + " ( घेणारा )";
                                mutationNameInMarathi.Add(fetchMarathiData);

                                FetchGiverTakerInfoData fetchEngData = new FetchGiverTakerInfoData();
                                fetchEngData.code = giverData.mutation_givertaker_id;
                                //fetchEngData.name = giverData.fullNameInEng + " ( Taker )";
                                fetchEngData.name = giverData.fullNameInEng + " ( Giver )";
                                mutationNameInEng.Add(fetchEngData);
                            }
                            //End
                            foreach (dynamic takerData in taker)
                            {
                                FetchGiverTakerInfoData fetchMarathiData = new FetchGiverTakerInfoData();
                                fetchMarathiData.code = takerData.mutation_givertaker_id;
                                //fetchMarathiData.name = takerData.fullNameInMarathi + " ( देणारा )";
                                fetchMarathiData.name = takerData.fullNameInMarathi + " ( घेणारा )";
                                mutationNameInMarathi.Add(fetchMarathiData);

                                FetchGiverTakerInfoData fetchEngData = new FetchGiverTakerInfoData();
                                fetchEngData.code = takerData.mutation_givertaker_id;
                                fetchEngData.name = takerData.fullNameInEng + " ( Taker )";
                                //fetchEngData.name = takerData.fullNameInEng + " ( Giver )";
                                mutationNameInEng.Add(fetchEngData);
                            }
                        }
                        else if (applicationDTL.mutation_type_code == "09")
                        {
                            foreach (dynamic giverData in giver)
                            {
                                FetchGiverTakerInfoData fetchMarathiData = new FetchGiverTakerInfoData();
                                fetchMarathiData.code = giverData.mutation_givertaker_id;
                                fetchMarathiData.name = giverData.fullNameInMarathi + " ( देणारा )";
                                mutationNameInMarathi.Add(fetchMarathiData);

                                FetchGiverTakerInfoData fetchEngData = new FetchGiverTakerInfoData();
                                fetchEngData.code = giverData.mutation_givertaker_id;
                                fetchEngData.name = giverData.fullNameInEng + " ( Giver )";
                                mutationNameInEng.Add(fetchEngData);
                            }
                            foreach (dynamic takerData in taker)
                            {
                                FetchGiverTakerInfoData fetchMarathiData = new FetchGiverTakerInfoData();
                                fetchMarathiData.code = takerData.mutation_dtl_id;
                                fetchMarathiData.name = takerData.fullNameInMarathi + " ( घेणारा )";
                                mutationNameInMarathi.Add(fetchMarathiData);

                                FetchGiverTakerInfoData fetchEngData = new FetchGiverTakerInfoData();
                                fetchEngData.code = takerData.mutation_dtl_id;
                                fetchEngData.name = takerData.fullNameInEng + " ( Taker )";
                                mutationNameInEng.Add(fetchEngData);
                            }
                        }
                        else
                        {
                            foreach (dynamic giverData in giver)
                            {
                                FetchGiverTakerInfoData fetchMarathiData = new FetchGiverTakerInfoData();
                                fetchMarathiData.code = giverData.mutation_dtl_id;
                                fetchMarathiData.name = giverData.fullNameInMarathi + " ( देणारा )";
                                mutationNameInMarathi.Add(fetchMarathiData);

                                FetchGiverTakerInfoData fetchEngData = new FetchGiverTakerInfoData();
                                fetchEngData.code = giverData.mutation_dtl_id;
                                fetchEngData.name = giverData.fullNameInEng + " ( Giver )";
                                mutationNameInEng.Add(fetchEngData);
                            }
                            foreach (dynamic takerData in taker)
                            {
                                FetchGiverTakerInfoData fetchMarathiData = new FetchGiverTakerInfoData();
                                fetchMarathiData.code = takerData.mutation_dtl_id;
                                fetchMarathiData.name = takerData.fullNameInMarathi + " ( घेणारा )";
                                mutationNameInMarathi.Add(fetchMarathiData);

                                FetchGiverTakerInfoData fetchEngData = new FetchGiverTakerInfoData();
                                fetchEngData.code = takerData.mutation_dtl_id;
                                fetchEngData.name = takerData.fullNameInEng + " ( Taker )";
                                mutationNameInEng.Add(fetchEngData);
                            }
                        }
                        mutation.Add(mutationNameInMarathi);
                        mutation.Add(mutationNameInEng);
                    }
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Giver And Taker Data Found", mutation))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation Giver And Taker Data Is Not Exists For The Given Application ID", ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Mutation Giver And Taker Info Exception - " + ex.StackTrace!.ToString());
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

        // Below code added on 16 Feb 2026
        //Bhadepatta nond 
        [Authorize]
        [HttpPost]
        [Route("CreateBhadepattaNondGiver")]
        public string CreateBhadepattaNondGiver([FromBody] string val)
        //BhadepattaGiverInputModel BhadepattaGiverData)
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
                    _logger.LogInformation("Create Bhadepatta Nond Giver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                BhadepattaGiverInputModel BhadepattaGiverData = JsonConvert.DeserializeObject<BhadepattaGiverInputModel>(decrypted!)!;
                _logger.LogInformation("Create Bhadepatta Giver Request Data - " + decrypted);
                BhadepattaGiverData.userid = UserID;
                //BhadepattaGiverData.userid = 361;
                string Response = mutationServices.SaveBhadepattaGiverData(BhadepattaGiverData);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(BhadepattaGiverData.applicationid!, "Create Bhadepatta Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Giver Is Created Successfully", null))));
                }
                else if (Response == "Update")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(BhadepattaGiverData.applicationid!, "Create Bhadepatta Nond Giver Data Updated Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Giver Is Updated Successfully", null))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Bhadepatta Nond Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Bhadepatta Nond Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetBhadepattaGiverData")]
        public async Task<string> GetBhadepattaGiverData([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Get Bhadepatta Giver Data - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("GetBhadepatta Giver Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchBhadepattaData> fetchBhadepattaDataList = new List<FetchBhadepattaData>();
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] BhadepattaGiverIds = applicationDTL.mutationgiverIDs.Split(",");
                    if (BhadepattaGiverIds.Length > 0)
                    {
                        for (int i = 0; i < BhadepattaGiverIds.Length; i++)
                        {
                            FetchBhadepattaData fetchBhadepattaData = new FetchBhadepattaData();
                            fetchBhadepattaData = await mutationServices.FetchBhadepattaGiver(Convert.ToInt32(BhadepattaGiverIds[i]));
                            if (fetchBhadepattaData != null)
                            {
                                fetchBhadepattaDataList.Add(fetchBhadepattaData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Giver Data Found", fetchBhadepattaDataList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Giver Data Not Found", fetchBhadepattaDataList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data not found", fetchBhadepattaDataList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Bhadepatta Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteBhadepattaGiver")]
        public string DeleteBhadepattaGiver([FromBody] string val)
        //DeleteMutation delete)
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
                    _logger.LogInformation("Delete Bhadepatta Giver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Bhadepatta for Giver Request Data - " + decrypted);
                string Response = string.Empty;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] kharediNondIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    if (kharediNondIDs.Length > 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                    }
                    if (kharediNondIDs.Length == 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                        if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                        {
                            string[] takerids = applicationDTL.mutationtakerIDs!.Split(",");
                            if (takerids.Length > 0)
                            {
                                for (int i = 0; i < takerids.Length; i++)
                                {
                                    delete.MutationId = Convert.ToInt32(takerids[i]);
                                    Response = mutationServices.DeleteMutationTaker(delete);
                                }
                            }
                        }
                        FetchBhadepattaInfoData fetchBhadepattaInfoData =  mutationServices.FetchBhadepattaInfoData(delete.applicationid!);
                        if (fetchBhadepattaInfoData != null)
                        {
                            DeleteBhadepattaInfo deleteBhadepattaInfo = new DeleteBhadepattaInfo();
                            deleteBhadepattaInfo.applicationid = fetchBhadepattaInfoData.applicationid;
                            deleteBhadepattaInfo.info_id = fetchBhadepattaInfoData.Info_id;
                            Response =  mutationServices.DeleteBhadepattaInfo(deleteBhadepattaInfo);
                        }
                    }
                }
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Bhadepatta Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Giver Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Bhadepatta For Giver Exception - " + ex.Source!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateBhadepattaNondTaker")]
        public string CreateBhadepattaNondTaker(
       //BhadepattaTakerInputModel bhadepattaTakerInputModel)
       [FromBody] string val)
        {
            try
            {
                var decrypted = Security.DeCryptData(val);
                BhadepattaTakerInputModel bhadepattaTakerInputModel = JsonConvert.DeserializeObject<BhadepattaTakerInputModel>(decrypted!)!;
                _logger.LogInformation("Create Bhadepatta For Taker Request Data - " + decrypted);

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
                bhadepattaTakerInputModel.userid = UserID;
                ReponseType type = ReponseType.Success;
                string Response =  mutationServices.SaveBhadepattaTakerData(bhadepattaTakerInputModel);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(bhadepattaTakerInputModel.applicationid!, "Create Bhadepatta Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta taker Is Created Successfully", null))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Bhadepatta Nond For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Bhadepatta Nond For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetBhadepattaTakerData")]
        public async Task<string> GetBhadepattaTakerData([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Get Bhadepatta Taker Data - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("GetBhadepatta Taker Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchBhadepattaData> fetchBhadepattaDataList = new List<FetchBhadepattaData>();
                if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                {
                    string[] BhadepattaTakerIds = applicationDTL.mutationtakerIDs.Split(",");
                    if (BhadepattaTakerIds.Length > 0)
                    {
                        for (int i = 0; i < BhadepattaTakerIds.Length; i++)
                        {
                            FetchBhadepattaData fetchBhadepattaData = new FetchBhadepattaData();
                            fetchBhadepattaData = await mutationServices.FetchBhadepattaTaker(Convert.ToInt32(BhadepattaTakerIds[i]));
                            if (fetchBhadepattaData != null)
                            {
                                fetchBhadepattaDataList.Add(fetchBhadepattaData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Taker Data Found", fetchBhadepattaDataList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Taker Data Not Found", fetchBhadepattaDataList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data not found", fetchBhadepattaDataList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Bhadepatta Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteBhadepattaTaker")]
        public string DeleteBhadepattaTaker([FromBody] string val)
        //DeleteMutation delete)
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
                    _logger.LogInformation("Delete Bhadepatta For Taker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Bhadepatta For Taker Request Data - " + decrypted);
                string Response = string.Empty;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                {
                    string[] BhadepattaIds = applicationDTL.mutationtakerIDs!.Split(",");
                    if (BhadepattaIds.Length == 1)
                    {
                        FetchBhadepattaInfoData fetchBhadepattaInfoData =  mutationServices.FetchBhadepattaInfoData(delete.applicationid!);
                        if (fetchBhadepattaInfoData != null)
                        {
                            DeleteBhadepattaInfo deleteBhadepattaInfo = new DeleteBhadepattaInfo();
                            deleteBhadepattaInfo.applicationid = fetchBhadepattaInfoData.applicationid;
                            deleteBhadepattaInfo.info_id = fetchBhadepattaInfoData.Info_id;
                            Response =  mutationServices.DeleteBhadepattaInfo(deleteBhadepattaInfo);
                        }
                    }
                }
                Response = mutationServices.DeleteMutationTaker(delete);
                List<FetchBhadepattaData> fetchBhadepattaList = new List<FetchBhadepattaData>();
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Bhadepatta nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Nond Taker Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Bhadepatta For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchBhadepattaList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Bhadepatta For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("SaveBhadepattaInfoData")]
        public string SaveBhadepattaInfoData(
      //BhadepattaInfoInputModel bhadepattaInfoInputModel)
      [FromBody] string val)
        {
            try
            {
                var decrypted = Security.DeCryptData(val);
                BhadepattaInfoInputModel bhadepattaInfoInputModel = JsonConvert.DeserializeObject<BhadepattaInfoInputModel>(decrypted!)!;
                _logger.LogInformation("Save Bhadepatta Info Request Data - " + decrypted);

                int UserID = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                var CallAPIForFlag = Request.Headers["CallAPIFor"];
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
                bhadepattaInfoInputModel.userid = UserID;
                ReponseType type = ReponseType.Success;
                string Response =  mutationServices.SaveBhadepattaInfoData(bhadepattaInfoInputModel);
                if (Response == "Success")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Info Data is Created Successfully", null))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Bhadepatta Info Data Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Bhadepatta Info Data Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetBhadepattaInfoData")]
        public string GetBhadepattaInfoData([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Get Bhadepatta Info Data - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Bhadepatta Info Data Request Data - " + ApplicationID);
                FetchBhadepattaInfoData fetchBhadepattaInfoData =  mutationServices.FetchBhadepattaInfoData(ApplicationID);
                if (fetchBhadepattaInfoData != null)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Bhadepatta Info Data Found", fetchBhadepattaInfoData))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Get Bhadepatta Info Response data Failed - " + "Something went wrong");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Bhadepatta Info Response data Failed", null))));
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Get  Bhadepatta Info Data Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteBhadepattaInfoData")]
        public string DeleteBhadepattaInfoData([FromBody] string val)
        //DeleteBhadepattaInfo delete)
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
                //check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Bhadepatta Info Data- Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteBhadepattaInfo delete = JsonConvert.DeserializeObject<DeleteBhadepattaInfo>(decrypted!)!;
                _logger.LogInformation("Delete Bhadepatta For Taker Request Data - " + decrypted);

                string data =  mutationServices.DeleteBhadepattaInfo(delete);
                if (data.ToLower() == "success")
                {
                    _logger.LogInformation("Bhadepatta Info Data Deleted Successfully" + data);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Info Data Deleted Successfully", null))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Bhadepatta Info Data error" + data);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Bhadepatta Info Data error", data))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Bhadepatta Info Data Exception - " + ex.StackTrace!.ToString());
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
        [Route("CreateAkumaiNondForGiver")]
        public string CreateAkumaiNondForGiver([FromBody] string val)
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
                    _logger.LogInformation("Create AkumaiNond For Giver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                AkumaiDataForGiver giverData = JsonConvert.DeserializeObject<AkumaiDataForGiver>(decrypted!)!;
                _logger.LogInformation("Create Akumai Nond For Giver Request Data - " + decrypted);
                giverData.userid = UserID;
                string Response = mutationServices.SaveAkumaiGiver(giverData);
                List<FetchKharediNondDataForGiver> fetchKharediNondList = new List<FetchKharediNondDataForGiver>();
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(giverData.applicationid!, "Create Akumai Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Akumai Nond Is Created Successfully", ""))));
                }
                else if (Response == "Update")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(giverData.applicationid!, "Create Akumai Nond Giver Data Updated Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Akumai Nond Is Updated Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Akumai Nond For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Akumai Nond For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteAkumaiNondForGiver")]
        public string DeleteAkumaiNondForGiver([FromBody] string val)
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
                    _logger.LogInformation("Delete Akumai Nond For Giver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Akumai Nond For Giver Request Data - " + decrypted);
                string Response = string.Empty;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] giverIDS = applicationDTL.mutationgiverIDs!.Split(",");
                    if (giverIDS.Length > 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                    }
                    if (giverIDS.Length == 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                        if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                        {
                            string[] takerids = applicationDTL.mutationtakerIDs!.Split(",");
                            if (takerids.Length > 0)
                            {
                                for (int i = 0; i < takerids.Length; i++)
                                {
                                    delete.MutationId = Convert.ToInt32(takerids[i]);
                                    Response = mutationServices.DeleteMutationTaker(delete);
                                }
                            }
                        }
                    }
                }
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Akumai Nond Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Akumai Nond Giver Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Akumai Nond For Giver Exception - " + ex.Source!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("GetAkumaiNondGiverInfo")]
        public string GetAkumaiNondGiverInfo([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Get AkumaiNond Giver Info - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Akumai Nond Giver Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchAkumaiNondDataForGiver> dataList = new List<FetchAkumaiNondDataForGiver>();
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs))
                {
                    string[] giverIDS = applicationDTL.mutationgiverIDs.Split(",");
                    if (giverIDS.Length > 0)
                    {
                        for (int i = 0; i < giverIDS.Length; i++)
                        {
                            FetchAkumaiNondDataForGiver fetchData = new FetchAkumaiNondDataForGiver();
                            fetchData = mutationServices.FetchAkumaiNondInformationDataForGiver(Convert.ToInt32(giverIDS[i]));
                            if (fetchData != null)
                            {
                                dataList.Add(fetchData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Akumai Nond Giver Information Data Found", dataList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", dataList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data not found", dataList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Akumai Nond Giver Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("CreateAkumaiNondForTaker")]
        public string CreateAkumaiNondForTaker([FromBody] string val)
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
                    _logger.LogInformation("Create Akumai Varas Nond - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                AkumaiDataForTaker varasnond = JsonConvert.DeserializeObject<AkumaiDataForTaker>(decrypted!)!;
                _logger.LogInformation("Create Akumai Nond Request Data - " + decrypted);
                varasnond.userid = UserID;
                string Response = mutationServices.SaveAkumaiTaker(varasnond);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(varasnond.applicationid!, "Create Akumai Varas Nond Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Akumai Varas Nond Is Created Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Akumai Varas Nond Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Akumai Varas Nond Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetAkumaiNondTakerInfo")]
        public string GetAkumaiNondTakerInfo([FromBody] string val)
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
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Get Akumai Nond Taker Info - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Akumai Nond Taker Info Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchAkumaiNondDataForTaker> dataList = new List<FetchAkumaiNondDataForTaker>();
                if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                {
                    string[] takerIDS = applicationDTL.mutationtakerIDs.Split(",");
                    if (takerIDS.Length > 0)
                    {
                        for (int i = 0; i < takerIDS.Length; i++)
                        {
                            FetchAkumaiNondDataForTaker fetchData = new FetchAkumaiNondDataForTaker();
                            fetchData = mutationServices.FetchAkumaiDataForTaker(Convert.ToInt32(takerIDS[i]));
                            if (fetchData != null)
                            {
                                dataList.Add(fetchData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Akumai Nond Taker Information Data Found", dataList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", dataList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Application Data not found", dataList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Akumai Nond Taker Info Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteAkumaiNondForTaker")]
        public string DeleteAkumaiNondForTaker([FromBody] string val)
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
                    _logger.LogInformation("Delete Akumai Nond For Taker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Akumai Nond For Taker Request Data - " + decrypted);
                string Response = mutationServices.DeleteMutationTaker(delete);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Akumai Nond Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Akumai Nond Taker Data Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Akumai For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Akumai Nond For Taker Exception - " + ex.StackTrace!.ToString());
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

        // mrutyu patra start
        [Authorize]
        [HttpPost]
        [Route("CreateMrutyuPatraInfoForGiver")]
        public string CreateMrutyuPatraInfoForGiver([FromBody] string val)
        //MrutyuPatraDataForGiver mrutyuPatraData)
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
                    _logger.LogInformation("Create Mrutyu Patra Info For Giver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                MrutyuPatraDataForGiver mrutyuPatraData = JsonConvert.DeserializeObject<MrutyuPatraDataForGiver>(decrypted!)!;
                _logger.LogInformation("Create Mrutyu Patra Info For Giver Request Data - " + decrypted);
                mrutyuPatraData.userid = UserID;
                string Response = mutationServices.SaveMrutyuPatraGiver(mrutyuPatraData);
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(mrutyuPatraData.applicationid!, "Create Mrutyu Patra Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Is Created Successfully", ""))));
                }
                else if (Response == "Update")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(mrutyuPatraData.applicationid!, "Create Mrutyu Patra Giver Data Updated Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Is Updated Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Mrutyu Patra Info For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Mrutyu Patra Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("EditMrutyuPatraInfoForGiver")]
        public string EditMrutyuPatraInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("Edit Mrutyu Patra Info For Giver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EditMrutyuPatraDataForGiver mrutyuPatraData = JsonConvert.DeserializeObject<EditMrutyuPatraDataForGiver>(decrypted!)!;
                _logger.LogInformation("Edit Mrutyu Patra Info For Giver Request Data - " + decrypted);
                mrutyuPatraData.userid = UserID;
                string Response = mutationServices.EditMrutyuPatraGiver(mrutyuPatraData);
                List<FetchMrutyuPatraForGiverData> fetchDataLits = new List<FetchMrutyuPatraForGiverData>();
                if (Response == "Success")
                {
                    ApplicationDTL applicationDTL = new ApplicationDTL();
                    applicationDTL = applicationServices.FetchApplicationData(mrutyuPatraData.applicationid!);
                    string[] mutationgiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    for (int i = 0; i < mutationgiverIDs.Length; i++)
                    {
                        FetchMrutyuPatraForGiverData fetchData = new FetchMrutyuPatraForGiverData();
                        //Gauri
                        //fetchData = mutationServices.FetchMrutyuPatraInfoForGiver(Convert.ToInt32(mutationgiverIDs[i]));
                        if (fetchData != null)
                        {
                            fetchDataLits.Add(fetchData);
                        }
                    }
                    applicationServices.SaveApplicationDataSubmittedHistory(mrutyuPatraData.applicationid!, "Edit Mrutyu Patra Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Updated Successfully", fetchDataLits))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Edit Mrutyu Patra Info For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchDataLits))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Edit Mrutyu Patra Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteMrutyuPatraInfoForGiver")]
        public string DeleteMrutyuPatraInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("Delete Mrutyu Patra Info For Giver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Mrutyu Patra Info For Giver Request Data - " + decrypted);
                string Response = string.Empty;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs) && !string.IsNullOrEmpty(applicationDTL.mayatIDs))
                {
                    string[] mutationgiverIDs = applicationDTL.mutationgiverIDs!.Split(",");
                    string[] mayatIDs = applicationDTL.mayatIDs!.Split(",");
                    if (mutationgiverIDs.Length == 1)
                    {
                        Response = mutationServices.DeleteMutationGiver(delete);
                        if (!string.IsNullOrEmpty(applicationDTL.uploadedDocIDs))
                        {
                            string[] uploadedDocIDs = applicationDTL.uploadedDocIDs!.Split(",");
                            for (int i = 0; i < uploadedDocIDs.Length; i++)
                            {
                                DeleteUploadedDocumentData deleteUploadedDocument = new DeleteUploadedDocumentData();
                                deleteUploadedDocument.applicationID = delete.applicationid;
                                deleteUploadedDocument.documentID = uploadedDocIDs[i];
                                Response = applicationServices.DeleteUploadedDocument(deleteUploadedDocument);
                            }
                        }
                        DeleteMayat deleteMayat = new DeleteMayat();
                        deleteMayat.MayatId = Convert.ToInt32(mayatIDs[0]);
                        deleteMayat.applicationid = delete.applicationid;
                        Response = mutationServices.DeleteMayat(deleteMayat);

                        if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                        {
                            string[] takerids = applicationDTL.mutationtakerIDs!.Split(",");
                            if (takerids.Length > 0)
                            {
                                for (int i = 0; i < takerids.Length; i++)
                                {
                                    delete.MutationId = Convert.ToInt32(takerids[i]);
                                    Response = mutationServices.DeleteMutationTaker(delete);
                                }
                            }
                        }
                    }
                }
                if (Response == "Success")
                {
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Mrutyu Patra Giver Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Giver Data Is Deleted Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Mrutyu Patra Info For Giver Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Mrutyu Patra Info For Giver Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetMrutyuPatraInfoForGiver")]
        public string GetMrutyuPatraInfoForGiver([FromBody] string val)
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
                    _logger.LogInformation("Get Mrutyu Patra Info For Giver - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Mrutyu Patra Info For Giver Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchMrutyuPatraForGiverData> fetchDataList = new List<FetchMrutyuPatraForGiverData>();
                if (!string.IsNullOrEmpty(applicationDTL.mutationgiverIDs) && !string.IsNullOrEmpty(applicationDTL.mayatIDs))
                {
                    string[] mutationgiverIDs = applicationDTL.mutationgiverIDs.Split(",");
                    string[] mayatIDs = applicationDTL.mayatIDs!.Split(",");
                    if (mutationgiverIDs.Length > 0 && mayatIDs.Length > 0)
                    {
                        for (int i = 0; i < mayatIDs.Length; i++)
                        {
                            for (int j = 0; j < mutationgiverIDs.Length; j++)
                            {
                                FetchMrutyuPatraForGiverData fetchData = new FetchMrutyuPatraForGiverData();
                                fetchData = mutationServices.FetchMrutyuPatraInfoForGiver(Convert.ToInt32(mutationgiverIDs[j]), Convert.ToInt32(mayatIDs[i]));
                                if (fetchData != null)
                                {
                                    fetchDataList.Add(fetchData);
                                }
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Data Found", fetchDataList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Data Not Found", fetchDataList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Data Not Found", fetchDataList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Mrutyu Patra Info For Giver Exception - " + ex.StackTrace!.ToString());
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

        //Gauri W
        [Authorize]
        [HttpPost]
        [Route("CreateMrutyuPatraInfoForTaker")]
        public string CreateMrutyuPatraInfoForTaker([FromBody] string val)
        //MrutyuPatraDataForTaker mrutyuPatraDataForTaker)
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
                    _logger.LogInformation("Create Mrutyu Patra Info For Taker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                MrutyuPatraDataForTaker mrutyuPatraDataForTaker = JsonConvert.DeserializeObject<MrutyuPatraDataForTaker>(decrypted!)!;
                _logger.LogInformation("Create Mrutyu Patra Info For Taker Request Data - " + mrutyuPatraDataForTaker);
                mrutyuPatraDataForTaker.userid = UserID;
                //mrutyuPatraDataForTaker.userid = 268;
                string Response = mutationServices.SaveMrutyuPatraTaker(mrutyuPatraDataForTaker);
                List<FetchMrutyuPatraForTakerData> fetchDataList = new List<FetchMrutyuPatraForTakerData>();
                if (Response == "Success")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(mrutyuPatraDataForTaker.applicationid!);
                    //string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    //for (int i = 0; i < kharediNondIDs.Length; i++)
                    //{
                    //    FetchMrutyuPatraForTakerData fetchData = new FetchMrutyuPatraForTakerData();
                    //    fetchData = mutationServices.FetchMrutyuPatraForTakerData(Convert.ToInt32(kharediNondIDs[i]));
                    //    if (fetchData != null)
                    //    {
                    //        fetchDataList.Add(fetchData);
                    //    }
                    //}
                    //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Is Created Successfully", fetchDataList))));
                    applicationServices.SaveApplicationDataSubmittedHistory(mrutyuPatraDataForTaker.applicationid!, "Create Mrutyu Patra Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Is Created Successfully", null))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Create Mrutyu Patra Info For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Create Mrutyu Patra Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        //[Route("EditMrutyuPatraInfoForTaker")]
        //public string EditMrutyuPatraInfoForTaker([FromBody] string val)
        //{
        //    try
        //    {
        //        var decrypted = Security.DeCryptData(val);
        //        EditMrutyuPatraDataForTaker mrutyuPatraDataForTaker = JsonConvert.DeserializeObject<EditMrutyuPatraDataForTaker>(decrypted!)!;
        //        _logger.LogInformation("Edit Mrutyu Patra Info For Taker Request Data - " + decrypted);

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
        //        mrutyuPatraDataForTaker.userid = UserID;
        //        ReponseType type = ReponseType.Success;
        //        string Response = mutationServices.EditMrutyuPatraTaker(mrutyuPatraDataForTaker);
        //        List<FetchMrutyuPatraForTakerData> fetchDataList = new List<FetchMrutyuPatraForTakerData>();

        //        if (Response == "Success")
        //        {
        //            ApplicationDTL applicationDTL = new ApplicationDTL();
        //            applicationDTL = applicationServices.FetchApplicationData(mrutyuPatraDataForTaker.applicationid!);
        //            string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
        //            for (int i = 0; i < kharediNondIDs.Length; i++)
        //            {
        //                FetchMrutyuPatraForTakerData fetchData = new FetchMrutyuPatraForTakerData();
        //                fetchData = mutationServices.FetchMrutyuPatraForTakerData(Convert.ToInt32(kharediNondIDs[i]));
        //                if (fetchData != null)
        //                {
        //                    fetchDataList.Add(fetchData);
        //                }
        //            }
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Updated Successfully", fetchDataList))));
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            _logger.LogInformation("Edit Mrutyu Patra Info For Taker Response Failed - " + Response);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, fetchDataList))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Edit Mrutyu Patra Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("DeleteMrutyuPatraInfoForTaker")]
        public string DeleteMrutyuPatraInfoForTaker([FromBody] string val)
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
                //mrutyuPatraDataForTaker.userid = UserID;
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Mrutyu Patra Info For Taker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                DeleteMutation delete = JsonConvert.DeserializeObject<DeleteMutation>(decrypted!)!;
                _logger.LogInformation("Delete Mrutyu Patra Info For Taker Request Data - " + decrypted);
                string Response = mutationServices.DeleteMutationTaker(delete);
                List<FetchMrutyuPatraForTakerData> fetchDataList = new List<FetchMrutyuPatraForTakerData>();
                if (Response == "Success")
                {
                    //ApplicationDTL applicationDTL = new ApplicationDTL();
                    //applicationDTL = applicationServices.FetchApplicationData(delete.applicationid!);
                    //if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                    //{
                    //    string[] kharediNondIDs = applicationDTL.mutationtakerIDs!.Split(",");
                    //    for (int i = 0; i < kharediNondIDs.Length; i++)
                    //    {
                    //        FetchMrutyuPatraForTakerData fetchData = new FetchMrutyuPatraForTakerData();
                    //        fetchData = mutationServices.FetchMrutyuPatraForTakerData(Convert.ToInt32(kharediNondIDs[i]));
                    //        if (fetchData != null)
                    //        {
                    //            fetchDataList.Add(fetchData);
                    //        }
                    //    }
                    //}
                    applicationServices.SaveApplicationDataSubmittedHistory(delete.applicationid!, "Delete Mrutyu Patra Taker Form", CallAPIForFlag);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Taker Data Deleted Successfully", null))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Delete Mrutyu Patra Info For Taker Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, null))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Delete Mrutyu Patra Info For Taker Exception - " + ex.StackTrace!.ToString());
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
        [Route("GetMrutyuPatraInfoForTaker")]
        public string GetMrutyuPatraInfoForTaker([FromBody] string val)
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
                    _logger.LogInformation("Get Mrutyu Patra Info For Taker - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string ApplicationID = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Mrutyu Patra Info For Taker Request Data - " + ApplicationID);
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);
                List<FetchMrutyuPatraForTakerData> fetchDataList = new List<FetchMrutyuPatraForTakerData>();
                if (!string.IsNullOrEmpty(applicationDTL.mutationtakerIDs))
                {
                    string[] mutationtakerIDs = applicationDTL.mutationtakerIDs.Split(",");

                    if (mutationtakerIDs.Length > 0)
                    {
                        for (int i = 0; i < mutationtakerIDs.Length; i++)
                        {
                            FetchMrutyuPatraForTakerData fetchData = new FetchMrutyuPatraForTakerData();
                            fetchData = mutationServices.FetchMrutyuPatraForTakerData(Convert.ToInt32(mutationtakerIDs[i]));
                            if (fetchData != null)
                            {
                                fetchDataList.Add(fetchData);
                            }
                        }
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Data Found", fetchDataList))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Data Not Found", fetchDataList))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mrutyu Patra Data not found", fetchDataList))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Mrutyu Patra Info For Taker Exception - " + ex.StackTrace!.ToString());
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
