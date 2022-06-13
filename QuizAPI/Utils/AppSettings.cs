namespace QuizAPI.Utils
{
    public class AppSettings 
    {
        AppSettings(){
            this.connectionString = "";
        }
        public string connectionString {get; set;}

        private static AppSettings? Instance {get; set;}

        public static AppSettings instance()
        {
            if(Instance == null)
            {
                Instance = new AppSettings();
            }

            return Instance;
        }

    }

}