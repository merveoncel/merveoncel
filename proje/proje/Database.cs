using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows;

namespace proje
{
    public static class Database
    {
        public static SqlConnection sc;
        public static string SQLConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=";
        public static string SQLConnectionString2 = ";Integrated Security=True";

        public static bool Connected
        {
            get
            {
                if (sc == null) return false;
                return sc.State == System.Data.ConnectionState.Open;
            }
        }


        public static void Connect()
        {
            if (Connected == true) 
                return;
            sc = new SqlConnection(SQLConnectionString + System.AppDomain.CurrentDomain.BaseDirectory + @"Database1.mdf" + SQLConnectionString2);
            sc.Open();
        }

    }
}
