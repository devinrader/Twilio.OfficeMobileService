using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Profile;

namespace Twilio.OfficeMobileService.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ConfigurationModel
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Your Mobile Number")]
        public string PhoneNumber { get; set; }

        //Twilio Account SID
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Account SID")]
        public string AccountSid { get; set; }

        //Twilio Auth Token
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Auth Token")]
        public string AuthToken { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Send replies via email")]
        public bool ReplyToMailbox { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Server Name")]
        public string SmtpServerName { get; set; }

        [Display(Name = "Port")]
        public int? SmtpServerPort { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string SmtpUserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string SmtpPassword { get; set; }

        [Display(Name = "Use SSL")]
        public bool UseSmtpSsl { get; set; }

        public static ConfigurationModel Initialize(ProfileBase profile)
        {
            ConfigurationModel model = new ConfigurationModel();
            model.PhoneNumber = (string)profile.GetPropertyValue("PhoneNumber");

            model.ReplyToMailbox = (bool)profile.GetPropertyValue("ReplyToMailbox");
            model.SmtpServerName = (string)profile.GetPropertyValue("SmtpServerName");
            model.SmtpServerPort = (int?)profile.GetPropertyValue("SmtpServerPort");
            model.SmtpUserName = (string)profile.GetPropertyValue("SmtpUserName");
            model.SmtpPassword = (string)profile.GetPropertyValue("SmtpPassword");
            model.UseSmtpSsl = (bool)profile.GetPropertyValue("UseSmtpSsl");

            model.AccountSid = (string)profile.GetPropertyValue("AccountSid");
            model.AuthToken = (string)profile.GetPropertyValue("AuthToken");

            return model;
        }
    }
}
