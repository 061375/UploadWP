using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace UploadWP
{
    class Attributes
    {
        public bool issrc = true;
        public string value = String.Empty;
    }
    sealed class HTML
    {
        private HtmlDocument doc;
        private List<string> discard = new List<string>()
        {
            ".php",
            ".xml",
            ".txt"
        };
        public HTML(HtmlDocument doc, string data, List<string> _discard = null)
        {
            this.doc = doc;
            this.doc.LoadHtml(data);
            if(null != _discard)
                this.discard = _discard;
        }
        public void StripComments()
        {
            Public.Helpers.LogMethod();
            // Script comments from the document. 
            if (doc.DocumentNode != null)
            {
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//comment()");
                if (nodes != null)
                {
                    foreach (HtmlNode node in from cmt in nodes
                                              where (cmt != null
                                                     && cmt.InnerText != null
                                                     && !cmt.InnerText.ToUpper().StartsWith("DOCTYPE"))
                                                     && cmt.ParentNode != null
                                              select cmt)
                    {
                        node.ParentNode.RemoveChild(node);
                    }
                }
            }
        }
        public void StripJSComments()
        {
            Public.Helpers.LogMethod();
            // Script comments from the document. 
            if (doc.DocumentNode != null)
            {
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//script");
                if (nodes != null)
                {
                    foreach (HtmlNode node in from cmt in nodes
                                              where (cmt != null
                                                     && cmt.InnerText != null
                                                     && !cmt.InnerText.ToUpper().StartsWith("DOCTYPE"))
                                                     && cmt.ParentNode != null
                                              select cmt)
                    {
                        string scr = node.InnerText;
                        if (scr == null || scr.Length == 0)
                            continue;
                        IEnumerable<HtmlAttribute> attr = node.GetAttributes();
                        string scriptag = "<script ";
                        foreach(HtmlAttribute a in attr)
                        {
                            scriptag += " " + a.Name + "=\"" + a.Value + "\"";
                        }
                        scriptag += ">";
                        scr = Regex.Replace(scr, @"(?<=^|[^\S])\/\/.+", "");
                        scr = Regex.Replace(scr, @"/\*(.*?)\*/", "");
                        node.ParentNode.ReplaceChild(HtmlTextNode.CreateNode(scriptag + scr + "</script>"), node);
                    }
                }
            }
        }
        ///
        /// 
        /// GETTERS
        /// 
        /// 
        /// 
        /// GETTERS MEDIA WRAPPERS
        /// 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetImages()
        {
            Public.Helpers.LogMethod();
            return GetFiles("img", new Dictionary<string, Attributes>()
            {
                {"src",new Attributes
                {
                    issrc = true
                } }
            });
        }
        public List<string> GetStylesheets()
        {
            Public.Helpers.LogMethod();
            return GetFiles("link", new Dictionary<string, Attributes>()
            {
                {
                    "href",new Attributes
                    {
                        issrc = true
                    } 
                },
                {
                    "rel",new Attributes
                    {
                        issrc = false,
                        value = "stylesheet"
                    }
                }
            });
        }
        public List<string> GetScripts()
        {
            Public.Helpers.LogMethod();
            return GetFiles("script", new Dictionary<string, Attributes>()
            {
                {"src",new Attributes
                {
                    issrc = true
                } }
            });
        }
        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public List<string> GetVideo()
        {
            Public.Helpers.LogMethod();
            return GetFiles("video", new Dictionary<string, Attributes>()
            {
                {
                    "src",new Attributes
                    {
                        issrc = true
                    }
                },
                {
                    "poster",new Attributes
                    {
                        issrc = false
                    }
                }
            });
        }
        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public List<string> GetMedia()
        {
            Public.Helpers.LogMethod();
            return new List<string>();
        }
        ///
        /// 
        /// END GETTERS MEDIA WRAPPERS
        /// 
        /// 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public List<string> GetFiles(string tag, Dictionary<string, Attributes> attributes)
        {
            bool skip = false;
            Public.Helpers.LogMethod();
            List<string> _return = new List<string>();
            if (doc.DocumentNode != null)
            {
                // search the documents DOM
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//" + tag);
                if (nodes != null)
                {
                    foreach (HtmlNode node in from cmt in nodes
                                              where (cmt != null
                                                     && cmt.InnerText != null
                                                     && !cmt.InnerText.ToUpper().StartsWith("DOCTYPE"))
                                                     && cmt.ParentNode != null
                                              select cmt)
                    {
                        skip = false;
                        int match = 0;
                        string srctmp = String.Empty;
                        // loop the attributes to determine if this is what we want to upload
                        IEnumerable<HtmlAttribute> attr = node.GetAttributes();
                        // loop the attributes
                        foreach (HtmlAttribute a in attr)
                        {
                            if (attributes.ContainsKey(a.Name))
                            {
                                match++;
                                if (attributes[a.Name].issrc)
                                {
                                    srctmp = a.Value;
                                    continue;
                                }
                                if(attributes[a.Name].value == a.Value)
                                    match++;
                            }
                        }
                        // if we have at least the correct number of expected attributes
                        if(match >= attributes.Count())
                        {
                            // only get local files ... nothing remote
                            if (srctmp.IndexOf(Constants.RootLocalURL) == -1)
                                continue;
                            // reassign src and strip localhost
                            string src = srctmp.Replace(Constants.RootLocalURL, "");
                            // strip version data
                            if(src.IndexOf("?") > -1)
                                src = src.Substring(0, src.IndexOf("?"));
                            // make sure this is a file and not a reference
                            if (!Public.Files.GetFileExists(Constants.WPFolder + src))
                                continue;
                            // discard unwanted files
                            foreach (string d in this.discard)
                                if (src.IndexOf(d) > -1)
                                {
                                    skip = true;
                                    break;
                                }
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
                                continue;
                            }
                            else
                            {
                                _return.Add(src);
                                Public.PersistData.files[src] = new Dates();
                                Public.PersistData.files[src].created = DateTime.Now;
                                SetFileUpdated(src);
                            }
                            Console.WriteLine(Constants.WPFolder + src);
                        }
                    }
                }
            }
            return _return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public List<string> GetInnerText(string tag)
        {
            Public.Helpers.LogMethod();
            List<string> _return = new List<string>();
            if (doc.DocumentNode != null)
            {
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//" + tag);
                if (nodes != null)
                {
                    foreach (HtmlNode node in from cmt in nodes
                                              where (cmt != null
                                                     && cmt.InnerText != null
                                                     && !cmt.InnerText.ToUpper().StartsWith("DOCTYPE"))
                                                     && cmt.ParentNode != null
                                              select cmt)
                    {
                        _return.Add(node.InnerText);
                    }
                }
            }
            return _return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Get()
        {
            return doc.DocumentNode.OuterHtml;
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
