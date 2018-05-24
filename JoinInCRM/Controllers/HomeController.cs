using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json;
using JoinInCRM.Models;
using JoinInCRM.Helpers;
using System.Data;
using System.Data.SqlClient;
using JoinInCRM.DataAPI;

namespace JoinInCRM.Controllers
{
    public class HomeController :BaseController
    {
        public ActionResult Index()
        {

            //ViewBag.nickname = "";
            //ViewBag.headimgurl = "";
            //ViewBag.openid = "";
            //ViewBag.sex ="";
            //ViewBag.province ="";
            //ViewBag.city ="";
            //ViewBag.country = "";
            //ViewBag.Code = Request.QueryString["Code"];

            if (CurrentWechatUserInfo != null)
            {
                //ViewBag.nickname = CurrentUserInfo.nickname;
                //ViewBag.headimgurl = CurrentUserInfo.headimgurl;
                //ViewBag.openid = CurrentUserInfo.openid;
                //ViewBag.sex = CurrentUserInfo.sex;
                //ViewBag.province = CurrentUserInfo.province;
                //ViewBag.city = CurrentUserInfo.city;
                //ViewBag.country = CurrentUserInfo.country;
            }
            else
            {
                return Content("无授权，进入系统失败！请从微信公众号菜单重新进入。");
                //    string Code = "";

                //    if (string.IsNullOrEmpty(ViewBag.Code) || ViewBag.Code == "")
                //    {


                //        //string getCodeUrl = @" https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxd682b8b6d9016fb4&redirect_uri="
                //                         //+ Url.Encode("http://joinincrm.natapp4.cc/joinincrm") + @"& response_type = code & scope = snsapi_base & state = 123#wechat_redirect";
                //        //WechatAuth wa = (WechatAuth)HttpHelper<WechatAuth>(getCodeUrl);
                //        return Content("未获取授权Code,请从公众号菜单重新进入本系统。");
                //    }
                //    else
                //    {
                //        Code = ViewBag.Code;
                //        string appid = System.Configuration.ConfigurationManager.AppSettings["WeiXinAppID"];
                //        string secret = System.Configuration.ConfigurationManager.AppSettings["WeiXinSecret"];
                //        string getOpenIDUrl = @"https://api.weixin.qq.com/sns/oauth2/access_token?appid="+ appid + "&secret="+ secret + "&code=" + Code + "&grant_type=authorization_code";
                //        HttpHelper req = new HttpHelper();
                //        string content = req.Requster(getOpenIDUrl);

                //        if (content.ToLower().Contains("errcode"))
                //        {
                //            ErrorCode er = JsonConvert.DeserializeObject<ErrorCode>(content);
                //            return Content("微信授权失败:" + er.errcode+"   " + er.errmsg);
                //        }
                //        WechatAuth openID = JsonConvert.DeserializeObject<WechatAuth>(content);

                //        if (string.IsNullOrEmpty(openID.openid) || openID.openid == "")
                //        {
                //            return Content("获取用户OpenID失败");
                //        }
                //        else
                //        {
                //            //从数据库获取：
                //            // List<WechatUserInfo> cl = new List<WechatUserInfo>();
                //            // cl.Clear();
                //            ViewBag.openid = openID.openid;
                //            SQLHelper dbo = new SQLHelper();
                //            DataTable dt = dbo.getsqlDatable("Select OpenID, nickname, sex, province, city, country, headimgurl from WechatUsers Where OpenID=@OpenID", new System.Data.SqlClient.SqlParameter("@OpenID", openID.openid));
                //            if (dt.Rows.Count == 1)
                //            {

                //                ViewBag.nickname = dt.Rows[0][1].ToString();
                //                ViewBag.sex = dt.Rows[0][2].ToString();
                //                ViewBag.province = dt.Rows[0][3].ToString();
                //                ViewBag.city = dt.Rows[0][4].ToString();
                //                ViewBag.headimgurl = dt.Rows[0][6].ToString();
                //                ViewBag.country = dt.Rows[0][5].ToString();

                //                Session["WechatUserInfo"] = new WechatUserInfo
                //                {
                //                    openid = dt.Rows[0][0].ToString(),
                //                    nickname = dt.Rows[0][1].ToString(),
                //                    sex = dt.Rows[0][2].ToString(),
                //                    province = dt.Rows[0][3].ToString(),
                //                    city = dt.Rows[0][4].ToString(),
                //                    headimgurl = dt.Rows[0][6].ToString(),
                //                    country = dt.Rows[0][5].ToString()
                //                };
                //            }
                //            else
                //            {   //从微信拉取用户信息
                //                // string getUserInfoUrl = "https://api.weixin.qq.com/sns/userinfo?access_token=XBZlBQdCFKuAC59pm9BPw6MvjGhpHjVr_qwwxM5gg7TC369gZWSllWyEUmpu2wjgdt8r32uSCjWQM987dHS1MU3r3erey5jq_VHYHU5FR04k5nL1AgkPB2SIphni0OGuVGSdAHAQBN&openid=" + openID.openid + "&lang=zh_CN";

                //                string tokenContent = req.GetAccessToken();
                //                if (tokenContent.ToLower().Contains("errcode"))
                //                {
                //                    return Content("获取公众号AccessToken失败！" + tokenContent.ToString());
                //                }
                //                AccessToken accessToken = JsonConvert.DeserializeObject<AccessToken>(tokenContent);

                //                string getUserInfoUrl = "https://api.weixin.qq.com/cgi-bin/user/info?access_token=" + accessToken.access_token + "&openid=" + openID.openid + "&lang=zh_CN";
                //                content =  req.Requster(getUserInfoUrl);

                //                if (content.ToLower().Contains("errcode"))
                //                {
                //                    return Content("获取用户信息失败！"+accessToken.access_token+accessToken.expires_in + content.ToString());
                //                }
                //                WechatUserInfo  wui= JsonConvert.DeserializeObject<WechatUserInfo>(content);

                //                ViewBag.nickname = wui.nickname;
                //                ViewBag.sex = wui.sex;
                //                ViewBag.province = wui.province;
                //                ViewBag.city = wui.city;
                //                ViewBag.headimgurl = wui.headimgurl;
                //                ViewBag.country = wui.country;
                //                Session["WechatUserInfo"] = wui;
                //                //TODO:存入数据库？？？？

                //                string sqlStr = "Insert into WechatUsers (OpenID, nickname, sex, province, city,headimgurl, country ) values (@OpenID,@NickName,@Sex,@Province,@City,@HeadImgUrl,@Country)";
                //                SqlParameter[] para = {
                //                        new SqlParameter("@OpenID", wui.openid),
                //                        new SqlParameter("@NickName",wui.nickname),
                //                        new SqlParameter("@Sex", wui.sex),
                //                        new SqlParameter("@Province", wui.province),
                //                        new SqlParameter("@City", wui.city),
                //                        new SqlParameter("@HeadImgUrl", wui.headimgurl),
                //                        new SqlParameter("@Country", wui.country)
                //                    };

                //                dbo.ExecuteNonQuery(sqlStr, para);
                //                //cl.Add(wui);

                //            }
                //        }

                //    }
            }
            return View();

        }

        //private string HttpHelper(string url)
        //{
        //    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
        //    myRequest.Method = "GET";
        //    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
        //    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
        //    string content = reader.ReadToEnd();
        //    FilesHelper fs = new FilesHelper();
        //    fs.WriteLog("Get url:"+ url +"\r\n Result:"+content);
        //    return content;
        //}

        public ActionResult About()
        {
            if (CurrentWechatUserInfo != null)
            {
                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
            }
 
            return View();
        }
        public ActionResult Cooperate()
        {
            if (CurrentWechatUserInfo != null)
            {
                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
            }
     
            return View();
        }
        public ActionResult Contact()
        {
            if (CurrentWechatUserInfo != null)
            {
                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
            }
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Remind()
        {
            if (CurrentWechatUserInfo != null)
            {
                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
            }
            ViewBag.Message = "提醒";
            return View();

        }
        public ActionResult Circle()
        {
            if (CurrentWechatUserInfo != null)
            {
                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
            }
            ViewBag.Message = "工作圈";
            return View();

        }

        public ActionResult WorkData()
        {
            if (CurrentWechatUserInfo != null)
            {
                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
            }
            ViewBag.Message = "数据";
            return View();

        }

        public ActionResult Setting()
        {
            if (CurrentWechatUserInfo != null)
            {
                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
            }
            ViewBag.Message = "设置";
            return View();

        }

        [HttpGet]
        public void Search()
        {
            if (CurrentWechatUserInfo != null)
            {
                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
            }
            string searchTxt = Request.QueryString["searchContent"];
            if (!(String.IsNullOrEmpty(searchTxt) || searchTxt.Trim() == ""))
            {

                Response.Redirect(Url.Action("Index", "Customer", new { SearchContent = searchTxt }));
            }
        }
        

        
    }
}