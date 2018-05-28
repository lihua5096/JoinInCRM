using JoinInCRM.Models;
using System;
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
                    RedirectToAction("UserBinding", "UserBinding");                     
                } 
            }
        }  
}