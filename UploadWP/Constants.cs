using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
namespace UploadWP
{
    sealed class Constants
    {
        public static readonly string PersistData = ConfigurationManager.AppSettings.Get("PersistData");
        public static readonly string WPFolder = ConfigurationManager.AppSettings.Get("WPFolder");
        public static readonly string WPCacheFolder = ConfigurationManager.AppSettings.Get("WPCacheFolder");
        public static readonly string RootLocalURL = ConfigurationManager.AppSettings.Get("RootLocalURL");
        public static readonly string FtpServer = ConfigurationManager.AppSettings.Get("FtpServer");
        public static readonly string FtpServerUserName = ConfigurationManager.AppSettings.Get("FtpServerUserName");
        public static readonly string FtpServerPassword = ConfigurationManager.AppSettings.Get("FtpServerPassword");
        public static readonly string FtpUploadPath = ConfigurationManager.AppSettings.Get("FtpServerPassword");
        public static readonly string SFTPServer = ConfigurationManager.AppSettings.Get("SFTPServer");
        public static readonly string SFTPServerUserName = ConfigurationManager.AppSettings.Get("SFTPServerUserName");
        public static readonly string SFTPServerPassword = ConfigurationManager.AppSettings.Get("SFTPServerPassword");
        public static readonly string SFTPUploadPath = ConfigurationManager.AppSettings.Get("SFTPUploadPath");
        public static readonly int SFTPport = Public.Helpers.GetToInt32(ConfigurationManager.AppSettings.Get("SFTPport"));

        public static readonly string EmailFrom = ConfigurationManager.AppSettings.Get("EmailFrom");
        public static readonly string SMTP = ConfigurationManager.AppSettings.Get("SMTP");
        public static readonly string EmailCredentialsUser = ConfigurationManager.AppSettings.Get("EmailCredentialsUser");
        public static readonly string EmailCredentialsPass = ConfigurationManager.AppSettings.Get("EmailCredentialsPass");
        public static readonly int SMTPPort = Public.Helpers.GetToInt32(ConfigurationManager.AppSettings.Get("SMTP"));
        public static readonly string ErrorEmailSubject = ConfigurationManager.AppSettings.Get("ErrorEmailSubject");
        public static readonly string ErrorEmails = ConfigurationManager.AppSettings.Get("ErrorEmails");
        public static readonly bool ConsoleLog = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("ConsoleLog"));

        public static readonly bool ScanForOrphaned = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("ScanForOrphaned"));
        public static readonly string ScanForOrphanedTime = ConfigurationManager.AppSettings.Get("ScanForOrphanedTime");
    }
}
