using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
namespace MrGutter.Domain
{
    public class MiscDataSetting : IMiscDataSetting
    {
        public async Task<string> GetCommaSeparatedValues(DataRow[] dataRows, string columnName)
        {
            // Use a StringBuilder for efficient string concatenation
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // Loop through each row in the DataTable
            foreach (DataRow row in dataRows)
            {
                // Append the value from the specified column to the StringBuilder
                sb.Append(row[columnName].ToString());

                // Append a comma to separate values (except for the last one)
                sb.Append(",");
            }

            // Remove the trailing comma (if any) and return the result
            return sb.Length > 0 ? sb.ToString(0, sb.Length - 1) : string.Empty;
        }
        public async Task<string> GetMD5Encryption(string val)
        {
            StringBuilder sb = new StringBuilder();
            MD5CryptoServiceProvider md5obj = new MD5CryptoServiceProvider();
            md5obj.ComputeHash(ASCIIEncoding.ASCII.GetBytes(val));
            byte[] result = md5obj.Hash;
            for (int i = 0; i < val.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }
            return sb.ToString();
        }
        public async Task<DataTable> GetDataTable(string query, string connStr)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    await connection.OpenAsync();
                    SqlDataAdapter adap = new SqlDataAdapter(command);
                    adap.Fill(dt);
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                ////LogFileWrite(ex);
            }
            return dt;
        }
        public async Task<DataTable> GetDataTable(string query)
        {
            DataTable dt = new DataTable();
            string connStr = UMSResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    await connection.OpenAsync();
                    SqlDataAdapter adap = new SqlDataAdapter(command);
                    adap.Fill(dt);
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                ////LogFileWrite(ex);
            }
            return dt;
        }
        public async Task<DataSet> GetDataSet(string query, string connStr)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    await connection.OpenAsync();
                    SqlDataAdapter adap = new SqlDataAdapter(command);
                    adap.Fill(ds);
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                // //LogFileWrite(ex);
            }
            return ds;
        }
        public async Task<DataSet> GetDataSet(string query)
        {
            DataSet ds = new DataSet();
            string connStr = UMSResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    await connection.OpenAsync();
                    SqlDataAdapter adap = new SqlDataAdapter(command);
                    adap.Fill(ds);
                    await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                ////LogFileWrite(ex);
            }
            return ds;
        }
        public async Task<int> ExecuteNonQuery(string query)
        {
            int result = 0;
            string connStr = UMSResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    result = await command.ExecuteNonQueryAsync();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                // //LogFileWrite(ex);
            }
            return result;
        }
        public void ExecuteNonQueryNonAsync(string query)
        {
            int result = 0;
            string connStr = UMSResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value).Replace("\\", "");
        }
        public string TestConvertToJSON(DataSet dataSet)
        {
            int counter = 0;
            var JSONString = new StringBuilder();
            string json = string.Empty;
            JSONString.Append("{");
            foreach (DataTable table in dataSet.Tables)
            {
                counter++;
                string tableName = table.TableName;
                if (tableName == null || String.IsNullOrEmpty(tableName))
                {
                    tableName = "Table" + counter.ToString();
                }
                //JSONString.Append("\"" + tableName + "\":");
                JSONString.Append("[");
                if (table.Rows.Count > 0)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        JSONString.Append("{");
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            string rowData = table.Rows[i][j].ToString();
                            StringWriter wr = new StringWriter();
                            var jsonWriter = new JsonTextWriter(wr);
                            jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                            new JsonSerializer().Serialize(jsonWriter, rowData);
                            rowData = wr.ToString();
                            //if (j < table.Columns.Count - 1)
                            //{
                            //    JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString().Trim().Replace(@"\r\n"," ").Replace(@"\n", " ").Replace(@"\r", " ").Replace(@"\n\r"," ").Replace(@"\", "\\").Replace(System.Environment.NewLine," ").Replace("<br/>"," ").Replace(@"\t"," ").Replace("<br />", " ") + "\",");
                            //}
                            //else if (j == table.Columns.Count - 1)
                            //{
                            //    JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString().Trim().Replace(@"\r\n", " ").Replace(@"\n", " ").Replace(@"\r", " ").Replace(@"\n\r", " ").Replace(@"\","\\").Replace(System.Environment.NewLine, " ").Replace("<br/>", " ").Replace(@"\t", " ").Replace("<br />", " ") + "\"");
                            //}
                            if (j < table.Columns.Count - 1)
                            {
                                JSONString.Append("" + table.Columns[j].ColumnName.ToString() + ":" + rowData + ",");
                            }
                            else if (j == table.Columns.Count - 1)
                            {
                                JSONString.Append("" + table.Columns[j].ColumnName.ToString() + ":" + rowData);
                            }
                        }
                        if (i == table.Rows.Count - 1)
                        {
                            JSONString.Append("}");
                        }
                        else
                        {
                            JSONString.Append("},");
                        }
                    }
                }
                else
                {
                    //JSONString.Append("{}");
                }
                if (counter == dataSet.Tables.Count)
                {
                    JSONString.Append("]");
                }
                else
                {
                    JSONString.Append("],");
                }
            }

            JSONString.Append("}");

            return JSONString.ToString();// JToken.Parse(JSONString.ToString()).ToString();
        }
        public string ConvertToJSON(DataSet dataSet)
        {
            int counter = 0;
            var JSONString = new StringBuilder();
            string json = string.Empty;
            JSONString.Append("{");
            foreach (DataTable table in dataSet.Tables)
            {
                counter++;
                string tableName = table.TableName;
                if (tableName == null || String.IsNullOrEmpty(tableName))
                {
                    tableName = "Table" + counter.ToString();
                }
                JSONString.Append("\"" + tableName + "\":");
                JSONString.Append("[");
                if (table.Rows.Count > 0)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        JSONString.Append("{");
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            string rowData = table.Rows[i][j].ToString();
                            StringWriter wr = new StringWriter();
                            var jsonWriter = new JsonTextWriter(wr);
                            jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                            new JsonSerializer().Serialize(jsonWriter, rowData);
                            rowData = wr.ToString();
                            //if (j < table.Columns.Count - 1)
                            //{
                            //    JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString().Trim().Replace(@"\r\n"," ").Replace(@"\n", " ").Replace(@"\r", " ").Replace(@"\n\r"," ").Replace(@"\", "\\").Replace(System.Environment.NewLine," ").Replace("<br/>"," ").Replace(@"\t"," ").Replace("<br />", " ") + "\",");
                            //}
                            //else if (j == table.Columns.Count - 1)
                            //{
                            //    JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString().Trim().Replace(@"\r\n", " ").Replace(@"\n", " ").Replace(@"\r", " ").Replace(@"\n\r", " ").Replace(@"\","\\").Replace(System.Environment.NewLine, " ").Replace("<br/>", " ").Replace(@"\t", " ").Replace("<br />", " ") + "\"");
                            //}
                            if (j < table.Columns.Count - 1)
                            {
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + rowData + ",");
                            }
                            else if (j == table.Columns.Count - 1)
                            {
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + rowData);
                            }
                        }
                        if (i == table.Rows.Count - 1)
                        {
                            JSONString.Append("}");
                        }
                        else
                        {
                            JSONString.Append("},");
                        }
                    }
                }
                else
                {
                    //JSONString.Append("{}");
                }
                if (counter == dataSet.Tables.Count)
                {
                    JSONString.Append("]");
                }
                else
                {
                    JSONString.Append("],");
                }
            }

            JSONString.Append("}");

            return JSONString.ToString();// JToken.Parse(JSONString.ToString()).ToString();
        }
        public string ConvertToJSON(DataTable table)
        {
            int counter = 0;
            var JSONString = new StringBuilder();
            string json = string.Empty;
            JSONString.Append("{");
            counter++;
            string tableName = table.TableName;
            if (tableName == null || String.IsNullOrEmpty(tableName))
            {
                tableName = "Table" + counter.ToString();
            }
            JSONString.Append("\"" + tableName + "\":");
            JSONString.Append("[");
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        string rowData = table.Rows[i][j].ToString();
                        StringWriter wr = new StringWriter();
                        var jsonWriter = new JsonTextWriter(wr);
                        jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                        new Newtonsoft.Json.JsonSerializer().Serialize(jsonWriter, rowData);
                        rowData = wr.ToString();
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + rowData + ",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + rowData);
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
            }
            JSONString.Append("]}");
            return JSONString.ToString();// JToken.Parse(JSONString.ToString()).ToString();
        }
        public void LogFileWrite(string msg)
        {
            string message = msg;
            // string rootPath = DNFINSResources.env.ContentRootPath;
            string rootPath = "";
            //String dir = pathProvider.MapPath("abc");
            String logDir = rootPath + "\\Logs";
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            String exDir = rootPath + "\\Logs\\Exception";
            if (!Directory.Exists(exDir))
            {
                Directory.CreateDirectory(exDir);
            }
            String logFileName = exDir + "\\log_" + DateTime.Today.ToString("yyyyMMdd") + "." + "txt";
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                if (!File.Exists(logFileName))
                {
                    using (File.Create(logFileName)) ;
                }
                fileStream = new FileStream(logFileName, FileMode.Append);
                streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine("===========================================================");
                streamWriter.WriteLine(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));
                streamWriter.WriteLine(message);
                streamWriter.WriteLine("===========================================================");
            }
            finally
            {
                if (streamWriter != null) streamWriter.Close();
                if (fileStream != null) fileStream.Close();
            }

        }
        public void LogFileWrite(Exception ex)
        {
            string message = CreateErrorMessage(ex);
            //string rootPath = DNFINSResources.env.ContentRootPath;
            string rootPath = "";
            //String dir = pathProvider.MapPath("abc");
            String logDir = rootPath + "\\Logs";
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            String exDir = rootPath + "\\Logs\\Exception";
            if (!Directory.Exists(exDir))
            {
                Directory.CreateDirectory(exDir);
            }
            String logFileName = exDir + "\\log_" + DateTime.Today.ToString("yyyyMMdd") + "." + "txt";
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                if (!File.Exists(logFileName))
                {
                    using (File.Create(logFileName)) ;
                }
                fileStream = new FileStream(logFileName, FileMode.Append);
                streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine("===========================================================");
                streamWriter.WriteLine(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));
                streamWriter.WriteLine(message);
                streamWriter.WriteLine("===========================================================");
            }
            finally
            {
                if (streamWriter != null) streamWriter.Close();
                if (fileStream != null) fileStream.Close();
            }

        }
        public string CreateErrorMessage(Exception serviceException)
        {
            StringBuilder messageBuilder = new StringBuilder();
            try
            {
                messageBuilder.Append("The Exception is:-");
                messageBuilder.Append("Exception :: " + serviceException.ToString());
                if (serviceException.InnerException != null)
                {
                    messageBuilder.Append("InnerException :: " + serviceException.InnerException.ToString());
                }
                return messageBuilder.ToString();
            }
            catch
            {
                messageBuilder.Append("Exception:: Unknown Exception.");
                return messageBuilder.ToString();
            }
        }
        public string CreateErrorMessageAsString(Exception ex)
        {
            return CreateErrorMessage(ex);
        }
    }

}
