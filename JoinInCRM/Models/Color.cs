using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoinInCRM.Models
{
    public class Color
    {
        public string ColorID { get; set; }
        public string ColorTypeID { get; set; }
        public string ColorType { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public string ColorActive { get; set; } 
        public string ColorStatus { get; set; }
    }
    public class ColorType
    {
        public string ColorTypeID { get; set; }
        public string ColorTypeName { get; set; }
    }
}