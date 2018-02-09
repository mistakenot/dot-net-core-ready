using System;
using System.Data.SqlClient;
using Npgsql;

namespace DotNetCoreReady.Extensions
{
    public static class UriExtensions
    {
        public static string ToConnectionString(this Uri uri)
        {
            var builder = new NpgsqlConnectionStringBuilder();
            
            builder.Add("Host", uri.Host);
            builder.Add("Port", uri.Port);
            builder.Add("Database", uri.LocalPath.Substring(1));
            builder.Add("Username", uri.UserInfo.Split(":")[0]);
            builder.Add("Password", uri.UserInfo.Split(":")[1]);

            return builder.ConnectionString;
        }
    }
}