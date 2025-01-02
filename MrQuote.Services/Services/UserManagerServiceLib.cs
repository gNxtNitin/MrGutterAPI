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
using UserManagementLibrary;

namespace MrQuote.Services.Services
{
    public class UserManagerServiceLib : IUserManagerServiceLib
    {
        public IConfiguration _configuration;
        ILogger _logger;
        IMiscDataSetting _miscDataSetting;
        IUMSService _UMSEmailService;
        private readonly IUserManagerService _userManagerService;
        EncDcService encDcService = new EncDcService();
        public UserManagerServiceLib(IConfiguration configuration, IMiscDataSetting miscDataSetting, IUMSService UMSEmailService, IUserManagerService userManagerService)
        {
            _configuration = configuration;
            _miscDataSetting = miscDataSetting;
            _UMSEmailService = UMSEmailService;
            _userManagerService = userManagerService;
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
                // _logger.LogError("GetGroup", ex);
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
        public async Task<ResponseModel> GetUsers(string? encReq)
        {
            ResponseModel response = new ResponseModel();
            string userId = await encDcService.Decrypt(encReq);
            try
            {
                // calling the usermanagement library to get the users
                var result = await _userManagerService.GetUsers(userId);
                response.code = result.code;
                response.msg=result.msg;
                response.data = result.data;
            }
            catch (Exception ex)
            {
                response.code = -3;
                response.msg = ex.Message;
                // _logger.LogError("GetGroup", ex);
            }

            return await Task.FromResult(response);
        }
        public async Task<ResponseModel> CreateOrSetUser(RequestModel req, char flag)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                string json = await encDcService.Decrypt(req.V);
                UserMasterReqModel rq = JsonConvert.DeserializeObject<UserMasterReqModel>(json);
                rq.Password = await encDcService.Encrypt(rq.Password);

                //map the api usermanagement req model with library usermanagement req model
                var userReqModel = new UserManagementLibrary.Models.UserMasterReqModel();
                userReqModel.UserId = rq.UserId.ToString();
                userReqModel.FirstName = rq.FirstName;
                userReqModel.LastName = rq.LastName;
                userReqModel.Mobile = rq.MobileNo;
                userReqModel.Email = rq.EmailID;
                 userReqModel.DOB = "30/08/2000";
                userReqModel.Password = rq.Password;
                userReqModel.FilePath = rq.FilePath;
                userReqModel.CreatedBy = rq.CreatedBy;

                var result = await _userManagerService.CreateUserMaster(userReqModel);
                response.code = result.code;
                response.msg = result.msg;
                response.data = result.data;
            }
            catch (Exception ex)
            {
                response.code = -1;
                response.data = ex.Message;
                // _logger.LogError("SendForgotPasswordEmail", ex);
            }
            return response;
        }
        public async Task<ResponseModel> DeleteUserMaster(string encUserId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string userId = await encDcService.Decrypt(encUserId);
                var result = await _userManagerService.DeleteUserMaster(Convert.ToInt32(userId));

                response.code = result.code;
                response.msg = result.msg;
                response.data = result.data;
            }
            catch (Exception ex)
            {
                response.code = -1;
                response.data = ex.Message;
                // _logger.LogError("SendForgotPasswordEmail", ex);
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
    }
}
