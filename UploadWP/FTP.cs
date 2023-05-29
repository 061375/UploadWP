using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace UploadWP
{
    sealed class FTP
    {
        private SessionOptions sessionOptions;
        public FTP(bool sftp = false)
        {
            if (sftp)
            {
                // Setup session options
                sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = Constants.SFTPServer,
                    UserName = Constants.SFTPServerUserName,
                    Password = Constants.SFTPServerPassword,
                    PortNumber = Constants.SFTPport
                };
            }
            else
            {
                // Setup session options
                sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Ftp,
                    HostName = Constants.FtpServer,
                    UserName = Constants.FtpServerUserName,
                    Password = Constants.FtpServerPassword
                };
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="localPath"></param>
        /// <returns></returns>
        public bool SetUploadFile(string remotePath, string localPath, bool delete =  false)
        {
            using (Session session = new Session())
            {
                // Will continuously report progress of transfer
                session.FileTransferProgress += SessionFileTransferProgress;
                // Upload files
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary;
                // Connect
                session.Open(sessionOptions);

                try
                {
                    // Download files and throw on any error
                    //session.PutFiles(localPath, remotePath,false, transferOptions);
                    session.PutFileToDirectory(localPath, remotePath, delete, transferOptions);
                    return true;
                }
                finally
                {
                    // Terminate line after the last file (if any)
                    if (_lastFileName != null)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public bool SetRemoveFile(string remotePath)
        {
            Public.Helpers.LogMethod(remotePath);
            using (Session session = new Session())
            {
                // Will continuously report progress of transfer
                session.FileTransferProgress += SessionFileTransferProgress;
                // Connect
                session.Open(sessionOptions);

                try
                {
                    Public.Helpers.WriteLog("Exists " + session.FileExists(remotePath));
                    if (session.FileExists(remotePath))
                    {
                        Public.Helpers.WriteLog("deleting " + remotePath + " ...");
                        session.RemoveFile(remotePath);
                        return true;
                    }
                    return false;
                }
                finally
                {
                    // Terminate line after the last file (if any)
                    if (_lastFileName != null)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public bool SetRemoveDirectory(string remotePath)
        {
            Public.Helpers.LogMethod(remotePath);
            using (Session session = new Session())
            {
                // Will continuously report progress of transfer
                session.FileTransferProgress += SessionFileTransferProgress;
                // Connect
                session.Open(sessionOptions);

                try
                {
                    // check that the directory is empty
                    if (session.FileExists(remotePath))
                    {
                        RemoteDirectoryInfo files = session.ListDirectory(remotePath);
                        if (files.Files.Count() < 2)
                        {
                            Console.WriteLine("removing directory " + remotePath);
                            // yes then delete
                            session.RemoveFiles(remotePath);
                            return true;
                        }
                    }
                    return false;
                }
                finally
                {
                    // Terminate line after the last file (if any)
                    if (_lastFileName != null)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public bool GetFileExists(string remotePath)
        {
            using (Session session = new Session())
            {
                // Will continuously report progress of transfer
                session.FileTransferProgress += SessionFileTransferProgress;
                // Connect
                session.Open(sessionOptions);

                try
                {
                    return session.FileExists(remotePath);
                }
                finally
                {
                    // Terminate line after the last file (if any)
                    if (_lastFileName != null)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="localPath"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public bool GetDirectoryExists(string remotePath, string localPath, bool create = false)
        {
            using (Session session = new Session())
            {
                // Connect
                session.Open(sessionOptions);

                // Enumerate files and directories to upload
                IEnumerable<FileSystemInfo> fileInfos =
                    new DirectoryInfo(localPath).EnumerateFileSystemInfos(
                        "*", SearchOption.AllDirectories);

                foreach (FileSystemInfo fileInfo in fileInfos)
                {
                    string remoteFilePath =
                        RemotePath.TranslateLocalPathToRemote(
                            fileInfo.FullName, localPath, remotePath);
                    if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
                    {
                        // Create remote subdirectory, if it does not exist yet
                        if (!session.FileExists(remotePath))
                        {
                            if(create)
                            {
                                session.CreateDirectory(remotePath);
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public List<string> GetRemoteFilePaths(string remotePath)
        {
            List<string> _return = new List<string>();
            using (Session session = new Session())
            {
                
                // Connect
                session.Open(sessionOptions);
                IEnumerable<RemoteFileInfo> _files = session.EnumerateRemoteFiles(remotePath, null, EnumerationOptions.AllDirectories);
                foreach(RemoteFileInfo _file in _files)
                {
                    _return.Add(_file.FullName);
                }
            }
            return _return;
        }
        private static void SessionFileTransferProgress(
        object sender, FileTransferProgressEventArgs e)
        {
            // New line for every new file
            if ((_lastFileName != null) && (_lastFileName != e.FileName))
            {
                Console.WriteLine();
            }

            // Print transfer progress
            Console.Write("\r{0} ({1:P0})", e.FileName, e.FileProgress);

            // Remember a name of the last file reported
            _lastFileName = e.FileName;
        }

        private static string _lastFileName;
    }
}
