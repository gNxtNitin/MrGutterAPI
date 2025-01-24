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
            string newRoleId = "0";
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@RoleId", newRoleId ?? "0", "INT", "I");
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
                    ds.Tables[0].TableName = "Users";
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
        public async Task<ResponseModel> CreateOrSetUser(UserMasterReqModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                string encPassword = req.Password ?? "";
                if (req.Password == null || req.Password=="")
                {
                    encPassword = await encDcService.Encrypt(req.FirstName + "@123");
                }
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
                            Value = (object)req.Mobile ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.Email ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@dob", SqlDbType.SmallDateTime)
                        {
                            Value = (object)req.DOB ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar, 255)
                        {                                            
                            Value = encPassword
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
                        command.Parameters.Add(new SqlParameter("@userStatus", SqlDbType.NVarChar, 15)
                        {
                            Value = (object)req.UserStatus ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@isActive", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.isActive ?? DBNull.Value
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

                if (req.RoleList.Count != 0 && req.RoleList != null)
                {
                    foreach (var item in req.RoleList)
                    {
                        item.UserId = response.code;
                        var res1 = CreateOrSetUserRole(item, req.CreatedBy);
                    }
                }
                if (req.CompanyList.Count != 0 && req.CompanyList != null)
                {
                    foreach (var item in req.CompanyList)
                    {
                        item.UserId = response.code;
                        var res1 = CreateOrSetUserCompany(item,req.CreatedBy);
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
        public async Task<ResponseModel> GetCompany(string? userId, string? companyId)
        {
            ResponseModel response = new ResponseModel();
            string flag = userId == null || userId == "" ? "G" : "I";
            if(companyId != null || companyId != "") { flag = "I"; }
            if(companyId == null && userId == null) { flag = "G"; }
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@UserID", userId == "" ? "0" : userId, "INT", "I");
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
                            Value = (object)req.CompanyId ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyName", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.CompanyName ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@PointOfContact", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.ContactPerson ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.CompanyEmail ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 25)
                        {
                            Value = (object)req.CompanyPhone ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.isActive ?? DBNull.Value
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

        public async Task<ResponseModel> CreateOrSetUserCompany(UserCompany userCompany, int createdBy)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteUserCompany", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Flag", SqlDbType.Char)
                        {
                            Value = "C"
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyID", SqlDbType.Int)
                        {
                            Value = (object)userCompany.CompanyId ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int)
                        {
                            Value = (object)userCompany.UserId ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit)
                        {
                            Value = (object)userCompany.IsActive ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int)
                        {
                            Value = (object) createdBy ?? 0
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
        public async Task<ResponseModel> CreateOrSetUserRole(UserRole userRole, int createdBy)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_AssignRole", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Flag", SqlDbType.Char)
                        {
                            Value = "C"
                        });
                        command.Parameters.Add(new SqlParameter("@RoleId", SqlDbType.Int)
                        {
                            Value = (object)userRole.RoleId ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int)
                        {
                            Value = (object)userRole.UserId ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit)
                        {
                            Value = (object)userRole.IsActive ?? 0
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
