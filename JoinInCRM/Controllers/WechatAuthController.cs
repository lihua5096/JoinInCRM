using JoinInCRM.Helpers;
using JoinInCRM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JoinInCRM.Controllers
{
    public class WechatAuthController : Controller
    {
        // GET: WechatAuth
        public ActionResult Index()
        {
            string Code = Request.QueryString["Code"];
            if (string.IsNullOrEmpty(Code) || Code == "")
            {
                //string getCodeUrl = @" https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxd682b8b6d9016fb4&redirect_uri="
                //+ Url.Encode("http://www.huangyanling.cn/joinincrm") + @"& response_type = code & scope = snsapi_base & state = 123#wechat_redirect";
                //WechatAuth wa = (WechatAuth)HttpHelper<WechatAuth>(getCodeUrl);
                return Content("未获取授权Code,请从公众号菜单重新进入本系统。");
            }
            else
            {
               
                string appid = System.Configuration.ConfigurationManager.AppSettings["WeiXinAppID"];
                string secret = System.Configuration.ConfigurationManager.AppSettings["WeiXinSecret"];
                string getOpenIDUrl = @"https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + secret + "&code=" + Code + "&grant_type=authorization_code";
   
                HttpHelper req = new HttpHelper();
                string content = req.Requster(getOpenIDUrl);

                if (content.ToLower().Contains("errcode"))
                {
                    ErrorCode er = JsonConvert.DeserializeObject<ErrorCode>(content);
                    return Content("请重新从公众号进入！");
                }
                WechatAuth openID = JsonConvert.DeserializeObject<WechatAuth>(content);

                if (string.IsNullOrEmpty(openID.openid) || openID.openid == "")
                {
                    return Content("获取用户OpenID失败");
                }
                else
                {
                    //从数据库获取：
                    // List<WechatUserInfo> cl = new List<WechatUserInfo>();
                    // cl.Clear();
                    //ViewBag.openid = openID.openid;
                    SQLHelper dbo = new SQLHelper();
                    DataTable dt = dbo.getsqlDatable("Select UserID, OpenID, nickname, sex, province, city, country, headimgurl from WechatUsers Where OpenID=@OpenID", new SqlParameter("@OpenID", openID.openid));
                    if (dt.Rows.Count >= 1)
                    {
                        //ViewBag.nickname = dt.Rows[0][1].ToString();
                        //ViewBag.sex = dt.Rows[0][2].ToString();
                        //ViewBag.province = dt.Rows[0][3].ToString();
                        //ViewBag.city = dt.Rows[0][4].ToString();
                        //ViewBag.headimgurl = dt.Rows[0][6].ToString();
                        //ViewBag.country = dt.Rows[0][5].ToString();

                        Session["WechatUserInfo"] = new WechatUserInfo
                        {
                            UserID = dt.Rows[0][0].ToString(),
                            openid = dt.Rows[0][1].ToString(),
                            nickname = dt.Rows[0][2].ToString(),
                            sex = dt.Rows[0][3].ToString(),
                            province = dt.Rows[0][4].ToString(),
                            city = dt.Rows[0][5].ToString(),
                            headimgurl = dt.Rows[0][7].ToString(),
                            country = dt.Rows[0][6].ToString()
                            
                        };
                       
                    }
                    else
                    {   //从微信拉取用户信息
                        // string getUserInfoUrl = "https://api.weixin.qq.com/sns/userinfo?access_token=XBZlBQdCFKuAC59pm9BPw6MvjGhpHjVr_qwwxM5gg7TC369gZWSllWyEUmpu2wjgdt8r32uSCjWQM987dHS1MU3r3erey5jq_VHYHU5FR04k5nL1AgkPB2SIphni0OGuVGSdAHAQBN&openid=" + openID.openid + "&lang=zh_CN";

                        string tokenContent = req.GetAccessToken();
                        if (tokenContent.ToLower().Contains("errcode"))
                        {
                            return Content("获取公众号AccessToken失败！" + tokenContent.ToString());
                        }
                        AccessToken accessToken = JsonConvert.DeserializeObject<AccessToken>(tokenContent);

                        string getUserInfoUrl = "https://api.weixin.qq.com/cgi-bin/user/info?access_token=" + accessToken.access_token + "&openid=" + openID.openid + "&lang=zh_CN";
                        content = req.Requster(getUserInfoUrl);

                        if (content.ToLower().Contains("errcode"))
                        {
                            return Content("获取用户信息失败！" + accessToken.access_token + accessToken.expires_in + content.ToString());
                        }
                        WechatUserInfo wui = JsonConvert.DeserializeObject<WechatUserInfo>(content);

                        ViewBag.nickname = wui.nickname;
                        ViewBag.sex = wui.sex;
                        ViewBag.province = wui.province;
                        ViewBag.city = wui.city;
                        ViewBag.headimgurl = wui.headimgurl;
                        ViewBag.country = wui.country;
                        Session["WechatUserInfo"] = wui;
                        //TODO:存入数据库？？？？
                        
                        string sqlStr = "if not exists (Select OpenID From WechatUsers Where OpenID=@OpenID) begin  Insert into WechatUsers (OpenID, nickname, sex, province, city,headimgurl, country ) values (@OpenID,@NickName,@Sex,@Province,@City,@HeadImgUrl,@Country) end";
                        SqlParameter[] para = {
                                    new SqlParameter("@OpenID", wui.openid),
                                    new SqlParameter("@NickName",wui.nickname),
                                    new SqlParameter("@Sex", wui.sex),
                                    new SqlParameter("@Province", wui.province),
                                    new SqlParameter("@City", wui.city),
                                    new SqlParameter("@HeadImgUrl", wui.headimgurl),
                                    new SqlParameter("@Country", wui.country)
                                };
                        dbo.ExecuteNonQuery(sqlStr, para);
                        //cl.Add(wui);
                    }

                    dt = dbo.getsqlDatable(@"SELECT D.UserID, A.CompanyID,A.StoreID, A.LeaderUserID, A.RoleID,A.EmpNo, A.EmpName, A.CardNo, A.CellPhone, A.Sex, A.HireDate,
                                                     CASE WHEN A.Active = 1 THEN '在职' ELSE '离职' END AS Active,C.RoleName,B.CompanyName,E.StoreName
                                              FROM USERS A
                                              INNER JOIN [Role] C ON A.RoleID = C.RoleID
                                              INNER JOIN [Store] E ON A.StoreID=E.StoreID
                                              INNER JOIN Company B ON E.CompanyID = B.CompanyID
                                              RIGHT JOIN WechatUsers D ON A.UserID = D.UserID
                                              Where D.OpenID =@OpenID ", new SqlParameter("OpenID", openID));
                    if (dt.Rows.Count==1)
                    {
                        Session["UserInfo"] = new User
                        {
                            UserID = dt.Rows[0][0].ToString(),
                            CompanyID = dt.Rows[0][1].ToString(),
                            StoreID = dt.Rows[0][2].ToString(),
                            LeaderUserID = dt.Rows[0][3].ToString(),
                            RoleID = dt.Rows[0][5].ToString(),
                            EmpNo = dt.Rows[0][6].ToString(),
                            EmpName = dt.Rows[0][6].ToString(),
                            CardNo = dt.Rows[0][7].ToString(),
                            CellPhone = dt.Rows[8][0].ToString(),
                            Sex = dt.Rows[0][9].ToString(),
                            HireDate = dt.Rows[0][10].ToString(),
                            Active = dt.Rows[0][11].ToString(),
                            RoleName = dt.Rows[0][12].ToString(),
                            CompanyName = dt.Rows[0][13].ToString(),
                            StoreName= dt.Rows[0][14].ToString()
                        };
                        if (String.IsNullOrEmpty(dt.Rows[0][1].ToString()) || dt.Rows[0][1].ToString()=="")
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                            {
                                return RedirectToAction("UserBinding", "Base");
                            }
                            else
                            {
                                return Content("未知错误，请重新从公众号进入系统，如问题仍然出现，请联系系统管理员！");
                            }
                        }
                    }
                    else
                    {
                        Session["UserInfo"] = new User
                        {
                            UserID ="",
                            CompanyID = "",
                            StoreID="",
                            LeaderUserID = "",
                            RoleID = "",
                            EmpNo = "",
                            EmpName ="",
                            CardNo = "",
                            CellPhone = "",
                            Sex = "",
                            HireDate ="",
                            Active = "",
                            RoleName = "",
                            CompanyName ="",
                            StoreName=""
                        };

                    }
                }

                return  RedirectToAction("Index","Home");
            }
        }
    }
}