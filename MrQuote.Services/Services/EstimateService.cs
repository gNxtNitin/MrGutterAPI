﻿using DatabaseManager;
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
    public class EstimateService : IEstimateService
    {
        IMiscDataSetting _miscDataSetting;
        public EstimateService(IMiscDataSetting miscDataSetting)
        {
                _miscDataSetting = miscDataSetting;
        }
        public async Task<ResponseModel> CreateOrSetEstimate(EstimateReqModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteEstimate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Flag", SqlDbType.Char)
                        {
                            Value = (object)req.Flag ?? "G"
                        });
                        command.Parameters.Add(new SqlParameter("@EstimateID", SqlDbType.Int)
                        {
                            Value = (object)req.EstimateID ?? 0
                        });
                       
                        command.Parameters.Add(new SqlParameter("@CompanyID", SqlDbType.Int)
                        {
                            Value = (object)req.CompanyID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@StatusID", SqlDbType.Int)
                        {
                            Value = (object)req.StatusID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@EstimateNo", SqlDbType.NVarChar, 150)
                        {
                            Value = (object)req.EstimateNo ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 150)
                        {
                            Value = (object)req.FirstName ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 150)
                        {
                            Value = (object)req.LastName ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@CompanyName", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.Company ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.PhoneNo ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.Email ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@AddressLine1", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.Addressline1 ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@AddressLine2", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.Addressline2 ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.City ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@State", SqlDbType.NVarChar, 255)
                        {
                            Value = (object)req.State ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@ZipCode", SqlDbType.NVarChar, 10)
                        {
                            Value = (object)req.ZipCode ?? DBNull.Value
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

        public async Task<ResponseModel> GetEstimate(EstimateQueryParameters req)
        {
            ResponseModel response = new ResponseModel();
            //string flag = req.CompanyID == 0 || req.UserId == 0 ? "A" : "I";
            string flag = (req.CompanyID == 0 && req.UserId == 0 && req.EstimateID==0) ? "A" : "I";
            if (req.EstimateID != 0)
            {
                flag = "E";
            }
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@UserId", req.UserId.ToString() == "" ? "0" : req.UserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@CompanyId", req.CompanyID.ToString() == "" ? "0" : req.CompanyID.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@EstimateID", req.EstimateID.ToString() == "" ? "0" : req.EstimateID.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetEstimateList", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["EstimateID"]?.ToString()))
                {
                    ds.Tables[0].TableName = "EstimateList";
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
        public async Task<ResponseModel> GetStatus(string? statusId)
        {
            ResponseModel response = new ResponseModel();
            string flag = statusId == null || statusId == "" ? "G" : "I";
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@StatusId", statusId == "" ? "0" : statusId, "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetEstimateStatus", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["StatusID"]?.ToString()))
                {
                    ds.Tables[0].TableName = "StatusList";
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
    }
}
