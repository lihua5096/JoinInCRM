using JoinInCRM.Helpers;
using JoinInCRM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JoinInCRM.Controllers
{
    public class BaseController : Controller
    {
        
        public WechatUserInfo CurrentWechatUserInfo { get; set; }
        public User CurrentUserInfo { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //从Session中得到用户登录信息
            Session["WechatUserInfo"] = new WechatUserInfo
            {
                nickname = "＊ST小散",
                headimgurl = "http://wx.qlogo.cn/mmopen/5mxuSU5RGhY0J2JiaCXazWbGkic0KalhYJVQOWfqxbQIeK3hzQoCAgQaD4eklDVC5Fic2vuNO0j78rdQ4oX5FZT8j0erf3SJQxu/0",
                openid = "oC86Z09y0dkSbyPXzxz6AOGF1U_o",
                sex = "",
                province = "广东",
                city = "广州",
                country = "中国"
            };
            Session["UserInfo"] = new User
            {
                CompanyID="",
                CompanyName="",
                UserID="",
                CardNo="", 
                StoreID = "",
                RoleID = "",
                LeaderUserID = "",                
                StoreName = "",
                CellPhone = "",
                Sex = "",
                EmpNo = "",
                EmpName = "",                
                RoleName = "",
                HireDate = "",
                Active = "" 
            };

            CurrentWechatUserInfo = Session["WechatUserInfo"] as WechatUserInfo; //绑定的是用户微信基本信息
            CurrentUserInfo = Session["UserInfo"] as User;                       //绑定的是用户系统基本信息
            if (CurrentWechatUserInfo == null )
            {
                string redirectUrl= System.Configuration.ConfigurationManager.AppSettings["redirectUrl"];
                //从新从微信获取授权
                Response.Redirect(@"https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxd682b8b6d9016fb4&redirect_uri="+redirectUrl+@"/WechatAuth&response_type=code&scope=snsapi_base&state=1#wechat_redirect");
            }
            else
            { 

                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
                ViewBag.CompanyID = CurrentUserInfo.CompanyID;
                ViewBag.CompanyName = CurrentUserInfo.CompanyName;
                ViewBag.RoleName = CurrentUserInfo.RoleName;
                ViewBag.LeaderUserID = CurrentUserInfo.LeaderUserID;
                ViewBag.CellPhone = CurrentUserInfo.CellPhone;
                ViewBag.UserID = CurrentUserInfo.UserID;
                ViewBag.EmpNo = CurrentUserInfo.EmpNo;
                ViewBag.EmpName = CurrentUserInfo.EmpName;
                ViewBag.StoreName = CurrentUserInfo.StoreName;
                ViewBag.StoreID = CurrentUserInfo.StoreID;

                if (String.IsNullOrEmpty(ViewBag.CompanyID) || ViewBag.CompanyID == "")
                    RedirectToAction("UserBinding", "Base");                     
                }

            }

        }


        [HttpGet]
        public ActionResult UserBinding()
        { 
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            ViewBag.CompanyList = apiBasecData.GetCompany();
            return View(); 
        }

        [HttpGet]
        //        用户绑定后显示信息
        public ActionResult BindingInfo()
        {
            SQLHelper dbo = new SQLHelper();
            DataTable dt= dbo.getsqlDatable(@"SELECT D.UserID, A.CompanyID,A.StoreID, A.LeaderUserID, A.RoleID,A.EmpNo, A.EmpName, A.CardNo, A.CellPhone, A.Sex, A.HireDate,
                                                     CASE WHEN A.Active = 1 THEN '在职' ELSE '离职' END AS Active,C.RoleName,B.CompanyName,E.StoreName
                                              FROM USERS A
                                              INNER JOIN [Role] C ON A.RoleID = C.RoleID
                                              INNER JOIN [Store] E ON A.StoreID=E.StoreID
                                              INNER JOIN Company B ON E.CompanyID = B.CompanyID
                                              RIGHT JOIN WechatUsers D ON A.UserID = D.UserID
                                              Where D.OpenID =@OpenID ", new SqlParameter("OpenID", CurrentWechatUserInfo.openid));
            if (dt.Rows.Count == 1)
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
                    StoreName = dt.Rows[0][14].ToString()
                };

                CurrentUserInfo = Session["UserInfo"] as User;
                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
                ViewBag.CompanyID = CurrentUserInfo.CompanyID;
                ViewBag.CompanyName = CurrentUserInfo.CompanyName;
                ViewBag.RoleName = CurrentUserInfo.RoleName;
                ViewBag.LeaderUserID = CurrentUserInfo.LeaderUserID;
                ViewBag.CellPhone = CurrentUserInfo.CellPhone;
                ViewBag.UserID = CurrentUserInfo.UserID;
                ViewBag.EmpNo = CurrentUserInfo.EmpNo;
                ViewBag.EmpName = CurrentUserInfo.EmpName;
                ViewBag.StoreName = CurrentUserInfo.StoreName;
                ViewBag.StoreID = CurrentUserInfo.StoreID;
                return View();
            }
            else
            {
                return Content("系统错误，请重新从公众号进入系统！");
            } 
        }
    }
 
}