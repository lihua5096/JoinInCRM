using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinInCRM.Models
{
    public class WechatAuth
    {
        public string  access_token { get; set; }
        public string  expires_in { get; set; }        
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }

    }
    public class WechatUserInfo
    {
        public string UserID { get; set; }
        public string openid { get; set; }
        public string nickname { get; set; }
        public string sex { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string headimgurl { get; set; }
        //public string privilege { get; set; }
        public string unionid { get; set; }
 
    }
    public class ErrorCode
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
    }

    public class AccessToken
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }

    }
    
}