using PDEWebAPIS.Data;
using System.Text;
using System.Xml;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace PDEWebAPIS.Services
{
    public class IGRAPIService
    {
        private AppDBContext context;
        private HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        public IGRAPIService(AppDBContext context)
        {
            this.context = context;
        }

        public async Task<string> DIGListAsync()
        {
            
            var soapRequestXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                      <soap:Body>
                                        <DIGList xmlns=""http://tempuri.org/"">
                                          <username>NIC</username>
                                          <password>e20926d7a470db054829b984100439ff</password>
                                        </DIGList>
                                      </soap:Body>
                                    </soap:Envelope>";
            // Create the HTTP content for the request
            var content = new StringContent(soapRequestXml, Encoding.UTF8, "text/xml");

            // Set the Content-Length header (this is optional as it's usually set automatically by HttpClient)
            //content.Headers.Add("Content-Length", content.Headers.ContentLength.ToString());
            //content.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            content.Headers.Add("SOAPAction", "http://tempuri.org/DIGList");

            try
            {
                // Send the SOAP request to the web service endpoint
                HttpResponseMessage response = await client.PostAsync("https://appl2igr.maharashtra.gov.in/igrservicepr/igrservice.asmx", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine("SOAP Response: " + responseContent);
                    string result = ExtractDIGListResult(responseContent);
                    return result + "-"+ (int)response.StatusCode;
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("Error: " + response.StatusCode);
                    return responseContent+"-"+(int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Request failed: " + ex.Message);
                return ex.Message +"-"+"500";
            }
        }

        public async Task<string> JDRListAsync(string digcode)
        {

            var soapRequestXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                      <soap:Body>
                                        <JDRList xmlns=""http://tempuri.org/"">
                                          <digcode>int</digcode>
                                          <username>NIC</username>
                                          <password>e20926d7a470db054829b984100439ff</password>
                                        </JDRList>
                                      </soap:Body>
                                    </soap:Envelope>";
            soapRequestXml = ReplaceDigcodeValue(soapRequestXml, digcode);
            // Create the HTTP content for the request
            var content = new StringContent(soapRequestXml, Encoding.UTF8, "text/xml");

            // Set the Content-Length header (this is optional as it's usually set automatically by HttpClient)
            //content.Headers.Add("Content-Length", content.Headers.ContentLength.ToString());
            //content.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            content.Headers.Add("SOAPAction", "http://tempuri.org/JDRList");
            try
            {
                // Send the SOAP request to the web service endpoint
                HttpResponseMessage response = await client.PostAsync("https://appl2igr.maharashtra.gov.in/igrservicepr/igrservice.asmx", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine("SOAP Response: " + responseContent);
                    string result = ExtractJDRListResult(responseContent);
                    return result +"-"+(int)response.StatusCode;
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error: " + response.StatusCode);
                    return responseContent+"-"+(int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Request failed: " + ex.Message);
                return ex.Message+"-"+"500";
            }
        }

        public async Task<string> SROListAsync(Sro sro)
        {
            var soapRequestXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                      <soap:Body>
                                        <SROList xmlns=""http://tempuri.org/"">
                                          <digcode>int</digcode>
                                          <jdrcode>int</jdrcode>
                                          <username>NIC</username>
                                          <password>e20926d7a470db054829b984100439ff</password>
                                        </SROList>
                                      </soap:Body>
                                    </soap:Envelope>";
            soapRequestXml = ReplacecodeValue(soapRequestXml, sro.digcode.ToString(), sro.jDRCode.ToString());
            // Create the HTTP content for the request
            var content = new StringContent(soapRequestXml, Encoding.UTF8, "text/xml");

            // Set the Content-Length header (this is optional as it's usually set automatically by HttpClient)
            //content.Headers.Add("Content-Length", content.Headers.ContentLength.ToString());
            //content.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            content.Headers.Add("SOAPAction", "http://tempuri.org/SROList");


            try
            {
                // Send the SOAP request to the web service endpoint
                HttpResponseMessage response = await client.PostAsync("https://appl2igr.maharashtra.gov.in/igrservicepr/igrservice.asmx", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine("SOAP Response: " + responseContent);
                    string result = ExtractSROListResult(responseContent);
                    return result + "_$" + (int)response.StatusCode;
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error: " + response.StatusCode);
                    return responseContent+ "_$" + (int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Request failed: " + ex.Message);
                return ex.Message+ "_$" + "500";
            }
            //return "";
        }

        public async Task<string> DocumentStatus(documentStatus doc)
        {
            var soapRequestXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                      <soap:Body>
                                        <documentstatus xmlns=""http://tempuri.org/"">
                                          <docnumber>int</docnumber>
                                          <srocode>int</srocode>
                                          <regyear>int</regyear>
                                          <username>NIC</username>
                                          <password>e20926d7a470db054829b984100439ff</password>
                                        </documentstatus>
                                      </soap:Body>
                                    </soap:Envelope>";
            soapRequestXml = ReplaceValue(soapRequestXml, doc.srocode.ToString(), doc.docnumber.ToString(), doc.regyear.ToString());
            // Create the HTTP content for the request
            var content = new StringContent(soapRequestXml, Encoding.UTF8, "text/xml");

            // Set the Content-Length header (this is optional as it's usually set automatically by HttpClient)
            //content.Headers.Add("Content-Length", content.Headers.ContentLength.ToString());
            //content.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            content.Headers.Add("SOAPAction", "http://tempuri.org/documentstatus");

            try
            {
                // Send the SOAP request to the web service endpoint
                HttpResponseMessage response = await client.PostAsync("https://appl2igr.maharashtra.gov.in/igrservicepr/igrservice.asmx", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine("SOAP Response: " + responseContent);
                    string result = ExtractDocumentStatusResult(responseContent);
                    return (int)response.StatusCode + "~"+result;
                }
                else
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Console.WriteLine("Error: " + response.StatusCode);
                    return responseContent +"-"+(int)response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Request failed: " + ex.Message);
                return ex.Message + "-" + "500";
            }
        }
        public string ExtractDIGListResult(string soapResponse)
        {
            // Load the SOAP response into an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResponse);

            // Define the namespace manager to handle the SOAP namespace
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");

            // Select the DIGListResult node
            XmlNode digListResultNode = xmlDoc.SelectSingleNode("//tempuri:DIGListResult", nsmgr)!;

            return digListResultNode?.InnerText ?? "DIGListResult not found";
        }
        public string ReplaceDigcodeValue(string xml, string newDigcode)
        {
            // Load the XML string into an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Create an XmlNamespaceManager to handle the namespace
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");

            // Find the digcode node and update its inner text
            XmlNode digcodeNode = xmlDoc.SelectSingleNode("//tempuri:digcode", nsmgr)!;
            if (digcodeNode != null)
            {
                digcodeNode.InnerText = newDigcode;
            }
            else
            {
                Console.WriteLine("digcode node not found");
            }

            return xmlDoc.OuterXml;
        }

        public string ExtractJDRListResult(string soapResponse)
        {
            // Load the SOAP response into an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResponse);

            // Define the namespace manager to handle the SOAP namespace
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");

            // Select the DIGListResult node
            XmlNode digListResultNode = xmlDoc.SelectSingleNode("//tempuri:JDRListResult", nsmgr)!;

            return digListResultNode?.InnerText ?? "JDRListResult not found";
        }

        public string ReplacecodeValue(string xml, string newDigcode, string newjrdcode)
        {
            // Load the XML string into an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Create an XmlNamespaceManager to handle the namespace
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");

            // Find the digcode node and update its inner text
            XmlNode digcodeNode = xmlDoc.SelectSingleNode("//tempuri:digcode", nsmgr)!;
            if (digcodeNode != null)
            {
                digcodeNode.InnerText = newDigcode;
            }
            else
            {
                Console.WriteLine("digcode node not found");
            }
            // Find the jdrcode node and update its inner text
            XmlNode jdrcodeNode = xmlDoc.SelectSingleNode("//tempuri:jdrcode", nsmgr)!;
            if (jdrcodeNode != null)
            {
                jdrcodeNode.InnerText = newjrdcode;
            }
            else
            {
                Console.WriteLine("jdrcode node not found");
            }

            return xmlDoc.OuterXml;
        }

        public string ExtractSROListResult(string soapResponse)
        {
            // Load the SOAP response into an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResponse);

            // Define the namespace manager to handle the SOAP namespace
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");

            // Select the DIGListResult node
            XmlNode digListResultNode = xmlDoc.SelectSingleNode("//tempuri:SROListResult", nsmgr)!;

            return digListResultNode?.InnerText ?? "SROListResult not found";
        }
        public string ReplaceValue(string xml, string srocode, string documentnumber, string year)
        {
            // Load the XML string into an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Create an XmlNamespaceManager to handle the namespace
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");

            // Find the digcode node and update its inner text
            XmlNode digcodeNode = xmlDoc.SelectSingleNode("//tempuri:srocode", nsmgr)!;
            if (digcodeNode != null)
            {
                digcodeNode.InnerText = srocode;
            }
            else
            {
                Console.WriteLine("SRO node not found");
            }
            // Find the document Number node and update its inner text
            XmlNode jdrcodeNode = xmlDoc.SelectSingleNode("//tempuri:docnumber", nsmgr)!;
            if (jdrcodeNode != null)
            {
                jdrcodeNode.InnerText = documentnumber;
            }
            else
            {
                Console.WriteLine("jdrcode node not found");
            }

            // Find the registration year node and update its inner text
            XmlNode yearNode = xmlDoc.SelectSingleNode("//tempuri:regyear", nsmgr)!;
            if (jdrcodeNode != null)
            {
                yearNode.InnerText = year;
            }
            else
            {
                Console.WriteLine("registration year node not found");
            }

            return xmlDoc.OuterXml;
        }
        public string ExtractDocumentStatusResult(string soapResponse)
        {
            // Load the SOAP response into an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResponse);

            // Define the namespace manager to handle the SOAP namespace
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("tempuri", "http://tempuri.org/");

            // Select the DIGListResult node
            XmlNode digListResultNode = xmlDoc.SelectSingleNode("//tempuri:documentstatusResult", nsmgr)!;

            return digListResultNode?.InnerText ?? "documentstatusResult not found";
        }

    }

    public class Sro
    {
        public int digcode { get; set; } = 1;
        public int jDRCode { get; set; } = 1;
    }
    public class documentStatus
    {
        public int srocode { get; set; }
        public int docnumber { get; set;}
        public int regyear { get; set; }
    }
}
