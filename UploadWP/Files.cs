using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWP
{
    sealed class Files
    {
        ///
        /// 
        /// GETTERS
        /// 
        /// 
        /// <summary>
        /// get the cached html files 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="last_ran"></param>
        /// <returns></returns>
        public static List<string> GetCacheFilesPaths(string directory, DateTime last_ran)
        {
            Public.Helpers.LogMethod(directory);
            List<string> _return = new List<string>();
            Public.Files.GetFilesInDirectoryRecursive(directory, "*.html");
            List<string> files = Public.Files.files_recursive;
            foreach (string file in files)
            {
                if(Public.Files.GetFileModified(file) > last_ran)
                {
                    _return.Add(file);
                }
            }
            return _return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache_path"></param>
        /// <returns></returns>
        public static FileData GetModifyCachedFile(string cache_path)
        {
            Public.Helpers.LogMethod(cache_path);
            FileData _return = new FileData();
            string file = Public.Files.GetFileData(cache_path);
            HTML _html = new HTML(new HtmlAgilityPack.HtmlDocument
            {
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true
            }, file);
            _html.StripComments();
            _html.StripJSComments();
            _html.GetImages();
            _html.GetScripts();
            _html.GetVideo();

            // get all css
            List<string> stylesheets = _html.GetStylesheets();
            List<string> stylesheets2 = _html.GetInnerText("style");
            foreach (string stylesheet in stylesheets)
            {
                stylesheets2.Add(stylesheet);
            }
            // get all URLs from css
            CSS _css = new CSS(stylesheets2);
            _css.GetAll();

            _return.text.Append(_html.Get().ToString());

            // modifiy the cached file
            _return.text = Helpers.GetReplaceHardLinks(_return.text);
            _return.text = Helpers.GetMinifyHTML(_return.text);
            //
            _return.path = Public.Files.GetFilePath(cache_path);
            _return.filename = Public.Files.GetFileName(cache_path);
            //
            Helpers.SetFileUpdated(cache_path);
            return _return;
        }
    }
}
