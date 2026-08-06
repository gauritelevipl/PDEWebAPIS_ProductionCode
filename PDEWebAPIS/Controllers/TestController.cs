using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using PDEWebAPIS.Data;
using PDEWebAPIS.Helpers;
using PDEWebAPIS.Services;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace PDEWebAPIS.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : Controller
    {
        // Test DB Connection 
        private AppDBContext _context;
        private AppDBContextR _contextR;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TestController> _logger;
        private readonly SMSService sMSService;
        public TestController(AppDBContext context, AppDBContextR contextR, IConfiguration configuration, ILogger<TestController> logger)
        {
            _context = context;
            _contextR = contextR;
            _configuration = configuration;
            _logger = logger;
            sMSService = new SMSService(context);
        }

        //[HttpGet("TestDBConnectionWP")]
        //public IActionResult TestDBConnectionWP()
        //{
        //    var connectionString = _configuration.GetConnectionString("PDEDBWP");
        //    try
        //    {
        //        using (var connection = new NpgsqlConnection(connectionString))
        //        {
        //            connection.Open(); // Open the connection
        //            return Ok("Database Connected to Server");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Database connection failed: {ex.Message}");
        //    }
        //}

        [HttpGet("TestDBConnectionW")]
        public IActionResult TestDBConnectionW()
        {
            var connectionString = _configuration.GetConnectionString("PDEDBW");
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open(); // Open the connection
                    return Ok("Database Connected to Server (Write Port)");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database connection failed: {ex.Message}");
            }
        }

        [HttpGet("TestDBConnectionR")]
        public IActionResult TestDBConnectionR()
        {
            var connectionString = _configuration.GetConnectionString("PDEDBR");
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open(); // Open the connection
                    return Ok("Database Connected to Server (Read Port)");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database connection failed: {ex.Message}");
            }
        }

        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var connectionString = _configuration.GetConnectionString("PDEDBR");
            var users = new List<dynamic>();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM public.usermaster", connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new
                            {
                                Id = reader["userid"],
                                FnameInMarathi = reader["fname_in_marathi"],
                                MnameInMarathi = reader["mname_in_marathi"],
                                LnameInMarathi = reader["lname_in_marathi"],
                                FnameInEng = reader["fname_in_eng"],
                                MnameInEng = reader["mname_in_eng"],
                                LnameInEng = reader["lname_in_eng"]
                            });
                        }
                    }
                    connection.Close();
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //[HttpGet("TestDBConnection")]
        //public async Task<IActionResult> CheckDatabaseConnection()
        //{
        //    try
        //    {
        //        // Check if the database can be reached
        //        await _context.Database.CanConnectAsync();
        //        return Ok("Database connection successful!");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Database connection failed: {ex.Message}");
        //    }
        //}

        //[HttpGet("TestDBConnectionW")]
        //public async Task<IActionResult> CheckDatabaseConnectionW()
        //{
        //    try
        //    {
        //        // Check if the database can be reached
        //        await _contextW.Database.CanConnectAsync();
        //        return Ok("Database connection successful!");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Database connection failed: {ex.Message}");
        //    }
        //}

        //[HttpGet("TestDBConnectionR")]
        //public async Task<IActionResult> CheckDatabaseConnectionR()
        //{
        //    try
        //    {
        //        // Check if the database can be reached
        //        await _contextR.Database.CanConnectAsync();
        //        return Ok("Database connection successful!");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Database connection failed: {ex.Message}");
        //    }
        //}

        //[HttpGet]
        //[Route("SMSAPIGet")]
        //public async Task<string> SMSAPIGet()
        //{
        //    try
        //    {
        //        ReponseType type = ReponseType.Success;
        //        var response = await sMSService.callSMSAPIGet("8793721412","666333",_logger);
        //        _logger.LogInformation("SMS API Resonse - " + response);
        //        return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "SMS Send Successfully", response)));
        //        //if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
        //        //{

        //        //    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
        //        //    //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "SMS Send Successfully", response))));
        //        //}

        //        //else
        //        //{
        //        //    type = ReponseType.NotFound;
        //        //    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All District List Data Not Found", response.Split("|")[0]))));
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
        //    }

        //}

        //[HttpGet]
        //[Route("SMSAPIPost")]
        //public async Task<string> SMSAPIPost()
        //{
        //    try
        //    {
        //        ReponseType type = ReponseType.Success;
        //        var response = await sMSService.callSMSAPIPost("8793721412", "666333", _logger);
        //        _logger.LogInformation("SMS API Resonse - " + response);
        //        return JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "SMS Send Successfully", response)));
        //        //if (Convert.ToInt32(response.Split("|")[1]) >= 200 && Convert.ToInt32(response.Split("|")[1]) <= 299)
        //        //{

        //        //    //_logger.LogInformation("IGR DIG List Resonse - " + response.Split("-")[0]);
        //        //    //return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "SMS Send Successfully", response))));
        //        //}

        //        //else
        //        //{
        //        //    type = ReponseType.NotFound;
        //        //    return Security.EnCryptData(JsonConvert.SerializeObject(Ok(ResponseHandler.GetAppResponse(type, "Get All District List Data Not Found", response.Split("|")[0]))));
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        return Security.EnCryptData(JsonConvert.SerializeObject(BadRequest(ResponseHandler.GetExceptionResponse(ex.Message.ToString()))));
        //    }

        //}
    }
}
