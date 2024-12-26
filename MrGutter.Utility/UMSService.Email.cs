using System.ComponentModel;
using System.Data;
using System.Net.Mail;
using System.Net;
using MrGutter.Domain;

namespace MrGutter.Utility
{
    public class UmSService : IUMSService
    {
        private readonly IMiscDataSetting _miscDataSetting;
        public UmSService(IMiscDataSetting miscDataSetting)
        {
            _miscDataSetting = miscDataSetting;
        }
        static bool mailSent = false;
        public async Task QueueEmail(string toEmailIds, string subject, string body, string token, int isHTML = 1)
        {
            body = isHTML == 1 ? WebUtility.HtmlEncode(body) : body;
            string str = $"INSERT INTO EmailNotification(Token,ToEmailIds,EmailSubject,EmailBody,HasAttachment,IsSent,CreatedDate,Status,IsHTML) VALUES('{token}','{toEmailIds}','{subject}','{body}',0,0,GETUTCDATE(),'A',{isHTML})";
            await _miscDataSetting.ExecuteNonQuery(str);
            await ClearEmailQueue();
        }
        public async Task ClearEmailQueue()
        {
            string? toEmailIds, subject, body, token = "";
            Boolean isHTML = false;
            string str = "Select ToEmailIds,EmailSubject,EmailBody,Token,IsHTML FROM EmailNotification WHERE IsSent = 0 AND Status='A' AND Len(Isnull(ToEmailIds,'')) > 0";
            DataSet ds = await _miscDataSetting.GetDataSet(str);
            if (ds != null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    toEmailIds = row["ToEmailIds"].ToString();
                    subject = row["EmailSubject"].ToString();
                    body = row["EmailBody"].ToString();
                    token = row["Token"].ToString();
                    isHTML = Convert.ToBoolean(row["IsHTML"].ToString());
                    body = isHTML ? WebUtility.HtmlDecode(body) : body;
                     await SendEmail(toEmailIds, subject, body, token);
                }
            }
        }
        public async Task SendEmail(string toEmailIds, string subject, string body, string token = "")
        {
            if (UMSResources.configuration.GetSection("ByPass:sendMail").Value == "Y")
            {
                string smtpServerAddress = UMSResources.configuration.GetSection("smtp:smtpServerAddress").Value;
                int smtpServerPort = Convert.ToInt32(UMSResources.configuration.GetSection("smtp:smtpServerPort").Value);
                string smtpServerUserId = UMSResources.configuration.GetSection("smtp:smtpServerUserId").Value;
                string smtpServerPassword = UMSResources.configuration.GetSection("smtp:smtpServerPassword").Value;
                string emailFromAddress = UMSResources.configuration.GetSection("smtp:emailFromAddress").Value;
                string displayName = UMSResources.configuration.GetSection("smtp:displayName").Value;
                try
                {
                    MailMessage mail = new MailMessage
                    {
                        From = new MailAddress(emailFromAddress, displayName)
                    };
                    if (toEmailIds.Contains(";"))
                    {
                        foreach (string item in toEmailIds.Split(';'))
                        {
                            mail.To.Add(item);
                        }
                    }
                    else
                    {
                        mail.To.Add(toEmailIds);
                    }

                    mail.Subject = $"{subject} for Date:{DateTime.Now.ToShortDateString()}";
                    //mail.Body = $"Hi <br>This is just a Test Email from .net.<br>No further action required.<br>{Environment.NewLine}<br>gNxt Systems, Thanks";
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(smtpServerAddress, smtpServerPort);
                    smtp.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(smtpServerUserId, smtpServerPassword);
                    string userState = token;
                    smtp.SendAsync(mail, token);
                    //Console.WriteLine($"\tEmail notification process finished at {DateTime.UtcNow}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
        }
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            string token = (string)e.UserState;
            string status = "";
            int isSent = 0;
            if (e.Cancelled)
            {
                isSent = 1;
                status = "C";
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                isSent = 1;
                status = "F";
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                isSent = 1;
                status = "S";
                Console.WriteLine("Message sent.");
            }
            string str = $"UPDATE EmailNotification SET IsSent = {isSent},SentDate=GETUTCDATE(),Status='{status}' WHERE Token = '{token}'";
            //_miscDataSetting.ExecuteNonQueryNonAsync(str);
            mailSent = true;
        }
    }

}
