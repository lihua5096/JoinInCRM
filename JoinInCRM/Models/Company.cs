using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinInCRM.Models
{
    /// <summary>
    /// 公司信息
    /// </summary>
    public class Company  
    {
        public string CompanyID { get; set; } 
	    public string CompanyName { get; set; }
        public string LinkMan { get; set; }
        public string Tel { get; set; }
        public string Addr { get; set; }
        public string Active { get; set; }
        public string Remark { get; set; }
    }
    /// <summary>
    /// 门店信息
    /// </summary>
    public class Store   
    {
        public string StoreID { get; set; }
        public string CompanyID { get; set; }
        public string StoreName { get; set; }
        public string Active { get; set; }
    }
}