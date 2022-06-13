using Microsoft.EntityFrameworkCore;
using QuizAPI.Model;
using System.Text.Json.Serialization;
using QuizAPI.Utils;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;
var appName = builder.Environment.ApplicationName; 

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddSecretsManager(region: Amazon.RegionEndpoint.EUWest1,
	configurator: options => 
	{
		options.KeyGenerator = (_, s) => s
            .Replace("__", ":"); 
	});

var connectionString = builder.Configuration.GetConnectionString("QuizDB");

var appSettings = AppSettings.instance();
appSettings.connectionString = connectionString; 

builder.Services.AddDbContext<TriviapiDBContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.AddControllers().AddJsonOptions(x =>
								x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();


// possibly extract from condition
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
