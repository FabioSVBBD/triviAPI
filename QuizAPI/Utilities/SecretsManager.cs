using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace QuizAPI.Utilities
{
	public class SecretsManager
	{
		public static string? GetSecret(string secretName)
		{
			// string secretName = "test/my-key";
			string region = "af-south-1";

			MemoryStream stream = new MemoryStream();

			IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

			GetSecretValueRequest request = new GetSecretValueRequest();
			request.SecretId = secretName;
			request.VersionStage = "AWSCURRENT";

			GetSecretValueResponse? response = null;

			try
			{
				response = client.GetSecretValueAsync(request).Result;
			}
			catch (Exception e)
			{
				// if resource not found, throws aggregate:
				// ResourceNotFoundException, and HttpErrorResponseException
				_ = e;
				return null;
			}

			if (response.SecretString != null)
				return response.SecretString;

			stream = response.SecretBinary;
			StreamReader reader = new StreamReader(stream);
			return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
		}
	}
}
