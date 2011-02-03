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
        public static T AddParm<T>(this T cmd, string parmName, object parmValue) where T : DbCommand
        {
            DbParameter parm = cmd.CreateParameter();
            parm.ParameterName = parmName;
            parm.Value = parmValue;
            cmd.Parameters.Add(parm);
            return cmd;
        }
        public static T SetCommandText<T>(this T command, string commandText) where T : DbCommand
        {
            command.CommandText = commandText;
            return command;
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
