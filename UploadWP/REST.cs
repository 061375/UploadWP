using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UploadWP
{
    sealed class REST
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="querystring"></param>
        /// <returns></returns>
        public string GET(string url, Dictionary<string, string> querystring = null)
        {
            Public.Helpers.LogMethod();
            // 
            using (var wb = new System.Net.WebClient())
            {
                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    if (querystring != null)
                    {
                        url += "?";
                        foreach (KeyValuePair<string, string> q in querystring)
                            url += q.Key + "=" + q.Value + "&";
                    }
                    Public.Helpers.WriteLog(url);
                    var response = wb.DownloadString(url);

                    return response;
                }
                catch (System.Net.WebException e)
                {// OOPS
                    if (e.Message.IndexOf("(401)") > -1)
                        return "401";
                    if (e.Status == System.Net.WebExceptionStatus.ProtocolError)
                    {
                        if (((System.Net.HttpWebResponse)e.Response).StatusCode.ToString() == "401")
                        {
                            return "401";
                        }
                    }
                    Public.Helpers.WriteLog("Web Exception");
                    Public.Helpers.LogError(e.ToString());
                    return null;
                }
                catch (Exception e)
                {
                    Public.Helpers.WriteLog("Web Exception");
                    Public.Helpers.LogError(e.ToString());
                    return null;
                }
            }
        }
    }
}
