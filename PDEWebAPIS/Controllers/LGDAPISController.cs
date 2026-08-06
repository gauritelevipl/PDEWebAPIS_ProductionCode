using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.Model;
using PDEWebAPIS.Services;

namespace PDEWebAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LGDAPISController : Controller
    {
        private readonly LGDAPIServices lgdapiServices;
        private readonly UserServices userServices;
        private readonly IConfiguration configuration;
        private readonly ILogger<LGDAPISController> _logger;
        private readonly ILogger<UserServices> _loggeruserService;

        public LGDAPISController(AppDBContext context, IConfiguration configuration, ILogger<LGDAPISController> logger, AppDBContextR contextR, ILogger<UserServices> loggeruserService)
        {

            lgdapiServices = new LGDAPIServices(context);
            userServices = new UserServices(context, contextR,loggeruserService);
            this.configuration = configuration;
            _logger = logger;
            _loggeruserService = loggeruserService;
        }

        [HttpPost, Route("GetValue")]
        public async Task<string> GetValue(RequestVillage request)
        {
            var response = await lgdapiServices.getDistrictofState(_logger); //await lgdapiServices.GetallDistrict(_logger);
            //var res = await lgdapiServices.getVillagesOfDistrictAndTaluka(request, _logger);

            return response;

        }

        [HttpPost]
        [Route("getDistrictofState")]
        public async Task<string> getDistrictofState()
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
                //int UserID = 0;
                /* var authorization = Request.Headers[HeaderNames.Authorization];
                 if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                 {
                     // we have a valid AuthenticationHeaderValue that has the following details:
                     var scheme = headerValue.Scheme;
                     var Token = headerValue.Parameter;
                     UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                     // scheme will be "Bearer"
                     // parmameter will be the token itself.
                 }
 */
                var response = await lgdapiServices.getDistrictofState(_logger);
                _logger.LogInformation("All District of State List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All District List", response.Split("|")[0]))));
                }

                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All District List Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }


        [HttpPost]
        [Route("getTalukasOfDistrict")]
        public async Task<string> getTalukasOfDistrict([FromBody] string val)
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
                //int UserID = 0;
                /*var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }*/
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("getTalukasOfDistrict - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string district_code = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get office by district Request Data - " + district_code);
                var response = await lgdapiServices.getTalukasOfDistrict(district_code, _logger);
                _logger.LogInformation("All Office By District Code List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Talukas by District List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Talukas by District List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        [HttpPost]
        [Route("getVillagesOfDistrictAndTaluka")]
        public async Task<string> getVillagesOfDistrictAndTaluka([FromBody] string val)
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
                //int UserID = 0;
                /*var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }*/
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("getVillagesOfDistrictAndTaluka - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                RequestVillage requestcts = JsonConvert.DeserializeObject<RequestVillage>(decrypted!)!;
                _logger.LogInformation("Get Vilage Details Request Data - " + requestcts);
                var response = await lgdapiServices.getVillagesOfDistrictAndTaluka(requestcts, _logger);
                _logger.LogInformation("Get Village Details Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get CTS Details Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get CTS Details Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

    }
}
