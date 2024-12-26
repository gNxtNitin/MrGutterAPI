using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrGutter.Domain
{
    public interface IMiscDataSetting
    {
        public Task<string> GetCommaSeparatedValues(DataRow[] dataRows, string columnName);
        public Task<string> GetMD5Encryption(string val);
        public Task<DataTable> GetDataTable(string query, string connStr);
        public Task<DataTable> GetDataTable(string query);
        public Task<DataSet> GetDataSet(string query, string connStr);
        public Task<DataSet> GetDataSet(string query);
        public Task<int> ExecuteNonQuery(string query);
        public void ExecuteNonQueryNonAsync(string query);
        public string Serialize(object value);
        public string TestConvertToJSON(DataSet dataSet);
        public string ConvertToJSON(DataSet dataSet);
        public string ConvertToJSON(DataTable table);
        public void LogFileWrite(string msg);
        public void LogFileWrite(Exception ex);
        public string CreateErrorMessage(Exception serviceException);
        public string CreateErrorMessageAsString(Exception ex);
    }

}
