using MrQuote.Domain;
using MrQuote.Domain.Models;
using MrQuote.Domain.Models.RequestModel;
using MrQuote.Services.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrQuote.Services.Services
{
    public class LayoutService : ILayoutService
    {
        public async Task<ResponseModel> CreateOrSetLayout(LayoutReqModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteLayout", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Flag", SqlDbType.Char)
                        {
                            Value = (object)req.Flag ?? "G"
                        });
                        command.Parameters.Add(new SqlParameter("@LayoutID", SqlDbType.Int)
                        {
                            Value = (object)req.LayoutID ?? 0
                        });

                         command.Parameters.Add(new SqlParameter("@LayoutName", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.LayoutName ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@IsShared", SqlDbType.Bit)
                        {
                            Value = (object)req.IsShared ?? false
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyID", SqlDbType.Int)
                        {
                            Value = (object)req.CompanyID ?? 1
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
