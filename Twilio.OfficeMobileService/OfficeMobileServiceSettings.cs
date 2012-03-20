using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Twilio.OfficeMobileService
{
    public class OfficeMobileServiceSettings : ConfigurationSection
    {
        private static OfficeMobileServiceSettings settings = ConfigurationManager.GetSection("OfficeMobileServiceSettings") as OfficeMobileServiceSettings;

        public static OfficeMobileServiceSettings Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty("serviceProviderName")]
        public string ServiceProviderName {
            get { return (string)this["serviceProviderName"]; }
            set { this["serviceProviderName"] = value; }
        }

        [ConfigurationProperty("officeMobileServiceUri")]
        public string OfficeMobileServiceUri
        {
            get { return (string)this["officeMobileServiceUri"]; }
            set { this["officeMobileServiceUri"] = value; }
        }

        [ConfigurationProperty("serviceProviderSignupUri")]
        public string ServiceProviderSignupUri
        {
            get { return (string)this["serviceProviderSignupUri"]; }
            set { this["serviceProviderSignupUri"] = value; }
        }

        [ConfigurationProperty("localLanguageServiceName")]
        public string LocalLanguageServiceName
        {
            get { return (string)this["localLanguageServiceName"]; }
            set { this["localLanguageServiceName"] = value; }
        }

        [ConfigurationProperty("targetLocale")]
        public string TargetLocale
        {
            get { return (string)this["targetLocale"]; }
            set { this["targetLocale"] = value; }
        }

        [ConfigurationProperty("serviceName")]
        public string ServiceName
        {
            get { return (string)this["serviceName"]; }
            set { this["serviceName"] = value; }
        }

        [ConfigurationProperty("authenticationType")]
        public string AuthenticationType
        {
            get { return (string)this["authenticationType"]; }
            set { this["authenticationType"] = value; }
        }
        
        [ConfigurationProperty("maxRecipientsPerMessage")]
        public string MaxRecipientsPerMessage
        {
            get { return (string)this["maxRecipientsPerMessage"]; }
            set { this["maxRecipientsPerMessage"] = value; }
        }
        
        [ConfigurationProperty("maxMessagesPerSend")]
        public string MaxMessagesPerSend
        {
            get { return (string)this["maxMessagesPerSend"]; }
            set { this["maxMessagesPerSend"] = value; }
        }
        
        [ConfigurationProperty("maxSbcsPerMessage")]
        public string MaxSbcsPerMessage
        {
            get { return (string)this["maxSbcsPerMessage"]; }
            set { this["maxSbcsPerMessage"] = value; }
        }
        
        [ConfigurationProperty("maxDbcsPerMessage")]
        public string MaxDbcsPerMessage
        {
            get { return (string)this["maxDbcsPerMessage"]; }
            set { this["maxDbcsPerMessage"] = value; }
        }
    }
}