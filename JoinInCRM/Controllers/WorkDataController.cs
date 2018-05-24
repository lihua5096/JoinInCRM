using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JoinInCRM.Controllers
{
    public class WorkDataController : Controller
    {
        // GET: WorkData/Basic  基础数据分析
        public ActionResult BasicAnalysis()
        {
            return View();
        }
        // 英雄榜
        public ActionResult HeroList()
        {
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            ViewBag.SourceList = apiBasecData.GetCustomerSource("");
            ViewBag.CarList = apiBasecData.GetCar();
            return View();
        }
        // 效率榜
        public ActionResult Efficiency()
        {
            return View();
        }
        // 勤奋榜
        public ActionResult HardWorking()
        {
            return View();
        }
        // 进店榜
        public ActionResult IntoStore()
        {
            return View();
        }

        // 短信榜
        public ActionResult MessageList()
        {
            return View();
        }
        // 客户总览
        public ActionResult OverView()
        {
            return View();
        }

        // 意向客户分析表
        public ActionResult Intention()
        {
            return View();
        }

        // 基盘留档战败表
        public ActionResult BaseFailed()
        {
            return View();
        }

        // 顾问绩效指标
        public ActionResult KPI()
        {
            return View();
        }

        // 订单分析
        public ActionResult OrderAnalysis()
        {
            return View();
        }


        // 客户来源分析
        public ActionResult SourceAnalysis()
        {
            //获取基本数据
            DataAPI.BasicDataController apiBasecData = new DataAPI.BasicDataController();
            ViewBag.SourceList = apiBasecData.GetCustomerSource("");
            ViewBag.CarList = apiBasecData.GetCar();
    
            return View();
        }

    }
}