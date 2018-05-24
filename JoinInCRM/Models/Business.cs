using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinInCRM.Models
{
    public class Business
    {
        public string BusinessID { set; get; }
        public string CustomerID { set; get; }
        public string PlanDate { set; get; }
        public string ActualDate { set; get; }
        public string BusinessDesc { set; get; }
        public string BusinessResult { set; get; }
        public string BusinessAction { set; get; }
        public string BusinessEvent { set; get; }        
        public string NextPlanDate { get; set; }
        public string QuotedID { get; set; }
   
        public string QuotedDate { get; set; }
        public string QuotedCar { get; set; }
        public string QuotedCarColor { get; set; }
        public string QuotedAmount { get; set; }
        public string OrderID { set; get; }
   
        public string OrderNo { set; get; }
        public string OrderDate { set; get; }
        public string OrderCar { set; get; }
        public string OrderCarColor { get; set; }      
        public string OrderAmount { set; get; }

        public string TestCar { get; set; }
        public string TestSatisfaction { get; set; }
     
        public string InterestedCar { get; set; }
        public string InterestedColor  { get; set; }
        public string Grade { get; set; }

    }
    public class Address
    {
        public string IPAddress { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
    }

    public class Quoted
    {
        public string QuotedID { get; set; }
        public string BusinessID { set; get; }
        public string QuotedDate { get; set; }
        public string QuotedCar { get; set; }
        public string QuotedCarColor { get; set; }
        public string QuotedAmount { get; set; }
        public string QuotedRemark { get; set; }
    }

    public class Order
    {
        public string OrderID { set; get; }
        public string BusinessID { set; get; }
        public string CustomerID { get; set; }
        public string OrderNo { set; get; }
        public string OrderDate { set; get; }
        public string OrderCar { set; get; }
        public string OrderCarColor { get; set; }
        public string ColorName { set; get; }
        public string CarStyle { set; get; }
        public string CarName { set; get; }
        public string OrderAmount { set; get; }
        public string OrderStatus { set; get; }
        public string CancelDate { set; get; }
        public string CancelRemark { set; get; } 
        public string DealDate { set; get; }
        public string VINCode { set; get; }
        public string PlateNO { set; get; }
        public string AfterSaleMan { set; get; }
        public string Insure { set; get; }
        public string Loan { set; get; }
 

    }
}