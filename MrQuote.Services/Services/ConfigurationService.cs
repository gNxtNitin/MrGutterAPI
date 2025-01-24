using MrQuote.Domain;
using MrQuote.Domain.Models;
using MrQuote.Domain.Models.RequestModel;
using MrQuote.Services.IServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Services.Services
{
    public class ConfigurationService : IConfigurationService
    {
        IMiscDataSetting _miscDataSetting;
        public ConfigurationService(IMiscDataSetting miscDataSetting)
        {
            _miscDataSetting = miscDataSetting;
        }
        public async Task<ResponseModel> CreateOrSetBranding(BrandingReqModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteBranding", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Flag", SqlDbType.Char)
                        {
                            Value = "C"
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyID", SqlDbType.Int)
                        {
                            Value = (object)req.CompanyId ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyName", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.AccountName ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyEmail", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.CompanyEmail ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyPhone", SqlDbType.NVarChar, 20)
                        {
                            Value = (object)req.CompanyPhone ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@BusinessNumber", SqlDbType.NVarChar,100)
                        {
                            Value = (object)req.BusinessNumber ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyLogo", SqlDbType.NVarChar)
                        {
                            Value = (object)req.CompanyLogoPath ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@WebAddress", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)req.WebAddress ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@AreaTitle", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)req.AreaTitle ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@AreaName1", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)req.AreaTitle ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@AreaName2", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)req.AreaTitle ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@PrimaryColor", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)req.Primary ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@SecondaryColor", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)req.Secondary ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int)
                        {
                            Value = (object)req.CreatedBy ?? 0
                        });
                        SqlParameter retParam = new SqlParameter("@ret", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(retParam);
                        SqlParameter errorMsgParam = new SqlParameter("@errorMsg", SqlDbType.NVarChar, 200)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(errorMsgParam);
                        await command.ExecuteNonQueryAsync();
                        response.code = Convert.ToInt32(retParam.Value);
                        response.msg = Convert.ToString(errorMsgParam.Value);
                    }
                }
                if (req.ThemeList.Count != 0 && req.ThemeList != null)
                {
                    foreach (var item in req.ThemeList)
                    {
                        item.CompanyId = response.code;
                        var res1 = CreateOrSetReportTheme(item, req.CreatedBy);
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = -1;
                response.msg = ex.Message;
            }
            return response;
        }
        public async Task<ResponseModel> GetBrandings(int companyId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", "G", "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@CompanyID", companyId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetDeleteBranding", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["CompanyID"]?.ToString()))
                {
                    ds.Tables[0].TableName = "CompanyBranding";
                    response.code = 200;
                    response.data = _miscDataSetting.ConvertToJSON(ds.Tables[0]);
                    response.msg = "Success";
                }
                else
                {
                    response.code = -2;
                    response.msg = "No data found.";
                }
            }
            catch (Exception ex)
            {
                response.code = -3;
                response.msg = ex.Message;
            }
            return await Task.FromResult(response);
        }
        public async Task<ResponseModel> GetCompanyTheme(int companyId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", "R", "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@CompanyID", companyId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetDeleteBranding", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["CompanyId"]?.ToString()))
                {
                    ds.Tables[0].TableName = "CompanyTheme";
                    response.code = 200;
                    response.data = _miscDataSetting.ConvertToJSON(ds.Tables[0]);
                    response.msg = "Success";
                }
                else
                {
                    response.code = -2;
                    response.msg = "No data found.";
                }
            }
            catch (Exception ex)
            {
                response.code = -3;
                response.msg = ex.Message;
            }
            return await Task.FromResult(response);
        }
        public async Task<ResponseModel> CreateOrSetReportTheme(ThemeModel req, int createdBy)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteBranding", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Flag", SqlDbType.Char)
                        {
                            Value = "T"
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyID", SqlDbType.Int)
                        {
                            Value = (object)req.CompanyId ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@ThemeId", SqlDbType.Int)
                        {
                            Value = (object)req.ThemeId ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@ThemePath", SqlDbType.NVarChar)
                        {
                            Value = (object)req.ThemePath ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit)
                        {
                            Value = (object)req.IsActive ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int)
                        {
                            Value = (object)createdBy ?? 0
                        });
                        SqlParameter retParam = new SqlParameter("@Ret", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(retParam);
                        SqlParameter errorMsgParam = new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 200)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(errorMsgParam);
                        await command.ExecuteNonQueryAsync();
                        response.code = Convert.ToInt32(retParam.Value);
                        response.msg = Convert.ToString(errorMsgParam.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = -1;
                response.msg = ex.Message;
            }
            return response;
        }
    }
}
