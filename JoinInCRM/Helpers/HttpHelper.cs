using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace JoinInCRM.Helpers
{
    public class HttpHelper
    {
        public string Requster(string url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            FilesHelper fs = new FilesHelper();
            fs.WriteLog("Get url:" + url + "\r\n Result:" + content);
            return content;
        }

        public string GetAccessToken()
        {
             
            string appid = System.Configuration.ConfigurationManager.AppSettings["WeiXinAppID"];
            string secret = System.Configuration.ConfigurationManager.AppSettings["WeiXinSecret"];

            string access_tokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret;
            string content = Requster(access_tokenUrl);
 

            return content;
        }
    }
}