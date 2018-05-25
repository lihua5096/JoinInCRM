using JoinInCRM.Helpers;
using JoinInCRM.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System;
using System.Data.SqlClient;

namespace JoinInCRM.DataAPI
{
    public class BasicDataController : ApiController
    {
        #region 公司信息
        public IEnumerable<Company> GetCompany(string CompanyID="")
        {
            return GetCompanyList(CompanyID).ToArray<Company>();
        }
        private List<Company> GetCompanyList(string companyID)
        {
            List<Company> cl = new List<Company>();
            SQLHelper dbo = new SQLHelper();
            string sqlStr = "select CompanyID, CompanyName, LinkMan, Tel, Addr, Active, Remark  from company where active=1 ";
            if (companyID!="")
            {
                sqlStr += " and CompanyID=" + companyID;
            }
    
            sqlStr = sqlStr + " Order By CompanyName";
            DataTable dt = dbo.getsqlDatable(sqlStr);
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(
                    new Company
                    {
                        CompanyID= item["CompanyID"].ToString(),
                        CompanyName= item["CompanyName"].ToString(),
                        Active= item["Active"].ToString(),
                        LinkMan= item["LinkMan"].ToString(),
                        Tel= item["Tel"].ToString(),
                        Addr= item["Addr"].ToString(),
                        Remark= item["Remark"].ToString()

                    }
                    );
            }
            return cl;
        }
        public IEnumerable<Store> GetStore(string CompanyID = "")
        {
            return GetStoreList(CompanyID).ToArray<Store>();
        }
        private List<Store> GetStoreList(string companyID)
        {
            List<Store> cl = new List<Store>();
            SQLHelper dbo = new SQLHelper();
            string sqlStr = " select StoreID, CompanyID, StoreName, Active from store where Active=1  ";
            if (companyID != "")
            {
                sqlStr += " and CompanyID=" + companyID;
            }

            sqlStr = sqlStr + " Order By StoreName";
            DataTable dt = dbo.getsqlDatable(sqlStr);
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(
                    new Store
                    {
                        StoreID = item["StoreID"].ToString(),
                        CompanyID = item["CompanyID"].ToString(),
                        StoreName = item["StoreName"].ToString(),
                        Active = item["Active"].ToString() 
                    }
                    );
            }
            return cl;
        }


        #endregion
        #region 用户信息
        public IEnumerable<User> GetUser(int UserID)
        {
            return GetUserList(UserID).ToArray<User>();
        }

        public IEnumerable<User> GetUser(string OpenID)
        {
            return GetUserList(0,OpenID).ToArray<User>();
        }

        public List<User> GetUserList(int UserID=0,string OpenID =""){
            List<User> cl = new List<User>();
            SQLHelper dbo = new SQLHelper();
            string sqlStr = "";
            if (UserID > 0)
            {
                sqlStr = @"SELECT A.UserID, A.CompanyID, A.LeaderUserID, A.RoleID,B.CompanyName,A.EmpNo, A.EmpName, A.CardNo, A.CellPhone, A.Sex, A.HireDate,C.RoleName,
                                     CASE WHEN A.Active = 1 THEN '在职' ELSE '离职' END AS Active
                              FROM USERS A 
                              INNER JOIN Company B ON A.CompanyID = B.CompanyID
                              INNER JOIN [Role] C ON A.RoleID = C.RoleID 
                              Where A.UserID=" + UserID.ToString();
            }
            if (OpenID!="")
            {
                sqlStr = @"SELECT A.UserID, A.CompanyID, A.LeaderUserID, A.RoleID,B.CompanyName,A.EmpNo, A.EmpName, A.CardNo, A.CellPhone, A.Sex, A.HireDate,C.RoleName,
                                     CASE WHEN A.Active = 1 THEN '在职' ELSE '离职' END AS Active
                              FROM USERS A 
                              INNER JOIN Company B ON A.CompanyID = B.CompanyID
                              INNER JOIN [Role] C ON A.RoleID = C.RoleID
                              INNER JOIN WechatUsers D ON A.UserID=D.UserID
                              Where D.OpenID='" + OpenID.ToString()+"'";
            }

           // sqlStr = sqlStr + " Order By EmpNo,EmpName";
            DataTable dt = dbo.getsqlDatable(sqlStr);
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(
                    new User
                    {
                        UserID= item["UserID"].ToString(),
                        LeaderUserID= item["LeaderUserID"].ToString(),
                        CompanyID= item["CompanyID"].ToString(),
                        RoleID= item["RoleID"].ToString(),
                        EmpNo= item["EmpNo"].ToString(),
                        EmpName= item["EmpName"].ToString(),
                        CardNo= item["CardNo"].ToString(),
                        CellPhone= item["CellPhone"].ToString(),
                        Sex= item["Sex"].ToString(),
                        HireDate= item["HireDate"].ToString(),
                        Active= item["Active"].ToString(),
                        RoleName= item["RoleName"].ToString(),
                        CompanyName= item["CompanyName"].ToString()

                    }
                    );
            }
            return cl;
        }

        public bool CheckBindingUser(string CompanyID,string EmpNo)
        {
            SQLHelper dbo = new SQLHelper();
            string sqlStr = "Select UserID FROM Users Where CompanyID=" + CompanyID + " And  EmpNo='"+EmpNo+"'";
             
            if (dbo.ReturnInteger(sqlStr, 0) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public string BindUser(User bindUser,string LastUser)
        {

            return "1";
        }

        #endregion
        #region 客户来源        
        public IEnumerable<CustomerSource> GetCustomerSource(string searchStr)
        {
            return GetSource(searchStr).ToArray<CustomerSource>();
        }
        private List<CustomerSource> GetSource(string searchStr)
        {
            List<CustomerSource> cl = new List<CustomerSource>();
            SQLHelper dbo = new SQLHelper();
            string sqlStr = "Select SourceID,SourceName,SourceDesc From CustomerSource";
            if (string.IsNullOrEmpty(searchStr) && searchStr != "")
            {
                sqlStr += " Where SourceName =" + dbo.DBStr(searchStr);
            }
            sqlStr += "  Order by SourceName";

            DataTable dt = dbo.getsqlDatable(sqlStr);
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(
                    new CustomerSource
                    {
                        SourceID = item["SourceID"].ToString(),
                        SourceName = item["SourceName"].ToString(),
                        SourceDesc = item["SourceDesc"].ToString()
                    }
                    );
            }
            return cl;

        }
        #endregion
        #region 车辆管理

        public IEnumerable<Car> GetCar()
        {
            return GetCarList().ToArray<Car>();
        }
        private List<Car> GetCarList()
        {
            List<Car> cl = new List<Car>();
            SQLHelper dbo = new SQLHelper();
            string sqlStr = "select CarID,CarType,CarStyle,CarName from car order by CarType,CarStyle,CarName";
            DataTable dt = dbo.getsqlDatable(sqlStr);
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(
                    new Car
                    {
                        CarID = item["CarID"].ToString(),
                        CarStyle = item["CarStyle"].ToString(),
                        CarType = item["CarType"].ToString(),
                        CarName = item["CarName"].ToString()
                    }
                    );
            }
            return cl;

        }

        #endregion
        #region 颜色管理
        public IEnumerable<Color> GetColor(string CompanyID,int ColorID = 0)
        {
            return GetColorList(CompanyID,ColorID).ToArray<Color>();
        }
        private List<Color> GetColorList(string CompanyID,int ColorID)
        {
            List<Color> cl = new List<Color>();
            SQLHelper dbo = new SQLHelper();

            string sqlStr = "select A.ColorID,ColorType,A.ColorName,A.ColorCode,A.ColorActive,Case WHEN A.ColorActive=1 THEN '已启用' ELSE '未启用' END AS ColorStatus from Color A INNER JOIN ColorType B "+
                            " On A.ColorType = B.ColorTypeID "+
                            " WHERE B.CompanyID = " +CompanyID;
            if (ColorID > 0)
            {
                sqlStr = sqlStr + " AND ColorID=" + ColorID.ToString();
            }

            sqlStr = sqlStr + " Order By ColorType,ColorName";
            DataTable dt = dbo.getsqlDatable(sqlStr);
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(
                    new Color
                    {
                        ColorID = item["ColorID"].ToString(),
                        ColorType = item["ColorType"].ToString(),
                        ColorName = item["ColorName"].ToString(),
                        ColorCode = item["ColorCode"].ToString(),
                        ColorActive=item["ColorActive"].ToString(),
                        ColorStatus=item["ColorStatus"].ToString()
                    }
                    );
            }
            return cl;

        }
        public bool CheckDuplicateColor(Color color,string CompanyID)
        {
            SQLHelper dbo = new SQLHelper();
            string sqlStr = "Select A.ColorID From [Color] A INNER JOIN ColorType B on A.ColorTypeID=B.ColorTypeID where B.CompanyID="+CompanyID+"  A.ColorName=" + dbo.DBStr(color.ColorName);
            int colorID = 0;
            int.TryParse(color.ColorID, out colorID);
            if (colorID > 0)
            {
                sqlStr += " And ColorID<>" + colorID.ToString();
            }
            if (dbo.ReturnInteger(sqlStr, 0) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
             
        }
        public string AddColor(Color color, string LastUser)
        {
            string result = "";

            if (string.IsNullOrEmpty(LastUser) || LastUser == "")
            {
                return "0";
            }
            else
            {
                //SQLHelper dbase = new SQLHelper();
                //int res = dbase.ReturnInteger("Select UserID FROM [dbo].[WechatUsers] WHERE OpenID=" + dbase.DBStr(LastUser), -1);

                //if (res > 0)
                //{
                //    LastUser = res.ToString();
                //}
                //else
                //{
                //    return "0";
                //}
            }

            string sqlStr = "Insert Into [Color](ColorName,ColorTypeID,ColorType,ColorCode,ColorActive) " +
                            " values (@ColorName,@ColorTypeID,@ColorType,@ColorCode,@ColorActive)";

            SqlParameter[] para = {new SqlParameter("ColorName", color.ColorName),
                                   new SqlParameter("ColorTypeID",color.ColorTypeID),
                                   new SqlParameter("ColorType", color.ColorType),
                                   new SqlParameter("ColorCode", color.ColorCode),
                                   new SqlParameter("ColorActive",color.ColorActive) 

            };


            SQLHelper dbo = new SQLHelper();
            int re = dbo.ExecuteNonQuery(sqlStr, para);
            if (re == 1)
            {
                result = "1";
  
            }
            else
            {
                result = "0";
            }

            return result;
        }
        public string EditColor(Color color, string LastUser)
        {
            string result = "";

            if (string.IsNullOrEmpty(LastUser) || LastUser == "")
            {
                return "0";
            }
            else
            {
                //SQLHelper dbase = new SQLHelper();
                //int res = dbase.ReturnInteger("Select UserID FROM [dbo].[WechatUsers] WHERE OpenID=" + dbase.DBStr(LastUser), -1);

                //if (res > 0)
                //{
                //    LastUser = res.ToString();
                //}
                //else
                //{
                //    return "0";
                //}
            }

            string sqlStr = " Update a Set ColorName=@ColorName,ColorTypeID=@ColorTypeID, " +
                            " ColorType=@ColorType,ColorCode=@ColorCode, ColorActive=@ColorActive " +
                            " From [Color] a " +
                            " Where ColorID=@ColorID";

            SqlParameter[] para = {new SqlParameter("ColorName",color.ColorName),
                                   new SqlParameter("ColorTypeID",color.ColorTypeID),
                                   new SqlParameter("ColorType", color.ColorType),
                                   new SqlParameter("ColorCode", color.ColorCode),
                                   new SqlParameter("ColorActive", color.ColorActive),
                                   new SqlParameter("ColorID",color.ColorID) 
            };
            SQLHelper dbo = new SQLHelper();
            int re = dbo.ExecuteNonQuery(sqlStr, para);
            if (re == 1)
            {
                result = "1";
            }
            else
            {
                result = "0";
            }

            return result;
        }
        public IEnumerable<ColorType> GetColorType(string CompanyID,int ColorTypeID = 0)
        {
            return GetColorTypeList(ColorTypeID,CompanyID).ToArray<ColorType>();
        }
        private List<ColorType> GetColorTypeList(int ColorTypeID,string CompanyID)
        {
            List<ColorType> cl = new List<ColorType>();
            SQLHelper dbo = new SQLHelper();

            string sqlStr = "Select ColorTypeID,ColorTypeName from ColorType Where CompanyID="+CompanyID;
            if (ColorTypeID > 0)
            {
                sqlStr = sqlStr + " And ColorTypeID=" + ColorTypeID.ToString();
            }
            sqlStr = sqlStr + " Order By ColorTypeName";
            DataTable dt = dbo.getsqlDatable(sqlStr);
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(
                    new ColorType
                    {
                        ColorTypeID = item["ColorTypeID"].ToString(),
                        ColorTypeName = item["ColorTypeName"].ToString()
                    }
                    );
            }
            return cl;

        }
        #endregion
        #region 客户等级

        public IEnumerable<CustomerGrade> GetCustomerGrade()
        {
            return GetGradeList().ToArray<CustomerGrade>();
        }
        private List<CustomerGrade> GetGradeList()
        {
            List<CustomerGrade> cl = new List<CustomerGrade>();
            SQLHelper dbo = new SQLHelper();
            string sqlStr = "select GradeID,GradeName,GradeDesc from CustomerGrade Order By GradeID";
            DataTable dt = dbo.getsqlDatable(sqlStr);
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(
                    new CustomerGrade
                    {
                        GradeID = item["GradeID"].ToString(),
                        GradeName = item["GradeName"].ToString(),
                        GradeDesc = item["GradeDesc"].ToString()
                    }
                    );
            }
            return cl;
        }
        #endregion
    }
}
