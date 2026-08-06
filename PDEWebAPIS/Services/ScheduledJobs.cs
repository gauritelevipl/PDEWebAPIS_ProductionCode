using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Nancy;
using Newtonsoft.Json;
using PDEWebAPIS;
using PDEWebAPIS.CommonMethods;
using PDEWebAPIS.ContractRepo;
using PDEWebAPIS.Controllers;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace PDEWebAPIS.Services
{
    public class ScheduledJobs
    {
        private readonly ILogger<ScheduledJobs> _logger;
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ApplicationServices applicationServices;
        private readonly NICService nICService;
        private readonly ILogger<TestFileUpload> _loggerTest;
        private readonly ILogger<UserServices> _loggeruserService;
        private readonly SMSService sMSService;

        public ScheduledJobs(ILogger<ScheduledJobs> logger, IServiceScopeFactory scopeFactory, AppDBContext context, AppDBContextR contextR, ILogger<UserServices> loggeruserService, ICommonRepository commonRepository, IMapper mapper)
        {
            _logger = logger;
            //_loggerTest = loggerTest;
            _loggeruserService = loggeruserService;
            _httpClient = new HttpClient();
            _scopeFactory = scopeFactory;
            applicationServices = new ApplicationServices(context,contextR,  _loggeruserService);
            nICService = new NICService(context, contextR,  _loggeruserService, commonRepository, mapper);
            sMSService = new SMSService(context);
        }

        //public async Task CallApi()
        //{
        //    _logger.LogInformation("API Call Job started at {Time}", DateTime.Now);

        //    try
        //    {
        //        using var client = new HttpClient();
        //        var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7240/api/MutationAPIS/DecryptData"); // Change method if needed

        //        request.Headers.Add("CallAPIFor", "web");
        //        request.Headers.Add("Accept", "text/plain");

        //        string requestBody = "\"+G8V1x5PkJBxAClTS/Ti2rMdNrKHDSWWufVdyjRIp9M=\"";
        //        //string requestBody = "+G8V1x5PkJBxAClTS/Ti2rMdNrKHDSWWufVdyjRIp9M=";
        //        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        //        var response = _httpClient.SendAsync(request).Result;

        //        string responseBody = response.Content.ReadAsStringAsync().Result; // Read response as string

        //        if (response.IsSuccessStatusCode)
        //        {
        //            _logger.LogInformation("API call successful at {Time}, Response: {Response}", DateTime.Now, responseBody);
        //        }
        //        else
        //        {
        //            _logger.LogWarning("API call failed with status code {StatusCode} at {Time}, Response: {Response}", response.StatusCode, DateTime.Now, responseBody);
        //            Console.WriteLine("Full API Response: " + responseBody);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while calling API at {Time}", DateTime.Now);
        //    }
        //}


        public List<string> FetchApplicationIDS()
        {
            try
            {
                _logger.LogInformation("FetchApplicationId method called");
                var startDate = new DateTime(2025, 04, 04, 0, 0, 0, DateTimeKind.Utc);
                //var endDate = new DateTime(2025, 01, 31, 23, 59, 59, DateTimeKind.Utc);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();

                    var statusNineRecords = dbContext.applicationDTL.Where(u => u.isDeleted == false && (u.status == 9) && u.inwardno == "NA" && u.createddatetime >= startDate);
                    //OrderByDescending(e => e.createddatetime).Select(u => string.Join(",", u.applicationid)).ToList();

                    var statusFifteenRecord = dbContext.applicationDTL.Where(u => u.isDeleted == false && u.status == 15 && u.inwardno == "NA" && u.createddatetime >= startDate
                    && dbContext.nICAPIResponses.Any(s => s.applicationid == u.applicationid && EF.Functions.ILike(s.inwardno!, "%timeout%")));


                    var result = statusNineRecords
                   .Union(statusFifteenRecord)
                   .OrderByDescending(u => u.createddatetime).Select(u => string.Join(",", u.applicationid)).ToList();


                    //Console.WriteLine("all applications: " + result);

                    return result;
                }

                //var result = _context.applicationDTL.Where(u=>u.isDeleted == false && u.status == 9).OrderByDescending(e => e.createddatetime).Select(u => string.Join(",", u.applicationid)).ToList();
                //return result;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }


        public void ResubmitApplication()
        {
            _logger.LogInformation("Checking for resubmission applications at {Time}", DateTime.Now);

            List<string> applicationIds = FetchApplicationIDS();

            if (applicationIds.Count == 0)
            {
                _logger.LogInformation("No applications found for resubmission.");
                return;
            }

            foreach (var applicationId in applicationIds)
            {
                _logger.LogInformation("Calling CallResubmitApi({applicationId}) at {Time}", applicationId, DateTime.Now);
                ReSubmitApplicationForJob(applicationId);
            }
        }
        public string ReSubmitApplicationForJob(string ApplicationID)
        {
            try
            {
                int UserID = 0;
                ApplicationDTL applicationDTL = new ApplicationDTL();
                applicationDTL = applicationServices.FetchApplicationData(ApplicationID);

                _logger.LogInformation("ReSubmitApplicationForJob Request Data - " + ApplicationID);
                ReponseType type = ReponseType.Success;
                string updateInwardNoRes = string.Empty;
                NICApiRes nICApiRes = new NICApiRes();
                UserID = applicationDTL!.userMaster!.userid;
                ApplicationDataForNIC fetchapplication = nICService.GetApplicationDataForNIC(UserID, ApplicationID);
                string response = string.Empty, saveNICResponse = string.Empty;
                NicApiResponseModel nicApiResponseModel = new NicApiResponseModel();
                if (fetchapplication != null)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        _logger.LogInformation("Call NIC Inward API Request - " + ApplicationID);
                        nICApiRes = nICService.CallNICInwardNoAPIForJob(HttpMethod.Post, i, _logger, fetchapplication);
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
                    }
                    if (updateInwardNoRes == "Success")
                    {
                        return JsonConvert.SerializeObject(ResponseHandler.GetAppResponse(type, "Inward No Updated Successfully For Application ID " + ApplicationID, ""));
                    }
                    else
                    {
                        type = ReponseType.Failure;
                        _logger.LogInformation("ReSubmitApplicationForJob Response Failed For Application ID - " + ApplicationID + " Message: " + updateInwardNoRes);
                        if (nICApiRes.statusCode == "500")
                        {
                            _logger.LogInformation(type + "-" + nICApiRes.statusMsg! + "-" + nICApiRes);
                            return JsonConvert.SerializeObject(ResponseHandler.GetAppResponse(type, nICApiRes.statusMsg!, nICApiRes));
                        }
                        else
                        {
                            _logger.LogInformation(type + "-" + nICApiRes.statusMsg! + "-" + nICApiRes);
                            return JsonConvert.SerializeObject(ResponseHandler.GetAppResponse(type, nICApiRes.inwardno!, nICApiRes));
                        }
                    }
                }
                else
                {
                    type = ReponseType.NotFound;
                    _logger.LogInformation(type + "-" + "Application Data Not Found For Application ID " + ApplicationID);
                    return JsonConvert.SerializeObject(ResponseHandler.GetAppResponse(type, "Application Data Not Found For Application ID " + ApplicationID, ""));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ReSubmitApplicationForJob Exception For ApplicationID - " + ApplicationID + " " + ex.StackTrace!.ToString());
                return JsonConvert.SerializeObject(ResponseHandler.GetExceptionResponse(ex.Message.ToString()));
            }
        }


        //private async Task CallResubmitApi(string applicationId)
        //{
        //    try
        //    {
        //        var request = new HttpRequestMessage(HttpMethod.Post,
        //        //"https://115.124.105.111:8844/PDEWebAPIS/api/ApplicationAPIS/ReSubmitApplicationForJob");
        //        "http://localhost:8844/PDEWebAPIS/api/ApplicationAPIS/ReSubmitApplicationForJob");
        //        //"https://115.124.105.111:8844/PDEWebAPIS/api/ApplicationAPIS/ReSubmitApplicationForJob");

        //        // Add Required Headers
        //        request.Headers.Add("Accept", "text/plain");

        //        // Set Request Body

        //        string requestBody = $"\"{applicationId}\""; // Send applicationId for resubmission
        //        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        //        // Send API Request
        //        var response = await _httpClient.SendAsync(request);
        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        if (response.IsSuccessStatusCode)
        //        {
        //            _logger.LogInformation("Successfully resubmitted application {ApplicationId}", applicationId+" Response -> "+ responseBody);
        //        }
        //        else
        //        {

        //            _logger.LogWarning("Failed to resubmit application {ApplicationId}, Status: {StatusCode}, Response: {Response}",
        //                applicationId, response.StatusCode, responseBody);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error resubmitting application {ApplicationId}", applicationId);
        //    }
        //}
    }
}
