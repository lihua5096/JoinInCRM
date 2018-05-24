using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinInCRM.Models
{
    public class User
    {
        public string UserID { get; set; }
        public string CompanyID { get; set; }
        public string StoreID { get; set; }
        public string RoleID { get; set; }
        public string LeaderUserID { get; set; }
        public string CompanyName { get; set; }
        public string StoreName { get; set; }
        public string CellPhone { get; set; }
        public string Sex { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public string CardNo { get; set; }
        public string RoleName { get; set; }
        public string HireDate { get; set; }
        public string Active { get; set; }
 
    }
}