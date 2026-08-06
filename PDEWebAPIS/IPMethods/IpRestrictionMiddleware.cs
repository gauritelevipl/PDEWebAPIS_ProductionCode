using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using PDEWebAPIS.Helpers;
using System.Net;

namespace PDEWebAPIS.IPMethods
{
    public class IpRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<string> _allowedIps;
        private readonly List<string> _allowedHostNames;

        public IpRestrictionMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            // Load allowed IPs from configuration or define them here
            _allowedIps = configuration.GetValue<string>("IPList")!.Split(";").ToList();
            _allowedHostNames = configuration.GetValue<string>("HostName")!.Split(";").ToList();
        }

        public async Task Invoke(HttpContext context)
        {
            var remoteIp = GetLocalIPv4();
            //var host = context.Request.Headers["Host"].ToString();
            bool hostFlag = CheckClientHostname(context.Request);
            bool exist = false;
            //var exist = _allowedIps.Exists(ip => ip!.Equals(IPAddress.Parse(remoteIp)));
            // If IP is not in the allowed list, deny access
            //if (!_allowedIps.Any(ip => IPAddress.Parse(ip).Equals(remoteIp)))
            if (hostFlag)
            {
                foreach (string ip in _allowedIps)
                {
                    if (remoteIp == ip)
                    {
                        exist = true;
                        break;
                    }
                }
                if (remoteIp == null || !exist)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden: Your IP Is Not Allowed.");
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: Your Host Name Is Incorrect.");
                return;
            }
            await _next(context);
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

        public bool CheckClientHostname(HttpRequest Request)
        {
            try
            {
                bool exist = false;
                var HostName = Request.Host.ToString();
                if (!string.IsNullOrEmpty(HostName))
                //&& HostName == "115.124.105.111:8844")
                {
                    foreach (string host in _allowedHostNames)
                    {
                        if (HostName == host)
                        {
                            exist = true;
                            break;
                        }
                    }
                    return exist;
                }
                else
                {
                    return exist;
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
