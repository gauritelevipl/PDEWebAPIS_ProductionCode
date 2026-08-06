using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Services;
using System.Net.Http.Headers;

namespace PDEWebAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EPCISAPISController : Controller
    {
        private readonly EPCISAPIService lgdapiServices;
        private readonly UserServices userServices;
        private readonly IConfiguration configuration;
        private readonly ILogger<EPCISAPISController> _logger;
        private readonly ILogger<UserServices> _loggeruserService;

        //
        private readonly GrievanceService grievanceService;
        private readonly ILogger<EPCISAPIService> _loggers;

        public EPCISAPISController(AppDBContext context, IConfiguration configuration, ILogger<EPCISAPISController> logger, ILogger<EPCISAPIService> loggers, IOptions<EPCISConfig> config, AppDBContextR contextR, ILogger <UserServices> loggeruserservices)
        {
            _logger = logger;
            _loggers = loggers;
            grievanceService = new GrievanceService(context, contextR);
            _loggeruserService = loggeruserservices;
            lgdapiServices = new EPCISAPIService(contextR, config, loggers); 
            userServices = new UserServices(context,contextR,loggeruserservices);
            this.configuration = configuration;
        }
        [HttpPost, Route("GetValue")]
        public async Task<string> GetValue(RequestCTSDetails body)
        {
           
            var response = await lgdapiServices.getCTSNoDetails(body,_logger); //await lgdapiServices.GetallDistrict(_logger);
            var res = await lgdapiServices.getFlatList(body,_logger);
            return response +" | " +res ;
        }

        // [Authorize]
        [HttpPost]
        [Route("allDistrictList")]
        public async Task<string>  allDistrictList()
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
                var response = await lgdapiServices.GetallDistrict(_logger);
                _logger.LogInformation("All District Code List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All District List Data Found", response.Split("|")[0]))));
                }

                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All District List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        //[Authorize]
        [HttpPost]
        [Route("getOfficeByDistrict")]
        public async Task<string> getOfficeByDistrict([FromBody] string val)
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
                    _logger.LogInformation("getOfficeByDistrict - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string district_code = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get office by district Request Data - " + district_code);
                var response = await lgdapiServices.getOfficeByDistrict(district_code,_logger);
                _logger.LogInformation("All Office By District Code List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Offices by District code List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All offices by District code List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        // [Authorize]

        //[HttpPost]
        //[Route("getVillageByOffice")]
        //public async Task<string> getVillageByOffice([FromBody] string val)
        //{
        //    try
        //    {
        //        string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        ReponseType type = ReponseType.Success;
        //        //int UserID = 0;
        //        /*var authorization = Request.Headers[HeaderNames.Authorization];
        //        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        //        {
        //            // we have a valid AuthenticationHeaderValue that has the following details:
        //            var scheme = headerValue.Scheme;
        //            var Token = headerValue.Parameter;
        //            UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }*/
        //        var decrypted = Security.DeCryptData(val);
        //        string office_code = JsonConvert.DeserializeObject<string>(decrypted!)!;
        //        _logger.LogInformation("Get Village Name by office Request Data - " + office_code);
        //        var response = await lgdapiServices.getVillageByOffice(office_code, _logger);
        //        _logger.LogInformation("All Village By office Code List Resonse - " + response);
        //        if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
        //        {
        //            //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by Office code List Data Found", response.Split("|")[0]))));
        //        }

        //        else
        //        {
        //            type = ReponseType.NotFound;
        //            return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by office code List Data Not Found", response.Split("|")[0]))));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
        //    }

        //}

        [HttpPost]
        [Route("getVillageByOffice")]
        public async Task<string> getVillageByOffice([FromBody] string val)
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
                    _logger.LogInformation("getVillageByOffice - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string office_code = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Village Name by office Request Data - " + office_code);
                var response = await lgdapiServices.getVillageByOffice(office_code, _logger);
                _logger.LogInformation("All Village By office Code List Resonse - " + response);



                string[] parts = response.Split("$VIPL");
                _logger.LogInformation("response.Split()[0] : " + parts[1]);
                _logger.LogInformation("response.Split()[1] : " + parts[0]);


                //if (!response.Split("|")[1].IsNullOrEmpty() && !response.Split("|")[0].IsNullOrEmpty() && Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                //{
                //    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                //    _logger.LogInformation(response.Split("|")[0]);
                //    _logger.LogInformation(response.Split("|")[1]);
                //    return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by Office code List Data Found", response.Split("|")[0])));

                //    //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by Office code List Data Found", response.Split("|")[0]))));
                //}

                //parts.Length > 1
                if (parts.Length > 1 && int.TryParse(parts[1], out int statusCode))
                {
                    if (statusCode >= 200 && statusCode <= 299)
                    {
                        //_logger.LogInformation(response.Split("|")[0]);
                        //_logger.LogInformation(response.Split("|")[1]);
                        _logger.LogInformation("Data found: " + parts[0]);
                        //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by Office code List Data Found", parts[0])));
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by Office code List Data Found", parts[0]))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        _logger.LogInformation("Data Not found: " + parts[0]);
                        //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by office code List Data Not Found", parts[0])));
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by office code List Data Not Found", parts[0]))));
                    }
                }

                else
                {
                    type = ReponseType.NotFound;
                    _logger.LogInformation("Data not found: " + parts[0]);
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by office code List Data Not Found", parts[0])));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by office code List Data Not Found", parts[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error->" + ex.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }
        // [Authorize]
        [HttpPost]
        [Route("pdeApplicationTypeList")]
        public async Task<string> pdeApplicationTypeList()
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
              /*  var authorization = Request.Headers[HeaderNames.Authorization];
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
                var response = await lgdapiServices.pdeApplicationTypeList(_logger);
                _logger.LogInformation("Get Application Type List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Application Type List Data Found", response.Split("|")[0]))));
                }

                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Application Typse List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

      //  [Authorize]
        [HttpPost]
        [Route("getMutationType")]
        public async Task<string> getMutationType([FromBody] string val)
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
                    _logger.LogInformation("getMutationType - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);
                var response = await lgdapiServices.getMutationType(mut_category,_logger);
                _logger.LogInformation("All Mutation Type List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Mutation Type List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Mutation Type List List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        //[Authorize]
        [HttpPost]
        [Route("getDocListMutationtype")]
        public async Task<string> getDocListMutationtype([FromBody] string val)
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
                 }*/
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("getDocListMutationtype - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EPCISgetDocListMutationtype ePCISgetDocListMutationtype = JsonConvert.DeserializeObject<EPCISgetDocListMutationtype>(decrypted!)!;
                _logger.LogInformation("Get Document List for Mutation Type Request Data - " + ePCISgetDocListMutationtype.mut_type + " " + ePCISgetDocListMutationtype.mut_category);
                var response = await lgdapiServices.getDocListMutationtype(ePCISgetDocListMutationtype, _logger);
                _logger.LogInformation("All Document List for Mutation Type List Resonse - " + response);
                //string mut_type = JsonConvert.DeserializeObject<string>(decrypted!)!;
                //_logger.LogInformation("Get Document List for Mutation Type Request Data - " + mut_type);
                //var response = await lgdapiServices.getDocListMutationtype(mut_type,_logger);
                _logger.LogInformation("All Document List for Mutation Type List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Document List for Mutation Type List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Document List for Mutation Type List List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

       // [Authorize]
        [HttpPost]
        [Route("applicationTypeList")]
        public async Task<string> applicationTypeList()
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
             /*   var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }*/
               /* var decrypted = Security.DeCryptData(val);
                string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.applicationTypeList(_logger);
                _logger.LogInformation("All application Type List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Applicants Type List Data Found", response.Split("|")[0]))));
                }

                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Applicants Type List List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        //[Authorize]
        [HttpPost]
        [Route("getCTSNoDetails")]
        public async Task<string> getCTSNoDetails([FromBody] string val)
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
                    _logger.LogInformation("getCTSNoDetails - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                RequestCTSDetails requestcts = JsonConvert.DeserializeObject<RequestCTSDetails>(decrypted!)!;
                _logger.LogInformation("Get CTS Details Request Data - " + requestcts);
                var response = await lgdapiServices.getCTSNoDetails(requestcts, _logger);
                _logger.LogInformation("Get CTS Details Resonse - " + response);
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
        //

        //[Authorize]
        [HttpPost]
        [Route("getOwnerNameInfo")]
        public async Task<string> getOwnerNameInfo([FromBody] string val)
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
                    _logger.LogInformation("getOwnerNameInfo - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                RequestOwnerNameInfo requestcts = JsonConvert.DeserializeObject<RequestOwnerNameInfo>(decrypted!)!;
                _logger.LogInformation("Get Owner Name Details Request Data - " + requestcts);
                var response = await lgdapiServices.getOwnerNameInfo(requestcts, _logger);
                _logger.LogInformation("Get Owner Name Details Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get CTS Details Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "माहिती उपलब्ध नाही, कृपया संबंधित कार्यालयाशी संपर्क साधा", response.Split("|")[0]))));
                    //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get CTS Details Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        //getOwnerDetails
       // [Authorize]
        [HttpPost]
        [Route("getOwnerDetails")]
        public async Task<string> getOwnerDetails([FromBody] string val)
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
                 }*/
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("getOwnerDetails - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                RequestOwnerDetails requestcts = JsonConvert.DeserializeObject<RequestOwnerDetails>(decrypted!)!;
                _logger.LogInformation("Get Owner Details Details Request Data - " + requestcts);
                var response = await lgdapiServices.getOwnerDetails(requestcts, _logger);
                _logger.LogInformation("Get Owner Details Details Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Owner Details Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Owner Details Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        //[Authorize]
        [HttpPost]
        [Route("nameTitleList")]
        public async Task<string> nameTitleList()
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
                var response = await lgdapiServices.nameTitleList(_logger);
                _logger.LogInformation("Get Name Title List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Name Title List Data Found", response.Split("|")[0])));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Name Title List Data Found", response.Split("|")[0]))));
                }

                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Name Title List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }


        //[Authorize]
        [HttpPost]
        [Route("getFlatList")]
        public async Task<string> getFlatList([FromBody] string val)
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
                    _logger.LogInformation("getFlatList - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                RequestCTSDetails requestcts = JsonConvert.DeserializeObject<RequestCTSDetails>(decrypted!)!;
                _logger.LogInformation("Get Flat Details Request Data - " + requestcts);
                var response = await lgdapiServices.getFlatList(requestcts, _logger);
                _logger.LogInformation("Get Flat Details Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Flat Details Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Flat Details Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

       // [Authorize]
        [HttpPost]
        [Route("getCTSDetails")]
        public async Task<string> getCTSDetails([FromBody] string val)
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
                    _logger.LogInformation("getCTSDetails - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                RequestCTSDetails requestcts = JsonConvert.DeserializeObject<RequestCTSDetails>(decrypted!)!;
                _logger.LogInformation("Get CTS Details Request Data - " + requestcts);
                var response = await lgdapiServices.getCTSDetails(requestcts, _logger);
                _logger.LogInformation("Get CTS Details Resonse - " + response);
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

       // [Authorize]
        [HttpPost]
        [Route("getSroOfficeList")]
        public async Task<string> getSroOfficeList([FromBody] string val)
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
                    _logger.LogInformation("getSroOfficeList - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                int district_code = JsonConvert.DeserializeObject<int>(decrypted!)!;
                _logger.LogInformation("Get SRO Office List By District Code Request Data - " + district_code);
                var response = await lgdapiServices.getSroOfficeList(district_code, _logger);
                _logger.LogInformation("Get SRO Office List By District Code Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get SRO Office List By District Code Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get SRO Office List By District Code Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get SRO Office List By District Code Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

       // [Authorize]
        [HttpPost]
        [Route("poaTypeList")]
        public async Task<string> poaTypeList()
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
               // int UserID = 0;
               /* var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }*/
                /* var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.poaTypeList(_logger);
                _logger.LogInformation("Get POA Type List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get POA Type List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get POA Type List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get POA Type List Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        //[Authorize]
        [HttpPost]
        [Route("caseTypeList")]
        public async Task<string> caseTypeList([FromBody] string val)
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
                    _logger.LogInformation("caseTypeList - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string district_code = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Case Type List By District Code Request Data - " + district_code);
                var response = await lgdapiServices.caseTypeList(district_code, _logger);
                _logger.LogInformation("Get Case Type List By District Code Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Case Type List By District Code Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Case Type List By District Code Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Case Type List By District Code Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        //[Authorize]
        [HttpPost]
        [Route("deathCertificateList")]
        public async Task<string> deathCertificateList()
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
                /* var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.deathCertificateList(_logger);
                _logger.LogInformation("Get Death Certificate List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Death Certificate List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Death Certificate List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Death Certificate List Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

       // [Authorize]
        [HttpPost]
        [Route("ownerStatusOrCategory")]
        public async Task<string> ownerStatusOrCategory()
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
                /* var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.ownerStatusOrCategory(_logger);
                _logger.LogInformation("Get Owner Status Or Category Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Owner Status Or Category Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Owner Status Or Category Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Owner Status Or Category Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

      //  [Authorize]
        [HttpPost]
        [Route("holderRelationList")]
        public async Task<string> holderRelationList()
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
              /*  var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }*/
                /* var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.holderRelationList(_logger);
                _logger.LogInformation("Get Holder Relation List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Holder Relation List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Holder Relation List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Holder Relation List Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        //[Authorize]
        [HttpPost]
        [Route("genderList")]
        public async Task<string> genderList()
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
               // int UserID = 0;
               /* var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }*/
                /* var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.genderList(_logger);
                _logger.LogInformation("Get Gender List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Gender List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Gender List List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Gender Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

       // [Authorize]
        [HttpPost]
        [Route("apkMasterList")]
        public async Task<string> apkMasterList()
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
                /* var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.apkMasterList(_logger);
                _logger.LogInformation("Get Apk Master List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Apk Master List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Apk Master List List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Apk Master Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        //[Authorize]
        [HttpPost]
        [Route("ownerAccountType")]
        public async Task<string> ownerAccountType()
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
               // int UserID = 0;
               /* var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    UserID = userServices.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }*/
                /* var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.ownerAccountType(_logger);
                _logger.LogInformation("Get Owner Account Type List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Owner Account Type List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Owner Account Type List List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Owner Account Type Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        //[Authorize]
        [HttpPost]
        [Route("bojaInstituteList")]
        public async Task<string> bojaInstituteList()
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
                }*/
                /* var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.bojaInstituteList(_logger);
                _logger.LogInformation("Get All Boja Institute List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Boja Institute List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Boja Institute List List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get All Boja Institute Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        //[Authorize]
        [HttpPost]
        [Route("getPropertyDetails")]
        public async Task<string> getPropertyDetails([FromBody] string val)
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
                 }*/
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("getPropertyDetails - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string mut_type = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Property Details By Village Code Request Data - " + mut_type);
                var response = await lgdapiServices.getPropertyDetails(mut_type, _logger);
                _logger.LogInformation("Get Property Details By Village Code Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Property Details By Village Code Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Property Details By Village Code Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get Property Details By Village Code Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        //[Authorize]
        [HttpPost]
        [Route("getULPINDetails")]
        public async Task<string> getULPINDetails([FromBody] string val)
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
                 }*/
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("getULPINDetails - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string mut_type = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get ULPIN Details By ULPIN No Request Data - " + mut_type);
                var response = await lgdapiServices.getULPINDetails(mut_type, _logger);
                _logger.LogInformation("Get ULPIN Details By ULPIN No Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get ULPIN Details By ULPIN No Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get ULPIN Details By ULPIN No Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get ULPIN Details By ULPIN No Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        [HttpPost]
        [Route("getFloorTypeList")]
        public async Task<string> getFloorTypeList()
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
                 }*/
                /* var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.getFloorTypeList(_logger);
                _logger.LogInformation("Get floor type List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Floor Type list Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Floor Type List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get All Floor Type List Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }


       
        [HttpPost]
        [Route("getUnitTypeList")]
        public async Task<string> getUnitTypeList()
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
                 }*/
                /* var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);*/
                var response = await lgdapiServices.getUnitTypeList(_logger);
                _logger.LogInformation("Get All Unit Type List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Unit Type List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Unit Type List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get All Boja Institute Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }


        /* [Authorize]
         [HttpPost]
         [Route("getMutationType")]
         public async Task<string> getMutationType([FromBody] string val)
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
                 var decrypted = Security.DeCryptData(val);
                 string mut_category = JsonConvert.DeserializeObject<string>(decrypted!)!;
                 _logger.LogInformation("Get Mutation Type Request Data - " + mut_category);
                 var response = await lgdapiServices.getMutationType(mut_category);
                 _logger.LogInformation("All Mutation Type List Resonse - " + response);
                 if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                 {
                     //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                     return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Mutation Type List Data Found", response.Split("|")[0]))));
                 }

                 else
                 {
                     type = ReponseType.NotFound;
                     return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Mutation Type List List Data Not Found", response.Split("|")[0]))));
                 }
             }
             catch (Exception ex)
             {
                 return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
             }

         }
 */



        //Gauri W
        //pincode
        //[Authorize]
        [HttpPost]
        [Route("GetPincode")]
        public string GetPincode(//string pincode)
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
                var decrypted = Security.DeCryptData(val);
                string pincode = JsonConvert.DeserializeObject<string>(decrypted!)!;

                var result = lgdapiServices.GetAndSavePincodeData(pincode);

                if (result != null && result.Count > 0)
                {
                    _logger.LogInformation("Get Pincode Data found" + result);
                    type = ReponseType.Success;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Pincode data found!", result))));
                }
                else
                {
                    _logger.LogError("pincode Data not found");
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Pincode data not found.", result))));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.ToString()}");
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(e.Message.ToString()))));
            }

        }

        //Region API from EPCIS
        //[Authorize]
        [HttpPost]
        [Route("GetRegion")]
        public async Task<string> GetRegion()
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
                 } */
                var response = await lgdapiServices.GetRegion(_logger);
                _logger.LogInformation("Region data List Resonse - " + response);

                string[] parts = response.Split("$");

                if (parts.Length > 1 && int.TryParse(parts[1], out int statusCode))
                {
                    if (statusCode >= 200 && statusCode <= 299)
                    {
                        _logger.LogInformation($"Region Data Found: {parts[0]}");
                        //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by Office code List Data Found", parts[0])));
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Region List Data Found", parts[0]))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        _logger.LogInformation($"Region Not Data Found: {parts[0]}");
                        //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by office code List Data Not Found", parts[0])));
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Region List Data Not Found", parts[0]))));
                    }
                }

                else
                {
                    type = ReponseType.NotFound;
                    _logger.LogInformation($"Region Not Data Found: {parts[0]}");
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Region List Data Not Found", parts[0])));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Region List Data Not Found", parts[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:->" + ex.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }
        //
        //[Authorize]
        [HttpPost]
        [Route("GetDistrictByRegion")]
        public async Task<string> GetDistrictByRegion([FromBody] string val)
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
                    } */
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetDistrictByRegion - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string regionCode = JsonConvert.DeserializeObject<string>(decrypted!)!;
                var response = await lgdapiServices.GetDistrictByRegion(Convert.ToInt32(regionCode), _logger);
                _logger.LogInformation("Districts by Region data List Resonse - " + response);

                string[] parts = response.Split("$");

                if (parts.Length > 1 && int.TryParse(parts[1], out int statusCode))
                {
                    if (statusCode >= 200 && statusCode <= 299)
                    {
                        _logger.LogInformation($"Districts by Region Data Found: {parts[0]}");
                        //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by Office code List Data Found", parts[0])));
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Districts by Region List Data Found", parts[0]))));
                    }
                    else
                    {
                        type = ReponseType.NotFound;
                        _logger.LogInformation($"Districts by Region Not Data Found: {parts[0]}");
                        //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Village by office code List Data Not Found", parts[0])));
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Districts by Region List Data Not Found", parts[0]))));
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    _logger.LogInformation($"Districts by Region Not Data Found: {parts[0]}");
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Region List Data Not Found", parts[0])));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Districts by Region List Data Not Found", parts[0]))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:->" + ex.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }


        //fetch applications id
        //[Authorize]
        [HttpPost]
        [Route("GetCountOfApplicationId")]
        public async Task<string> GetCountOfApplicationId([FromBody] string val)
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
                    GUserID = grievanceService.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }

                _logger.LogInformation("GetAllApplicationIds count Request Data - ");
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetCountOfApplicationId - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GetAllApplicationIdForReport getAllApplicationIdForReport = JsonConvert.DeserializeObject<GetAllApplicationIdForReport>(decrypted)!;
                var res = await lgdapiServices.FetchCountOfApplicationsAsync(getAllApplicationIdForReport);

                if (res != null)
                {
                    _logger.LogInformation("Get count of application id data " + JsonConvert.SerializeObject(res));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "ApplicationIds count Found", res))));
                }
                else
                {
                    _logger.LogInformation("ApplicationIds count Response Failed - " + res);
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "ApplicationIds Count Not Found", res))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ApplicationID count data Exception - " + ex.StackTrace!.ToString());

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
        [Route("GetCountOfMutations")]
        public async Task<string> GetCountOfMutations([FromBody] string val)
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
                    GUserID = grievanceService.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }

                _logger.LogInformation("GetAllApplicationIds count Request Data - ");
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetCountOfMutations - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GetAllApplicationIdForReport getAllApplicationIdForReport = JsonConvert.DeserializeObject<GetAllApplicationIdForReport>(decrypted)!;
                var res = await lgdapiServices.FetchMutationCountAsync(getAllApplicationIdForReport);

                if (res != null)
                {
                    _logger.LogInformation("Get count of Mutations data " + JsonConvert.SerializeObject(res));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Mutation count Found", res))));
                }
                else
                {
                    _logger.LogInformation("Mutation count Response Failed - " + res);
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "mutation Count Not Found", res))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Mutation count data Exception - " + ex.StackTrace!.ToString());

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
        [Route("exportExcel")]
        public ActionResult<string> exportExcel([FromBody] string val)
        //GetAllApplicationIdForReport getAllApplicationIdForReport)
        {
            try
            {
                var CallAPIForFlag = Request.Headers["CallAPIFor"];
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;

                int GuserId = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GuserId = grievanceService.FetchUserIDThroughToken(Token!, CallAPIForFlag!);

                }
                //UserId = 268;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("exportExcel - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GetAllApplicationIdForReport getAllApplicationIdForReport = JsonConvert.DeserializeObject<GetAllApplicationIdForReport>(decrypted!)!;

                _logger.LogInformation("Export excel request Data - " + getAllApplicationIdForReport);// decrypted);
                DateTime createdDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                var excelFile = lgdapiServices.ExportToExcel(getAllApplicationIdForReport);

                if (excelFile != null)
                {
                    //return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-Applicationdata.xlsx");
                    string file = Security.EnCryptData(JsonConvert.SerializeObject(Ok(File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-Applicationdata.xlsx"))));
                    return file;
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Excel export error");
                    var errorResponse = ResponseHandler.GetAppResponse(type, "Error exporting Excel", "");
                    var encryptedError = Security.EnCryptData(JsonConvert.SerializeObject(Ok(errorResponse)));
                    return encryptedError;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("excel export failed  - " + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    ReponseType type = ReponseType.Failure;
                    var errorResponse = ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString());
                    var encryptedError = Security.EnCryptData(JsonConvert.SerializeObject(Ok(errorResponse)));
                    return encryptedError;
                }
                else
                {
                    var errorResponse = ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString());
                    var encryptedError = Security.EnCryptData(JsonConvert.SerializeObject(Ok(errorResponse)));
                    return encryptedError;
                }
            }
        }



        [Authorize]
        [HttpPost]
        [Route("exportStatusWiseExcel")]
        public ActionResult<string> exportStatusWiseExcel([FromBody] string val)
        //GetAllApplicationIdForReport getAllApplicationIdForReport)
        {
            try
            {
                var CallAPIForFlag = Request.Headers["CallAPIFor"];
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;

                int GuserId = 0;
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GuserId = grievanceService.FetchUserIDThroughToken(Token!, CallAPIForFlag!);
                }
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("exportStatusWiseExcel - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GetAllApplicationIdForReport getAllApplicationIdForReport = JsonConvert.DeserializeObject<GetAllApplicationIdForReport>(decrypted!)!;

                _logger.LogInformation("Export Status Wise excel request Data - " + getAllApplicationIdForReport);// decrypted);
                DateTime createdDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                var excelFile = lgdapiServices.ExportToExcelPerticularApplicationStatusWise(getAllApplicationIdForReport);

                if (excelFile != null)
                {
                    if (getAllApplicationIdForReport.statusId == 15)
                    {
                        //return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-InwardNoError.xlsx");
                        string file = Security.EnCryptData(JsonConvert.SerializeObject(Ok(File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-InwardNoError.xlsx"))));
                        return file;
                    }
                    else if (getAllApplicationIdForReport.statusId == 10)
                    {
                        //return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-ApplicationSubmittedtoEPCIS.xlsx");
                        string file = Security.EnCryptData(JsonConvert.SerializeObject(Ok(File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-ApplicationSubmittedtoEPCIS.xlsx"))));
                        return file;

                    }
                    else if (getAllApplicationIdForReport.statusId == 11)
                    {
                        //return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-TrutiPatraGenerated.xlsx");
                        string file = Security.EnCryptData(JsonConvert.SerializeObject(Ok(File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-TrutiPatraGenerated.xlsx"))));
                        return file;

                    }
                    else if (getAllApplicationIdForReport.statusId == 12)
                    {
                        //return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-ApplicationRejected.xlsx");
                        string file = Security.EnCryptData(JsonConvert.SerializeObject(Ok(File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-ApplicationRejected.xlsx"))));
                        return file;
                    }
                    else if (getAllApplicationIdForReport.statusId == 13)
                    {
                        //return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-NikaliPatra.xlsx");
                        string file = Security.EnCryptData(JsonConvert.SerializeObject(Ok(File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-NikaliPatra.xlsx"))));
                        return file;
                    }
                    else if (getAllApplicationIdForReport.statusId == 14)
                    {
                        //return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-NoticeNine.xlsx");
                        string file = Security.EnCryptData(JsonConvert.SerializeObject(Ok(File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", createdDateTime.ToString("dd-MM-yyyy") + "-NoticeNine.xlsx"))));
                        return file;
                    }
                    else
                        return File(excelFile, "", null);

                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Export Status Wise excel error");
                    var errorResponse = ResponseHandler.GetAppResponse(type, "Error exporting Excel", "");
                    var encryptedError = Security.EnCryptData(JsonConvert.SerializeObject(Ok(errorResponse)));
                    return encryptedError;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Export Status Wise excel failed  - " + ex.StackTrace!.ToString());
                if (ex.Message.ToString() == "User Not Found")
                {
                    ReponseType type = ReponseType.Failure;
                    var errorResponse = ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString());
                    var encryptedError = Security.EnCryptData(JsonConvert.SerializeObject(Ok(errorResponse)));
                    return encryptedError;
                }
                else
                {
                    var errorResponse = ResponseHandler.GetUnauthorisedResponse(ex.Message.ToString());
                    var encryptedError = Security.EnCryptData(JsonConvert.SerializeObject(Ok(errorResponse)));
                    return encryptedError;
                }
            }
        }


        [HttpPost]
        [Route("GetCountOfApplicationIdForVerticalChart")]
        public async Task<string> GetCountOfApplicationIdForVerticalChart([FromBody] string val)
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
                var authorization = Request.Headers[HeaderNames.Authorization];
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    // we have a valid AuthenticationHeaderValue that has the following details:
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GUserID = grievanceService.FetchUserIDThroughToken(Token!, CallAPIForFlag);
                    // scheme will be "Bearer"
                    // parmameter will be the token itself.
                }

                _logger.LogInformation("GetAllApplicationIds count Request Data - ");
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("GetCountOfApplicationId - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                GetAllApplicationIdForReport getAllApplicationIdForReport = JsonConvert.DeserializeObject<GetAllApplicationIdForReport>(decrypted)!;
                var res = await lgdapiServices.FetchCountOfApplicationIdForVerticalChartAsync(getAllApplicationIdForReport);

                if (res != null)
                {
                    _logger.LogInformation("Get count of application id data " + JsonConvert.SerializeObject(res));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "ApplicationIds count Found", res))));
                }
                else
                {
                    _logger.LogInformation("ApplicationIds count Response Failed - " + res);
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "ApplicationIds Count Not Found", res))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ApplicationID count data Exception - " + ex.StackTrace!.ToString());

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
        [Route("validateMultipleMutationApplications")]
        public async Task<string> validateMultipleMutationApplications([FromBody] string val)
        //RequestvalidateMultipleMutationApplications requestData)
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
                    _logger.LogInformation("validateMultipleMutationApplications - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                RequestvalidateMultipleMutationApplications requestData = JsonConvert.DeserializeObject<RequestvalidateMultipleMutationApplications>(decrypted!)!;
                _logger.LogInformation("Get CTS Details Request Data - " + requestData);
                var response = await lgdapiServices.validateMultipleMutationApplications(requestData, _logger);
                _logger.LogInformation("Get CTS Details Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "सदर न. भू. क्र. वरील धारकाची या पूर्वी दाखल केलेली फेरफार प्रक्रिया चालू आहे, चालू फेरफारची प्रक्रिया पूर्ण झाल्यानंतरच नवीन अर्ज दाखल करावा.", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    List<string> responsedata = new List<string>();
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Validate multiple mutation application Data Not Found", responsedata))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        // 19 Jan 2026
        [HttpPost]
        [Route("reasonForOwnerNameChange")]
        public async Task<string> reasonForOwnerNameChange()
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
                var response = await lgdapiServices.reasonForOwnerNameChange(_logger);
                _logger.LogInformation("All Reason List For Owner Name Change Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Reason List For Owner Name Change Data Found", response.Split("|")[0])));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Reason List For Owner Name Change Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All Reason List For Owner Name Change Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        [HttpPost]
        [Route("entryDetailsOfRegisteredMutation")]
        public async Task<string> entryDetailsOfRegisteredMutation([FromBody] string val)
        //EPCISentryDetailsOfRegisteredMutationRequestData ePCISentryDetails)
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
                 }*/
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("getDocListMutationtype - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EPCISentryDetailsOfRegisteredMutationRequestData ePCISentryDetails = JsonConvert.DeserializeObject<EPCISentryDetailsOfRegisteredMutationRequestData>(decrypted!)!;
                _logger.LogInformation("Get Entry Details Of Registered Mutation Request Data - " + ePCISentryDetails.district_code + " " + ePCISentryDetails.office_code + " " + ePCISentryDetails.village_code + " " + ePCISentryDetails.cts_no);
                var response = await lgdapiServices.entryDetailsOfRegisteredMutation(ePCISentryDetails, _logger);
                _logger.LogInformation("All Document List for Mutation Type List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Entry Details Of Registered Mutation Data Found", response.Split("|")[0])));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Entry Details Of Registered Mutation Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Entry Details Of Registered Mutation Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        // Below code added on 23 Jan 26
        [HttpPost]
        [Route("orderGivenByAuthorityNames")]
        public async Task<string> orderGivenByAuthorityNames()
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
                 } */
                var response = await lgdapiServices.orderGivenByAuthorityNames(_logger);
                _logger.LogInformation("All Order Given By Authority Names Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Order Given By Authority Names Data Found", response.Split("|")[0])));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Order Given By Authority Names Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Order Given By Authority Names Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        [HttpPost]
        [Route("getTenureList")]
        public async Task<string> getTenureList([FromBody] string val)
        //string district_code)
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
                    _logger.LogInformation("getTenureList - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                string district_code = JsonConvert.DeserializeObject<string>(decrypted!)!;
                _logger.LogInformation("Get Tenure List Request Data - " + district_code);
                var response = await lgdapiServices.getTenureList(district_code, _logger);
                _logger.LogInformation("All Tenure List Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Tenure List Data Found", response.Split("|")[0])));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Tenure List Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Tenure List Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }

        [HttpPost]
        [Route("getTenure")]
        public async Task<string> getTenure([FromBody] string val)
        //EPCISgetTenureRequestData ePCISgetTenureRequestData)
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
                 }*/
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("getDocListMutationtype - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                EPCISgetTenureRequestData ePCISgetTenureRequestData = JsonConvert.DeserializeObject<EPCISgetTenureRequestData>(decrypted!)!;
                _logger.LogInformation("Get getTenure Request Data - " + ePCISgetTenureRequestData.village_code + " " + ePCISgetTenureRequestData.cts_no);
                var response = await lgdapiServices.getTenure(ePCISgetTenureRequestData, _logger);
                _logger.LogInformation("getTenure Resonse - " + response);
                if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
                {
                    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Entry Details Of Registered Mutation Data Found", response.Split("|")[0])));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Tenure Data Found", response.Split("|")[0]))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get Tenure Data Not Found", response.Split("|")[0]))));
                }
            }
            catch (Exception ex)
            {
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }

        }
    }
}
