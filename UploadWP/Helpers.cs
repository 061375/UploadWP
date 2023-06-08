using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWP
{
    sealed class Helpers
    {
        ///
        /// 
        /// GETTERS
        /// 
        /// 
        public static void GetFlags(string[] args)
        {
            for(int i=0;i<args.Length;i++)
                Public.Flags.Add(args[i]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static StringBuilder GetReplaceHardLinks(StringBuilder file)
        {
            file.Replace(Constants.RootLocalURL + "/", "/");
            file.Replace(Constants.RootLocalURL, "/");
            return file;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static StringBuilder GetMinifyHTML(StringBuilder file)
        {
            file = file.Replace("\r", "");
            file = file.Replace("\n", "");
            return file;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="pres"></param>
        /// <returns></returns>
        public static StringBuilder GetReplacePre(StringBuilder file, List<string> pre)
        {
            int i = 0;
            foreach(string s in pre)
            {
                string p = "[pre id=" + i + "]";
                i++;
                file = file.Replace(p, s);
            }
            return file;
        }
        /// <summary>
        /// originally intended to get the last time program was run the object allows any necessary persisted data
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static PersistData GetPersistData(string file)
        {
            // 
            string p_data_string = String.Empty;
            // chck if the persistant json file exists
            if (Public.Files.GetFileExists(file)) {
                p_data_string = Public.Files.GetFileData(file);
                if (p_data_string.Trim().Length == 0 || p_data_string == null)
                {
                    PersistData tmp = new PersistData();
                    p_data_string = JsonConvert.SerializeObject(tmp, Formatting.Indented);
                }
            } else {
                PersistData tmp = new PersistData();
                p_data_string = JsonConvert.SerializeObject(tmp, Formatting.Indented);
            }
            return JsonConvert.DeserializeObject<PersistData>(p_data_string);
        }
        ///
        /// 
        /// SETTERS
        /// 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="data"></param>
        public static void SetLastRan(string file, PersistData data)
        {
            data.last_run = DateTime.Now;
            Public.Files.SetWriteToFile(file, JsonConvert.SerializeObject(data,Formatting.Indented));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        public static void SetFileUpdated(string filepath)
        {
            // this path should be correct but check just in case
            if (Public.Files.GetFileExists(Constants.WPFolder + filepath))
            {
                if (!Public.PersistData.files.ContainsKey(filepath))
                    Public.PersistData.files[filepath] = new Dates();
                // get the last file updated date
                Public.PersistData.files[filepath].updated = Public.Files.GetFileModified(
                    Constants.WPFolder + filepath);
            }
        }
    }
}
