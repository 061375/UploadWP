using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace UploadWP
{
    sealed class Program
    {
        /** 
         * 
         * 
         *                      ᓚᘏᗢ
         *                      
         *                      
         * @title WP Minifier and Uploader
         * @about Gets the cached HTML version of a Wordpress website produced by the Comet Cache plugin and minifies it and uploads it to a web server
         *        The result is what looks like a WP website that os totally HTML and very light weight. Perfect for a webite this is essentially just a blog and doesn't need constant updates
         *        It's affectively hack-proof ... as long as your server doesn't get hacked
         * @author Jeremy Heminger <contact@jeremyheminger.com>
         * @date May 3, 2023
         * @last_update May 25, 2023
         * 
         * @bug     still collecting php files and probably other unwanted extensions
         *          - looks like this is fixed but needs more testing
         * @bug     seems to be skipping files occasionally 
         * @todo    program needs to dig through stylesheets to find assets and ( maybe ) javascript - DONE CSS
         * @todo    connect to WP DB ( might be useful )
         * @todo    detect theme change
         * @todo    scan for uploaded orphaned files
         * 
         * @dependency AQFiles 
         * @dependency AQHelpers 
         * @dependency HTMLAgiltyPack
         * @dependency Newtonsoft.JSON
         * @dependency Renci.SSHNet
         * 
         * @version 1.0.0.3
         *  @feature digs through stylsheets to find assets
         * @version 1.0.0.2
         *  @bugfix unwanted file types no longer uploaded
         * @version 1.0.0.1
         *  @feature delete removed pages
         * @version 1.0.0.0
         * 
         * 
         * */
        static void Main(string[] args)
        {
            try
            {
                Helpers.GetFlags(args);
                if (Public.Flags.Contains("-h"))
                {
                    Console.WriteLine("-c crawl website to reset any cache file ... faster to do this from WP plugin");
                    System.Environment.Exit(1);
                }
                // set Helper email stuff
                Public.Helpers.SetEmailFrom(Constants.EmailFrom);
                Public.Helpers.SetSMTP(Constants.SMTP);
                Public.Helpers.SetEmailCredentialsUser(Constants.EmailCredentialsUser);
                Public.Helpers.SetEmailCredentialsPass(Constants.EmailCredentialsPass);
                Public.Helpers.SetSMTPPort(Constants.SFTPport);
                Public.Helpers.SetErrorEmailSubject(Constants.ErrorEmailSubject);
                Public.Helpers.SetErrorEmails(Constants.ErrorEmails);
                Public.Helpers.SetConsoleLog(Constants.ConsoleLog);

                // check for WP
                if(!Public.Files.GetFileExists(Constants.WPFolder + "index.php"))
                {
                    Console.WriteLine("Wordpress not found at location specified ... expected " + Constants.WPFolder + "index.php");
                    System.Environment.Exit(1);
                }
                //
                // check for XML Sitemap Generator for Google plugin 
                //
                if (!Public.Files.GetFileExists(Constants.WPFolder + "wp-content\\plugins\\google-sitemap-generator\\sitemap.php"))
                {
                    Console.WriteLine("XML Sitemap Generator plugin not found at location specified ... expected " + Constants.WPFolder + "index.php");
                    System.Environment.Exit(1);
                }
                // check for WP Super Cache plugin
                //wp-super-cache
                if (!Public.Files.GetFileExists(Constants.WPFolder + "wp-content\\plugins\\wp-super-cache\\wp-cache.php"))
                {
                    Console.WriteLine("WP Super Cache plugin not found at location specified ... expected " + Constants.WPFolder + "index.php");
                    System.Environment.Exit(1);
                }


                // crawl local website to ensure that all files are cached
                if (Public.Flags.Contains("-c"))
                {
                    Crawler _c = new Crawler();
                    Console.WriteLine("______________________________");
                    Console.WriteLine("crawling local site to update cache ...");
                    Console.WriteLine("______________________________");
                    _c.Recursive(Constants.RootLocalURL + "/sitemap.xml");
                }
                Console.WriteLine("______________________________");
                Console.WriteLine("getting cached data ...");
                Console.WriteLine("______________________________");

                // get the last time this program was run
                Public.PersistData = Helpers.GetPersistData(Constants.PersistData);

                // instantiate
                FTP _ftp = new FTP();
                // 
                List<string> cache_paths = Files.GetCacheFilesPaths(Constants.WPCacheFolder, Public.PersistData.last_run);
                Public.Helpers.WriteLog("Found " + cache_paths.Count().ToString() + " pages");
                foreach (string cache_path in cache_paths)
                {
                    // organize and minify cached file
                    FileData _file = Files.GetModifyCachedFile(cache_path);
                    //
                    Public.Files.SetWriteToFile(_file.filename, _file.text.ToString(), _file.path);

                    // if this is new page then add it to the persistant data
                    if (!Public.PersistData.pages.ContainsKey(_file.path))
                    {
                        FileData tmp = _file;
                        Public.PersistData.pages[cache_path] = new Dates()
                        {
                            created = Public.Files.GetFileCreated(cache_path),
                            updated = Public.Files.GetFileModified(cache_path)
                        };
                    }

                    string local = _file.path + "\\" + _file.filename;
                    string remote = _file.path.Replace(Constants.WPCacheFolder, "").Replace("\\","/") + "/";

                    Console.WriteLine("______________________________");
                    Console.WriteLine("uploading html ...");
                    Console.WriteLine("______________________________");
                    Console.WriteLine("remote " + remote);
                    Console.WriteLine("local " + local);
                    if (Public.Files.GetFileExists(local))
                    {
                        _ftp.SetUploadFile(
                            remote,
                            local
                        );
                    }
                    else {
                        Console.WriteLine("deleting page ...");
                        _ftp.SetRemoveFile(remote + "/" + Public.Files.GetFileName(_file.filename));
                    }
                } // end cache_paths loop

                Console.WriteLine("______________________________");
                Console.WriteLine("remove deleted pages and directories...");
                Console.WriteLine("______________________________");

                List<string> remove = new List<string>();
                foreach (KeyValuePair<string, UploadWP.Dates> file in Public.PersistData.pages)
                {
                    if (!Public.Files.GetFileExists(file.Key))
                    {
                        string remote = file.Key.Replace(Constants.WPCacheFolder, "").Replace("\\", "/");
                        //Console.WriteLine("deleting page " + remote + " ...");
                        if(_ftp.SetRemoveFile(remote))
                        {
                            if (!remove.Contains(file.Key))
                                remove.Add(file.Key);
                        }
                        // delete the directory if it's empty
                        if(_ftp.SetRemoveDirectory(Public.Files.GetFilePath(remote).Replace("\\", "/")))
                        {
                            if(!remove.Contains(file.Key))
                                remove.Add(file.Key);
                        }
                    }
                } // end Public.PersistData.pages loop
                // remove from the dictionary safely
                if(remove.Count > 0)
                {
                    foreach(string key in remove)
                    {
                        if(Public.PersistData.pages.ContainsKey(key))
                            Public.PersistData.pages.Remove(key);
                    }
                }

                //
                Console.WriteLine("______________________________");
                Console.WriteLine("uploading files ...");
                Console.WriteLine("______________________________");
                // 
                foreach (KeyValuePair<string, UploadWP.Dates> file in Public.PersistData.files)
                {
                    string local = (Constants.WPFolder + file.Key).Replace("/","\\");
                    string remote = Public.Files.GetDirectoryFromPath(file.Key).Replace("\\", "/") + "/";
                    Console.WriteLine(local);
                    if (!Public.Files.GetFileExists(local))
                    {
                        Public.Helpers.WriteLog("local version of media no longer exists ...");
                        Public.Helpers.WriteLog("deleting remote version of " + Public.Files.GetFileName(file.Key) + " ...");
                        _ftp.SetRemoveFile(remote + "/" + Public.Files.GetFileName(file.Key));
                        continue;
                    }
                    //
                    if(_ftp.GetFileExists(remote + "/" + Public.Files.GetFileName(file.Key)))
                    {
                        // if file has not been updated then skip
                        if (file.Value.updated < Public.PersistData.last_run)
                        {
                            Public.Helpers.WriteLog("file has not recently been updated so ... skipping");
                            continue;
                        }
                    }
                    // 
                    if (Public.Files.GetFileExists(local))
                    {
                        Public.Helpers.WriteLog("uploading media "+ Public.Files.GetFileName(file.Key) + " ...");
                        _ftp.SetUploadFile(
                            remote,
                            local
                        );
                    }
                    Console.WriteLine("______________________________");
                } // end Public.PersistData.files loop

                Helpers.SetLastRan(Constants.PersistData, Public.PersistData);

                Console.WriteLine("______________________________");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Done !!!");
                Console.WriteLine("______________________________");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Public.Helpers.LogError(e.ToString());
                Console.ReadKey();
            }
        }
    }
}
