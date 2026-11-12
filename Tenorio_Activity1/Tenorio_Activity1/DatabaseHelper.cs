using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.OleDb;

namespace Tenorio_Activity1
{
    public static class DatabaseHelper
    {
        public static string ConnectionString =>
            ConfigurationManager.ConnectionStrings["Act1DB"].ConnectionString;

        public static OleDbConnection GetConnection()
        {
            return new OleDbConnection(ConnectionString);
        }
    }
}