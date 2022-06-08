using System.Text.Json;
using QuizAPI.Utilities;
using Newtonsoft.Json.Linq;

namespace QuizAPI.Middleware
{
	public class APIKeyMiddleware
	{
		private static bool authEnabled = false;
		private readonly RequestDelegate _next;
		private const string APIKEY = "key";
		private const string SECRET = "secret";

		public APIKeyMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (!authEnabled) {
				await _next(context);
				return;
			}

			// extract secret name
			if (!context.Request.Headers.TryGetValue(APIKEY, out var extractedApiKey))
			{
				context.Response.StatusCode = 401;
				await context.Response.WriteAsync("Api Key was not provided.");
				return;
			}

			// extract secret
			if (!context.Request.Headers.TryGetValue(SECRET, out var extractedSecret))
			{
				context.Response.StatusCode = 401;
				await context.Response.WriteAsync("Secret Key was not provided.");
				return;
			}

			var response = SecretsManager.GetSecret(extractedApiKey);

			if (response == null)
			{
				context.Response.StatusCode = 400;
				await context.Response.WriteAsync("Invalid API Key");
				return;
			}

			var secrets = JObject.Parse(response);

			// I think this block is dead code
			if (secrets == null)
			{
				context.Response.StatusCode = 401;
				await context.Response.WriteAsync("Unauthorised client");
				return;
			}

			if (!secrets.ContainsKey(APIKEY))
			{
				// this would hit if something goes wrong on AWS side or if key in secret is not "secret"
				context.Response.StatusCode = 400;
				await context.Response.WriteAsync("Either something went wrong on AWS (which is highly unlikely) or we have a malformed secret.");
				return;
			}

			if (!extractedSecret.Equals(secrets[APIKEY]?.ToString()))
			{
				context.Response.StatusCode = 401;
				await context.Response.WriteAsync("Unauthorised client");
				return;
			}

			await _next(context);
		}
	}
}
