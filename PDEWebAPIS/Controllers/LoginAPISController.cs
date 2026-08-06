using Microsoft.AspNetCore.Mvc;
using PDEWebAPIS.Data;
using PDEWebAPIS.InputDataModel;
using PDEWebAPIS.Model;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.Services;
using PDEWebAPIS.ViewModel;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using PDEWebAPIS.EncryptDecrypt;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PDEWebAPIS.CommonMethods;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using PDEWebAPIS.Repository;
using PDEWebAPIS.TokenMethods;
using System.Net;

// Old Code 
// Production Code
namespace PDEWebAPIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginAPISController : ControllerBase
    {
        private readonly DBHelper dbHelper;
        private readonly UserServices userServices;
        private readonly GrievanceService grievanceServices;
        private readonly IConfiguration configuration;
        private readonly ILogger<LoginAPISController> _logger;
        private readonly ILogger<UserServices> _loggeruserService;
        private readonly TokenBlacklistForGrievanceService tokenBlacklistServiceForGrievance;


        RequestHelper requestHelper = new RequestHelper();
        public LoginAPISController(AppDBContext context, IConfiguration configuration, ILogger<LoginAPISController> logger, AppDBContextR contextR, ILogger<UserServices> loggeruserService, IServiceScopeFactory scopeFactory)
        {
            dbHelper = new DBHelper(context);
            _loggeruserService = loggeruserService;
            userServices = new UserServices(context, contextR, loggeruserService);
            tokenBlacklistServiceForGrievance = new TokenBlacklistForGrievanceService(scopeFactory);
            grievanceServices = new GrievanceService(context, contextR);
            this.configuration = configuration;
            _logger = logger;
        }

        static string GetLocalIPv4()
        {
            string localIP = string.Empty;
            foreach (var address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // IPv4
                {
                    localIP = address.ToString();
                    break;
                }
            }
            return localIP;
        }

        [HttpPost]
        [Route("RequestForOTP")]
        public string RequestForOTP(
          [FromBody] string val)
        //VerificationData VerificationData)
        {
            try
            {
                _logger.LogInformation("Request For OTP Request Data - " + val);
                var CallAPIForFlag = Request.Headers["CallAPIFor"];
                var HostName = Request.Host.ToString();
                //bool hosflag = requestHelper.CheckClientHostname(Request);
                var remoteIP = HttpContext.Connection.RemoteIpAddress;
                string localIPv4 = GetLocalIPv4();
                ReponseType type = ReponseType.Success;

                //// Retrieve the hostname of the client making the request
                //string? clientHostName = HttpContext.Connection.RemoteIpAddress?.ToString();

                //// Alternatively, you can use the Host header if it's available
                //string? hostHeader = Request.Headers["Host"].ToString();
                //if (hosflag)
                //{
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                int StatusCode;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("RequestForOTP - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                VerificationData VerificationData = JsonConvert.DeserializeObject<VerificationData>(decrypted!)!;

                if (VerificationData.VERIFICATIONTYPE.Trim().ToUpper() != "MOBILENO" && VerificationData.VERIFICATIONTYPE.Trim().ToUpper() != "EMAILID")
                {
                    throw new HandleException("Verification Type Should Be MOBILENO Or EMAILID");
                }
                ApiResponse ApiResponse = new ApiResponse();
                StatusCode = userServices.FetchUserID(VerificationData.DESCRIPTION.Trim(), VerificationData.VERIFICATIONTYPE);
                if (StatusCode != 0)
                {
                    string OTP = userServices.GenerateOTP();
                    //long OTP = Convert.ToInt64("123456");
                    Mailid_And_Mobileno_VerificationModel Mailid_And_Mobileno_VerificationModel = new Mailid_And_Mobileno_VerificationModel();
                    Mailid_And_Mobileno_VerificationModel.description = VerificationData.DESCRIPTION;
                    Mailid_And_Mobileno_VerificationModel.otp = Convert.ToInt64(OTP);
                    Mailid_And_Mobileno_VerificationModel.verificationtype = VerificationData.VERIFICATIONTYPE;

                    string Response =  userServices.SaveVerificationData(Mailid_And_Mobileno_VerificationModel);
                    if (Response == "Success")
                    {
                        //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Sent Successfully ->" + remoteIP!.ToString() + " IPV4 -> " + localIPv4, "")));
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Sent Successfully", ""))));
                        //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Sent Successfully ->" + remoteIP!.ToString() + " IPV4 -> " + localIPv4, ""))));
                        //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Sent Successfully -> " + Request.Host.ToString() + " IPV4 -> " + localIPv4,  ""))));
                    }
                    else
                    {
                        type = ReponseType.Failure;
                        _logger.LogInformation("Request For OTP Response Failed - " + Response);
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                    }
                }
                else
                {
                    type = ReponseType.Failure;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "User Is Not Registered", ""))));
                }
                //}
                //else
                //{
                //    type = ReponseType.Failure;
                //    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Host Name Is Incorrect " + Request.Host.ToString(), "")));
                //    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Host Name Is Incorrect " + Request.Host.ToString(), ""))));
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError("Request For OTP Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }
        //[HttpPost]
        //[Route("RequestForOTP")]
        //public IActionResult RequestForOTP(VerificationData VerificationData)
        //{
        //    try
        //    {
        //        var CallAPIForFlag = Request.Headers["CallAPIFor"];
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }
        //        ReponseType type = ReponseType.Success;
        //        int StatusCode;
        //        AesOperation AesOperation = new AesOperation();
        //        var encryptedString = AesOperation.EncryptObjectToBytes<VerificationData>(VerificationData, AesOperation.EncDeckey);

        //        if (VerificationData.VERIFICATIONTYPE.Trim().ToUpper() != "MOBILENO" && VerificationData.VERIFICATIONTYPE.Trim().ToUpper() != "EMAILID")
        //        {
        //            throw new HandleException("Verification Type Should Be MOBILENO Or EMAILID");
        //        }
        //        ApiResponse ApiResponse = new ApiResponse();
        //        StatusCode = userServices.FetchUserID(VerificationData.DESCRIPTION.Trim(), VerificationData.VERIFICATIONTYPE);
        //        if (StatusCode != 0)
        //        {
        //            string NUM = userServices.GenerateOTP();
        //            long OTP = Convert.ToInt64("123456");
        //            Mailid_And_Mobileno_VerificationModel Mailid_And_Mobileno_VerificationModel = new Mailid_And_Mobileno_VerificationModel();
        //            Mailid_And_Mobileno_VerificationModel.description = VerificationData.DESCRIPTION;
        //            Mailid_And_Mobileno_VerificationModel.otp = OTP;
        //            Mailid_And_Mobileno_VerificationModel.verificationtype = VerificationData.VERIFICATIONTYPE;
        //            string Response = userServices.SaveVerificationData(Mailid_And_Mobileno_VerificationModel);
        //            if (Response == "Success")
        //            {
        //                return Ok(ResponseHandler.GetAppResponse(type, "OTP Sent Successfully", ""));
        //            }
        //            else
        //            {
        //                type = ReponseType.Failure;
        //                return Ok(ResponseHandler.GetAppResponse(type, Response, ""));
        //            }
        //        }
        //        else
        //        {
        //            type = ReponseType.Failure;
        //            return Ok(ResponseHandler.GetAppResponse(type, "User Is Not Registered", ""));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()));
        //    }
        //}

        [HttpPost]
        [Route("VerifyOTP")]
        public string VerifyOTP(
          [FromBody] string val)
        //VerifyOTPData VerifyOTPData)
        {
            JwtSecurityToken token = new JwtSecurityToken();
            string Status = "0";
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                ReponseType type = ReponseType.Success;
                bool check = true;
                check = Security.IsBase64String(val);
                if (!check)
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("VerifyOTP - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                VerifyOTPData VerifyOTPData = JsonConvert.DeserializeObject<VerifyOTPData>(decrypted!)!;
                //bool hosflag = requestHelper.CheckClientHostname(Request);

                //if (hosflag)
                //{
                Status = userServices.CheckOTP(VerifyOTPData);
                //AesOperation AesOperation = new AesOperation();
                //var encryptedString = AesOperation.EncryptObjectToBytes<VerifyOTPData>(VerifyOTPData, AesOperation.EncDeckey);
                if (Status == "0")
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetFailure("Wrong OTP"))));
                }
                else if (Status == "1")
                {
                    int UserID = userServices.FetchUserID(VerifyOTPData.DESCRIPTION!.Trim(), VerifyOTPData.VERIFICATIONTYPE!);
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,configuration["Jwt:Subject"]!),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim("UserID",Security.EnCryptData( UserID.ToString())),
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
                    Status = userServices.UpdateToken(tokenValue, UserID, CallAPIForFlag);
                    if (Status == "Success")
                    {
                        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                        FetchUserData userData = new FetchUserData();
                        userData = userServices.FetchUserData(UserID);
                        string userNameMarathi = string.Empty, userNameEnglish = string.Empty;
                        if (userData != null)
                        {
                            if (userData.usertype_code == 1)
                            {
                                userNameMarathi = userData.fname_in_marathi + " " + userData.lname_in_marathi;
                                userNameEnglish = userData.fname_in_eng + " " + userData.lname_in_eng;
                            }
                            else
                            {
                                userNameMarathi = userData.company_name_in_marathi!;
                                userNameEnglish = userData.company_name_in_eng!;
                            }
                        }
                        keyValuePairs.Add("userNameMarathi", userNameMarathi!);
                        keyValuePairs.Add("userNameEnglish", userNameEnglish);
                        //keyValuePairs.Add("UserID",Security.EnCryptData( UserID.ToString()));
                        keyValuePairs.Add("AccessToken", tokenValue);
                        type = ReponseType.Success;
                        //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Verified Successfully", keyValuePairs)));
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "OTP Verified Successfully", keyValuePairs))));
                    }
                    else
                    {
                        type = ReponseType.Failure;
                        _logger.LogInformation("Verify OTP Response Failed - " + Status);
                        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Status, ""))));
                    }
                }
                else
                {
                    //return StatusCode(500, Status);
                    return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(Status))));
                }
                //}
                //else
                //{
                //    type = ReponseType.Failure;
                //    //return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Host Name Is Incorrect " + Request.Host.ToString(), "")));
                //    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Host Name Is Incorrect " + Request.Host.ToString(), ""))));
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError("Verify OTP Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }
        //[HttpPost]
        //[Route("VerifyOTP")]
        //public IActionResult VerifyOTP(VerifyOTPData VerifyOTPData)
        //{
        //    JwtSecurityToken token = new JwtSecurityToken();
        //    string Status = "0";
        //    try
        //    {
        //        string CallAPIForFlag = Request.Headers["CallAPIFor"];
        //        if (string.IsNullOrEmpty(CallAPIForFlag))
        //        {
        //            throw new HandleException("Send CallAPIFor Flag In Header");
        //        }

        //        ReponseType type = ReponseType.Success;
        //        Status = userServices.CheckOTP(VerifyOTPData);
        //        //AesOperation AesOperation = new AesOperation();
        //        //var encryptedString = AesOperation.EncryptObjectToBytes<VerifyOTPData>(VerifyOTPData, AesOperation.EncDeckey);
        //        if (Status == "0")
        //        {
        //            return Ok(ResponseHandler.GetFailure("Wrong OTP"));
        //        }
        //        else if (Status == "1")
        //        {
        //            int UserID = userServices.FetchUserID(VerifyOTPData.DESCRIPTION.Trim(), VerifyOTPData.VERIFICATIONTYPE);
        //            var claims = new[]
        //            {
        //                new Claim(JwtRegisteredClaimNames.Sub,configuration["Jwt:Subject"]),
        //                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        //                new Claim("UserID",UserID.ToString()),
        //                new Claim("CurrentDateTime",DateTime.Now.ToString()),
        //            };
        //            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        //            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //            if (CallAPIForFlag.ToUpper() == "WEB")
        //            {
        //                token = new JwtSecurityToken(
        //               configuration["Jwt:Issuer"],
        //               configuration["Jwt:Audience"],
        //               claims,
        //               expires: DateTime.UtcNow.AddMinutes(120),
        //               signingCredentials: signIn
        //               );
        //            }
        //            else
        //            {
        //                token = new JwtSecurityToken(
        //               configuration["Jwt:Issuer"],
        //               configuration["Jwt:Audience"],
        //               claims,
        //               expires: DateTime.UtcNow.AddMonths(6),
        //               signingCredentials: signIn
        //               );
        //            }
        //            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        //            //Save Token In User Master table
        //            Status = userServices.UpdateToken(tokenValue, UserID, CallAPIForFlag);

        //            if (Status == "Success")
        //            {

        //                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        //                FetchUserData userData = new FetchUserData();
        //                userData = userServices.FetchUserData(UserID);
        //                string userNameMarathi = string.Empty, userNameEnglish = string.Empty;
        //                if (userData != null)
        //                {
        //                    if (userData.usertype_code == 1)
        //                    {
        //                        userNameMarathi = userData.fname_in_marathi + " " + userData.lname_in_marathi;
        //                        userNameEnglish = userData.fname_in_eng + " " + userData.lname_in_eng;
        //                    }
        //                    else
        //                    {
        //                        userNameMarathi = userData.company_name_in_marathi;
        //                        userNameEnglish = userData.company_name_in_eng;
        //                    }
        //                }
        //                keyValuePairs.Add("userNameMarathi", userNameMarathi);
        //                keyValuePairs.Add("userNameEnglish", userNameEnglish);
        //                //keyValuePairs.Add("UserID", UserID.ToString());
        //                keyValuePairs.Add("AccessToken", tokenValue);
        //                type = ReponseType.Success;
        //                return Ok(ResponseHandler.GetAppResponse(type, "OTP Verified Successfully", keyValuePairs));
        //            }
        //            else
        //            {
        //                type = ReponseType.Failure;
        //                return Ok(ResponseHandler.GetAppResponse(type, Status, ""));
        //            }
        //        }
        //        else
        //        {
        //            //return StatusCode(500, Status);
        //            return BadRequest(ResponseHandler.GetExceptionResponse(Status));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()));
        //    }
        //}

        [HttpPost]
        [Route("RegisterUser")]
        public string RegisterUser([FromBody] string val)
        {
            try
            {
                var CallAPIForFlag = Request.Headers["CallAPIFor"];
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
                    _logger.LogInformation("RegisterUser - Inout String Is Not Encrypted");
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Inout String Is Not Encrypted", ""))));
                }
                var decrypted = Security.DeCryptData(val);
                CreateUserData createUserData = JsonConvert.DeserializeObject<CreateUserData>(decrypted!)!;
                _logger.LogInformation("Register User Request Data - " + createUserData);
                string Response = userServices.SaveUserData(createUserData);
                //"Success";
                //userServices.SaveUserData(CreateUserData);
                if (Response == "Success")
                {
                    //return Ok(ResponseHandler.GetAppResponse(type, "User Created Successfully", result));
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "User Created Successfully", ""))));
                }
                else
                {
                    type = ReponseType.Failure;
                    _logger.LogInformation("Register User Response Failed - " + Response);
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, Response, ""))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Register User Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }


        [Authorize]
        [HttpPost]
        [Route("AddLabel")]
        public IActionResult AddLabel(AddLabelData labelData)
        {
            try
            {
                ReponseType type = ReponseType.Success;
                string StatusCode = string.Empty;
                //var key = "b14ca5898a4e4133bbce2ea2315a1916";
                var key = "pdeappfornic12345678910111213145";
                AesOperation AesOperation = new AesOperation();
                var encryptedString = AesOperation.EncryptObjectToBytes<AddLabelData>(labelData, key);
                var decryptedString = AesOperation.DecryptString(key, encryptedString);
                var result = JsonConvert.DeserializeObject<AddLabelData>(decryptedString);

                string Response = userServices.SaveLabelData(labelData);
                //"Success";
                if (Response == "Success")
                {
                    //return Ok(ResponseHandler.GetAppResponse(type, "Label Added Successfully", result));
                    return Ok(ResponseHandler.GetAppResponse(type, "Label Added Successfully", ""));
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

        [Authorize]
        [HttpPost]
        [Route("FetchLabelData")]
        public IActionResult FetchLabelData(int screenid, int mutationtypeid)
        {
            try
            {
                string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
                if (string.IsNullOrEmpty(CallAPIForFlag))
                {
                    throw new HandleException("Send CallAPIFor Flag In Header");
                }
                int UserID;
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
                var Data = DateTime.Now.AddMinutes(1);
                ReponseType type = ReponseType.Success;
                string StatusCode = string.Empty;
                List<FetchLabelData> fetchLabels = new List<FetchLabelData>();
                FetchLabelDataInArray labelDataInArray = new FetchLabelDataInArray();
                labelDataInArray = userServices.FetchLabels(screenid, mutationtypeid);
                if (labelDataInArray != null)
                {
                    return Ok(ResponseHandler.GetAppResponse(type, "Data Found", labelDataInArray));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", labelDataInArray));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()));
            }
        }


        [Authorize]
        [HttpPost]
        [Route("GetUserData")]
        public string GetUserData()
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

                var result = userServices.FetchUserData(UserID);
                if (result != null)
                {
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Found", result))));
                }
                else
                {
                    type = ReponseType.NotFound;
                    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Data Not Found", result))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Get User Data Exception - " + ex.StackTrace!.ToString());
                return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
            }
        }

        //[AllowAnonymous]
        //[HttpPost("Logout")]
        //public string Logout([FromServices] TokenBlacklistService blacklistService)
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
        //            blacklistService.BlacklistToken(Token!);
        //            string Status = userServices.DeleteToken(UserID, CallAPIForFlag);
        //            // scheme will be "Bearer"
        //            // parmameter will be the token itself.
        //        }
        //        return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Logged out successfully", null))));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Logout Exception - " + ex.StackTrace!.ToString());
        //        return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
        //    }
        //}



        //gauri W


        [Authorize]
        [HttpPost("logout")]
        public string Logout()
        {
            string CallAPIForFlag = Request.Headers["CallAPIFor"]!;
            if (string.IsNullOrEmpty(CallAPIForFlag))
            {
                throw new HandleException("Send CallAPIFor Flag In Header");
            }
            int GUserID = 0;
            int UserID = 0;
            ReponseType type = ReponseType.Success;
            var authorization = Request.Headers[HeaderNames.Authorization];
            try
            {
                if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    var scheme = headerValue.Scheme;
                    var Token = headerValue.Parameter;
                    GUserID = grievanceServices.FetchUserIDThroughTokenForLogout(Token!, CallAPIForFlag);
                    UserID = userServices.FetchUserIDThroughTokenForLogout(Token!, CallAPIForFlag);

                    if (GUserID != 0 && UserID == 0)
                    {
                        var expirationClaim = User.FindFirst("exp")?.Value;
                        DateTime expirationTime = DateTime.UtcNow;

                        if (expirationClaim != null)
                        {
                            DateTimeOffset utcExpirationOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim));

                            expirationTime = utcExpirationOffset.UtcDateTime;

                            DateTime databaseExpirationTime = DateTime.SpecifyKind(expirationTime, DateTimeKind.Utc);

                            TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                            DateTime istExpirationTime = TimeZoneInfo.ConvertTimeFromUtc(databaseExpirationTime, indianTimeZone);

                        }
                        tokenBlacklistServiceForGrievance.AddToBlacklist(Token!, expirationTime);
                    }
                    else if (UserID != 0 && GUserID == 0)
                    {
                        var expirationClaim = User.FindFirst("exp")?.Value;
                        DateTime expirationTime = DateTime.UtcNow;

                        if (expirationClaim != null)
                        {
                            DateTimeOffset utcExpirationOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim));

                            expirationTime = utcExpirationOffset.UtcDateTime;

                            DateTime databaseExpirationTime = DateTime.SpecifyKind(expirationTime, DateTimeKind.Utc);

                            TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                            DateTime istExpirationTime = TimeZoneInfo.ConvertTimeFromUtc(databaseExpirationTime, indianTimeZone);

                        }
                        tokenBlacklistServiceForGrievance.AddToBlacklist(Token!, expirationTime);
                        //blacklistService.BlacklistToken(Token!);
                        //string Status = userServices.DeleteToken(UserID, CallAPIForFlag);
                    }
                }
                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Logout successful!", ""))));
            }
            catch (Exception ex)
            {
                type = ReponseType.Failure;
                return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, ex.Message.ToString(), ""))));
            }
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint()
        {
            return Ok(new { message = "You have access to this endpoint." });
        }
    }
}
