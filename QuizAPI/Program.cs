using Microsoft.EntityFrameworkCore;
using QuizAPI.Model;
using static QuizAPI.Middleware.APIKeyMiddleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("QuizDB");
builder.Services.AddDbContext<TriviapiDBContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

var app = builder.Build();


// possibly extract from condition
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<QuizAPI.Middleware.APIKeyMiddleware>();

app.Run();
