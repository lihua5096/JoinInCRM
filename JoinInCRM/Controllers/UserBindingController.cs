using JoinInCRM.Helpers;
using JoinInCRM.Models;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace JoinInCRM.Controllers
{
    public class UserBindingController : Controller
    {
        // GET: UserBinding
        [HttpGet]
        public ActionResult UserBinding()
        {            
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            ViewBag.CompanyList = apiBasecData.GetCompany();
            return View();
        }

        [HttpPost]
        public ActionResult UserBinding(FormCollection collection)
        {
            WechatUserInfo CurrentWechatUserInfo = Session["WechatUserInfo"] as WechatUserInfo;
            string LastUser = "";
            if (CurrentWechatUserInfo != null)
            {
                LastUser = CurrentWechatUserInfo.openid;
            }
            else
            {
                return Content("长时间未操作，系统已自动退出，请从微信公众号菜单重新进入！");
            }
            try
            {
                // Save BindingInfo

                User user = new User();
                user.UserID = CurrentWechatUserInfo.UserID;
                user.EmpNo = collection.GetValue("EmpNo").AttemptedValue.Trim();
                user.CompanyID=collection.GetValue("CompanyID").AttemptedValue.Trim();
                user.StoreID= collection.GetValue("StoreID").AttemptedValue.Trim();
                user.Active = "1";

                //is exists 
                if (string.IsNullOrEmpty(user.EmpNo) || user.EmpNo == "")
                {
                    return Content("请填写工号！");
                }
                if (string.IsNullOrEmpty(user.CompanyID) || user.CompanyID == "")
                {
                    return Content("请选择公司！");
                }
                if (string.IsNullOrEmpty(user.StoreID) || user.StoreID == "")
                {
                    return Content("请选择所在！");
                }
                DataAPI.BasicDataController dtc = new DataAPI.BasicDataController();
                string checkBinding = "";
                checkBinding = dtc.CheckBindingUser(user);
                if (checkBinding == "1")
                {
                    return Content("该工号已被绑定，请检查！");
                }
                if (checkBinding == "2")
                {
                    return Content("查不到该工号的信息，无法绑定，请联系系统管理员！");
                }
                 
                if (dtc.AddBindingUser(user, LastUser) == "1")
                {

                    return Content("1");
                }
                else
                {
                    return Content("绑定失败！");
                }



            }
            catch
            {
                return Content("-1");
            }
        }

        [HttpGet]
        //        用户绑定后显示信息
        public ActionResult BindingInfo()
        {
            WechatUserInfo CurrentWechatUserInfo = Session["WechatUserInfo"] as WechatUserInfo;
            SQLHelper dbo = new SQLHelper();
            System.Data.DataTable dt = dbo.getsqlDatable(@"SELECT D.UserID, A.CompanyID,A.StoreID, A.LeaderUserID, A.RoleID,A.EmpNo, A.EmpName, A.CardNo, A.CellPhone, A.Sex, A.HireDate,
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
                    RoleID = dt.Rows[0][4].ToString(),
                    EmpNo = dt.Rows[0][5].ToString(),
                    EmpName = dt.Rows[0][6].ToString(),
                    CardNo = dt.Rows[0][7].ToString(),
                    CellPhone = dt.Rows[0][8].ToString(),
                    Sex = dt.Rows[0][9].ToString(),
                    HireDate = dt.Rows[0][10].ToString(),
                    Active = dt.Rows[0][11].ToString(),
                    RoleName = dt.Rows[0][12].ToString(),
                    CompanyName = dt.Rows[0][13].ToString(),
                    StoreName = dt.Rows[0][14].ToString()
                };

                
                ViewBag.nickname = CurrentWechatUserInfo.nickname;
                ViewBag.headimgurl = CurrentWechatUserInfo.headimgurl;
                ViewBag.openid = CurrentWechatUserInfo.openid;
                ViewBag.sex = CurrentWechatUserInfo.sex;
                ViewBag.province = CurrentWechatUserInfo.province;
                ViewBag.city = CurrentWechatUserInfo.city;
                ViewBag.country = CurrentWechatUserInfo.country;
                ViewBag.CompanyID = dt.Rows[0][1].ToString();
                ViewBag.CompanyName = dt.Rows[0][13].ToString();
                ViewBag.RoleName = dt.Rows[0][12].ToString();
                ViewBag.LeaderUserID = dt.Rows[0][3].ToString();
                ViewBag.CellPhone = dt.Rows[8][0].ToString();
                ViewBag.UserID = dt.Rows[0][0].ToString();
                ViewBag.EmpNo = dt.Rows[0][5].ToString();
                ViewBag.EmpName = dt.Rows[0][6].ToString();
                ViewBag.StoreName = dt.Rows[0][14].ToString();
                ViewBag.StoreID = dt.Rows[0][2].ToString();
                return View();
            }
            else
            {
                return Content("系统错误，请重新从公众号进入系统！");
            }
        }
    }
}