using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWP
{
    sealed class PurgeOrphaned
    {
        private readonly FTP _ftp;
        public PurgeOrphaned(bool sftp = false)
        {
            _ftp = new FTP();

            DateTime LastRan = DateTime.Now;
            // options = hourly , daily , weekly , monthly , yearly
            switch (Constants.ScanForOrphanedTime)
            {
                case "hourly":
                    LastRan = DateTime.Now.AddHours(-1);
                    break;
                case "daily":
                    LastRan = DateTime.Now.AddDays(-1);
                    break;
                case "weekly":
                    LastRan = DateTime.Now.AddDays(-7);
                    break;
                case "monthly":
                    LastRan = DateTime.Now.AddMonths(-1);
                    break;
                case "yearly":
                    LastRan = DateTime.Now.AddYears(-1);
                    break;
            }
            if (LastRan != DateTime.Now)
            {
                if (Public.PersistData.last_purged_orphaned < LastRan)
                {
                    Public.Helpers.WriteLog("Finding orphaned files ...");
                    //  loop FTP file system
                    List<string> local_files = new List<string>();
                    List<string> remote_files = _ftp.GetRemoteFilePaths("/");
                    foreach (KeyValuePair<string, Dates> lf in Public.PersistData.pages)
                    {
                        string f = lf.Key.Replace(Constants.WPCacheFolder, "").Replace("\\", "/");
                        local_files.Add(f);
                    }
                    foreach (KeyValuePair<string, Dates> lf in Public.PersistData.files)
                    {
                        string f = lf.Key.Replace(Constants.WPCacheFolder, "").Replace("\\", "/");
                        local_files.Add(f);
                    }
                    //  compare to local
                    foreach (string remote_file in remote_files)
                    {
                        //  not matching then delete remote
                        if (!local_files.Contains(remote_file))
                        {
                            if (remote_file.IndexOf("index.html") > -1)
                                _ftp.SetRemoveFile(remote_file);
                        }
                        if (!local_files.Contains(remote_file))
                        {
                            if (remote_file.IndexOf("index.html") == -1)
                                _ftp.SetRemoveFile(remote_file);
                        }
                    }
                }
            }
        }

        ///
        /// GETTERS
        ///

        ///
        /// SETTERS
        ///

    }
}