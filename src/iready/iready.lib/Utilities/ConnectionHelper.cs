using System.Linq;
using System.Collections.Generic;
using iready.lib.Model;

namespace iready.lib.Utilities
{
  public static class ConnectionHelper
    {
        public static string Mysql { get; set; }
        public static List<ConnectionModel> Redis { get; set; }

        static ConnectionHelper()
        {
            Mysql = default;
            Redis = new List<ConnectionModel>();
        }
    }
}