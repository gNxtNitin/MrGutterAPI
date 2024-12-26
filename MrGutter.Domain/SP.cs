using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace MrGutter.Domain
{
    public class SP
    {
        internal class SPArgBuild
        {
            internal string parameterName = "";
            internal string parameterValue = "";
            /// <summary>
            /// Write full data type, such as SqlDBType.VarChar.
            /// </summary>
            internal string pramValueType = "";
            internal string parameterDirection = "";
            /// <summary>
            /// Use to create SP Argument Build conestruction.
            /// </summary>
            /// <param name="pramName">SP Argument Parameter Name.</param>
            /// <param name="pramValue">SP Argument Parameter Value.</param>
            internal SPArgBuild(string pramName, string pramValue, string pramValueType, string parameterDirection)
            {
                this.parameterName = pramName;
                this.parameterValue = pramValue;
                this.pramValueType = pramValueType;
                this.parameterDirection = parameterDirection;
            }
        }

        /// <summary>
        /// This function built an Array List, which is collection of some SP parameter's Name, Value and Data type.
        /// </summary>
        /// <param name="arrLst">Array List which will store all argument.</param>
        /// <param name="spParmName">SP Argument Parameter Name.</param>
        /// <param name="spParmValue">SP Argument Parameter Value.</param>
        /// <param name="spPramValueType">Parameter value type EXACTLY same as SqlDBType. E.g. 'SqlDbType.BigInt' will 'BigInt'. </param>
        /// <returns></returns>
        public static ArrayList spArgumentsCollection(ArrayList arrLst, string spParmName, string spParmValue, string spPramValueType, string parameterDirection)
        {
            SPArgBuild spArgBuiltObj = new SPArgBuild(spParmName, spParmValue, spPramValueType, parameterDirection);
            arrLst.Add(spArgBuiltObj);
            return arrLst;
        }

        /// <summary>
        /// Run a stored procedure of Select SQL type.
        /// </summary>
        /// <param name="dbConnStr">Connection String to connect Sql Server</param>
        /// <param name="ds">DataSet which will return after filling Data</param>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="spPramArrList">Parameters in ArrayList</param>
        /// <returns>Return DataSet after filing data by SQL.</returns>
        public static DataSet RunStoredProcedure(string dbConnStr, DataSet ds, string spName, ArrayList spPramArrList)
        {
            SqlConnection conn = new SqlConnection(dbConnStr);
            conn.Open();
            SqlCommand cmd = new SqlCommand();

            cmd = ArrangeParamter(spPramArrList);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;
            cmd.CommandText = spName;
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            adap.Fill(ds);
            conn.Close();
            return ds;

        }

        /// <summary>
        /// Run a stored procedure which will execure some nonquery SQL.
        /// </summary>
        /// <param name="dbConnStr">Connection String to connect Sql Server</param>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="spPramArrList">Parameters in a ArrayList</param>
        public static void RunStoredProcedure(string dbConnStr, string spName, ArrayList spPramArrList)
        {
            SqlConnection conn = new SqlConnection(dbConnStr);
            conn.Open();
            SqlCommand cmd = new SqlCommand();


            cmd = ArrangeParamter(spPramArrList);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;
            cmd.CommandText = spName;
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        public static int RunStoredProcedureRet(string dbConnStr, string spName, ArrayList spPramArrList)
        {
            SqlConnection conn = new SqlConnection(dbConnStr);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd = ArrangeParamter(spPramArrList);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;
            cmd.CommandText = spName;
            cmd.ExecuteNonQuery();
            int ret = Convert.ToInt32(cmd.Parameters["@ret"].Value);
            conn.Close();
            return ret;
        }
        internal static SqlCommand ArrangeParamter(ArrayList spPramArrList)
        {
            SqlCommand cmd = new SqlCommand();
            string spPramName = "";
            string spPramValue = "";
            string spPramDataType = "";
            string spParameterDirection = "";
            for (int i = 0; i < spPramArrList.Count; i++)
            {
                spPramName = ((SPArgBuild)spPramArrList[i]).parameterName;
                spPramValue = ((SPArgBuild)spPramArrList[i]).parameterValue;
                spPramDataType = ((SPArgBuild)spPramArrList[i]).pramValueType;
                spParameterDirection = ((SPArgBuild)spPramArrList[i]).parameterDirection;
                SqlParameter pram = null;
                #region SQL DB TYPE AND VALUE ASSIGNMENT
                switch (spPramDataType.ToUpper())
                {
                    case "BIGINT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.BigInt);
                        pram.Value = spPramValue;
                        break;

                    case "BINARY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Binary);
                        pram.Value = spPramValue;
                        break;

                    case "BIT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Bit);
                        pram.Value = spPramValue;
                        break;

                    case "CHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Char);
                        pram.Value = spPramValue;
                        break;

                    case "DATETIME":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.DateTime);
                        pram.Value = spPramValue;
                        break;

                    case "DECIMAL":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Decimal);
                        pram.Value = spPramValue;
                        break;

                    case "FLOAT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Float);
                        pram.Value = spPramValue;
                        break;

                    case "IMAGE":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Image);
                        pram.Value = spPramValue;
                        break;

                    case "INT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Int);
                        pram.Value = spPramValue;
                        break;

                    case "MONEY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Money);
                        pram.Value = spPramValue;
                        break;

                    case "NCHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.NChar);
                        pram.Value = spPramValue;
                        break;

                    case "NTEXT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.NText);
                        pram.Value = spPramValue;
                        break;

                    case "NVARCHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.NVarChar);
                        pram.Value = spPramValue;
                        break;

                    case "REAL":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Real);
                        pram.Value = spPramValue;
                        break;

                    case "SMALLDATETIME":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.SmallDateTime);
                        pram.Value = spPramValue;
                        break;

                    case "SMALLINT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.SmallInt);
                        pram.Value = spPramValue;
                        break;

                    case "SMALLMONEY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.SmallMoney);
                        pram.Value = spPramValue;
                        break;

                    case "TEXT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Text);
                        pram.Value = spPramValue;
                        break;

                    case "TIMESTAMP":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Timestamp);
                        pram.Value = spPramValue;
                        break;

                    case "TINYINT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.TinyInt);
                        pram.Value = spPramValue;
                        break;

                    case "UDT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Udt);
                        pram.Value = spPramValue;
                        break;

                    case "UMIQUEIDENTIFIER":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.UniqueIdentifier);
                        pram.Value = spPramValue;
                        break;

                    case "VARBINARY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.VarBinary);
                        pram.Value = spPramValue;
                        break;

                    case "VARCHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.VarChar);
                        pram.Value = spPramValue;
                        break;

                    case "VARIANT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Variant);
                        pram.Value = spPramValue;
                        break;

                    case "XML":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Xml);
                        pram.Value = spPramValue;
                        break;
                }
                switch (spParameterDirection.ToUpper())
                {
                    case "I":
                        pram.Direction = ParameterDirection.Input;
                        break;

                    case "O":
                        pram.Direction = ParameterDirection.Output;
                        break;

                    case "R":
                        pram.Direction = ParameterDirection.ReturnValue;
                        break;

                    case "IO":
                        pram.Direction = ParameterDirection.InputOutput;
                        break;
                }
                #endregion
            }
            return cmd;
        }
    }

}
