namespace BackEnd.Helpers
{
    public class AuthSettings
    {
        public static string PrivateKey { get; set; } = Environment.GetEnvironmentVariable("PrivateKey");
    }
}
