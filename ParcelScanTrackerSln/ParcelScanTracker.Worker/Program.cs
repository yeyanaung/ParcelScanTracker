using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.EntityFrameworkCore;
using ParcelScanTracker.Worker;
using System.Reflection;

// Entry point for the Worker Service application.
var builder = Host.CreateApplicationBuilder(args);

// Example: run this in a console app or test to generate encrypted password
/* 
string plainPassword = "DBP@ssword";
string encrypted = CryptoHelper.Encrypt(plainPassword);
Console.WriteLine(encrypted); // Copy this value to appsettings.json "Encrypted"
return;
*/

// Configure log4net for logging.
ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
builder.Services.AddSingleton<LogHelper>();

// Set up logging providers (only log4net).
builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");

// Register the Worker background service.
builder.Services.AddHostedService<Worker>();

// Configure database connection using decrypted password.
var encryptedPassword = builder.Configuration["Encrypted"];
var decryptedPassword = CryptoHelper.Decrypt(encryptedPassword);
var connStrTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
var connStr = connStrTemplate.Replace("{EncryptedPassword}", decryptedPassword);
Console.WriteLine("DB = " + connStrTemplate);

// Register EF Core DbContext for scan event tracking.
builder.Services.AddDbContext<ScanEventDbContext>(options =>
    options.UseSqlServer(connStr));

// Bind ScanEventApiOptions from configuration.
builder.Services.Configure<ScanEventApiOptions>(
    builder.Configuration.GetSection("ScanEventApi"));

// Register HttpClient for API calls.
builder.Services.AddHttpClient();

// Build and run the host.
var host = builder.Build();
host.Run();
