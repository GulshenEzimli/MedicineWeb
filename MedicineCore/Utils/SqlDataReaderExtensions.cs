using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace HospitalCore.Utils
{
    internal static class SqlDataReaderExtensions
    {
        internal static bool IsDBNull(this SqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName));
        }
        internal static int GetInt32(this SqlDataReader reader, string columnName)
        {
            return reader.GetInt32(reader.GetOrdinal(columnName));
        }

        internal static string GetString(this SqlDataReader reader, string columnName)
        {
            return reader.GetString(reader.GetOrdinal(columnName));
        }

        internal static DateTime GetDateTime(this SqlDataReader reader, string columnName)
        {
            return reader.GetDateTime(reader.GetOrdinal(columnName));
        }

        internal static bool GetBoolean(this SqlDataReader reader, string columnName)
        {
            return reader.GetBoolean(reader.GetOrdinal(columnName));
        }

        internal static decimal GetDecimal(this SqlDataReader reader, string columnName)
        {
            return reader.GetDecimal(reader.GetOrdinal(columnName));
        }

        internal static byte GetByte(this SqlDataReader reader, string columnName)
        {
            return reader.GetByte(reader.GetOrdinal(columnName));
        }
    }
}
