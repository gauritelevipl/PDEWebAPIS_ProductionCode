using Newtonsoft.Json;
using PDEWebAPIS.Data;
using PDEWebAPIS.Model;
using System.Net.Http.Headers;
using System.Text;

namespace PDEWebAPIS.Services
{
    public class LGDAPIServices
    {
        private AppDBContext context;
        private static readonly HttpClient client = new HttpClient();
        Decryptor decryptor = new Decryptor();
        public LGDAPIServices(AppDBContext context)
        {
            this.context = context;
        }

        public async Task<string> SendRequestAsync(string urlmethod, HttpMethod method, ILogger _logger, object? body = null)
        {

            string url = "https://api.mahabhumi.gov.in/api/lgd/" + urlmethod;
            string bearerToken = "";
            string apiKey = "";
            string secretKey = "";
            // Set up headers
            //Production
            /*   bearerToken = "0uXCpuv3E1ZDnJRDO7xJiQhHuaM8PjYC71r9BBcnRHWvfMG1JQinb8E4rzCR5Mpa";
               apiKey = "e06ae416-d3db-4b7e-8cbd-edc73f6c706a";
               secretKey = "ObvLNRgvtbIYe2IC9t3IpzEmclzKEjkqHdjM6iP7ggN76zSoWP9M1pekkdYDEb7c";*/

            //Local
            bearerToken = "0uXCpuv3E1ZDnJRDO7xJiQhHuaM8PjYC71r9BBcnRHWvfMG1JQinb8E4rzCR5Mpa";//"URqyR0fRlC3B9dxaAlTR1Ra31QKZ9HHnVaTPMihixMlbvKnhCJPAtQ3qYPnCKbIB";
            apiKey = "f3c040ae-4264-f1d1-ac58-486e2453";
            secretKey = "9z3g7YaHCzwj4diHacM2Cdt8Cg1FOYVLjh2nOtRjGBz67Ygh3UiYzwcOe5By";


            // Create a new HttpRequestMessage
            var request = new HttpRequestMessage(method, url);

            // Add body if provided
            if (body != null)
            {
                string jsonBody = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            // Add headers
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            request.Headers.Add("API-KEY", apiKey);
            request.Headers.Add("SECRET-KEY", secretKey);

            // Send the request
            HttpResponseMessage response = await client.SendAsync(request);
            _logger.LogInformation("Send Request Async ePICS  - " + response);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response: " + responseBody);
                _logger.Log(logLevel: LogLevel.Warning, "Send Request Async ePICS - " + responseBody);
                var jsonobject = JsonConvert.DeserializeObject<LgdApiResponse>(responseBody);
                var decrypted = decryptor.DecryptData(jsonobject!.Data!.ToString()!);
                // var districts = JsonConvert.DeserializeObject<List<LGDDistrict>>(decrypted);
                return decrypted + "|" + (int)response.StatusCode;
                //return jsonobject!.Data + "~" + (int)response.StatusCode;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent + "|" + (int)response.StatusCode;
            }
            // Read and return the response
            /* string responseContent = await response.Content.ReadAsStringAsync();
             return (responseContent, (int)response.StatusCode);*/
        }
        public async Task<string> getDistrictofState(ILogger _logger)
        {
            string response = await SendRequestAsync("getDistrictofState", HttpMethod.Post, _logger);
            if (response.Split("|")[1] == "200")
            {
                var districts = JsonConvert.DeserializeObject<List<LGDDistrict>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
            }
            else return response;
            //return response;

        }

        public async Task<string> getTalukasOfDistrict(string district_code, ILogger _logger)
        {
            var body = new Dictionary<string, string>();
            body.Add("distcode", district_code);
            string response = await SendRequestAsync("getTalukasOfDistrict", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var districts = JsonConvert.DeserializeObject<List<LGDTaluka>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
            }
            else return response;
        }
        
        public async Task<string> getVillagesOfDistrictAndTaluka(RequestVillage body, ILogger _logger)
        {
            /*var body = new Dictionary<string, string>();
            body.Add("mut_type", mut_type);*/
            string response = await SendRequestAsync("getVillagesOfDistrictAndTaluka", HttpMethod.Post, _logger, body);
            if (response.Split("|")[1] == "200")
            {
                var districts = JsonConvert.DeserializeObject<List<LGDVillage>>(response.Split("|")[0]);
                return JsonConvert.SerializeObject(districts) + "|" + response.Split("|")[1];
            }
            else return response;
            //return response;

        }

    }
}
