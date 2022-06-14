namespace QuizAPI.Utils
{
    public class UrlHelper
    {
        public static string scheme { get; set; }
        public static string host { get; set; }
        public static string path { get; set; }
        public static string baseURL { get; set; }

        public static void setBaseUrl(string scheme, string host, string path)
        {
            scheme = scheme.ToLower();
            host = host.ToLower();
            path = path.ToLower();

            baseURL = $"{scheme}://{host}{path}";
        }
    }
}
