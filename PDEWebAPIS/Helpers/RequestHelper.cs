using Nancy;
using System.Net;

namespace PDEWebAPIS.Helpers
{
    public class RequestHelper
    {
        public bool CheckClientHostname(HttpRequest Request)
        {
            try
            {
                var HostName = Request.Host.ToString();
                if (!string.IsNullOrEmpty(HostName) && HostName == "115.124.105.111:8844")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            // Retrieve the IP address of the client
            //var ipAddress = httpContext.Connection.RemoteIpAddress;

            //if (ipAddress == null)
            //{
            //    return "Unknown";
            //}

            //try
            //{
            //    // Perform DNS lookup to resolve hostname
            //    var hostEntry = Dns.GetHostEntry(ipAddress);
            //    return hostEntry.HostName;
            //}
            //catch
            //{
            //    // Return the IP address if hostname resolution fails
            //    return ipAddress.ToString();
            //}
        }
    }
}
