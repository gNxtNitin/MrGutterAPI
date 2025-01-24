using DatabaseManager;
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
                        command.Parameters.Add(new SqlParameter("@EstimatorID", SqlDbType.Int)
                        {
                            Value = (object)req.UserID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@StatusID", SqlDbType.Int)
                        {
                            Value = (object)req.StatusID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@EstimateNo", SqlDbType.NVarChar, 150)
                        {
                            Value = (object)req.EstimateNo ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@EstimateCreatedDate", SqlDbType.SmallDateTime)
                        {
                            Value = (object)req.EstimateCreatedDate ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@EstimateRevenue", SqlDbType.Decimal)
                        {
                            Value = (object)req.EstimateRevenue ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@NextCallDate", SqlDbType.SmallDateTime)
                        {
                            Value = (object)req.NextCallDate ?? DBNull.Value
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
        public async Task<ResponseModel> CreateOrSetMeasurementCat(MeasurementCatReqModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetMeasurementCat", connection))
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
                        command.Parameters.Add(new SqlParameter("@MCatID", SqlDbType.Int)
                        {
                            Value = (object)req.MCatID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@CategoryName", SqlDbType.NVarChar, 50)
                        {
                            Value = (object)req.CategoryName ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@OrderNo", SqlDbType.Int)
                        {
                            Value = (object)req.OrderNo ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit)
                        {
                            Value = (object)req.IsActive ?? DBNull.Value
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
        public async Task<ResponseModel> CreateOrSetMeasurementToken(MeasurementTokenReqModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteMeasurementToken", connection))
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
                        command.Parameters.Add(new SqlParameter("@EstimateId", SqlDbType.Int)
                        {
                            Value = (object)req.EstimateID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@MTokenID", SqlDbType.Int)
                        {
                            Value = (object)req.MTokenID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@MCatID", SqlDbType.Int)
                        {
                            Value = (object)req.MCatID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@UMID", SqlDbType.Int)
                        {
                            Value = (object)req.UmID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@TokenName", SqlDbType.NVarChar, 50)
                        {
                            Value = (object)req.TokenName ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@TokenValue", SqlDbType.Decimal)
                        {
                            Value = (object)req.TokenValue ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@OrderNo", SqlDbType.Int)
                        {
                            Value = (object)req.OrderNo ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit)
                        {
                            Value = (object)req.IsActive ?? DBNull.Value
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
            string flag = (req.CompanyID == 0 && req.UserId == 0 && req.EstimateID == 0) ? "A" : "I";
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
        public async Task<ResponseModel> GetMeasurementCat(int mCatId, int companyId)
        {
            ResponseModel response = new ResponseModel();
            string flag = mCatId == 0 ? "G" : "I";
            if (mCatId == 0 && companyId != 0)
            {
                flag = "A";
            }
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@MCatID", mCatId.ToString() == "" ? "0" : mCatId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@CompanyID", companyId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetMeasurementCat", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["MCatID"]?.ToString()))
                {
                    ds.Tables[0].TableName = "MeasurementCat";
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
        public async Task<ResponseModel> UnitOfMeasurement(int uMId, int companyId)
        {
            ResponseModel response = new ResponseModel();
            string flag = uMId == 0 ? "G" : "I";
            if (uMId == 0 && companyId != 0)
            {
                flag = "A";
            }
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@UMID", uMId.ToString() == "" ? "0" : uMId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@CompanyID", companyId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetDeleteUnitOfMeasurement", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["UMID"]?.ToString()))
                {
                    ds.Tables[0].TableName = "MeasurementUnit";
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
        public async Task<ResponseModel> GetMeasurementToken(int estimateId, int companyId, int mTokenId)
        {
            ResponseModel response = new ResponseModel();
            string flag = estimateId == 0 && mTokenId==0 ? "G" : "I";
            if (estimateId == 0 && mTokenId == 0 && companyId != 0)
            {
                flag = "A";
            }
            try
            {
                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Flag", flag, "CHAR", "I");
                SP.spArgumentsCollection(arrList, "@MTokenID", mTokenId.ToString() == "" ? "0" : mTokenId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@CompanyID", companyId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@EstimateId", estimateId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetSetDeleteMeasurementToken", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["MTokenID"]?.ToString()))
                {
                    ds.Tables[0].TableName = "MeasurementToken";
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
        public async Task<ResponseModel> CreateOrSetMeasurementTokenValue(MeasurementTokenReqModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetSetDeleteMeasurementToken", connection))
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
                        command.Parameters.Add(new SqlParameter("@MTokenID", SqlDbType.Int)
                        {
                            Value = (object)req.MTokenID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@MCatID", SqlDbType.Int)
                        {
                            Value = (object)req.MCatID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@UMID", SqlDbType.Int)
                        {
                            Value = (object)req.UmID ?? 0
                        });
                        command.Parameters.Add(new SqlParameter("@TokenName", SqlDbType.NVarChar, 50)
                        {
                            Value = (object)req.TokenName ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@OrderNo", SqlDbType.Int)
                        {
                            Value = (object)req.OrderNo ?? DBNull.Value
                        });
                        command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit)
                        {
                            Value = (object)req.IsActive ?? DBNull.Value
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
