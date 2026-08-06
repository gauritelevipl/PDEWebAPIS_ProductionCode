using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.Model;
using PDEWebAPIS.Repository;
using PDEWebAPIS.ViewModel.ModelForNICData;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PDEWebAPIS.Services
{
    public class SMSService
    {
        private static readonly HttpClient client = new HttpClient();
        private AppDBContext _context;
        public SMSService(AppDBContext context)
        {
            _context = context;
        }
        public async Task<string> SendRequestAsync(HttpMethod method, ILogger _logger, object? body = null)
        {
            //using (HttpClient client = new HttpClient())
            //{
            //    try
            //    {
            //        string apiUrl = "https://push3.aclgateway.com/servlet/com.aclwireless.pushconnectivity.listeners.TextListener";
            //        var postData = new
            //        {
            //            title = "foo",
            //            body = "bar",
            //            userId = 1
            //        };

            //        // Serialize the object to JSON
            //        string json = JsonSerializer.Serialize(postData);
            //        var content = new StringContent(json, Encoding.UTF8, "application/json");

            //        // Send POST request
            //        HttpResponseMessage response = await client.PostAsync(apiUrl, content);
            //        response.EnsureSuccessStatusCode();

            //        string responseBody = await response.Content.ReadAsStringAsync();
            //        Console.WriteLine(responseBody);
            //        return responseBody;
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError("Exception occurred  IN SMS API -" + ex.Message.ToString());
            //        return ex.Message;
            //    }
            //}

            string url = "https://push3.aclgateway.com/servlet/com.aclwireless.pushconnectivity.listeners.TextListener";
            string jsonObject = string.Empty;
            //int maxRetries = 3;
            //int delayMilliseconds = 1000;
            //int attempt = 0;
            //while (attempt < maxRetries)
            //{
            //    attempt++;
            try
            {
                // Create a new HttpRequestMessage
                var request = new HttpRequestMessage(method, url);
                // Add body if provided
                if (body != null)
                {
                    string jsonBody = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                }

                // Send the request
                HttpResponseMessage response = await client.SendAsync(request);
                _logger.LogInformation("Send SMS Request  -" + url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        _logger.Log(LogLevel.Warning, "Get Response Async SMS - " + responseBody);
                        jsonObject = responseBody;
                        return jsonObject.ToString()!;
                    }
                }
                else if ((int)response.StatusCode == 500)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    jsonObject = "500 Status Code For SMS - " + responseBody;
                    _logger.Log(LogLevel.Warning, "Response Status 500 For SMS - " + responseBody);
                    return jsonObject.ToString()!;
                }
                else
                {
                    _logger.LogError("SMS Error -" + response.StatusCode);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    jsonObject = responseContent;
                    return jsonObject.ToString()!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Attempt Exception occurred  IN SMS API - " + ex.Message);
                return ex.Message.ToString();
            }
            //if (attempt < maxRetries)
            //{
            //    _logger.LogWarning($"Retrying... Attempt For SMS API {attempt + 1} in {delayMilliseconds}ms");
            //    await Task.Delay(delayMilliseconds); // Wait before retrying
            //}
            //}
            //_logger.LogError("All retry attempts failed.");
            //return "Data Not Found" + "|" + "400";
            return jsonObject!;
        }

        public async Task<string> SendSMSRequest(HttpMethod method, ILogger _logger, string to, string otp)
        {
            string status = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // API URL
                    string apiUrl = "https://push3.aclgateway.com/servlet/com.aclwireless.pushconnectivity.listeners.TextListener?appid=mahaitdlr&userId=MahaITDLR&pass=mahait_8&contenttype=3&from=MHLAND&alert=1&selfid=true&intflag=false&to=" + to + "&text=Dear User, " + otp + " is your OTP for mobile verification. Please DO NOT share with anyone.  Regards, Land Record Dept.&dtm=1707170548673245915&dpi=1701162866674017074";

                    // Send GET request
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode(); // Throw if not a success code
                    _logger.LogInformation("Send SMS OTP " + apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        // Read response content
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation(responseBody);
                        status = "Success";
                    }
                    else
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("SMS OTP API Is Failed For MOBNO -" + to + " - " + responseBody);
                        status = "Failed";
                    }
                    return status;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Attempt Exception occurred  IN SMS API - " + ex.Message);
                    return ex.Message.ToString();
                }
            }
        }
        //public async Task<string> callSMSAPIGet(string to, string otp, ILogger _logger)
        //{
        //    string response = await getRequestAsync(HttpMethod.Get, _logger, to, otp);
        //    return response.ToString();
        //}
        //public async Task<string> callSMSAPIPost(string to, string otp, ILogger _logger)
        //{
        //    SMSDataModel sMSData = new SMSDataModel();
        //    sMSData.appid = "mahaitdlr";
        //    sMSData.userId = "MahaITDLR";
        //    sMSData.pass = "mahait_8";
        //    sMSData.contenttype = "3";
        //    sMSData.from = "MHLAND";
        //    sMSData.alert = "1";
        //    sMSData.selfid = "true";
        //    sMSData.intflag = "false";
        //    sMSData.to = to;
        //    sMSData.text = "Dear User, " + otp + " is your OTP for mobile verification. Please DO NOT share with anyone.  Regards, Land Record Dept.";
        //    string response = await SendRequestAsync(HttpMethod.Post, _logger, sMSData);
        //    return response.ToString();
        //}

        //async Task<string>
        public string RequestOTPForApp(string mobileno, long otp, ILogger _logger)
        {
            string response = string.Empty;
            try
            {
                OtpVerify dbTable = new OtpVerify();
                dbTable = _context.otpVerifies.Where(d => d.mobileno.Equals(mobileno)).FirstOrDefault()!;
                if (dbTable != null)
                {
                    _context.Remove(dbTable);
                    _context.SaveChanges();
                    dbTable.mobileno = mobileno.Trim();
                    dbTable.otp = otp;
                    _context.otpVerifies.Add(dbTable);
                    _context.SaveChanges();
                }
                else
                {
                    OtpVerify dbTable1 = new OtpVerify();
                    dbTable1.mobileno = mobileno.Trim();
                    dbTable1.otp = otp;
                    _context.otpVerifies.Add(dbTable1);
                    _context.SaveChanges();
                }
                response = sendOTPMSGUsingCDAC(mobileno, otp.ToString());
                // await SendSMSRequest(HttpMethod.Get, _logger, mobileno, otp.ToString());
                return response;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public string VerifyOTPForApp(string mobileno, long otp)
        {
            string response = "Failed";
            try
            {
                OtpVerify dbTable = new OtpVerify();
                dbTable = _context.otpVerifies.Where(d => d.mobileno.Equals(mobileno) && d.otp == otp).FirstOrDefault()!;
                if (dbTable != null)
                {
                    _context.Remove(dbTable);
                    _context.SaveChanges();
                    response = "Success";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new HandleException(ex.Message.ToString());
            }
        }

        public string sendOTPMSGUsingCDAC(string mobileNo, string otp)
        {
            string message = "Dear User, " + otp.ToString() + " is your OTP for mobile verification. Please DO NOT share with anyone.  Regards, Land Record Dept";
            string username = "deskofficerit-docity";
            //"deskofficerit";
            string password = "Scdlr@1234";
            string senderid = "MHLAND";
            string secureKey = "2ce15441-e379-478f-b854-7ff9a43dc852";
            //"39d45108-04ac-4c04-a681-f9e6b3ec72a2";
            string templateid = "1707170548673245915";
            Stream dataStream;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //forcing .Net framework to use TLSv1.2

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://msdgweb.mgov.gov.in/esms/sendsmsrequestDLT");
            request.ProtocolVersion = HttpVersion.Version10;
            request.KeepAlive = false;
            request.ServicePoint.ConnectionLimit = 1;

            //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
            ((HttpWebRequest)request).UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";

            request.Method = "POST";
            //System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();

            String encryptedPassword = encryptedPasswod(password);
            String key = hashGenerator(username.Trim(), senderid.Trim(), message.Trim(), secureKey.Trim());

            String smsservicetype = "otpmsg"; //For OTP message.

            String query = "username=" + HttpUtility.UrlEncode(username.Trim()) +
            "&password=" + HttpUtility.UrlEncode(encryptedPassword) +

            "&smsservicetype=" + HttpUtility.UrlEncode(smsservicetype) +

            "&content=" + HttpUtility.UrlEncode(message.Trim()) +

            "&mobileno=" + HttpUtility.UrlEncode(mobileNo) +

            "&senderid=" + HttpUtility.UrlEncode(senderid.Trim()) +
            "&key=" + HttpUtility.UrlEncode(key.Trim()) +

            "&templateid=" + HttpUtility.UrlEncode(templateid.Trim());

            byte[] byteArray = Encoding.ASCII.GetBytes(query);

            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = byteArray.Length;

            dataStream = request.GetRequestStream();

            dataStream.Write(byteArray, 0, byteArray.Length);

            dataStream.Close();

            WebResponse response = request.GetResponse();

            String Status = ((HttpWebResponse)response).StatusDescription;

            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);

            String responseFromServer = reader.ReadToEnd();

            reader.Close();

            dataStream.Close();

            response.Close();
            if (responseFromServer.Contains("402"))
            {
                return "Success";
            }
            else
            {
                return responseFromServer;
            }
            //return Status;
        }

        protected String encryptedPasswod(String password)
        {

            byte[] encPwd = Encoding.UTF8.GetBytes(password);
            //static byte[] pwd = new byte[encPwd.Length];
            HashAlgorithm sha1 = HashAlgorithm.Create("SHA1");
            byte[] pp = sha1.ComputeHash(encPwd);
            // static string result = System.Text.Encoding.UTF8.GetString(pp);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in pp)
            {

                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();

        }

        protected String hashGenerator(String Username, String sender_id, String message, String secure_key)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(Username).Append(sender_id).Append(message).Append(secure_key);
            byte[] genkey = Encoding.UTF8.GetBytes(sb.ToString());
            //static byte[] pwd = new byte[encPwd.Length];
            HashAlgorithm sha1 = HashAlgorithm.Create("SHA512");
            byte[] sec_key = sha1.ComputeHash(genkey);

            StringBuilder sb1 = new StringBuilder();
            for (int i = 0; i < sec_key.Length; i++)
            {
                sb1.Append(sec_key[i].ToString("x2"));
            }
            return sb1.ToString();
        }

        public String sendUnicodeSMSForInwardNo(string mobileNos, string invno)
        {
            string Unicodemessage = "आपला फेरफार नोंदी करीता अर्ज प्राप्त झाला आहे त्याचा आवक क्रमांक " + invno + " असून पुढील स्थिति जाणून घेणेसाठी -https://epsit.mahabhumi.gov.in/#/login या संकेतस्थळाचा वापर करावा";
            string username =
            "deskofficerit-docity";
            //"deskofficerit";
            string password = "Scdlr@1234";
            string senderid = "MHLAND";
            string secureKey =
            "2ce15441-e379-478f-b854-7ff9a43dc852";
            //"39d45108-04ac-4c04-a681-f9e6b3ec72a2";
            string templateid = "1707176044610704985";
            Stream dataStream;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //forcing .Net framework to use TLSv1.2

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://msdgweb.mgov.gov.in/esms/sendsmsrequestDLT");
            request.ProtocolVersion = HttpVersion.Version10;
            request.KeepAlive = false;
            request.ServicePoint.ConnectionLimit = 1;

            //((HttpWebRequest)request).UserAgent = ".NET Framework Example Client";
            ((HttpWebRequest)request).UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";

            request.Method = "POST";

            //System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
            String U_Convertedmessage = "";

            foreach (char c in Unicodemessage)
            {
                int j = (int)c;
                String sss = "&#" + j + ";";
                U_Convertedmessage = U_Convertedmessage + sss;
            }
            String encryptedPassword = encryptedPasswod(password);
            String NewsecureKey = hashGenerator(username.Trim(), senderid.Trim(), U_Convertedmessage.Trim(), secureKey.Trim());


            String smsservicetype = "unicodemsg"; // for unicode msg
            String query = "username=" + HttpUtility.UrlEncode(username.Trim()) +
            "&password=" + HttpUtility.UrlEncode(encryptedPassword) +
            "&smsservicetype=" + HttpUtility.UrlEncode(smsservicetype) +
            "&content=" + HttpUtility.UrlEncode(U_Convertedmessage.Trim()) +
            "&bulkmobno=" + HttpUtility.UrlEncode(mobileNos) +
            "&senderid=" + HttpUtility.UrlEncode(senderid.Trim()) +
            "&key=" + HttpUtility.UrlEncode(NewsecureKey.Trim()) +

            "&templateid=" + HttpUtility.UrlEncode(templateid.Trim());



            byte[] byteArray = Encoding.ASCII.GetBytes(query);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            String Status = ((HttpWebResponse)response).StatusDescription;
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            String responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            if (responseFromServer.Contains("402"))
            {
                return "Success";
            }
            else
            {
                return responseFromServer;
            }
            //return responseFromServer;
        }
    }
}
