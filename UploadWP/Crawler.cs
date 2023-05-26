using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadWP
{
    sealed class Crawler
    {
        REST _r = new REST();
        XML _x = new XML();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void Recursive(string url)
        {
            Public.Helpers.LogMethod(url);
            // TODO if the URL is NOT XML then check if this has recently been updated
            // get the XML data from the sitemap(s)
            string data = _r.GET(url);
            // if there is data
            if(data != null)
                // if this is an xml file
                if(data.IndexOf("<?xml version='1.0' encoding='UTF-8'?>") > -1)
                {
                    // we only need the <loc> tag
                    List<string> urls = _x.GETTAG(data, "loc");
                    foreach(string u in urls)
                    {
                        // recurse into each file
                        Recursive(u);
                    }
                }
                else
                {
                    url = url.Replace(Constants.RootLocalURL, "");
                    if (!Public.PersistData.pages.ContainsKey(url))
                    {
                        Public.PersistData.pages[url] = new Dates();
                        Public.PersistData.pages[url].created = DateTime.Now;
                        Public.PersistData.pages[url].updated = DateTime.Now;
                    }
                }
        }
    }
}
