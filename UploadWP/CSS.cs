using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UploadWP
{
    sealed class CSS
    {
        private List<string> data;
        private List<string> discard = new List<string>()
        {
            ".php",
            ".xml",
            ".txt",
            "#"
        };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_data"></param>
        public CSS(List<string> _data)
        {
            this.data = _data;
        }
        /// <summary>
        /// 
        /// </summary>
        public void GetAll()
        {
            foreach (var item in data)
            {
                GetURLS(item);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public List<string> GetFiles(string sheet, string pattern)
        {
            bool skip = false;
            // Create a regular expression object
            Regex regex = new Regex(pattern);
            // Find all matches in the stylesheet
            MatchCollection matches = regex.Matches(sheet);
            List<string> _return = new List<string>();
            // Iterate over the matches
            foreach (Match match in matches)
            {
                skip = false;
                string src = match.Groups["url"].Value;
                // discard SVG and Base64, etc ...
                if (src.IndexOf("data:image") > -1)
                    continue;
                // discard unwanted files
                foreach (string d in this.discard)
                    if (src.IndexOf(d) > -1)
                        skip = true;
                if (skip)
                {
                    continue;
                }
                // if we have seen this image previously
                if (Public.PersistData.files.ContainsKey(src))
                {
                    SetFileUpdated(src);
                    // if this image was recently updated 
                    if (Public.PersistData.files[src].updated > Public.PersistData.last_run)
                    {
                        _return.Add(src);
                    }
                }
                else
                {
                    _return.Add(src);
                    Public.PersistData.files[src] = new Dates();
                    Public.PersistData.files[src].created = DateTime.Now;
                    SetFileUpdated(src);
                }
            }
            return _return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public List<string> GetURLS(string sheet)
        {
            return GetFiles(sheet, @"url\(['""]?(?<url>.*?)['""]?\)");
        }
        ///
        /// 
        /// SETTERS
        /// 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        private void SetFileUpdated(string filepath)
        {
            Helpers.SetFileUpdated(filepath);
        }
    }
}
