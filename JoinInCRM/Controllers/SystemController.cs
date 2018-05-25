using JoinInCRM.Models;
using System.Web.Mvc;

namespace JoinInCRM.Controllers
{
    public class SystemController : BaseController
    {
        // GET: System/UserAccount 我的账户
        public ActionResult UserAccount()
        {
            return View();
        }
        #region 用户绑定
        // GET: System/UserBinding    
        //[HttpGet]
        //public ActionResult BindingInfo()
        //{
        //    //if (CurrentUserInfo.CompanyID != "")
        //    //{
        //        return View("BindingInfo");
        //    //}
        //    //else
        //    //{
        //    //    DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
        //    //    ViewBag.CompanyList = apiBasecData.GetCompany();

        //    //    return View();
        //    //}

        //}
        [HttpPost]
        public ActionResult UserBinding(FormCollection bindingInfo)
        {
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
                // Save Cusotmer

                User bindUser = new User();


                bindUser.CompanyID = bindingInfo.GetValue("CompanyID").AttemptedValue.Trim();
                bindUser.StoreID = bindingInfo.GetValue("StoreID").AttemptedValue.Trim();
                bindUser.EmpNo = bindingInfo.GetValue("EmpNo").AttemptedValue.Trim();
                bindUser.UserID = CurrentWechatUserInfo.UserID;
                
          
                if (string.IsNullOrEmpty(bindUser.CompanyID) || bindUser.CompanyID == "")
                {
                    return Content("请选择你要绑定的公司！");
                }
                if (string.IsNullOrEmpty(bindUser.StoreID) || bindUser.StoreID == "")
                {
                    return Content("请选择绑定的分部！");
                }

                if (string.IsNullOrEmpty(bindUser.EmpNo) || bindUser.EmpNo == "")
                {
                    return Content("请输入你的工号！");
                }

                DataAPI.BasicDataController dtc = new DataAPI.BasicDataController();

                //获取导入绑定基础信息，由用户维护。

                
            
                if (dtc.CheckBindingUser(bindUser.CompanyID, bindUser.EmpNo) == false)
                {
                    return Content("改工号已被绑定，请检查！");
                }
           
                if (dtc.BindUser(bindUser, LastUser) == "1")
                {
                    return Content("1");
                }
                else
                {
                    return Content("保存失败！");
                }



            }
            catch
            {
                return Content("-1");
            }

        }
        #endregion
 
        // System/BindingInfo       用户绑定信息
        public ActionResult BindingInfo()
        {
            return View();
        }
        // GET: System/CompanyInfo     公司信息
        public ActionResult CompanyInfo()
        {
            return View();
        }
        // GET: System/RoleSettings     角色设置
        public ActionResult RoleSettings()
        {
            return View();
        }

        // GET: System/SourceSettings     来源设置
        public ActionResult SourceSettings()
        {
            return View();
        }

        // GET: System/ModelSettings     车型管理
        public ActionResult ModelSettings()
        {
            return View();
        }
        #region 颜色管理
        // GET: System/ColorSettings     
        public ActionResult ColorSettings()
        {
            //获取基本数据
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            ViewBag.ColorList = apiBasecData.GetColor(CurrentUserInfo.CompanyID);
            return View();
        }
        // GET: System/AddColor
        [HttpGet]
        public ActionResult AddColor()
        {
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();          
            ViewBag.ColorTypeList = apiBasecData.GetColorType(CurrentUserInfo.CompanyID);
            return View();
        }
        [HttpGet]
        public ActionResult EditColor(int ColorID)
        {
            //获取基本数据
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            ViewBag.ColorList = apiBasecData.GetColor(CurrentUserInfo.CompanyID,ColorID);
            ViewBag.ColorTypeList = apiBasecData.GetColorType(CurrentUserInfo.CompanyID);
            return View();

        }
        [HttpPost]
        public ActionResult DeleteColor()
        {
            return View("ColorSettings");
        }
        [HttpPost]
        public ActionResult UpdateColor(FormCollection collection)
        {
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
                // Save Cusotmer
                
                Color color = new Color();
                color.ColorID = collection.GetValue("ColorID").AttemptedValue.Trim();
                color.ColorName= collection.GetValue("ColorName").AttemptedValue.Trim();
                color.ColorType = collection.GetValue("ColorType").AttemptedValue.Trim();
                color.ColorCode = collection.GetValue("ColorCode").AttemptedValue.Trim();
                color.ColorTypeID = collection.GetValue("ColorTypeID").AttemptedValue.Trim();
                color.ColorActive = "0";
                foreach (var item in collection)
                {
                    if (item.ToString()=="ColorActive")
                    {
                        color.ColorActive = collection.GetValue("ColorActive").AttemptedValue.Trim();
                        break;
                    } 
                } 
               
                

                
                if (string.IsNullOrEmpty(color.ColorName) || color.ColorName == "")
                {
                    return Content("请填写颜色名称！");
                }
                if (string.IsNullOrEmpty(color.ColorType) || color.ColorType == "")
                {
                    return Content("请选择颜色类型！");
                }
                DataAPI.BasicDataController dtc = new DataAPI.BasicDataController();
                if (dtc.CheckDuplicateColor(color,CurrentUserInfo.CompanyID) == false)
                {
                    return Content("你录入的颜色已存在系统中！");
                }
                if (color.ColorActive.ToLower()=="on")
                {
                    color.ColorActive = "1";
                }
                if (dtc.EditColor(color, LastUser) == "1")
                {

                    return Content("1");
                }
                else
                {
                    return Content("保存失败！");
                }



            }
            catch
            {
                return Content("-1");
            }
            
        }
        [HttpPost]
        public ActionResult AddColor(FormCollection collection)
        {
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
                // Save Cusotmer

                Color color = new Color();
                color.ColorID = "0";
                color.ColorName = collection.GetValue("ColorName").AttemptedValue.Trim();
                color.ColorType = collection.GetValue("ColorType").AttemptedValue.Trim();
                color.ColorCode = collection.GetValue("ColorCode").AttemptedValue.Trim();
                color.ColorTypeID = collection.GetValue("ColorTypeID").AttemptedValue.Trim();
                color.ColorActive = "0";
                foreach (var item in collection)
                {
                    if (item.ToString() == "ColorActive")
                    {
                        color.ColorActive = collection.GetValue("ColorActive").AttemptedValue.Trim();
                        break;
                    }
                    
                }

                if (string.IsNullOrEmpty(color.ColorName) || color.ColorName == "")
                {
                    return Content("请填写颜色名称！");
                }
                if (string.IsNullOrEmpty(color.ColorType) || color.ColorType == "")
                {
                    return Content("请选择颜色类型！");
                }
                DataAPI.BasicDataController dtc = new DataAPI.BasicDataController();
                if (dtc.CheckDuplicateColor(color,CurrentUserInfo.CompanyID) == false)
                {
                    return Content("你录入的颜色已存在系统中！");
                }
                if (color.ColorActive.ToLower()=="on")
                {
                    color.ColorActive = "1";
                }
                if (dtc.AddColor(color, LastUser) == "1")
                {

                    return Content("1");
                }
                else
                {
                    return Content("保存失败！");
                }



            }
            catch
            {
                return Content("-1");
            }
            
        }
        #endregion
        // GET: System/OtherSettings     其它管理
        public ActionResult OtherSettings()
        {
            return View();
        }

    }
}