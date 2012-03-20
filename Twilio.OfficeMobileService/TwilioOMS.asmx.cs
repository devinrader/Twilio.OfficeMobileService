using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using System.Xml;
using Twilio;
using System.Web.Services.Protocols;
using System.Web.Security;
using System.Web.Profile;

namespace Twilio.OfficeMobileService
{    
    /// <summary>
    /// Summary description for Twilio
    /// </summary>
    [WebService(Namespace = "http://schemas.microsoft.com/office/Outlook/2006/OMS")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement)]
    [System.ComponentModel.ToolboxItem(false)]
    public class TwilioOMS : System.Web.Services.WebService
    {
        public const string OMSSERVICEINFONAMESPACE = "http://schemas.microsoft.com/office/Outlook/2006/OMS/serviceInfo";
        public const string OMSNAMESPACE = "http://schemas.microsoft.com/office/Outlook/2006/OMS";

        public  string SERVICEPROVIDER;

        public  string SERVICEURI;
        public  string SIGNUPPAGE;

        public  string TARGETLOCALE;
        public  string LOCALNAME;
        public  string ENGLISHNAME;
        public  string AUTHENTICATIONTYPE;

        public  string RECIPIENTSPERMESSAGE;
        public  string MESSAGESPERSEND;
        public  string SINGLEBYTECHARSPERMESSAGE;
        public  string DOUBLEBYTECHARSPERMESSAGE;

        public void InitializeSettings()
        {
                    
            SERVICEPROVIDER = OfficeMobileServiceSettings.Settings.ServiceProviderName;

            SERVICEURI = OfficeMobileServiceSettings.Settings.OfficeMobileServiceUri;
            SIGNUPPAGE = OfficeMobileServiceSettings.Settings.ServiceProviderSignupUri;

            TARGETLOCALE = OfficeMobileServiceSettings.Settings.TargetLocale;
            LOCALNAME = OfficeMobileServiceSettings.Settings.ServiceName;
            ENGLISHNAME = OfficeMobileServiceSettings.Settings.LocalLanguageServiceName;
            AUTHENTICATIONTYPE = OfficeMobileServiceSettings.Settings.AuthenticationType;

            RECIPIENTSPERMESSAGE = OfficeMobileServiceSettings.Settings.MaxRecipientsPerMessage;
            MESSAGESPERSEND = OfficeMobileServiceSettings.Settings.MaxMessagesPerSend;
            SINGLEBYTECHARSPERMESSAGE = OfficeMobileServiceSettings.Settings.MaxSbcsPerMessage;
            DOUBLEBYTECHARSPERMESSAGE = OfficeMobileServiceSettings.Settings.MaxDbcsPerMessage;

        }

        /// <summary>
        /// GetServiceInfo() returns an XML-formatted string called serviceInfo that contains basic properties of the OMS Web service, such as supported service types, parameters of supported services, and authentication types.
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetServiceInfo()
        {
            InitializeSettings();

            XNamespace ns = OMSSERVICEINFONAMESPACE;

            //TODO: Add code here that validates that the config properties exist with valid values

            XDocument doc = new XDocument(
             new XDeclaration("1.0", "utf-16", "true"),             
             new XElement(ns + "serviceInfo", 
                 new XElement(ns+"serviceProvider", SERVICEPROVIDER),
                 new XElement(ns+"serviceUri", SERVICEURI),
                 new XElement(ns+"signUpPage", SIGNUPPAGE),
                 new XElement(ns+"targetLocale", TARGETLOCALE),
                 new XElement(ns+"localName", LOCALNAME),
                 new XElement(ns+"englishName", ENGLISHNAME),
                 new XElement(ns+"authenticationType", AUTHENTICATIONTYPE),
                 new XElement(ns+"supportedService",
                     new XElement(ns+"SMS_SENDER",
                         new XAttribute("maxRecipientsPerMessage", RECIPIENTSPERMESSAGE),
                         new XAttribute("maxMessagesPerSend",MESSAGESPERSEND),
                         new XAttribute("maxSbcsPerMessage",SINGLEBYTECHARSPERMESSAGE),
                         new XAttribute("maxDbcsPerMessage", DOUBLEBYTECHARSPERMESSAGE)
                         )
                    )
                )
            );

            string result = doc.ToString();

            return result;
        }

        /// <summary>
        /// The OMS client built into Outlook retrieves a user’s mobile phone number and other information by calling GetUserInfo() with an xmsUser string as the parameter.
        /// </summary>
        /// <param name="xmsUser">xmsUser is an XML-formatted string that contains the user’s authentication information, including user ID and password</param>
        /// <returns>GetUserInfo() returns an XML-formatted string called userInfo that contains an error element with its severity attribute set to "failure".</returns>
        [WebMethod]
        public string GetUserInfo(string xmsUser)
        {
            InitializeSettings();

            XNamespace ns = OMSNAMESPACE;
            XDocument result = new XDocument(
                new XDeclaration("1.0", "utf-8", "true")
            );

            XElement info = new XElement(ns + "userInfo");
            result.Add(info);

            XDocument xmsUserDoc = XDocument.Parse(xmsUser);
            string userId = xmsUserDoc.Element(ns + "xmsUser").Element(ns + "userId").Value;
            string password = xmsUserDoc.Element(ns + "xmsUser").Element(ns + "password").Value;

            if (!Membership.ValidateUser(userId, password))
            {
                info.Add(
                    new XElement(ns + "error",
                        new XAttribute("code", "unregistered"),
                        new XAttribute("severity", "failure")
                    )
                );

                return result.ToString();
            }

            var profile = ProfileBase.Create(userId);

            //validate the user exists in twilio by passing the account sid and auth token to the API
            //var client = new TwilioRestClient( (string)profile.GetPropertyValue("AccountSid"), (string)profile.GetPropertyValue("AuthToken") );
            //var acct = client.GetAccount();
            //if ((acct.RestException!=null) && (acct.RestException.Status == "401"))
            //{ 
            //    info.Add(
            //        new XElement(ns + "error",
            //            new XAttribute("code", "unregistered"),
            //            new XAttribute("severity", "failure")
            //        )
            //    );

            //    return result.ToString();
            //}

            //TODO: Validate that passing back blank reply/smtp data is OK here since its included in later calls

            info.Add(
                new XElement(ns + "replyPhone", (string)profile.GetPropertyValue("PhoneNumber")),
                new XElement(ns + "smtpAddress", (string)profile.GetPropertyValue("SmtpServerName")),
                new XElement(ns + "error",
                    new XAttribute("code", "ok"),
                    new XAttribute("severity", "neutral")
                )
            );

            return result.ToString();
        }

        /// <summary>
        /// The OMS client that is built into Outlook or SharePoint calls DeliverXms() to deliver a mobile message to the service provider. The message content is packaged as an XML-formatted string called xmsData.
        /// </summary>
        /// <param name="xmsData">The xmsData string is designed to package either a text message or a multimedia message, which means that text messages and multimedia messages share the same schema. The following examples show an xmsData string that contains an MMS-formatted message and an xmsData string that contains an SMS-formatted message.</param>
        /// <returns>After the service provider has attempted to send the message, it returns an XML-formatted string called xmsResponse that contains one or more error elements that indicate the success or failure of the attempt to send the message to each intended recipient.</returns>
        [WebMethod]
        public string DeliverXms(string xmsData)
        {
            InitializeSettings();

            XNamespace ns = OMSNAMESPACE;
            XDocument result = new XDocument(
                new XDeclaration("1.0", "utf-8", "true")
            );

            XElement response = new XElement(ns + "xmsResponse");
            result.Add(response);

            XDocument xmsDataDoc = XDocument.Parse(xmsData);
            string userId = xmsDataDoc.Element(ns+"xmsData").Element(ns + "user").Element(ns+"userId").Value;
            string password = xmsDataDoc.Element(ns + "xmsData").Element(ns + "user").Element(ns + "password").Value;

            if (!Membership.ValidateUser(userId, password))
            {
                response.Add(
                    new XElement(ns + "error",
                        new XAttribute("code", "unregistered"),
                        new XAttribute("severity", "failure")
                    )
                );

                return result.ToString();
            }

            var user = Membership.GetUser(userId);
            var profile = ProfileBase.Create(userId);

            //check to make sure this is not an MMS since we don't support
            string service = xmsDataDoc.Element(ns+"xmsData").Element(ns+"xmsHead").Element(ns+"requiredService").Value;
            if (service != "SMS_SENDER")
            {
                response.Add(
                    new XElement(ns+"error",
                        new XAttribute("code", "unregisteredService"),
                        new XAttribute("severity", "failure"),
                        new XElement(ns+"content", "MMS")
                    )
                );

                return result.ToString();
            }

            //TODO: The specification includes the notion of schedule messages. Do we need to implement that?

            string replyPhone = xmsDataDoc.Element(ns + "xmsData").Element(ns + "user").Element(ns + "replyPhone").Value;
            var recipients = xmsDataDoc.Element(ns + "xmsData").Element(ns + "xmsHead").Element(ns+"to").Elements(ns + "recipient");
            var contents = xmsDataDoc.Element(ns + "xmsData").Element(ns + "xmsBody").Elements(ns+"content");

            TwilioRestClient client = new TwilioRestClient( (string)profile.GetPropertyValue("AccountSid"), (string)profile.GetPropertyValue("AuthToken") );

            bool _hasErrors = false;
            foreach (var recipient in recipients)
            {
                foreach (var content in contents)
                {
                    var sms = client.SendSmsMessage(replyPhone, recipient.Value, content.Value);
                    if (sms.RestException != null)
                    {
                        response.Add(
                            new XElement(ns+"error",
                                new XAttribute("code", "invalidRecipient"),
                                new XAttribute("severity", "failure"),
                                new XElement(ns + "content", sms.RestException.Message),
                                new XElement(ns + "recipientList", recipient.Value)
                            )
                        );

                        _hasErrors = true;
                    }

                }
            }

            if (!_hasErrors)
            {
                response.Add(
                    new XElement(ns + "error",
                        new XAttribute("code", "ok"),
                        new XAttribute("severity", "neutral")
                    )
                );
            }

            return result.ToString();
        }

        /// <summary>
        /// The OMS client that is built into SharePoint calls DeliverXmsBatch() to deliver a number of xmsData elements in a single XML transaction to the service provider.
        /// </summary>
        /// <param name="packageXml">The message content is packaged as an XML-formatted string called packageXml.</param>
        /// <returns>After the service provider attempts to send the message, it returns an XML-formatted string called xmsResponses that contains one or more error elements that indicate the success or failure of each attempt to send the message to each intended recipient.</returns>
        [WebMethod]
        public string DeliverXmsBatch(string packageXml)
        {
            InitializeSettings();

            XNamespace ns = OMSNAMESPACE;
            XDocument result = new XDocument(
                new XDeclaration("1.0", "utf-8", "true")
            );

            XElement response = new XElement(ns + "xmsResponses");
            result.Add(response);

            XDocument xmsDataDoc = XDocument.Parse(packageXml);

            //can I just parse the doc for all of the xmsData elements an call DeliverXms for each???

            return result.ToString();

        }
    }
}
