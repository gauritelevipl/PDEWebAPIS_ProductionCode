using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.Repository;
using PDEWebAPIS.Services;
using PDEWebAPIS.ViewModel;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using PDEWebAPIS.InputDataModel;

namespace PDEWebAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IGRAPIController : Controller
    {
        private readonly IGRAPIService igrapiServices;
        private readonly UserServices userServices;
        private readonly IConfiguration configuration;
        private readonly ILogger<LoginAPISController> _logger;
        private readonly ILogger<UserServices> _loggeruserService;

        public IGRAPIController(AppDBContext context, IConfiguration configuration, ILogger<LoginAPISController> logger, AppDBContextR contextR, ILogger<UserServices> loggeruserService)
        {
            _loggeruserService = loggeruserService;
            igrapiServices = new IGRAPIService(context);
            userServices = new UserServices(context, contextR, _loggeruserService);
            this.configuration = configuration;
            _logger = logger;
        }

        [Authorize]
        [HttpPost]
        [Route("DIGList")]
        public async Task<string> DIGListAsync()
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

                var response = await igrapiServices.DIGListAsync();
                _logger.LogInformation("IGR DIG List Resonse - " + response);
                if (Convert.ToInt32(response.Split("-")[1]) >= 200 && Convert.ToInt32(response.Split("-")[1]) <= 299)
                {
                    _logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "DIG List Data Found", response.Split("-")[0]))));
                }

                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "DIG List Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }



        }

        [Authorize]
        [HttpPost]
        [Route("JDRList")]
        public string JDRListAsync([FromBody] string val)
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
                    _logger.LogInformation("JDRListAsync - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string digcode = JsonConvert.DeserializeObject<string>(decrypted!)!;
                var response = igrapiServices.JDRListAsync(digcode);
                if (Convert.ToInt32(response.Result.Split("-")[1]) >= 200 && Convert.ToInt32(response.Result.Split("-")[1]) <= 299)
                {
                    _logger.LogInformation("IGR JDR List Resonse - " + response.Result.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "JDR List Data Found", response.Result.Split("-")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "JDR List Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        [Authorize]
        [HttpPost]
        [Route("SROList")]
        public string SROListAsync([FromBody] string val)
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
                    _logger.LogInformation("SROListAsync - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                Sro sro = JsonConvert.DeserializeObject<Sro>(decrypted!)!;
                var response = igrapiServices.SROListAsync(sro);
                if (Convert.ToInt32(response.Result.Split("_$")[1]) >= 200 && Convert.ToInt32(response.Result.Split("_$")[1]) <= 299)
                {
                    _logger.LogInformation("IGR SRO List Resonse - " + response.Result.Split("_$")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "SRO List Data Found", response.Result.Split("_$")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "SRO List Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }


        [Authorize]
        [HttpPost]
        [Route("DocumentStatus")]
        public async Task<string> DocumentStatus([FromBody] string val)
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
                    _logger.LogInformation("DocumentStatus - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                documentStatus doc = JsonConvert.DeserializeObject<documentStatus>(decrypted!)!;
                var response = await igrapiServices.DocumentStatus(doc);
                Console.WriteLine(response.Split("~")[0]);
                if (Convert.ToInt32(response.Split("~")[0]) >= 200 && Convert.ToInt32(response.Split("~")[0]) <= 299)
                {
                    _logger.LogInformation("IGR Document Status Resonse - " + response.Split("~")[1]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Document Status Data Found", response.Split("~")[1]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Document Status Data Not Found", null))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }
    }
}
