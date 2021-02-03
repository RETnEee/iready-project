namespace iready.lib.Common
{
  public static class ConnectionHelper
    {
        public static string Mysql { get; set; }
        public static List<ConnectionModel> Redis { get; set; }

        static DatabaseConnectionHelper()
        {
            Mysql = default;
            Redis = new List<ConnectionModel>();
        }
    }
}