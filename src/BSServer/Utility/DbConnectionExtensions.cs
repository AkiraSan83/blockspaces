using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace JollyBit.BS.Server.Utility
{
    public static class DbConnectionExtensions
    {
        public static DbCommand AddParm<T>(this DbCommand cmd, string parmName, T parmValue)
        {
            SqlParameter parm = new SqlParameter(parmName, parmValue);
            cmd.Parameters.Add(parm);
            return cmd;
        }
        public static DbCommand CreateCommand<T>(this T conn, string commandText) where T : DbConnection
        {
            DbCommand comm = conn.CreateCommand();
            comm.CommandText = commandText;
            return comm;
        }
        public static DataSet ExecuteQuery<T>(this T cmd) where T : DbCommand
        {
            if (cmd is SQLiteCommand)
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd as SQLiteCommand);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            else throw new System.NotImplementedException();
        }
    }
}
