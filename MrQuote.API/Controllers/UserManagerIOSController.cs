using System.Net.Http.Headers;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrQuote.Domain.Models.RequestModel;
using MrQuote.Services.IServices;
using Newtonsoft.Json;
using MrQuote.Domain.Models;
using Azure;
using Newtonsoft.Json.Linq;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MrQuote.Services.Services;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace MrQuote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserManagerIOSController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserManager _userManager;
        private readonly ILogger<UserManagerIOSController> _logger;
        EncDcService encDcService = new EncDcService();

        public UserManagerIOSController(ILogger<UserManagerIOSController> logger, IUserManager userManager, IAccountService accountService)
        {
            _accountService = accountService;
            _logger = logger;
            _userManager = userManager;
        }

        private static readonly string Key = "0987654321!@#$%^"; // 16 characters for AES-128

        /*AES is configured with:
Key: 0987654321!@#$%^ (16 bytes for AES-128).
IV: A zero-filled array of 16 bytes.
Padding: PKCS7 (default in .NET for AES).
The plain text is encrypted, and the result is Base64-encoded for transmission.*/
        public static string AESEncrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = new byte[16]; // Initialization vector (IV) of zeros for simplicity

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        [HttpPost("AuthenticateAndGetData_IOS")]
        public async Task<IActionResult> AuthenticateAndGetData_IOS(LoginReqModel rq)
        {
            var authResponse = await _accountService.AuthenticateUser(rq);
            var token = authResponse.data;

            if (authResponse.msg == "Success")
            {

                var getUsersResponseWith = await _userManager.GetUsers(null);
                var getIntroPageResponse = await _userManager.GetIntroPages();


                var response = await _userManager.Get_API_0_Data(Convert.ToString(authResponse.code));

                DataSet ds2 = JsonConvert.DeserializeObject<DataSet>(response.data);

                Dictionary<string, string> userPasswordDict = new Dictionary<string, string>();

                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    userPasswordDict[row["userId"].ToString()] = row["passwordHash"].ToString();
                }


                foreach (var key in userPasswordDict.Keys.ToList())
                {
                    string encryptedHash = userPasswordDict[key];
                    string plainText = await encDcService.Decrypt(encryptedHash);
                    userPasswordDict[key] = plainText;
                }

                foreach (var key in userPasswordDict.Keys.ToList())
                {
                    string plainText = userPasswordDict[key];
                    string encryptedHash = AESEncrypt(plainText);
                    userPasswordDict[key] = encryptedHash;
                }


                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    string key = row["userId"].ToString();
                    if (userPasswordDict.ContainsKey(key))
                    {
                        row["passwordHash"] = userPasswordDict[key];
                    }
                }

                string finalResponse = JsonConvert.SerializeObject(ds2);


                //object obj1 = JsonConvert.DeserializeObject(getUsersResponse.data);
                //object obj2 = JsonConvert.DeserializeObject(getLayoutResponse.data);

                // object Obj = {UserDetails = getLayoutResponse.data };

                //getUsersResponse.data = JsonConvert.SerializeObject(getUsersResponse.data);
                //getLayoutResponse.data = JsonConvert.SerializeObject(getLayoutResponse.data);

                //getLayoutResponse.data = JsonConvert.SerializeObject(getLayoutResponse.data).Replace("\\", "");
                //getUsersResponse.data = JsonConvert.SerializeObject(getUsersResponse.data).Replace("\\", "");



                // var result = new { message = "Login successful", Token = token, userDetails = getUsersResponse.data, layoutDetails = getLayoutResponse.data };
                var result = new
                {
                    message = "Login successful",
                    Token = token,
                    DataResponse = finalResponse
                    //,IntroPageResponse = getIntroPageResponse.data,
                    //UResponse = getUsersResponseWith.data
                };

                return result == null ? NotFound() : Ok(result);
            }
            else return NotFound();
        }


        [Authorize]
        [HttpGet("RetrieveData_IOS")]
        public async Task<IActionResult> RetrieveData_IOS(string? userIdd)
        {

            //var getUsersResponseWith = await _userManager.GetUsers(null);
            //var getIntroPageResponse = await _userManager.GetIntroPages();


            var response = await _userManager.Get_API_1_Retrieve_Data(Convert.ToString(userIdd));

            //DataSet ds2 = JsonConvert.DeserializeObject<DataSet>(response.data);

            //string finalResponse = JsonConvert.SerializeObject(ds2);

            var result = new
            {
                Message = "Retrieved successful",
                DataResponse = response.data
            };



            return response.data == null ? NotFound() : Ok(result);

        }

        [Authorize]
        [HttpPost("UploadData_IOS")]
        public async Task<IActionResult> UploadData_IOS([FromQuery] string id, [FromBody] string Payload)
        {

            DataSet ds = JsonConvert.DeserializeObject<DataSet>(Payload);

            var response = await _userManager.Get_API_2_Upload_Data(Convert.ToString(id), ds);

            //DataSet ds2 = JsonConvert.DeserializeObject<DataSet>(response.data);

            //string finalResponse = JsonConvert.SerializeObject(ds2);

            var result = new
            {
                Message = "Uploaded successful",
                DataResponse = response.data
            };



            return response.data == null ? NotFound() : Ok(result);

        }
    }
}
