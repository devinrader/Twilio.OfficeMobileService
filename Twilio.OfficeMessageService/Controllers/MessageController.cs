using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Profile;
using System.Text.RegularExpressions;

namespace Twilio.OfficeMobileService.Controllers
{
    public class MessageController : Controller
    {
        private string NormalizePhone(string phone)
        {
            phone = Regex.Replace(phone, "[^0-9]", "");
            if ( (phone.Length==11) && (phone.StartsWith("1")) ){
                phone = phone.Substring(1);
            }

            return phone;

        }
        public ActionResult Inbound(string From, string To, string Body)
        {
            //Find a user whose phone number patches the "To" value
            var users = Membership.GetAllUsers().Cast<MembershipUser>();
            
            MembershipUser user = null;
            ProfileBase profile = null;

            foreach (var u in users)
            {
                profile = ProfileBase.Create(u.UserName, true);
                string phone = (string)profile["PhoneNumber"];

                if (string.IsNullOrEmpty(phone))
                {
                    continue; 
                }

                phone = NormalizePhone(phone);
                To = NormalizePhone(To);

                if ( phone == To)
                {
                    user = u;
                    break;
                }
            }

            if (user == null)
                throw new HttpException(500, "No matching user found");

            //TODO: handle user not found issues, log to ELMAH


                if (!(bool)profile.GetPropertyValue("ReplyToMailbox"))
                {
                    //var client = new TwilioRestClient((string)profile["AccountSid"], (string)profile["AuthToken"]);
                    //client.SendSmsMessage("", "", "");
                }
                else
                {
                    //forward the message to their mail account
                    MailMessage msg = new MailMessage();
                    msg.Headers.Add("Content-class", "MS-OMS-SMS");
                    msg.Headers.Add("X-MS-Reply-To-Mobile", From);
                    msg.To.Add(user.Email);
                    msg.From = new MailAddress(user.Email);
                    msg.Headers.Add("Content-Type", "text/plain");
                    msg.Headers.Add("Content-Transfer-Encoding", "quoted-printable");
                    msg.IsBodyHtml = false;
                    msg.Body = Body;

                    //TODO: Handle SMTP Server send failures, log to ELMAH
                    SmtpClient client = new SmtpClient((string)profile.GetPropertyValue("SmtpServerName"), (int)profile.GetPropertyValue("SmtpServerPort"));
                    client.EnableSsl = (bool)profile.GetPropertyValue("UseSmtpSsl");
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential((string)profile.GetPropertyValue("SmtpUserName"), (string)profile.GetPropertyValue("SmtpPassword"));
                    client.Send(msg);
                
            }

            return new EmptyResult();
        }
    }
}