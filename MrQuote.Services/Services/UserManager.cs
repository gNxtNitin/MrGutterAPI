using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MrQuote.Domain.Models;
using MrQuote.Domain;
using MrQuote.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MrQuote.Utility;
using Newtonsoft.Json;
using MrQuote.Domain.Models.RequestModel;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MrQuote.Services.Services
{
    public class UserManager : IUserManager
    {
        public IConfiguration _configuration;
        ILogger _logger;
        IMiscDataSetting _miscDataSetting;
        IUMSService _UMSEmailService;
        EncDcService encDcService = new EncDcService();
        public UserManager(IConfiguration configuration, IMiscDataSetting miscDataSetting, IUMSService UMSEmailService)
        {
            _configuration = configuration;
            _miscDataSetting = miscDataSetting;
            _UMSEmailService = UMSEmailService;
        }
        public async Task<ResponseModel> GetGroupMaster(string? encReq)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                string groupId = await encDcService.Decrypt(encReq);
                string flag = groupId == null || groupId == "" ? "G" : "I";
                groupId = groupId == null || groupId == "" ? "0" : groupId;
                DataSet ds = new DataSet();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@GroupId", groupId ?? "0", "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetDeleteGroup", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["GroupName"]?.ToString()))
                {
                    ds.Tables[0].TableName = "Groups";
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
                response.code = -1;
                response.msg = ex.Message;
            }
            return response;
        }
        public async Task<ResponseModel> CreateOrSetGroupMaster(RequestModel encReq, char flag)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                string json = await encDcService.Decrypt(encReq.V);
                GroupMasterReqModel rq = JsonConvert.DeserializeObject<GroupMasterReqModel>(json);
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteGroup", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the parameters required by the stored procedure
                        command.Parameters.Add(new SqlParameter("@Flag", SqlDbType.Char)
                        {
                            Value = flag
                        });
                        command.Parameters.Add(new SqlParameter("@GroupId", SqlDbType.Int)
                        {
                            Value = (object)rq.GroupId ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)rq.GroupName ?? DBNull.Value
                        });

                        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)rq.Description ?? DBNull.Value
                        });
                        // Output parameters
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
        public async Task<ResponseModel> GetRoleMaster(string? encReq)
        {
            ResponseModel response = new ResponseModel();
            //string flag = roleId.HasValue ? "I" : "G";
            string roleId = await encDcService.Decrypt(encReq);
            string flag = roleId == null || roleId == "" ? "G" : "I";
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@RoleId", roleId ?? "0", "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetDeleteRole", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["RoleName"]?.ToString()))
                {
                    ds.Tables[0].TableName = "Roles";
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
        public async Task<ResponseModel> GetRoleByUserId(string encReq)
        {
            ResponseModel response = new ResponseModel();
            //string flag = roleId.HasValue ? "I" : "G";
            string userId = await encDcService.Decrypt(encReq);
            string flag = userId == null || userId == "" ? "G" : "I";
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@UserId", userId ?? "0", "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetDeleteRole", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["RoleName"]?.ToString()))
                {
                    ds.Tables[0].TableName = "Roles";
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
                // _logger.LogError("GetGroup", ex);
            }

            return await Task.FromResult(response);
        }
        public async Task<ResponseModel> CreateOrSetRoleMaster(RequestModel encReq, char flag)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                string json = await encDcService.Decrypt(encReq.V);
                RoleMasterReqModel rq = JsonConvert.DeserializeObject<RoleMasterReqModel>(json);
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteRole", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the parameters required by the stored procedure
                        command.Parameters.Add(new SqlParameter("@Flag", SqlDbType.Char)
                        {
                            Value = flag
                        });

                        command.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int)
                        {
                            Value = (object)rq.RoleId ?? DBNull.Value
                        });

                        command.Parameters.Add(new SqlParameter("@RoleName", SqlDbType.NVarChar, 100)
                        {
                            Value = (object)rq.RoleName ?? DBNull.Value
                        });

                        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)rq.Description ?? DBNull.Value
                        });
                        // Output parameters
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

                        // Execute the stored procedure
                        await command.ExecuteNonQueryAsync();

                        // Retrieve output parameters
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
        public async Task<ResponseModel> GetUsers(string? userId)
        {
            ResponseModel response = new ResponseModel();
            string flag = userId == null || userId == "" ? "G" : "I";
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@userId", userId == "" ? "0" : userId, "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetDeleteUsers", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["UserID"]?.ToString()))
                {
                    // ds.Tables[0].TableName = "Users";
                    response.code = 200;
                    //response.data = _miscDataSetting.ConvertToJSON(ds.Tables[0]);
                    response.data = JsonConvert.SerializeObject(ds.Tables[0]); //in this way, in the response, the table name as 'Layouts and the key name as 'Table' is removed; now the response is a string starting from array of objects without table name.
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
        public async Task<ResponseModel> CreateOrSetUser(UserMasterReqModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Flag", SqlDbType.Char)
                        {
                            Value = (object)req.Flag ?? "G"
                        });
                        command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int)
                        {
                            Value = (object)req.UserId ?? 0
                        });
                        //command.Parameters.Add(new SqlParameter("@groupId", SqlDbType.Int)
                        //{
                        //    Value = (object)req.GroupId ?? 0
                        //});
                        command.Parameters.Add(new SqlParameter("@roleId", SqlDbType.Int)
                        {
                            Value = (object)req.RoleId ?? 0
                        });
                        //command.Parameters.Add(new SqlParameter("@userName", SqlDbType.NVarChar, 100)
                        //{
                        //    Value = (object)req.UserName ?? DBNull.Value
                        //});
                        command.Parameters.Add(new SqlParameter("@firstName", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.FirstName ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@lastName", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.LastName ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@mobile", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.MobileNo ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.EmailID ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@dob", SqlDbType.SmallDateTime, 255)
                        {
                            Value = (object)req.DOB ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.Password ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@address1", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.Address1 ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@address2", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.Address2 ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@city", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.City ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@state", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.State ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@pin", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.PinCode ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@filepath", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.FilePath ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@isActive", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.IsActive ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@createdBy", SqlDbType.Int)
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
                //using (SqlConnection connection = new SqlConnection(connStr))
                //{
                //    await connection.OpenAsync();
                //    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteUsers", connection))
                //    {
                //        command.CommandType = CommandType.StoredProcedure;

                //        // Helper function to add parameters
                //        void AddParameter(string name, SqlDbType type, object value, int size = 0, ParameterDirection direction = ParameterDirection.Input)
                //        {
                //            var param = new SqlParameter(name, type) { Value = value ?? DBNull.Value, Direction = direction };
                //            if (size > 0) param.Size = size;
                //            command.Parameters.Add(param);
                //        }

                //        // Adding parameters
                //        AddParameter("@Flag", SqlDbType.Char, req.Flag ?? "G");
                //        AddParameter("@userId", SqlDbType.Int, req.UserId ?? 0);
                //        AddParameter("@roleId", SqlDbType.Int, req.RoleId ?? 0);
                //        AddParameter("@firstName", SqlDbType.NVarChar, req.FirstName, 255);
                //        AddParameter("@lastName", SqlDbType.NVarChar, req.LastName, 255);
                //        AddParameter("@mobile", SqlDbType.NVarChar, req.MobileNo, 255);
                //        AddParameter("@email", SqlDbType.NVarChar, req.EmailID, 255);
                //        AddParameter("@dob", SqlDbType.SmallDateTime, req.DOB);
                //        AddParameter("@password", SqlDbType.NVarChar, req.Password, 255);
                //        AddParameter("@address1", SqlDbType.NVarChar, req.Address1, 255);
                //        AddParameter("@address2", SqlDbType.NVarChar, req.Address2, 255);
                //        AddParameter("@city", SqlDbType.NVarChar, req.City, 255);
                //        AddParameter("@state", SqlDbType.NVarChar, req.State, 255);
                //        AddParameter("@pin", SqlDbType.NVarChar, req.PinCode, 255);
                //        AddParameter("@filepath", SqlDbType.NVarChar, req.FilePath, 255);
                //        AddParameter("@isActive", SqlDbType.NVarChar, req.IsActive, 255);
                //        AddParameter("@createdBy", SqlDbType.Int, req.CreatedBy?? 0);

                //        // Output parameters
                //        AddParameter("@ret", SqlDbType.Int, null, direction: ParameterDirection.Output);
                //        AddParameter("@errorMsg", SqlDbType.NVarChar, null, 200, ParameterDirection.Output);

                //        await command.ExecuteNonQueryAsync();

                //        response.code = Convert.ToInt32(command.Parameters["@ret"].Value);
                //        response.msg = Convert.ToString(command.Parameters["@errorMsg"].Value);
                //    }
                //}


            }
            catch (Exception ex)
            {
                response.code = -1;
                response.msg = ex.Message;
            }
            return response;
        }
        public async Task<ResponseModel> DeleteUserMaster(RequestModel req, char flag)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                string userId = await encDcService.Decrypt(req.V);
                UserMasterReqModel rq = new UserMasterReqModel();
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_GetSetDeleteUsers";
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter parameter = new SqlParameter();

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@flag", SqlDbType.Char);
                        parameter.Value = flag;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@userId", SqlDbType.Int);
                        parameter.Value = userId;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@createdBy", SqlDbType.Int);
                        parameter.Value = rq.CreatedBy;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@ret", SqlDbType.Int);
                        parameter.Direction = ParameterDirection.Output;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@errorMsg", SqlDbType.VarChar, 200);
                        parameter.Direction = ParameterDirection.Output;

                        int res = await command.ExecuteNonQueryAsync();
                        response.code = Convert.ToInt32(command.Parameters["@ret"].Value);
                        response.msg = command.Parameters["@errorMsg"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = -1;
                response.data = ex.Message;
            }
            return response;
        }
        public async Task<ResponseModel> CreateLogHistory(RequestModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                string json = await encDcService.Decrypt(req.V);
                LogHistoryReqModel rq = JsonConvert.DeserializeObject<LogHistoryReqModel>(json);


                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_GetSetLogHistory";
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter parameter = new SqlParameter();

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@flag", SqlDbType.Char);
                        parameter.Value = 'G';
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@userId", SqlDbType.Int);
                        parameter.Value = rq.UserId;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@actions", SqlDbType.TinyInt);
                        parameter.Value = rq.Actions;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@details", SqlDbType.VarChar);
                        parameter.Value = rq.Details;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@ret", SqlDbType.Int);
                        parameter.Direction = ParameterDirection.Output;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@errorMsg", SqlDbType.VarChar, 200);
                        parameter.Direction = ParameterDirection.Output;

                        int res = await command.ExecuteNonQueryAsync();
                        response.code = Convert.ToInt32(command.Parameters["@ret"].Value);
                        response.msg = command.Parameters["@errorMsg"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = -1;
                response.data = ex.Message;
                // _logger.LogError("SendForgotPasswordEmail", ex);
            }
            return response;
        }
        public async Task<ResponseModel> GetLogHistory()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", "G", "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetLogHistory", arrList);

                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["LogId"]?.ToString()))
                {
                    ds.Tables[0].TableName = "LogHistory";
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
                // _logger.LogError("GetGroup", ex);
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseModel> GetCompany(string? companyId)
        {
            ResponseModel response = new ResponseModel();
            string flag = companyId == null || companyId == "" ? "G" : "I";
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@CompanyID", companyId == "" ? "0" : companyId, "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetDeleteCompany", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["CompanyID"]?.ToString()))
                {
                    ds.Tables[0].TableName = "Company";
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

        public async Task<ResponseModel> CreateOrSetCompany(CompanyReqModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteCompany", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Flag", SqlDbType.Char)
                        {
                            Value = (object)req.Flag ?? "G"
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyID", SqlDbType.Int)
                        {
                            Value = (object)req.CompanyID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyName", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.CompanyName ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@PointOfContact", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.PointOfContact ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.Email ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 25)
                        {
                            Value = (object)req.PhoneNumber ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.IsActive ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int)
                        {
                            Value = (object)req.CreatedBy ?? 0
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

        public async Task<ResponseModel> Get_API_0_Data(string? userId)
        {
            ResponseModel response = new ResponseModel();
            string flag = userId == null || userId == "" ? "G" : "I";
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();

                var tableAliasMapping = new Dictionary<string, string>
        {
            { "Table", "Users" },
            { "Table1", "Roles" },
            { "Table2", "UserRoles" },
            { "Table3", "Company" },
            { "Table4", "UserCompany" },
            { "Table5", "Layout" },
            { "Table6", "UserLayout" },
            { "Table7", "Pages" },
            { "Table8", "LayoutPages" },
            { "Table9", "TitlePageContent" },
            { "Table10", "IntroductionPageContent" },
            { "Table11", "TermConditionsPageContent" },
            { "Table12", "WarrantyPageContent" },
            { "Table13", "AuthorizationPageContent" },
            { "Table14", "InspectionPageContent" },
            { "Table15", "InspectionPageSection" },
            { "Table16", "InspectionSectionItems" },
            { "Table17", "QuotePagePriceSection" },
            { "Table18", "QuotePageSection" },
            { "Table19", "QuotePageContent" },
            { "Table20", "ProductsPricing" },
            { "Table21", "SectionStyle" },
            { "Table22", "Templates" },
            { "Table23", "TemplatePages" },
            { "Table24", "TemplatesCategory" },
            { "Table25", "MeasurementCategory" },
            { "Table26", "UnitOfMeasurement" },
            { "Table27", "MeasurementToken" },
            { "Table28", "AuthPagePriceSection" },
            { "Table29", "AuthPrimarySigner" },
            { "Table30", "AuthProductSelection" }
        };


                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@userId", userId == "" ? "0" : userId, "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");

                ds = SP.RunStoredProcedure(connStr, ds, "sp_API_0_getData", arrList);

                //if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["LayoutID"]?.ToString()))
                if (ds.Tables[0].Rows.Count > 0 && ds.Tables != null)
                {
                    //ds.Tables[0].TableName = "Layouts";
                    foreach (DataTable dt in ds.Tables)
                    {

                        if (tableAliasMapping.ContainsKey(dt.TableName))
                        {
                            dt.TableName = tableAliasMapping[dt.TableName];
                        }
                    }

                    ManageFetchHistory(Convert.ToInt32(userId));

                    response.code = 200;
                    //response.data = _miscDataSetting.ConvertToJSON(ds.Tables[0]);

                    response.data = JsonConvert.SerializeObject(ds);  //in this way, in the response, the table name as 'Layouts and the key name as 'Table' is removed; now the response is a string starting from array of objects without table name.
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
                // _logger.LogError("GetGroup", ex);
            }

            return await Task.FromResult(response);
        }


        public async Task<ResponseModel> GetIntroPages()
        {
            ResponseModel response = new ResponseModel();
            try
            {

                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", "G", "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetIntroPages", arrList);

                if (ds.Tables[0].Rows.Count > 0 && ds.Tables != null)
                {
                    //ds.Tables[0].TableName = "Layouts";
                    response.code = 200;
                    //response.data = _miscDataSetting.ConvertToJSON(ds.Tables[0]);
                    response.data = JsonConvert.SerializeObject(ds.Tables[0]);  //in this way, in the response, the table name as 'Layouts and the key name as 'Table' is removed; now the response is a string starting from array of objects without table name.
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
                // _logger.LogError("GetGroup", ex);
            }

            return await Task.FromResult(response);
        }



        public async Task<ResponseModel> Get_API_1_Retrieve_Data(string? userId)
        {
            ResponseModel response = new ResponseModel();
            string flag = userId == null || userId == "" ? "G" : "I";
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();

                var tableAliasMapping = new Dictionary<string, string>
        {
            { "Table", "Company" },
            { "Table1", "Report" },
            { "Table2", "Templates" },
            { "Table3", "TemplatePages" },
            { "Table4", "QuotePageContent" },
            { "Table5", "QuotePagePriceSection" }
        };

                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@userId", userId == "" ? "0" : userId, "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");

                ds = SP.RunStoredProcedure(connStr, ds, "sp_API_1_RetrieveData", arrList);

                //if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["LayoutID"]?.ToString()))
                if (ds.Tables[0].Rows.Count > 0 && ds.Tables != null)
                {
                    foreach (DataTable dt in ds.Tables)
                    {

                        if (tableAliasMapping.ContainsKey(dt.TableName))
                        {
                            dt.TableName = tableAliasMapping[dt.TableName];
                        }
                    }

                    ManageFetchHistory(Convert.ToInt32(userId));

                    response.code = 200;
                    //response.data = _miscDataSetting.ConvertToJSON(ds.Tables[0]);

                    response.data = JsonConvert.SerializeObject(ds);  //in this way, in the response, the table name as 'Layouts and the key name as 'Table' is removed; now the response is a string starting from array of objects without table name.
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
                // _logger.LogError("GetGroup", ex);
            }

            return await Task.FromResult(response);
        }


        public async Task<ResponseModel> Get_API_2_Upload_Data(string? userId, DataSet ds1)
        {
            ResponseModel response = new ResponseModel();
            string flag = userId == null || userId == "" ? "G" : "I";
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();



                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@userId", userId == "" ? "0" : userId, "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");

                ds = SP.RunStoredProcedure(connStr, ds, "sp_API_2_UploadData", arrList);

                //if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["LayoutID"]?.ToString()))
                if (ds.Tables[0].Rows.Count > 0 && ds.Tables != null)
                {
                    //ds.Tables[0].TableName = "Layouts";

                    ManageFetchHistory(Convert.ToInt32(userId));

                    response.code = 200;
                    //response.data = _miscDataSetting.ConvertToJSON(ds.Tables[0]);

                    response.data = JsonConvert.SerializeObject(ds);
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
                // _logger.LogError("GetGroup", ex);
            }

            return await Task.FromResult(response);
        }

        private static async void ManageFetchHistory(int userId)
        {

            string connStr = MrQuoteResources.GetConnectionString();
            ResponseModel response = new ResponseModel();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_IOSFetchHistory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Adding parameters
                        command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
                        command.Parameters.Add(new SqlParameter("@ret", SqlDbType.TinyInt) { Direction = ParameterDirection.Output });
                        command.Parameters.Add(new SqlParameter("@errorCode", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output });

                        await command.ExecuteNonQueryAsync();

                        // Retrieving output values
                        response.code = Convert.ToInt32(command.Parameters["@ret"].Value);
                        response.msg = command.Parameters["@errorCode"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = -1;
                response.data = ex.Message;
                // _logger.LogError("SetVerificationCode", ex);
            }
        }

    }
}
