using MrQuote.Domain;
using MrQuote.Domain.Models;
using MrQuote.Domain.Models.RequestModel;
using MrQuote.Services.IServices;
using MrQuote.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections;
using System.Data;
using System.Data.SqlClient;



namespace MrQuote.Services.Services
{
    public class AccountService : IAccountService
    {
        public IConfiguration _configuration;
        ILogger _logger;
        IMiscDataSetting _miscDataSetting;
        IUMSService _emailService;
        IJwtAuthService _jwtAuthService;
        EncDcService encDcService = new EncDcService();
        public AccountService(IConfiguration configuration, IJwtAuthService jwtAuthService, IMiscDataSetting miscDataSetting, IUMSService emailService)
        {
            _configuration = configuration;
            _miscDataSetting = miscDataSetting;
            _emailService = emailService;
            _jwtAuthService = jwtAuthService;
        }
        public async Task<ResponseModel> AuthenticateUser(LoginReqModel rq)
        {
            ResponseModel response = new ResponseModel();
            EncDcService encDcService = new EncDcService();
            try
            {
                // string json = await encDcService.Decrypt(encReq.V);
                // LoginReqModel rq = JsonConvert.DeserializeObject<LoginReqModel>(json);
                rq.Password = await encDcService.Encrypt(rq.Password);
                // rq.Password = await encDcService.Encrypt(rq.Password);

                DataSet ds = new DataSet();
                string connStr = MrQuoteResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@MobileOrEmailId", rq.MobileOrEmail, "VARCHAR", "I");

                SP.spArgumentsCollection(arrList, "@Password", rq.Password, "VARCHAR", "I");
                SP.spArgumentsCollection(arrList, "@Ret", "", "INT", "O");
                SP.spArgumentsCollection(arrList, "@ErrorMsg", "", "VARCHAR", "O");

                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetAuthenticatedUser", arrList);
                if (ds.Tables[0].Rows.Count > 0 && !string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["UserID"]?.ToString()))
                {
                    //If user requested with password
                    if (Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]) != 0)
                    {
                        string userRole = ds.Tables[0].Rows[0]["RoleName"].ToString();
                        string jwtToken = await _jwtAuthService.GenerateJwtToken(rq.MobileOrEmail, userRole);
                        //Insert into login history
                        ManageLoginHistroy(Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]));
                        response.code = Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]);
                        response.data = jwtToken;
                        response.msg = "Success";
                    }
                    else
                    {
                        response.code = -1;
                        //response.data = ds.Tables[0].Rows[0]["UserID"].ToString();
                        response.msg = "Username Or Password Incorrect.";
                    }
                }
                else
                {
                    response.code = -2;
                    response.msg = "Invalid User";
                }
            }
            catch (Exception ex)
            {
                response.code = -3;
                response.msg = ex.Message;
                //_logger.LogError("GetUser", ex);
            }
            return response;
        }
        public async Task<ResponseModel> SetOTP(RequestModel encReq)
        {
            Random r = new Random();
            int randNum = r.Next(10000);
            // string verificationCode = randNum.ToString("D4");
            string verificationCode = "1111";

            string json = await encDcService.Decrypt(encReq.V);
            UserMasterReqModel usermodel = JsonConvert.DeserializeObject<UserMasterReqModel>(json);
            RequestModel req = new RequestModel();
            req.MobileOrEmail = usermodel.EmailID;
            string connStr = MrQuoteResources.GetConnectionString();
            ResponseModel response = new ResponseModel();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_SetOTP";
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter parameter = new SqlParameter(); ;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@MobileOrEmail", SqlDbType.VarChar);
                        parameter.Value = req.MobileOrEmail;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@VerificationCode", SqlDbType.VarChar);
                        parameter.Value = verificationCode;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@IsResendCode", SqlDbType.TinyInt);
                        parameter.Value = req.IsResendCode;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@ret", SqlDbType.Int);
                        parameter.Direction = ParameterDirection.Output;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@errormsg", SqlDbType.VarChar, 200);
                        parameter.Direction = ParameterDirection.Output;

                        int res = await command.ExecuteNonQueryAsync();
                        response.code = Convert.ToInt32(command.Parameters["@ret"].Value);
                        response.msg = command.Parameters["@errormsg"].Value.ToString();
                        response.data = verificationCode;
                        if (response.code > 0)
                        {
                            string encverificationcode = await encDcService.Encrypt(verificationCode);
                            string encemail = await encDcService.Encrypt(req.MobileOrEmail);

                            var url = MrQuoteResources.configuration.GetSection("hosting:baseurl").Value;
                            //string verificationurl = $"<a href='{url+"Account/ValidateOtp?Otp="+ encverificationcode}'>Please verify your email</a>";
                            string verificationurl = $"<a href='{url + "Account/ValidateOtp?Otp=" + encverificationcode + "&email=" + req.MobileOrEmail}'>Please verify your email</a>";
                            System.IO.StreamReader file = new System.IO.StreamReader("VerificationCode.html");
                            string _body = await file.ReadToEndAsync();
                            file.Close();
                            _body = _body.Replace("$EmailId", req.MobileOrEmail).Replace("$Verificationurl", verificationurl);
                            await _emailService.QueueEmail(req.MobileOrEmail, "Verification Code", _body, Guid.NewGuid().ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = -1;
                response.data = ex.Message;
                // _logger.LogError("SetVerificationCode", ex);
            }
            return response;
        }
        public async Task<ResponseModel> ValidateOTP(RequestModel encReq)
        {
            string verificationCode = "";
            string userRole = "A";
            string json = await encDcService.Decrypt(encReq.V);
            RequestModel req = JsonConvert.DeserializeObject<RequestModel>(json);

            string connStr = MrQuoteResources.GetConnectionString();
            ResponseModel response = new ResponseModel();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_ValidateOTP";
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter parameter = new SqlParameter();

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@MobileOrEmail", SqlDbType.VarChar);
                        parameter.Value = req.MobileOrEmail;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@OTP", SqlDbType.VarChar, 10);
                        parameter.Direction = ParameterDirection.Output;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@Ret", SqlDbType.Int);
                        parameter.Direction = ParameterDirection.Output;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@ErrorMsg", SqlDbType.VarChar, 200);
                        parameter.Direction = ParameterDirection.Output;

                        //parameter = new SqlParameter();
                        //parameter = command.Parameters.Add("@UserRole", SqlDbType.VarChar, 20);
                        //parameter.Direction = ParameterDirection.Output;

                        int res = await command.ExecuteNonQueryAsync();
                        //userRole = command.Parameters["@UserRole"].Value.ToString();
                        response.code = Convert.ToInt32(command.Parameters["@Ret"].Value);
                        response.msg = command.Parameters["@ErrorMsg"].Value.ToString();
                        verificationCode = command.Parameters["@OTP"].Value.ToString();


                        if (response.code > 0)
                        {
                            if (verificationCode == req.VerificationCode)
                            {
                                // Generate JWT token
                                // string username = req.MobileOrEmail;
                                //Insert into login history
                                string jwtToken = await _jwtAuthService.GenerateJwtToken(req.MobileOrEmail, userRole);

                                ManageLoginHistroy(response.code);
                                response.data = jwtToken;
                                response.msg = "Success";
                            }
                            else
                            {
                                response.code = -2;
                                response.msg = "OTP mismatched";
                            }
                        }
                        else
                        {
                            response.code = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = -1;
                response.data = ex.Message;
                //_logger.LogError("ValidateVerificationCode", ex);
            }
            return response;
        }

        public async Task<ResponseModel> SendForgotPasswordEmail(RequestModel encReq, string? userType)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                // Decrypt the request
                string json = await encDcService.Decrypt(encReq.V);
                RequestModel req = JsonConvert.DeserializeObject<RequestModel>(json);

                // Check if the email exists in the userMaster table
                string connStr = MrQuoteResources.GetConnectionString();
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("sp_GetAuthenticatedUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MobileOrEmailId", req.MobileOrEmail);
                        command.Parameters.AddWithValue("@Flag", "F");
                        command.Parameters.AddWithValue("@ErrorMsg", "");
                        command.Parameters.AddWithValue("@Ret", "");

                        int userCount = (int)await command.ExecuteScalarAsync();
                        if (userCount == 0)
                        {
                            response.code = -1;
                            response.msg = "Email not found.";
                            return response;
                        }
                    }
                }

                //// If email exists, proceed with sending the reset password email
                //System.IO.StreamReader file = new System.IO.StreamReader("ResetPassword.html");
                //string _body = await file.ReadToEndAsync();
                //file.Close();
                //string email = await encDcService.Encrypt(req.MobileOrEmail);

                //// To manage the email body as per users
                //string hostingURL = "https://localhost:7048/";
                //_body = _body.Replace("$code", email).Replace("$hostingURL", hostingURL);

                //// To send the forgot password link
                //await _emailService.QueueEmail(req.MobileOrEmail, "Reset password", _body, Guid.NewGuid().ToString());


              //  string encemail = await encDcService.Encrypt(req.MobileOrEmail);
                var url = MrQuoteResources.configuration.GetSection("hosting:baseurl").Value;
                string resetPasswordUrl = $"<a href='{url + "Account/ChangePassword?email=" + req.MobileOrEmail}'>SET PASSWORD</a>";
                System.IO.StreamReader file = new System.IO.StreamReader("ResetPassword.html");
                string _body = await file.ReadToEndAsync();
                file.Close();
                _body = _body.Replace("$EmailId", req.MobileOrEmail).Replace("$ResetPasswordurl", resetPasswordUrl);
                await _emailService.QueueEmail(req.MobileOrEmail, "Password Reset", _body, Guid.NewGuid().ToString());
                response.code = 1;
                response.msg = "Password reset link sent.";


            }
            catch (Exception ex)
            {
                response.code = -1;
                response.msg = "Forgot password link not sent.";
                response.data = ex.Message;
            }
            return response;
        }
        public async Task<ResponseModel> ResetPassword(RequestModel req)
        {
            ResponseModel response = new ResponseModel();
            string connStr = MrQuoteResources.GetConnectionString();
            try
            {
                //string json = await encDcService.Decrypt(req.V);
               // req = JsonConvert.DeserializeObject<RequestModel>(json);
                string encPassword = await encDcService.Encrypt(req.Password);
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_ResetPassword";
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter parameter = new SqlParameter();

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@MobileOrEmailId", SqlDbType.VarChar);
                        parameter.Value = req.MobileOrEmail;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@Password", SqlDbType.NVarChar);
                        parameter.Value = encPassword;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@Ret", SqlDbType.Int);
                        parameter.Direction = ParameterDirection.Output;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@ErrorCode", SqlDbType.VarChar, 200);
                        parameter.Direction = ParameterDirection.Output;

                        int res = await command.ExecuteNonQueryAsync();
                        response.code = Convert.ToInt32(command.Parameters["@Ret"].Value);
                        response.msg = command.Parameters["@ErrorCode"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                response.code = -1;
                response.msg = "Reset password unsuccessful.";
                response.data = ex.Message;
                // _logger.LogError("SendForgotPasswordEmail", ex);
            }
            return response;
        }
        private static async void ManageLoginHistroy(int userId)
        {
            string connStr = MrQuoteResources.GetConnectionString();
            ResponseModel response = new ResponseModel();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_InsertLoginHistory";
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter parameter = new SqlParameter();

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@userId", SqlDbType.Int);
                        parameter.Value = userId;
                        parameter.Direction = ParameterDirection.Input;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@ret", SqlDbType.TinyInt);
                        parameter.Direction = ParameterDirection.Output;

                        parameter = new SqlParameter();
                        parameter = command.Parameters.Add("@errorCode", SqlDbType.VarChar, 200);
                        parameter.Direction = ParameterDirection.Output;

                        int res = await command.ExecuteNonQueryAsync();
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
