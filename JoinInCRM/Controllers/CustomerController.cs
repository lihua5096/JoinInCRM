using JoinInCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JoinInCRM.Controllers
{
    public class CustomerController : BaseController
    {

        // GET: Customer/Details/5
        public ActionResult Details(int id, string  editPermission="YES")
        {
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.SourceList = apiBasecData.GetCustomerSource("");
            ViewBag.CarList = apiBasecData.GetCar();
            ViewBag.ColorList = apiBasecData.GetColor(CurrentUserInfo.CompanyID);
            ViewBag.GradeList = apiBasecData.GetCustomerGrade();
            ViewBag.CustInfo = apiCust.GetCustomerObj(id);
            ViewBag.editPermission = editPermission;
            return View();
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            //获取基本数据
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            ViewBag.SourceList = apiBasecData.GetCustomerSource("");
            ViewBag.CarList = apiBasecData.GetCar();
            ViewBag.ColorList = apiBasecData.GetColor(CurrentUserInfo.CompanyID);
            ViewBag.GradeList = apiBasecData.GetCustomerGrade();

            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            string LastUser = "";
            if (CurrentWechatUserInfo != null)
            {
                LastUser = CurrentWechatUserInfo.openid;

            }
            else
            {
                //return Content("长时间未操作，系统已自动退出，请从微信公众号菜单重新进入！");
                //TODO:
                //LastUser = "oC86Z09y0dkSbyPXzxz6AOGF1U_o";
            }
            try
            {
                // Save Cusotmer
                Customer cust = new Customer();
                cust.CustomerName = collection.GetValue("CustomerName").AttemptedValue.Trim();
                cust.Sex = collection.GetValue("Sex").AttemptedValue.Trim();
                cust.Source = collection.GetValue("Source").AttemptedValue.Trim();
                cust.Telphone = collection.GetValue("Telphone").AttemptedValue.Trim();
                cust.Remark = collection.GetValue("Remark").AttemptedValue.Trim();
                cust.InterestedCar = collection.GetValue("InterestedCar").AttemptedValue.Trim();
                cust.InterestedColor = collection.GetValue("InterestedColor").AttemptedValue.Trim();
                cust.CompetingProduct = collection.GetValue("CompetingProduct").AttemptedValue.Trim();
                cust.Budget = collection.GetValue("Budget").AttemptedValue.Trim();
                cust.LicensePlate = collection.GetValue("LicensePlate").AttemptedValue.Trim();
                cust.CardArea = collection.GetValue("CardArea").AttemptedValue.Trim();
                cust.VehicleRemark = collection.GetValue("VehicleRemark").AttemptedValue.Trim();
                cust.Grade = collection.GetValue("Grade").AttemptedValue.Trim();
                cust.PlanDate = collection.GetValue("PlanDate").AttemptedValue.Trim();
                cust.CompanyID = CurrentUserInfo.CompanyID;
                if (string.IsNullOrEmpty(cust.CustomerName) || cust.CustomerName == "")
                {
                    return Content("请填写姓名！");
                }
                if (string.IsNullOrEmpty(cust.Telphone) || cust.Telphone == "")
                {
                    return Content("请填写电话！");
                }
                DataAPI.CustomerController dtc = new DataAPI.CustomerController();
                if (dtc.CheckDuplicate(cust,CurrentUserInfo.CompanyID) == false)
                {
                    return Content("你录入的该电话号码的客户已存在系统中！");
                }

                if (dtc.AddNew(cust, LastUser) == "1")
                {

                    return Content("1");
                }
                else
                {
                    return Content("0");
                }



            }
            catch
            {
                return Content("-1");
            }
        }

        // GET: Customer/Business
        [HttpGet]

        public ActionResult Individual()
        {
            string OpenID = CurrentWechatUserInfo.openid;
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            if (ViewBag.CustList == null)
            {
                ViewBag.CustList = apiCust.GetCustomerIndividualList(OpenID, 2, "");
            }

            return View();
        }
        [HttpGet]
        public ActionResult Business(int id,string BusinessType="")
        {
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.SourceList = apiBasecData.GetCustomerSource("");
            ViewBag.CarList = apiBasecData.GetCar();
            ViewBag.ColorList = apiBasecData.GetColor(CurrentUserInfo.CompanyID);
            ViewBag.GradeList = apiBasecData.GetCustomerGrade();
            ViewBag.CustInfo = apiCust.GetCustomerObj(id);
            ViewBag.BusinessList = apiCust.GetBusinessList(id);
            ViewBag.OrderList = apiCust.GetOrderList(id);
            ViewBag.BusinessType = BusinessType;
            return View();
        }

        [HttpPost]
        public ActionResult Business(FormCollection collection)
        {
            string LastUser = "";
            if (CurrentWechatUserInfo != null)
            {
                LastUser = CurrentWechatUserInfo.openid;
            }
            else
            {
                return Content("长时间未操作，系统已自动退出，请从微信公众号菜单重新进入！");
                //TODO:
                //LastUser = "oC86Z09y0dkSbyPXzxz6AOGF1U_o";
            }
            try
            {
                // Save Cusotmer
                Business bus = new Business();
                bus.BusinessID = collection.GetValue("BusinessID").AttemptedValue.Trim();
                bus.PlanDate = collection.GetValue("PlanDate_Hidden").AttemptedValue.Trim();
                bus.ActualDate = collection.GetValue("ActualDate").AttemptedValue.Trim();
                //bus.BusinessAction = collection.GetValue("BusinessAction").AttemptedValue.Trim();
                bus.BusinessDesc = collection.GetValue("BusinessDesc").AttemptedValue.Trim();
                //bus.BusinessResult = collection.GetValue("BusinessResult").AttemptedValue.Trim();
                bus.InterestedCar = collection.GetValue("InterestedCar").AttemptedValue.Trim();
                bus.InterestedColor = collection.GetValue("InterestedColor").AttemptedValue.Trim();

                bus.CustomerID = collection.GetValue("CustomerID").AttemptedValue.Trim();

                DataAPI.CustomerController dtc = new DataAPI.CustomerController();

                if (string.IsNullOrEmpty(collection.GetValue("BusinessEvent").AttemptedValue.Trim()))
                {
                    return Content("请选择营业事件！");
                }
                else
                {
                    bus.BusinessEvent = collection.GetValue("BusinessEvent").AttemptedValue.Trim();
                }
                if (string.IsNullOrEmpty(collection.GetValue("Grade").AttemptedValue.Trim()))
                {
                    return Content("请选择客户等级！");
                }
                else
                {
                    bus.Grade = collection.GetValue("Grade").AttemptedValue.Trim();
                }

                if (string.IsNullOrEmpty(bus.BusinessDesc) || bus.BusinessDesc == "")
                {
                    return Content("请填写营业经过！");
                }
                if (string.IsNullOrEmpty(collection.GetValue("NextPlanDate").AttemptedValue.Trim()))
                {
                    bus.NextPlanDate =null;
                }
                else
                {
                    bus.NextPlanDate = collection.GetValue("NextPlanDate").AttemptedValue.Trim();
                }

                string savetype = collection.GetValue("mode").AttemptedValue.Trim();
                if (string.IsNullOrEmpty(bus.ActualDate) || bus.ActualDate == "")
                {
                    bus.ActualDate = DateTime.Now.ToString("yyyy-MM-dd");
                }

                //下订
                if (bus.BusinessEvent == "下订")
                {
                    bus.OrderAmount = collection.GetValue("OrderAmount").AttemptedValue.Trim();
                    bus.OrderCar = collection.GetValue("InterestedCar").AttemptedValue.Trim(); 
                    bus.OrderCarColor= collection.GetValue("InterestedColor").AttemptedValue.Trim();
                    bus.OrderNo = GenerateOrderNo();
                    //检查当前有无尚未处理订单
                    if (dtc.CheckOrderStatus(int.Parse(bus.CustomerID))==false)
                    {
                        return Content("当前客户有未处理完订单，请先处理订单！");
                    }


                }
                //报价
                if (bus.BusinessEvent == "报价")
                {
                    bus.QuotedAmount = collection.GetValue("QuotedAmount").AttemptedValue.Trim();
                    bus.QuotedCar = collection.GetValue("InterestedCar").AttemptedValue.Trim();
                    bus.QuotedCarColor = collection.GetValue("InterestedColor").AttemptedValue.Trim();
                }

                if (bus.BusinessEvent == "试驾")
                {
                    bus.TestCar= collection.GetValue("TestCar").AttemptedValue.Trim();
                    bus.TestSatisfaction = collection.GetValue("TestSatisfaction").AttemptedValue.Trim();
                }
                else
                {
                    bus.TestCar = "-1";
                    bus.TestSatisfaction = "";
                }

                
                //if (dtc.CheckDuplicate(bus) == false)
                //{
                //    return Content("你录入的该电话号码的客户已存在系统中！");
                //}
                string re = "0";
                if (savetype == "edit")
                {
                    re = dtc.EditBus(bus, LastUser);
                }
                else if (savetype == "add")
                {
                    re = dtc.AddBus(bus, LastUser);
                }
                else if (savetype == "del")
                {

                }

                if (re == "1")
                {
                    return Content("1");
                }
                else
                {
                    return Content("0");
                }

            }
            catch
            {
                return Content("-1");
            }

        }

        [HttpGet]
        public ActionResult BusinessDetail(int id)
        {
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.BusinessDetail = apiCust.GetBusinessDetail(id);
            return View();
        }

        [HttpGet]
        public ActionResult Order(int OrderID,string editType="")
        {
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();         
            ViewBag.OrderList = apiCust.GetOrderDetail(OrderID);
            ViewBag.editType = editType;
            return View(); 
        }
        [HttpGet]
        public ActionResult OrderDeal(int OrderID)
        {
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.OrderList = apiCust.GetOrderDetail(OrderID);
            return View();

        }
        [HttpPost]
        public ActionResult OrderDeal(FormCollection collection)
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
                // Save order
                Order order = new Order();
                order.CustomerID = collection.GetValue("CustomerID").AttemptedValue.Trim();
                order.OrderID = collection.GetValue("OrderID").AttemptedValue.Trim();
                order.DealDate = collection.GetValue("DealDate").AttemptedValue.Trim();
                order.VINCode = collection.GetValue("VINCode").AttemptedValue.Trim(); 
                order.PlateNO = collection.GetValue("PlateNO").AttemptedValue.Trim(); 
                order.AfterSaleMan = collection.GetValue("AfterSaleMan").AttemptedValue.Trim();
                order.Loan = collection.GetValue("Loan").AttemptedValue.Trim();
                order.Insure = collection.GetValue("Insure").AttemptedValue.Trim();
                DataAPI.CustomerController dtc = new DataAPI.CustomerController();
                  
                string re = "0";

                if (dtc.GetOrderStatus(int.Parse(order.OrderID)) ==1)
                {
                    return Content("该订单已交车！");
                }
                if (dtc.GetOrderStatus(int.Parse(order.OrderID)) ==2)
                {
                    return Content("该订单已取消！");
                }

                re =dtc.EditOrder(order);

                if (re == "1")
                {
                    return Content("1");
                }
                else
                {
                    return Content("0");
                }

            }
            catch
            {
                return Content("-1");
            }
        }
        [HttpGet]
        public ActionResult OrderCancel(int OrderID) {
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.GradeList = apiBasecData.GetCustomerGrade();
            ViewBag.OrderList = apiCust.GetOrderDetail(OrderID);
            return View();
        }
        [HttpPost]
        public ActionResult OrderCancel(FormCollection collection)
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
                // Save order
                Order order = new Order();
                order.CustomerID = collection.GetValue("CustomerID").AttemptedValue.Trim();
                order.OrderID = collection.GetValue("OrderID").AttemptedValue.Trim();
                order.CancelDate = collection.GetValue("CancelDate").AttemptedValue.Trim();
                order.CancelRemark = collection.GetValue("CancelRemark").AttemptedValue.Trim();
                Customer cust = new Customer();

                cust.CustomerID=int.Parse( collection.GetValue("CustomerID").AttemptedValue.Trim());
                cust.Grade= collection.GetValue("Grade").AttemptedValue.Trim();
                cust.NextPlanDate = collection.GetValue("NextPlanDate").AttemptedValue.Trim();

                Business bus = new Business();
                bus.NextPlanDate= collection.GetValue("NextPlanDate").AttemptedValue.Trim();
                bus.CustomerID= collection.GetValue("CustomerID").AttemptedValue.Trim();
                bus.BusinessDesc= collection.GetValue("CancelRemark").AttemptedValue.Trim();
                bus.BusinessEvent= collection.GetValue("BusinessEvent").AttemptedValue.Trim();
                bus.Grade= collection.GetValue("Grade").AttemptedValue.Trim();
                 

                DataAPI.CustomerController dtc = new DataAPI.CustomerController();

                string re = "0";
                if (dtc.GetOrderStatus(int.Parse(order.OrderID)) == 1)
                {
                    return Content("该订单已交车！");
                }
                if (dtc.GetOrderStatus(int.Parse(order.OrderID)) == 2)
                {
                    return Content("该订单已取消！");
                }

                re = dtc.EditOrder(order,cust,bus, LastUser);

                if (re == "1")
                {
                    return Content("1");
                }
                else
                {
                    return Content("0");
                }

            }
            catch
            {
                return Content("-1");
            }
        }
        private string GenerateOrderNo()
        {
            string OrderNo = "";
            int len = CurrentWechatUserInfo.openid.Length;
            OrderNo = "_"+ CurrentWechatUserInfo.openid.Substring(len - 5, 5)+ DateTime.Now.ToString("yyyyMMddHHMMss");
            return OrderNo;
        }

        // GET: Customer/BusinessEdit/5
        public ActionResult CreateBusiness(int id)
        {
            
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.SourceList = apiBasecData.GetCustomerSource("");
            ViewBag.CarList = apiBasecData.GetCar();
            ViewBag.ColorList = apiBasecData.GetColor(CurrentUserInfo.CompanyID);
            ViewBag.GradeList = apiBasecData.GetCustomerGrade();
            ViewBag.CustInfo = apiCust.GetCustomerObj(id);
            // ViewBag.BusinessList = apiCust.GetBusinessList(id);
            ViewBag.NextPlanDate = apiCust.GetCustNextPlanDate(id);
            return View();
          
        }

        // POST: Customer/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            string LastUser = "";
            if (CurrentWechatUserInfo != null)
            {
                LastUser = CurrentWechatUserInfo.openid;

            }
      
            try
            {
                // Save Cusotmer
                Customer cust = new Customer();
                cust.CustomerID =int.Parse(collection.GetValue("CustomerID").AttemptedValue.Trim());
                cust.CustomerName = collection.GetValue("CustomerName").AttemptedValue.Trim();
                cust.Sex = collection.GetValue("Sex").AttemptedValue.Trim();
                cust.Source = collection.GetValue("Source").AttemptedValue.Trim();
                cust.Telphone = collection.GetValue("Telphone").AttemptedValue.Trim();
                cust.Remark = collection.GetValue("Remark").AttemptedValue.Trim();
                cust.InterestedCar = collection.GetValue("InterestedCar").AttemptedValue.Trim();
                cust.InterestedColor = collection.GetValue("InterestedColor").AttemptedValue.Trim();
                cust.CompetingProduct = collection.GetValue("CompetingProduct").AttemptedValue.Trim();
                cust.Budget = collection.GetValue("Budget").AttemptedValue.Trim();
                cust.LicensePlate = collection.GetValue("LicensePlate").AttemptedValue.Trim();
                cust.CardArea = collection.GetValue("CardArea").AttemptedValue.Trim();
                cust.VehicleRemark = collection.GetValue("VehicleRemark").AttemptedValue.Trim();
                cust.Grade = collection.GetValue("Grade").AttemptedValue.Trim();
                cust.CompanyID = CurrentUserInfo.CompanyID;
                //cust.PlanDate = collection.GetValue("PlanDate").AttemptedValue.Trim();
                if (string.IsNullOrEmpty(cust.CustomerName) || cust.CustomerName == "")
                {
                    return Content("请填写姓名！");
                }
                if (string.IsNullOrEmpty(cust.Telphone) || cust.Telphone == "")
                {
                    return Content("请填写电话！");
                }
                DataAPI.CustomerController dtc = new DataAPI.CustomerController();
                if (dtc.CheckDuplicate(cust,CurrentUserInfo.CompanyID) == false)
                {
                    return Content("你录入的该电话号码的客户已存在系统中！");
                }

                if (dtc.Edit(cust, LastUser) == "1")
                {

                    return Content("1");
                }
                else
                {
                    return Content("0");
                }



            }
            catch
            {
                return Content("-1");
            }
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(int id)
        {
            DataAPI.CustomerController dapi = new DataAPI.CustomerController();
            if (dapi.Delete(id))
            {
                return Content("1");
            }
            else
            {
                return Content("0");
            }

        }

        // POST: Customer/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult SearchCustomer()
        {
            string OpenID = CurrentWechatUserInfo.openid;
            string SearchAllTxt = Request.QueryString["AllCustSearchContent"];
            string SearchBookedTxt = Request.QueryString["BookedSearchContent"];
            string SearchHoldingTxt = Request.QueryString["HoldingSearchContent"];
            string SearchNoRecordTxt = Request.QueryString["NoRecordSearchContent"];
            string SearchUnFinishTxt = Request.QueryString["UnFinishSearchContent"];
            string SearchIndexTxt = Request.QueryString["IndexSearchContent"];
            string SearchReturnOrderTxt = Request.QueryString["ReturnOrderSearchContent"];
            string SearchReturnCarTxt = Request.QueryString["ReturnCarSearchContent"];
            string SearchFailedTxt = Request.QueryString["FailedSearchContent"];
            string SearchIndividualTxt = Request.QueryString["Individual"];

            int custStatus = -1;
            string returnViewName = "";
            string SearchTxt = "";

            if (!string.IsNullOrEmpty(SearchAllTxt))
            {
                returnViewName = "AllCust";        
                SearchTxt = SearchAllTxt;
            }

            if (!string.IsNullOrEmpty(SearchNoRecordTxt))
            {
                returnViewName = "NoRecord";
                custStatus =8;
                SearchTxt = SearchNoRecordTxt;
            }
            if (!string.IsNullOrEmpty(SearchUnFinishTxt))
            {
                returnViewName = "UnFinish";
                custStatus = 1;
                SearchTxt = SearchUnFinishTxt;
            }

            
            if (!string.IsNullOrEmpty(SearchIndexTxt))
            {
                returnViewName = "Index";
                custStatus = 2;
                SearchTxt = SearchIndexTxt;
            }
            if (!string.IsNullOrEmpty(SearchIndividualTxt))
            {
                returnViewName = "Individual";
                custStatus = 2;
                SearchTxt = SearchIndividualTxt;
            }
            

            if (!string.IsNullOrEmpty(SearchBookedTxt))
            {
                returnViewName = "Booked";
                custStatus = 3;
                SearchTxt = SearchBookedTxt;
            }
             
            if (!string.IsNullOrEmpty(SearchReturnOrderTxt))
            {
                returnViewName = "ReturnOrder";
                custStatus = 4;
                SearchTxt = SearchReturnOrderTxt;
            }
            

            if (!string.IsNullOrEmpty(SearchHoldingTxt))
            {
                returnViewName = "Holding";
                custStatus = 5;
                SearchTxt = SearchHoldingTxt;
            }
            if (!string.IsNullOrEmpty(SearchReturnCarTxt))
            {
                returnViewName = "ReturnCar";
                custStatus = 6;
                SearchTxt = SearchReturnCarTxt;
            }

            if (!string.IsNullOrEmpty(SearchFailedTxt))
            {
                returnViewName = "Failed";
                custStatus = 7;
                SearchTxt = SearchFailedTxt;
            }
            
 

            if (!String.IsNullOrEmpty(SearchTxt) || SearchTxt != "")
            {

                DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
                if (returnViewName == "AllCust")
                {
                    ViewBag.CustList = apiCust.GetCustomerList(SearchTxt);
                }
                else
                {
                    ViewBag.CustList = apiCust.GetCustomerIndividualList(OpenID, custStatus, SearchTxt);
                }

                int count = ((Array)ViewBag.CustList).Length;
                if (count > 0)
                {
                    ViewBag.Result = count.ToString() + " 条记录。";
                }
                else
                {
                    ViewBag.Result = "";
                }
            }
            else
            {

                ViewBag.SearchContent = "";
                ViewBag.CustList = "";
                ViewBag.Result = "";
            }
            return View(returnViewName);
        }

        public ActionResult CustomerBusList(int CustomerID)
        {
            string content = "";
            if (CustomerID > 0)
            {
                DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
                ViewBag.buslist = apiCust.GetBusinessList(CustomerID);

                foreach (var item in ViewBag.buslist)
                {
                    content += "<tr>";
                    content += "<td>" + item.PlanDate + @"</td>";
                    content += "<td>" + item.ActualDate + @"</td>";
                    content += "<td>" + item.BusinessResult + @"</td>";
                    content += "<td class=\"action_col\">";
                    content += "<a href = \"javascript:;\" style = \"align-self:center;\">";
                    content += "<i class=\"icon icon-edit2\" onclick=\"EditBus(" + item.BusinessID + ")\" style=\"color:chocolate;\">" + @"</i>";
                    content += @"</a>";
                    content += @"</td>";
                    content += @"</tr>";
                }
                content = "<table>" + content;
                content += @"</table>";
            }
            else
            {
                content = "<table>" + content;
                content += "@</table>";
            }
            return Content(content);
        }
 
        //未留档    按用户  Status=8
        [HttpGet]
        public ActionResult NoRecord()
        {
            string OpenID = CurrentWechatUserInfo.openid;
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.CustList = apiCust.GetCustomerIndividualList(OpenID, 8, "");
            ViewBag.Result = ((Array)ViewBag.CustList).Length.ToString() + " 条记录。";
            return View();
        }
        //未完善    按用户  Status=1
        [HttpGet]
        public ActionResult UnFinish()
        {
            string OpenID = CurrentWechatUserInfo.openid;
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.CustList = apiCust.GetCustomerIndividualList(OpenID, 1, "");
            ViewBag.Result = ((Array)ViewBag.CustList).Length.ToString() + " 条记录。";
            return View();
        }
        //意向     按用户  Status=2 
        [HttpGet]
        public ActionResult Index()
        { 
            string OpenID = CurrentWechatUserInfo.openid;
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.CustList = apiCust.GetCustomerIndividualList(OpenID, 2, "");
            ViewBag.Result = ((Array)ViewBag.CustList).Length.ToString() + " 条记录。";
            return View();
        }
        //已订      按用户  Status=3
        [HttpGet]
        public ActionResult Booked()
        {
            string OpenID = CurrentWechatUserInfo.openid;
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.CustList = apiCust.GetCustomerIndividualList(OpenID, 3, "");
            ViewBag.Result = ((Array)ViewBag.CustList).Length.ToString() + " 条记录。";
            return View();
        }
        //退订      按用户  Status=4
        [HttpGet]
        public ActionResult ReturnOrder()
        {
            string OpenID = CurrentWechatUserInfo.openid;
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.CustList = apiCust.GetCustomerIndividualList(OpenID, 4, "");
            ViewBag.Result = ((Array)ViewBag.CustList).Length.ToString() + " 条记录。";
            return View();
        }
        //保有      按用户  Status=5
        [HttpGet]
        public ActionResult Holding()
        {
            string OpenID = CurrentWechatUserInfo.openid;
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.CustList = apiCust.GetCustomerIndividualList(OpenID, 5, "");
            ViewBag.Result = ((Array)ViewBag.CustList).Length.ToString() + " 条记录。";
            return View();
        }
        //退车      按用户  Status=6
        [HttpGet]
        public ActionResult ReturnCar()
        {
            string OpenID = CurrentWechatUserInfo.openid;
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.CustList = apiCust.GetCustomerIndividualList(OpenID, 6, "");
            ViewBag.Result = ((Array)ViewBag.CustList).Length.ToString() + " 条记录。";
            return View();
        }
        //战败      按用户  Status=7
        [HttpGet]
        public ActionResult Failed()
        {
            string OpenID = CurrentWechatUserInfo.openid;
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.CustList = apiCust.GetCustomerIndividualList(OpenID, 7, "");
            ViewBag.Result = ((Array)ViewBag.CustList).Length.ToString() + " 条记录。";
            return View();
        }
        //所有客户  不分用户
        [HttpGet]
        public ActionResult AllCust()
        {
            DataAPI.CustomerController apiCust = new DataAPI.CustomerController();
            ViewBag.CustList = apiCust.GetCustomerList();
            ViewBag.Result = ((Array)ViewBag.CustList).Length.ToString() + " 条记录。";

            return View();
        }
    }
}
