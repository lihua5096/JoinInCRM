using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinInCRM.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Sex { get; set; }
        public string BirthDay { get; set; }
        public string Telphone { get; set; }
        public string Source { get; set; }
        public string Grade { get; set; }
        public string Salesman { get; set; }
        public string InterestedCar { get; set; }
        public string InterestedColor { get; set; }
        public string CompetingProduct { get; set; }
        public string Budget { get; set; }
        public string LicensePlate { get; set; }
        public string CardArea { get; set; }
        public string Remark { get; set; }
        public string VehicleRemark { get; set; }
        public string CreateDate { get; set; }
        public string PlanDate { get; set; }
        public string LastUpdate { get; set; }
        public string LastUser { get; set; }
        public string Status { get; set; }
        public string SortLetter { set; get; }
        public string NextPlanDate { get; set; }
        public string CompanyID { get; set; } //进入公共池后将不属于saleman，该公司的各个顾问都可抢客户

    }
    public class CustomerGrade {
        public string GradeID { get; set; }
        public string GradeName { get; set; }
        public string GradeDesc { get; set; }
    }
}