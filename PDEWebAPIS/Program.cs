using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PDEWebAPIS.Auth;
using PDEWebAPIS.Data;
using PDEWebAPIS.IPMethods;
using PDEWebAPIS.RequestHeader;
using PDEWebAPIS.ResponseHeader;
using PDEWebAPIS.Services;
using PDEWebAPIS.TokenMethods;
using Serilog;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using YourNamespace.Data;

var builder = WebApplication.CreateBuilder(args);

// Below Code is added for HSTS
//builder.Services.AddHsts(options =>
//{
//    options.Preload = true;
//    options.IncludeSubDomains = true;
//    options.MaxAge = TimeSpan.FromDays(365);
//    options.ExcludedHosts.Add("example.com");
//    options.ExcludedHosts.Add("www.example.com");
//});
//builder.Services.AddHttpsRedirection(options =>
//{
//    options.RedirectStatusCode = Status;
//    options.HttpsPort = 5001;
//});
//End

// Below code for cors error

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()   // Allows all origins
                   .AllowAnyMethod()   // Allows all HTTP methods (GET, POST, etc.)
                   .AllowAnyHeader();  // Allows any headers
        });

    options.AddPolicy("AllowSpecificOrigin",
      policy =>
      {
          policy.WithOrigins("https://epsit.mahabhumi.gov.in") // <-- allowed origin
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
});

//End code

// Configure Rate Limiting
//builder.Services.AddRateLimiter(options =>
//{
//    options.AddPolicy("fixed", httpContext => RateLimitPartition.GetFixedWindowLimiter(
//        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
//        factory: _ => new FixedWindowRateLimiterOptions
//        {
//            PermitLimit = 20,             // Max 3 requests
//            Window = TimeSpan.FromSeconds(1), // Per 10 seconds
//            //QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//            //QueueLimit = 0
//        }));
//    // options.RejectionStatusCode = StatusCodes.Status429TooManyRequests; // Return 429 status code
//    // Correct way to handle rejected requests
//    // Handle rejected requests explicitly
//    options.OnRejected = async (context, cancellationToken) =>
//    {
//        var response = context.HttpContext.Response;
//        response.StatusCode = StatusCodes.Status429TooManyRequests;
//        response.ContentType = "application/json";
//        // Explicitly write the response to avoid IIS interference
//       // await response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
//        await response.WriteAsync("{\"Message\": \"Too many requests. Please try again later.\"}", cancellationToken);
//        // Mark as handled to prevent other middleware from triggering 500
//        // context.HttpContext.Abort();
//    };
//});
// End 

//Add EPCIS Cofig

builder.Services.Configure<EPCISConfig>(builder.Configuration.GetSection("EPCISConfig"));

// Add services to the container.
builder.Services.AddDbContext<AppDBContext>(
    o => o.UseNpgsql(builder.Configuration.GetConnectionString("PDEDBW"))
    );


//HangFire 
builder.Services.AddDbContext<HangfireDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PDEDBR")));
builder.Services.AddDbContext<HangfireDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PDEDBW")));

var ConnectionStringR = builder.Configuration.GetConnectionString("PDEDBR");
var ConnectionStringW = builder.Configuration.GetConnectionString("PDEDBW");

builder.Services.AddHangfire(config => config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("PDEDBR")));
builder.Services.AddHangfire(config => config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("PDEDBW")));

builder.Services.AddHangfireServer();


//builder.Services.AddDbContext<AppDBContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("PDEDBR"),
//        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
//            maxRetryCount: 5,
//            maxRetryDelay: TimeSpan.FromSeconds(10),
//            errorCodesToAdd: null) // Use default PostgreSQL transient error codes
//    ));
builder.Services.AddDbContext<AppDBContextR>(
    o => o.UseNpgsql(builder.Configuration.GetConnectionString("PDEDBR"))
    //.EnableSensitiveDataLogging()
    );

//Below Code Added on 11 March 2026

//instead of manually adding scope we add automatically as below
//builder.Services.AddScoped<IMrutyuOrIcchaPatraNondUnregRepository, MrutyuOrIcchaPatraNondUnregRepository>();
var assembly = Assembly.GetExecutingAssembly();
var types = assembly.GetTypes();

var repoTypes = types.Where(t =>
    t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"));

foreach (var impl in repoTypes)
{
    var iface = impl.GetInterface("I" + impl.Name);
    if (iface != null)
    {
        builder.Services.AddScoped(iface, impl);
    }
}

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//End 

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// For Serilog 

var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
// End Serilog

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(3600); // Set session timeout
    options.Cookie.HttpOnly = true; // Cookie is not accessible via JavaScript
    options.Cookie.IsEssential = true; // Needed for session to work when using cookie consent
});

// Start Below Code is added for Bearer Token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    //options.RequireHttpsMetadata = false;
    //options.SaveToken = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Add bearer token to all API

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.ApiKey
    });

    //builder.Services.AddTransient<EPCISAPIService>();
    ////For Header
    options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
    // End Header

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

//For Token Logout
builder.Services.AddSingleton<TokenBlacklistService>();

//Hangfire
builder.Services.AddScoped<ScheduledJobs>();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    //options.SwaggerDoc("v1", new OpenApiInfo { Title = "JadeWebAPI", Version = "v1" });

//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Description = "Please Enter Token",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        BearerFormat="JWT",
//        //Type = SecuritySchemeType.Http,
//        //Scheme = JwtBearerDefaults.AuthenticationScheme
//        Scheme = "bearer"
//    });
//    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//    {
//        {
//        new OpenApiSecurityScheme
//        {
//            //Name="CallAPIFor",
//            //In=ParameterLocation.Header,
//            Reference=new OpenApiReference
//            {
//                Id="Bearer",
//                Type=ReferenceType.SecurityScheme
//            } },new List<string>()
//        }
//    });
//});

//End Bearer Token

var app = builder.Build();


// Add IP Restriction Middleware
//Gauri
app.UseMiddleware<IpRestrictionMiddleware>();

// Add SQL Injection middleware early
app.UseMiddleware<SqlInjectionMiddleware>();
//app.UseMiddleware<TimingMiddleware>(); // ⬅️ this is where the magic happens

//app.UseMiddleware<AdminSafeListMiddleware>(builder.Configuration["AdminSafeList"]);

//End


// Below code added By Gauri To Hide the IIS 10 Version
// Middleware to remove 'Server' header from the response
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Remove("Server");
        return Task.CompletedTask;
    });
    await next();
});
//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Remove("Server");
//    await next();
//});

//End  Added By Gauri To Hide the IIS 10 Version


// For Token Expire
app.UseMiddleware<JwtMiddleware>("Ymasdfhasdhkhasd1232134654446Ymasdfhasdhkhasd1232134654446");

//app.UseMiddleware<JwtMiddleware>("Ymasdfhasdhkhasd1232134654446Ymasdfhasdhkhasd1232134654446", new TokenBlacklistService());



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseDeveloperExceptionPage();
    //app.UseExceptionHandler("/Error");
}
//app.UseMiddleware<ExceptionHandlingMiddleware>();

//Below Code is added for HSTS
//if (!app.Environment.IsDevelopment())
//{
//    // Enable HSTS only in production
//    app.UseHsts();
//}
//End

// Swagger UI to show all APIs  Kajal
//app.UseSwagger();
//app.UseSwaggerUI();

// End 


// Use CORS middleware
//app.UseCors("AllowAllOrigins");
app.UseCors("AllowSpecificOrigin");


app.UseHttpsRedirection();

app.UseSession();

app.UseAuthentication();
//app.UseRateLimiter();  // Apply Rate Limiting
app.UseAuthorization();
app.MapControllers();
//app.MapControllers().RequireRateLimiting("fixed"); 
//For Response Header Commented Because we have added it on IIS Server
//app.UseXFrameOptions();

//Hangfire
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<ScheduledJobs>(
    "resubmit-applications",
    service => service.ResubmitApplication(), //=> Console.WriteLine("Hangfire job executed"),
                                              //Cron.Daily(23));
                                              //Cron.Daily);
                                              //"*/30 * * * *");//
    "0 0,8 * * *");
//"0 23 * * *"
app.Run();