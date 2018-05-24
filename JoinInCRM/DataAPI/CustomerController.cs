using JoinInCRM.Helpers;
using JoinInCRM.Models;
using NPinyin;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace JoinInCRM.DataAPI
{
    public class CustomerController : ApiController
    {
        // 简单客户列表 GET: 返回全部客户
        public IEnumerable<Customer> GetCustomerList()
        {
            return GetCustomer("").ToArray<Customer>();
        }
        // 简单客户列表 GET: 根据手机号或客户姓名返回客户列表
        public IEnumerable<Customer> GetCustomerList(string searchStr)
        {
            return GetCustomer(searchStr).ToArray<Customer>();
        }
        // 简单客户列表 GET: 根据客户ID返回客户明细信息
        public IEnumerable<Customer> GetCustomerList(int CustomerID)
        {
            return GetCustomer(CustomerID);
        }
        // 简单客户列表 GET: 根据用户OpenID,客户Status，客户姓名 返回客户列表
        public IEnumerable<Customer> GetCustomerIndividualList(string OpenID, int Status, string SearchText)
        {
            return GetCustomerIndividual(OpenID, Status, SearchText).ToArray<Customer>();
        }
        // 简单营业列表 GET: api/Customer/
        public IEnumerable<Business> GetBusinessList(int CustomerID)
        {
            return GetCustBusiness(CustomerID).ToArray<Business>();
        }
        // 简单订单列表 GET
        public IEnumerable<Order> GetOrderList(int CustomerID) {
            return GetOrder(CustomerID).ToArray<Order>();
        }
        private IList<Business> GetBusiness(int BusinessID)
        {
            string sqlStr = "Select BusinessID, CustomerID,convert(nvarchar(10),plandate,120) as PlanDate, convert(nvarchar(10),ActualDate,120) as ActualDate,convert(nvarchar(10),NextPlanDate,120) as NextPlanDate,BusinessDesc, BusinessResult, BusinessAction From Business a" +
                          " where BusinessID=" + BusinessID.ToString();

            SQLHelper dbo = new SQLHelper();

            DataTable dt = dbo.getsqlDatable(sqlStr);
            List<Business> cl = new List<Business>();
            cl.Clear();
            cl.Add(new Business
            {
                CustomerID = dt.Rows[0]["CustomerID"].ToString(),
                BusinessID = dt.Rows[0]["BusinessID"].ToString(),
                PlanDate = dt.Rows[0]["PlanDate"].ToString(),
                ActualDate = dt.Rows[0]["ActualDate"].ToString(),
                NextPlanDate=dt.Rows[0]["NextPlanDate"].ToString(),
                BusinessAction = dt.Rows[0]["BusinessAction"].ToString(),
                BusinessDesc = dt.Rows[0]["ActualDate"].ToString(),
                BusinessResult = dt.Rows[0]["BusinessResult"].ToString()
            });
            return cl;
        }
        private List<Customer> GetCustomer(string queryStr)
        {
            //string sqlStr = "Select CustomerID,CustomerName,case when sex =1 then '男' else '女' end as Sex,Telphone,Remark From Customers";
            string sqlStr = "Select a.CustomerID,a.CustomerName,b.GradeName as Grade,c.NickName as Salesman,d.StatusDesc as [Status],case when a.sex =1 then '男' else '女' end as Sex,a.Telphone,a.Remark,a.SortLetter From Customers a" +
                           " inner join CustomerGrade b on a.GradeID = b.GradeID" +
                           " inner join WechatUsers c on a.SalesManID = c.UserID" +
                           " inner join [Status] d on a.[Status] = d.[Status]";

            SQLHelper dbo = new SQLHelper();
            if (!string.IsNullOrEmpty(queryStr) && queryStr != "")
            {
                sqlStr += " Where a.CustomerName like '%" + queryStr.Replace("'", "''") + "%' Or a.Telphone like '%" + queryStr.Replace("'", "''") + "%'";
            }
            sqlStr += " Order By a.SortLetter, a.CustomerName ";

            DataTable dt = dbo.getsqlDatable(sqlStr);
            List<Customer> cl = new List<Customer>();
            cl.Clear();
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(new Customer
                {
                    CustomerID = int.Parse(item["CustomerID"].ToString()),
                    CustomerName = item["CustomerName"].ToString(),
                    Grade = item["Grade"].ToString(),
                    Salesman = item["Salesman"].ToString(),
                    Status = item["Status"].ToString(),
                    Sex = item["Sex"].ToString(),
                    Telphone = item["Telphone"].ToString(),
                    Remark = item["Remark"].ToString(),
                    SortLetter = item["SortLetter"].ToString()
                });

            }
            return cl;
        }
        private List<Customer> GetCustomer(int CustomerID)
        {
            string strSQL = @"Select a.[CustomerID],a.[CustomerName],b.[GradeName] as [Grade],c.[NickName] as [Salesman],d.[StatusDesc] as [Status],
                                a.[Sex],a.[Telphone],a.[SalesManID],a.[SourceID],a.[GradeID],a.[InterestedCar],a.[InterestedColor],a.[CompetingProduct],a.[Budget],a.[LicensePlate],
                                a.[CardArea],a.[Remark],a.[VehicleRemark]
                                From [Customers] a
                                inner join[CustomerGrade] b on a.GradeID = b.GradeID
                                inner join[WechatUsers] c on a.SalesManID = c.UserID
                                inner join[Status] d on a.[Status] = d.[Status] Where a.CustomerID=" + CustomerID.ToString();

            SQLHelper dbo = new SQLHelper();
            DataTable dt = dbo.getsqlDatable(strSQL);
            List<Customer> cl = new List<Customer>();
            cl.Clear();
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(new Customer
                {
                    CustomerID = int.Parse(item["CustomerID"].ToString()),
                    CustomerName = item["CustomerName"].ToString(),
                    Sex = item["Sex"].ToString(),
                    Telphone = item["Telphone"].ToString(),
                    InterestedCar = item["InterestedCar"].ToString(),
                    InterestedColor = item["InterestedColor"].ToString(),
                    CompetingProduct = item["CompetingProduct"].ToString(),
                    Budget = item["Budget"].ToString(),
                    LicensePlate = item["LicensePlate"].ToString(),
                    CardArea = item["CardArea"].ToString(),
                    Remark = item["Remark"].ToString(),
                    VehicleRemark = item["VehicleRemark"].ToString(),
                    Grade = item["GradeID"].ToString(),
                    Source = item["SourceID"].ToString(),
                    Salesman = item["Salesman"].ToString(),
                    Status = item["Status"].ToString()
                });
            }
            return cl;

        }
        private List<Customer> GetCustomerIndividual(string OpenID, int CustStatus = -1, string SearchText = "")
        {
            //string sqlStr = "Select CustomerID,CustomerName,case when sex =1 then '男' else '女' end as Sex,Telphone,Remark From Customers";
            string sqlStr = "Select a.CustomerID,a.CustomerName,b.GradeName as Grade,e.SourceName,convert(nvarchar(10),f.NextPlanDate,120) as NextPlanDate ," +
                           " case when a.sex =1 then '男' else '女' end as Sex,a.Telphone,a.Remark," +
                           " c.NickName as Salesman,d.StatusDesc as [Status], a.SortLetter From Customers a" +
                           " inner join CustomerGrade b on a.GradeID = b.GradeID" +
                           " inner join WechatUsers c on a.SalesManID = c.UserID" +
                           " inner join [Status] d on a.[Status] = d.[Status]" +
                           " inner join [CustomerSource] e on a.SourceID=e.SourceID"+
                           " inner join (select CustomerID,max(nextplandate) as NextPlanDate from Business Group by CustomerID) f on a.customerid=f.customerid " +
                           " Where 1=1 ";
            SQLHelper dbo = new SQLHelper();
            if (!string.IsNullOrEmpty(OpenID) && OpenID != "")
            {
                sqlStr += " And c.OpenID =" + dbo.DBStr(OpenID);
            }
            if (CustStatus != -1)
            {
                sqlStr += " And a.Status=" + CustStatus;

            }
            if (SearchText != "")
            {
                sqlStr += " And a.CustomerName like '%" + SearchText.Replace("'", "''") + "%' Or a.Telphone like '%" + SearchText.Replace("'", "''") + "%'";
            }
            if (CustStatus==2)
            {
                sqlStr += " Order By b.GradeID,f.NextPlanDate desc";
            }
            else
            {
                sqlStr += " Order By a.SortLetter,a.CustomerName";
            }
            

            DataTable dt = dbo.getsqlDatable(sqlStr);
            List<Customer> cl = new List<Customer>();
            cl.Clear();
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(new Customer
                {
                    CustomerID = int.Parse(item["CustomerID"].ToString()),
                    CustomerName = item["CustomerName"].ToString(),
                    Grade = item["Grade"].ToString(),
                    Salesman = item["Salesman"].ToString(),
                    Status = item["Status"].ToString(),
                    Sex = item["Sex"].ToString(),
                    Telphone = item["Telphone"].ToString(),
                    Remark = item["Remark"].ToString(),
                    SortLetter = item["SortLetter"].ToString(),
                    Source = item["SourceName"].ToString(),
                    NextPlanDate = item["NextPlanDate"].ToString()
                });
            }
            return cl;
        }
        private List<Business> GetCustBusiness(int CustomerID)
        {
            string strSQL = " Select a.BusinessID, a.CustomerID, convert(nvarchar(10),a.PlanDate,120) as PlanDate," +
                            " convert(nvarchar(10),a.ActualDate,120) as ActualDate,convert(nvarchar(10),a.NextPlanDate,120) as NextPlanDate," +
                            " a.BusinessDesc, a.BusinessResult, a.BusinessAction,a.BusinessEvent,a.[TestCar],a.[TestSatisfaction],a.[InterestedCar],a.[InterestedColor],b.[GradeName] as Grade " +
                            " from Business a inner join CustomerGrade b on a.Grade=b.GradeID " +
                            " Where a.CustomerID=" + CustomerID.ToString() + " Order by a.ActualDate desc";
            
            SQLHelper dbo = new SQLHelper();
            DataTable dt = dbo.getsqlDatable(strSQL);
            List<Business> cl = new List<Business>();
            cl.Clear();
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(new Business
                {
                    CustomerID = item["CustomerID"].ToString(),
                    BusinessID = item["BusinessID"].ToString(),
                    PlanDate = item["PlanDate"].ToString(),
                    ActualDate = item["ActualDate"].ToString(),
                    NextPlanDate=item["NextPlanDate"].ToString(),
                    BusinessDesc = item["BusinessDesc"].ToString(),
                    BusinessResult = item["BusinessResult"].ToString(),
                    BusinessAction = item["BusinessAction"].ToString(),
                    BusinessEvent=item["BusinessEvent"].ToString(),
                    Grade=item["Grade"].ToString()
 
                });
            }

            return cl;

        }
        private List<Order>GetOrder(int CustomerID)
        {
            string strSQL = " SELECT a.[OrderID],a.[BusinessID],e.CustomerID "+
                            "  , a.[OrderNo],a.[OrderDate],a.[OrderCar],b.CarName,b.CarStyle,a.[OrderAmount]"+
                            "  , Case a.[OrderStatus] when 0 then '待交车' when 1 then '已交车' when 2 then '已取消' else '未知' end  as OrderStatus"+
                            "  , a.[CancelDate],a.[CancelRemark],a.[OrderCarColor]"+
                            "  , c.ColorName,a.[DealDate],a.[VINCode],a.[PlateNO]"+
                            "  , a.[AfterSaleMan],a.[Insure],a.[Loan]"+
                          " FROM [Order] a " +
                          " inner join Car b on a.orderCar= b.CarID"+
                          " inner join Color c on a.orderCarColor= c.ColorID"+
                          " inner join Business d on d.BusinessID= a.BusinessID"+
                          " inner join Customers e on d.CustomerID= e.CustomerID"+
                          " Where e.CustomerID=" + CustomerID.ToString() + " Order by a.OrderDate desc";

            SQLHelper dbo = new SQLHelper();
            DataTable dt = dbo.getsqlDatable(strSQL);
            List<Order> cl = new List<Order>();
            cl.Clear();
            foreach (DataRow item in dt.Rows)
            {
                cl.Add(new Order
                {
                    OrderID = item["OrderID"].ToString(),
                    BusinessID = item["BusinessID"].ToString(),
                    CustomerID = item["CustomerID"].ToString(),
                    OrderNo = item["OrderNo"].ToString(),
                    OrderDate = item["OrderDate"].ToString(),
                    OrderCar = item["OrderCar"].ToString(),
                    OrderCarColor = item["OrderCarColor"].ToString(),
                    ColorName=item["ColorName"].ToString(),
                    CarStyle = item["CarStyle"].ToString(),
                    CarName=item["CarName"].ToString(),
                    OrderAmount = item["OrderAmount"].ToString(),
                    OrderStatus = item["OrderStatus"].ToString(),
                    CancelDate = item["CancelDate"].ToString(),
                    CancelRemark = item["CancelRemark"].ToString(),
                    DealDate = item["DealDate"].ToString(),
                    VINCode = item["VINCode"].ToString(),
                    PlateNO = item["PlateNO"].ToString(),
                    AfterSaleMan = item["AfterSaleMan"].ToString(),
                    Insure = item["Insure"].ToString(),
                    Loan = item["Loan"].ToString()
                });
            }

            return cl;

        }
        [HttpGet]
        public Customer GetCustomerObj(int CustomerID)
        {
            string strSQL = @"Select a.[CustomerID],a.[CustomerName],b.[GradeName] as [Grade],c.[NickName] as [Salesman],d.[StatusDesc] as [Status],
                                a.[Sex],a.[Telphone],a.[SalesManID],a.[SourceID],a.[GradeID],a.[InterestedCar],a.[InterestedColor],a.[CompetingProduct],a.[Budget],a.[LicensePlate],
                                a.[CardArea],a.[Remark],a.[VehicleRemark]
                                From [Customers] a
                                inner join[CustomerGrade] b on a.GradeID = b.GradeID
                                inner join[WechatUsers] c on a.SalesManID = c.UserID
                                inner join[Status] d on a.[Status] = d.[Status] Where a.CustomerID=" + CustomerID.ToString();

            SQLHelper dbo = new SQLHelper();
            DataTable dt = dbo.getsqlDatable(strSQL);
            if (dt.Rows.Count == 1)
            {
                return new Customer
                {
                    CustomerID = int.Parse(dt.Rows[0]["CustomerID"].ToString()),
                    CustomerName = dt.Rows[0]["CustomerName"].ToString(),
                    Sex = dt.Rows[0]["Sex"].ToString(),
                    Telphone = dt.Rows[0]["Telphone"].ToString(),
                    InterestedCar = dt.Rows[0]["InterestedCar"].ToString(),
                    InterestedColor = dt.Rows[0]["InterestedColor"].ToString(),
                    CompetingProduct = dt.Rows[0]["CompetingProduct"].ToString(),
                    Budget = dt.Rows[0]["Budget"].ToString(),
                    LicensePlate = dt.Rows[0]["LicensePlate"].ToString(),
                    CardArea = dt.Rows[0]["CardArea"].ToString(),
                    Remark = dt.Rows[0]["Remark"].ToString(),
                    VehicleRemark = dt.Rows[0]["VehicleRemark"].ToString(),
                    Grade = dt.Rows[0]["GradeID"].ToString(),
                    Source = dt.Rows[0]["SourceID"].ToString(),
                    Salesman = dt.Rows[0]["Salesman"].ToString(),
                    Status = dt.Rows[0]["Status"].ToString()//,
                    //PlanDate = dt.Rows[0]["PlanDate"].ToString()
                };
            }
            else
            {
                return new Customer
                {
                    CustomerID = 0,
                    CustomerName = "",
                    Sex = "",
                    Telphone = "",
                    InterestedCar = "",
                    InterestedColor = "",
                    CompetingProduct = "",
                    Budget = "",
                    LicensePlate = "",
                    CardArea = "",
                    Remark = "",
                    VehicleRemark = "",
                    Grade = "",
                    Source = "",
                    Salesman = "",
                    Status = ""

                };
            }
        }
        [HttpGet]
        public string GetCustNextPlanDate(int CustomerID) {
            string strSQL = " Select CustomerID, convert(nvarchar(10),MAX(NextPlanDate),120) as NextPlanDate  from Business Where CustomerID=" + CustomerID.ToString() + " Group BY CustomerID";

            SQLHelper dbo = new SQLHelper();
            DataTable dt = dbo.getsqlDatable(strSQL);
 
            if (dt.Rows.Count==1)
            {
                 return dt.Rows[0]["NextPlanDate"].ToString();
            }
            else
            {
                return "";
            }
             
             
        }
        [HttpGet]
        public Business GetBusinessByID(int BusinessID)
        {
            string sqlStr = "Select BusinessID, CustomerID,convert(nvarchar(10),plandate,120) as PlanDate, convert(nvarchar(10),ActualDate,120) as ActualDate, BusinessDesc, BusinessResult, BusinessAction From Business a" +
              " where BusinessID=" + BusinessID.ToString();
            SQLHelper dbo = new SQLHelper();
            DataTable dt = dbo.getsqlDatable(sqlStr);
            if (dt.Rows.Count == 1)
            {
                return new Business
                {
                    CustomerID = dt.Rows[0]["CustomerID"].ToString(),
                    BusinessID = dt.Rows[0]["BusinessID"].ToString(),
                    PlanDate = dt.Rows[0]["PlanDate"].ToString(),
                    ActualDate = dt.Rows[0]["ActualDate"].ToString(),
                    BusinessAction = dt.Rows[0]["BusinessAction"].ToString(),
                    BusinessDesc = dt.Rows[0]["BusinessDesc"].ToString(),
                    BusinessResult = dt.Rows[0]["BusinessResult"].ToString()
                };
            }
            else
            {
                return new Business
                {
                    CustomerID = "",
                    BusinessID = "",
                    PlanDate = "",
                    ActualDate = "",
                    BusinessAction = "",
                    BusinessDesc = "",
                    BusinessResult = ""
                };
            }


        }
        [HttpGet]
        public Order GetOrderDetail(int OrderID)
        {
            string strSQL = " SELECT a.[OrderID],a.[BusinessID],e.CustomerID " +
                           "  , a.[OrderNo],a.[OrderDate],a.[OrderCar],b.CarName,b.CarStyle,a.[OrderAmount]" +
                           "  ,a.[OrderStatus]" +
                           "  , a.[CancelDate],a.[CancelRemark],a.[OrderCarColor]" +
                           "  , c.ColorName,a.[DealDate],a.[VINCode],a.[PlateNO]" +
                           "  , a.[AfterSaleMan],a.[Insure],a.[Loan]" +
                         " FROM [Order] a " +
                         " inner join Car b on a.orderCar= b.CarID" +
                         " inner join Color c on a.orderCarColor= c.ColorID" +
                         " inner join Business d on d.BusinessID= a.BusinessID" +
                         " inner join Customers e on d.CustomerID= e.CustomerID" +
                         " where a.OrderID=" + OrderID.ToString();
            SQLHelper dbo = new SQLHelper();
            DataTable dt = dbo.getsqlDatable(strSQL);
            if (dt.Rows.Count == 1)
            {
                return new Order
                {
                    OrderID = dt.Rows[0]["OrderID"].ToString(),
                    OrderNo = dt.Rows[0]["OrderNo"].ToString(),
                    OrderCarColor = dt.Rows[0]["OrderCarColor"].ToString(),
                    OrderCar = dt.Rows[0]["OrderCar"].ToString(),
                    CarName = dt.Rows[0]["CarName"].ToString(),
                    ColorName = dt.Rows[0]["ColorName"].ToString(),
                    CarStyle = dt.Rows[0]["CarStyle"].ToString(),
                    OrderAmount = dt.Rows[0]["OrderAmount"].ToString(),
                    OrderStatus = dt.Rows[0]["OrderStatus"].ToString(),
                    CancelDate = dt.Rows[0]["CancelDate"].ToString(),
                    CancelRemark = dt.Rows[0]["CancelRemark"].ToString(),
                    DealDate = dt.Rows[0]["DealDate"].ToString(),
                    VINCode = dt.Rows[0]["VINCode"].ToString(),
                    PlateNO = dt.Rows[0]["PlateNO"].ToString(),
                    AfterSaleMan = dt.Rows[0]["AfterSaleMan"].ToString(),
                    Insure = dt.Rows[0]["Insure"].ToString(),
                    Loan = dt.Rows[0]["Loan"].ToString(),
                    CustomerID = dt.Rows[0]["CustomerID"].ToString(),
                    BusinessID = dt.Rows[0]["BusinessID"].ToString(),
                    OrderDate = dt.Rows[0]["OrderDate"].ToString()
                    
                };
            }
            else
            {
                return new Order
                {
                    OrderID = "",
                    OrderNo = "",
                    OrderCarColor = "",
                    OrderCar = "",
                    CarName = "",
                    ColorName = "",
                    CarStyle = "",
                    OrderAmount = "",
                    OrderStatus = "",
                    CancelDate = "",
                    CancelRemark = "",
                    DealDate = "",
                    VINCode = "",
                    PlateNO = "",
                    AfterSaleMan = "",
                    Insure = "",
                    Loan = "",
                    CustomerID = "",
                    BusinessID = "",
                    OrderDate = ""
                };
            }
        }
        [HttpGet]
        public Business GetBusinessDetail(int BusinessID)
        {
            string strSQL = " SELECT a.[CustomerID],a.[BusinessID]," +
                            " a.[ActualDate],a.[BusinessEvent],a.[BusinessDesc],a.[NextPlanDate]," +
                            " b.[CarName] as [TestCar], a.TestSatisfaction,c.GradeName as [Grade]" +
                            " FROM [Business] a left join [Car] b on a.TestCar= b.CarID inner join CustomerGrade c on a.Grade=c.GradeID " +
                            " where a.BusinessID=" + BusinessID.ToString();
            SQLHelper dbo = new SQLHelper();
            DataTable dt = dbo.getsqlDatable(strSQL);
            Business reBus = new Business();
            if (dt.Rows.Count == 1)
            {
                reBus.CustomerID = dt.Rows[0]["CustomerID"].ToString();
                reBus.BusinessID = dt.Rows[0]["BusinessID"].ToString();
                reBus.ActualDate = dt.Rows[0]["ActualDate"].ToString();
                reBus.BusinessEvent = dt.Rows[0]["BusinessEvent"].ToString();
                reBus.BusinessDesc = dt.Rows[0]["BusinessDesc"].ToString();
                reBus.NextPlanDate = dt.Rows[0]["NextPlanDate"].ToString();
                reBus.TestCar = dt.Rows[0]["TestCar"].ToString();
                reBus.TestSatisfaction = dt.Rows[0]["TestSatisfaction"].ToString();
                reBus.Grade = dt.Rows[0]["Grade"].ToString();
                strSQL = " SELECT a.[OrderID],a.[BusinessID]" +
                        " ,a.[OrderNo],a.[OrderDate],b.[CarName] as [OrderCar],a.[OrderAmount]" +
                        " , c.ColorName as [OrderCarColor]" +
                        " FROM [Order] a " +
                        " inner join Car b on a.orderCar= b.CarID" +
                        " inner join Color c on a.orderCarColor= c.ColorID" +
                        " where a.BusinessID=" + BusinessID.ToString();

                DataTable dtOrder = dbo.getsqlDatable(strSQL);
                if (dtOrder.Rows.Count == 1)
                {
                    reBus.OrderID = dtOrder.Rows[0]["OrderID"].ToString();
                    reBus.OrderNo = dtOrder.Rows[0]["OrderNo"].ToString();
                    reBus.OrderDate = dtOrder.Rows[0]["OrderDate"].ToString();
                    reBus.OrderCar = dtOrder.Rows[0]["OrderCar"].ToString();
                    reBus.OrderAmount = dtOrder.Rows[0]["OrderAmount"].ToString();
                    reBus.OrderCarColor = dtOrder.Rows[0]["OrderCarColor"].ToString();

                }
                else
                {
                    reBus.OrderID = "0";
                }

                strSQL = " SELECT a.[QuotedID],a.[BusinessID]" +
                        " ,a.[QuotedDate],b.[CarName] as [QuotedCar],a.[QuotedAmount]" +
                        " , c.ColorName as [QuotedCarColor]" +
                        " FROM [Quoted] a " +
                        " inner join Car b on a.QuotedCar= b.CarID" +
                        " inner join Color c on a.QuotedCarColor= c.ColorID" +
                        " where a.BusinessID=" + BusinessID.ToString();
                DataTable dtQuoted = dbo.getsqlDatable(strSQL);
                if (dtQuoted.Rows.Count == 1)
                {
                    reBus.QuotedID = dtQuoted.Rows[0]["QuotedID"].ToString();
                    reBus.QuotedDate = dtQuoted.Rows[0]["QuotedDate"].ToString();
                    reBus.QuotedCar = dtQuoted.Rows[0]["QuotedCar"].ToString();
                    reBus.QuotedCarColor = dtQuoted.Rows[0]["QuotedCarColor"].ToString();
                    reBus.QuotedAmount = dtQuoted.Rows[0]["QuotedAmount"].ToString();
                }
                else
                {
                    reBus.QuotedID = "0";
                }

            }
            else
            {
                reBus.BusinessID = "0";
                reBus.OrderID = "0";
                reBus.QuotedID = "0";
            }
            return reBus;
        }


        // POST: api/Customer
        public void Post([FromBody]string value)
        {
        }
        // PUT: api/Customer/5
        public void Put(int id, [FromBody]string value)
        {
        }
        // DELETE: api/Customer/5
        public bool Delete(int id)
        {
            if (id > 0)
            {
                SQLHelper dbo = new SQLHelper();
                string sqlStr = "Delete From Customers where CustomerID=" + id.ToString();
                return dbo.exeSQL(sqlStr);
            }
            else
            {
                return false;
            }
        }
        // api/Customer/AddNew
        public string AddBus(Business bus, string LastUser)
        {
            string result = "";
            if (string.IsNullOrEmpty(LastUser) || LastUser == "")
            {
                return "0";
            }
            else
            {
                SQLHelper dbase = new SQLHelper();
                int res = dbase.ReturnInteger("Select UserID FROM [dbo].[WechatUsers] WHERE OpenID=" + dbase.DBStr(LastUser), -1);

                if (res > 0)
                {
                    LastUser = res.ToString();
                }
                else
                {
                    return "0";
                }
            }
            SQLHelper dbo = new SQLHelper();
            SqlParameter[] para = {new SqlParameter("CustomerID", bus.CustomerID),
                                    new SqlParameter("PlanDate", bus.PlanDate),
                                    //new SqlParameter("ActualDate", bus.ActualDate),
                                    new SqlParameter("NextPlanDate",bus.NextPlanDate),
                                    new SqlParameter("BusinessDesc",bus.BusinessDesc),
                                    //new SqlParameter("BusinessResult",bus.BusinessResult),
                                    // new SqlParameter("BusinessAction",bus.BusinessAction),
                                    new SqlParameter("InterestedCar",bus.InterestedCar),
                                    new SqlParameter("InterestedColor",bus.InterestedColor),
                                    new SqlParameter("Grade",bus.Grade),
                                    new SqlParameter("BusinessEvent",bus.BusinessEvent),
                                    new SqlParameter("TestCar",bus.TestCar),
                                    new SqlParameter("TestSatisfaction",bus.TestSatisfaction),
                                    new SqlParameter("UpdUser",LastUser)
                //new SqlParameter("Status", cust.Status)            
            };
            int re = 0;
            //新增一次营业的时候将以往的营业中NextPlanDate计划回访日期清空，只允许一个计划回访日期
            string sqlStr = "Insert Into Business(CustomerID,PlanDate,ActualDate,NextPlanDate,BusinessDesc,BusinessEvent,InterestedCar,InterestedColor,Grade,TestSatisfaction,TestCar,UpdUser,UpdDate) " +
                            " values (@CustomerID,@PlanDate,Getdate(),@NextPlanDate,@BusinessDesc,@BusinessEvent,@InterestedCar,@InterestedColor,@Grade,@TestSatisfaction,@TestCar,@UpdUser,Getdate())";

            if (bus.NextPlanDate==null)
            {
                sqlStr = "Insert Into Business(CustomerID,PlanDate, ActualDate,NextPlanDate,BusinessDesc, BusinessEvent,InterestedCar,InterestedColor,Grade,TestSatisfaction,TestCar,UpdUser,UpdDate) " +
                            " values (@CustomerID,@PlanDate,GetDate(),Null,@BusinessDesc,@BusinessEvent,@InterestedCar,@InterestedColor,@Grade,@TestSatisfaction,@TestCar,@UpdUser,Getdate())";
                SqlParameter[] para_nonPlanDate = {
                                    new SqlParameter("CustomerID", bus.CustomerID),
                                    new SqlParameter("PlanDate", bus.PlanDate),
                                    //new SqlParameter("ActualDate", bus.ActualDate),                                   
                                    new SqlParameter("BusinessDesc",bus.BusinessDesc),
                                    new SqlParameter("BusinessEvent",bus.BusinessEvent),
                                     new SqlParameter("InterestedCar",bus.InterestedCar),
                                    new SqlParameter("InterestedColor",bus.InterestedColor),
                                    new SqlParameter("Grade",bus.Grade),
                                    new SqlParameter("TestCar",bus.TestCar),
                                    new SqlParameter("TestSatisfaction",bus.TestSatisfaction),
                                    //new SqlParameter("BusinessAction",bus.BusinessAction),
                                    new SqlParameter("UpdUser",LastUser)
                //new SqlParameter("Status", cust.Status)            
                };
                re = dbo.ExecuteNonQuery(sqlStr, para_nonPlanDate);
            }
            else
            {//以往的营业中NextPlanDate计划回访日期清空,不管有没有做营业
                dbo.ExecuteNonQuery("Update Business Set NextPlanDate=null From Business Where CustomerID=@CustomerID ", new SqlParameter("CustomerID", bus.CustomerID));
                re = dbo.ExecuteNonQuery(sqlStr, para);
            }
 
            
            
            if (re == 1)
            {

                //插入营业成功，再分事件做报价或下订
                if (bus.BusinessEvent == "报价")
                {
                    dbo.ExecuteNonQuery("Update Customers Set GradeID=@Grade,Status=2,InterestedCar=@InterestedCar,InterestedColor=@InterestedColor From Customers Where CustomerID=" + bus.CustomerID, 
                        new SqlParameter[] { new SqlParameter("Grade", bus.Grade),
                                            new SqlParameter("InterestedCar",bus.InterestedCar),
                                            new SqlParameter("InterestedColor",bus.InterestedColor) });
                    sqlStr = "Insert Into Quoted([BusinessID],[QuotedDate],[QuotedCar],[QuotedCarColor],[QuotedAmount]) " +
                            " Select max(businessID) as BusinessID,Getdate() as QuotedDate,@QuotedCar,@QuotedCarColor,@QuotedAmount From Business Where CustomerID=" + bus.CustomerID;
                    
                    SqlParameter[] para_Quoted = { 
                                    new SqlParameter("QuotedCar", bus.QuotedCar),
                                    new SqlParameter("QuotedCarColor",bus.QuotedCarColor),
                                    new SqlParameter("QuotedAmount", bus.QuotedAmount)
                                    };
                    re = dbo.ExecuteNonQuery(sqlStr, para_Quoted);
                    if (re==1)
                    {
                        result = "1";
                    }
                    else
                    {
                        result = "0";
                    }
                }
                else if (bus.BusinessEvent == "下订")
                {
                    dbo.ExecuteNonQuery("Update Customers Set GradeID=@Grade,Status=3,InterestedCar=@InterestedCar,InterestedColor=@InterestedColor From Customers Where CustomerID=" + bus.CustomerID,
                         new SqlParameter[] { new SqlParameter("Grade", bus.Grade),
                                            new SqlParameter("InterestedCar",bus.InterestedCar),
                                            new SqlParameter("InterestedColor",bus.InterestedColor) });
                    sqlStr = "Insert Into [Order](BusinessID, OrderNo, OrderDate, OrderCar,OrderCarColor, OrderAmount, OrderStatus) " +
                             " Select max(businessID) as BusinessID,@OrderNo,Getdate() as OrderDate,@OrderCar,@OrderCarColor,@OrderAmount,0 as status From Business Where CustomerID=" + bus.CustomerID;

                    SqlParameter[] para_Order = {
                                    new SqlParameter("OrderCar", bus.OrderCar),
                                    new SqlParameter("OrderCarColor",bus.OrderCarColor),
                                    new SqlParameter("OrderAmount", bus.OrderAmount),
                                     new SqlParameter("OrderNo", bus.OrderNo)
                                    };
                    re = dbo.ExecuteNonQuery(sqlStr, para_Order);
                    if (re == 1)
                    {
                        result = "1";
                    }
                    else
                    {
                        result = "0";
                    }
                }
      
                else if (bus.BusinessEvent == "邀约" || bus.BusinessEvent == "试驾" || bus.BusinessEvent == "其它")
                {
                    string strStatus = "2";
                    if (bus.Grade=="5")
                    {
                        strStatus = "7";
                    }
                    dbo.ExecuteNonQuery("Update Customers Set GradeID=@Grade,Status="+ strStatus + ",InterestedCar=@InterestedCar,InterestedColor=@InterestedColor From Customers Where CustomerID=" + bus.CustomerID, 
                        new SqlParameter[] { new SqlParameter("Grade", bus.Grade),
                                            new SqlParameter("InterestedCar",bus.InterestedCar),
                                            new SqlParameter("InterestedColor",bus.InterestedColor) }); 
                    result = "1";
                }
                else
                {
                    result = "1";
                } 

            }
            else
            {
                result = "0";
            }
            return result;
        }
        // api/Customer/AddNew
        public string EditBus(Business bus, string LastUser)
        {
            string result = "";
            if (string.IsNullOrEmpty(LastUser) || LastUser == "")
            {
                return "0";
            }
            else
            {
                SQLHelper dbase = new SQLHelper();
                int res = dbase.ReturnInteger("Select UserID FROM [dbo].[WechatUsers] WHERE OpenID=" + dbase.DBStr(LastUser), -1);
                if (res > 0)
                {
                    LastUser = res.ToString();
                }
                else
                {
                    return "0";
                }
            }
            SQLHelper dbo = new SQLHelper();
            int re = 0;
            string sqlStr = " Update a Set ActualDate=@ActualDate,NextPlanDate=@NextPlanDate,BusinessDesc=@BusinessDesc,BusinessResult=@BusinessResult,BusinessAction=@BusinessAction,UpdUser=@UpdUser,UpdDate=Getdate()" +
                            " From Business a where a.BusinessID=" + bus.BusinessID;

            SqlParameter[] para = {new SqlParameter("CustomerID", bus.CustomerID),
                                   //new SqlParameter("PlanDate", bus.PlanDate),
                                   new SqlParameter("ActualDate", bus.ActualDate), 
                                   new SqlParameter("NextPlanDate",bus.NextPlanDate),
                                   new SqlParameter("BusinessDesc",bus.BusinessDesc),
                                   new SqlParameter("BusinessResult",bus.BusinessResult),
                                   new SqlParameter("BusinessAction",bus.BusinessAction),
                                   new SqlParameter("UpdUser",LastUser)
                                   //new SqlParameter("Status", cust.Status)

            };
            if (bus.NextPlanDate == null)
            {
                sqlStr = " Update a Set ActualDate=@ActualDate,NextPlanDate=Null,BusinessDesc=@BusinessDesc,BusinessResult=@BusinessResult,BusinessAction=@BusinessAction,UpdUser=@UpdUser,UpdDate=Getdate()" +
                           " From Business a where a.BusinessID=" + bus.BusinessID;

                SqlParameter[] para_nonPlan = {new SqlParameter("CustomerID", bus.CustomerID),
                                   //new SqlParameter("PlanDate", bus.PlanDate),
                                   new SqlParameter("ActualDate", bus.ActualDate),                                    
                                   new SqlParameter("BusinessDesc", bus.BusinessDesc),
                                   new SqlParameter("BusinessResult", bus.BusinessResult),
                                   new SqlParameter("BusinessAction", bus.BusinessAction),
                                   new SqlParameter("UpdUser", LastUser)
                                   //new SqlParameter("Status", cust.Status)
                            };
                re = dbo.ExecuteNonQuery(sqlStr, para_nonPlan);
            }
            else
            {
                re = dbo.ExecuteNonQuery(sqlStr, para);
            }
            
            
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
        // 新增客户到数据库
        public string AddNew(Customer cust, string LastUser)
        {
            string result = "";

            if (string.IsNullOrEmpty(LastUser) || LastUser == "")
            {
                return "0";
            }
            else
            {
                SQLHelper dbase = new SQLHelper();
                int res = dbase.ReturnInteger("Select UserID FROM [dbo].[WechatUsers] WHERE OpenID=" + dbase.DBStr(LastUser), -1);

                if (res > 0)
                {
                    LastUser = res.ToString();
                }
                else
                {
                    return "0";
                }
            }

            string sqlStr = "Insert Into Customers(CustomerName,Telphone,Status,Sex,GradeID,SourceID,SalesManID,CreateDate,LastUpdate,LastUser,Remark,InterestedCar,InterestedColor,CompetingProduct,Budget,LicensePlate,CardArea,SortLetter,VehicleRemark,CompanyID) " +
                            " values (@CustomerName,@Telphone,2,@Sex,@GradeID,@Source,@SalesManID,Getdate(),Getdate(),@LastUser,@Remark,@InterestedCar,@InterestedColor,@CompetingProduct,@Budget,@LicensePlate,@CardArea,@SortLetter,@VehicleRemark,@CompanyID)";

            SqlParameter[] para = {new SqlParameter("CustomerName", cust.CustomerName),
                                   new SqlParameter("Telphone", cust.Telphone),
                                   new SqlParameter("Remark", cust.Remark),
                                   new SqlParameter("Sex",cust.Sex),
                                   new SqlParameter("Source",cust.Source),
                                   new SqlParameter("SalesManID",LastUser),
                                   new SqlParameter("InterestedCar",cust.InterestedCar),
                                   new SqlParameter("InterestedColor",cust.InterestedColor),
                                   new SqlParameter("CompetingProduct",cust.CompetingProduct),
                                   new SqlParameter("Budget",cust.Budget),
                                   new SqlParameter("LicensePlate",cust.LicensePlate),
                                   new SqlParameter("CardArea",cust.CardArea),
                                   new SqlParameter("VehicleRemark",cust.VehicleRemark),
                                   new SqlParameter("LastUser",LastUser),
                                   new SqlParameter("GradeID", cust.Grade),
                                   new SqlParameter("CompanyID", cust.CompanyID),
                                   new SqlParameter("SortLetter",GetSpellCode(cust.CustomerName[0].ToString()))

            };


            SQLHelper dbo = new SQLHelper();
            int re = dbo.ExecuteNonQuery(sqlStr, para);
            if (re == 1)
            {
                result = "1";

                SqlParameter[] paraBus = {  new SqlParameter("PlanDate", cust.PlanDate),
                                            new SqlParameter("Telphone", cust.Telphone),
                                            new SqlParameter("NextPlanDate",cust.PlanDate),
                                            new SqlParameter("InterestedCar",cust.InterestedCar),
                                            new SqlParameter("InterestedColor",cust.InterestedColor),
                                            new SqlParameter("Grade",cust.Grade)
                                         };
                re = dbo.ExecuteNonQuery("Insert Into Business(CustomerID,BusinessEvent,BusinessDesc,InterestedCar,InterestedColor,Grade,PlanDate,ActualDate,NextPlanDate) Select CustomerID,'新建客户','首次录入客户基本资料',@InterestedCar,@InterestedColor,@Grade,@PlanDate,Getdate(),@NextPlanDate From Customers Where Telphone=@Telphone", paraBus);
                if (re == 1)
                {
                    result = "1";
                }
                else
                {
                    result = "0";
                }

            }
            else
            {
                result = "0";
            }

            return result;

        }
        // 修改客户数据
        public string Edit(Customer cust,string LastUser)
        {
            string result = "";

            if (string.IsNullOrEmpty(LastUser) || LastUser == "")
            {
                return "0";
            }
            else
            {
                SQLHelper dbase = new SQLHelper();
                int res = dbase.ReturnInteger("Select UserID FROM [dbo].[WechatUsers] WHERE OpenID=" + dbase.DBStr(LastUser), -1);

                if (res > 0)
                {
                    LastUser = res.ToString();
                }
                else
                {
                    return "0";
                }
            }

            string sqlStr = " Update a Set CustomerName=@CustomerName, " +
                            "   Telphone=@Telphone,Sex=@Sex, SourceID=@Source," +
                            "   LastUpdate=getdate(),LastUser=@LastUser,Remark=@Remark," +
                            "   InterestedCar=@InterestedCar,InterestedColor=@InterestedColor," +
                            "   CompetingProduct=@CompetingProduct,Budget=@Budget,LicensePlate=@LicensePlate,"+
                            "   CardArea=@CardArea,SortLetter=@SortLetter,VehicleRemark=@VehicleRemark "+
                            " From Customers a " +
                            " Where CustomerID=@CustomerID";

            SqlParameter[] para = {new SqlParameter("CustomerID",cust.CustomerID),
                                   new SqlParameter("CustomerName", cust.CustomerName),
                                   new SqlParameter("Telphone", cust.Telphone),
                                   new SqlParameter("Remark", cust.Remark),
                                   new SqlParameter("Sex",cust.Sex),
                                   new SqlParameter("Source",cust.Source),
                                   new SqlParameter("InterestedCar",cust.InterestedCar),
                                   new SqlParameter("InterestedColor",cust.InterestedColor),
                                   new SqlParameter("CompetingProduct",cust.CompetingProduct),
                                   new SqlParameter("Budget",cust.Budget),
                                   new SqlParameter("LicensePlate",cust.LicensePlate),
                                   new SqlParameter("CardArea",cust.CardArea),
                                   new SqlParameter("VehicleRemark",cust.VehicleRemark),
                                   new SqlParameter("LastUser",LastUser),
                                   new SqlParameter("SortLetter",GetSpellCode(cust.CustomerName[0].ToString()))
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

        public string EditOrder(Order order)
        {
            SQLHelper dbo = new SQLHelper();

            SqlParameter[] paraBus = {  new SqlParameter("OrderID", order.OrderID),
                                            new SqlParameter("VINCode", order.VINCode),
                                            new SqlParameter("DealDate",order.DealDate),
                                            new SqlParameter("PlateNO",order.PlateNO),
                                            new SqlParameter("AfterSaleMan",order.AfterSaleMan),
                                            new SqlParameter("Loan",order.Loan),
                                            new SqlParameter("Insure",order.Insure)
                                         };
            int re = dbo.ExecuteNonQuery("Update [Order] Set [OrderStatus]=1,DealDate=@DealDate,VINCode=@VINCode,PlateNO=@PlateNO,AfterSaleMan=@AfterSaleMan,Loan=@Loan,Insure=@Insure From [Order] Where OrderID=@OrderID ", paraBus);
            if (re==1)
            {
                re = dbo.ExecuteNonQuery("Update Customers Set [Status]=5 From Customers where CustomerID=@CustomerID", new SqlParameter("CustomerID", order.CustomerID));
            }
            return re.ToString();
        }
        public string EditOrder(Order order,Customer cust,Business bus,string LastUser)
        {
            SQLHelper dbo = new SQLHelper();
            if (string.IsNullOrEmpty(LastUser) || LastUser == "")
            {
                return "0";
            }
            else
            {
                SQLHelper dbase = new SQLHelper();
                int res = dbase.ReturnInteger("Select UserID FROM [dbo].[WechatUsers] WHERE OpenID=" + dbase.DBStr(LastUser), -1);

                if (res > 0)
                {
                    LastUser = res.ToString();
                }
                else
                {
                    return "0";
                }
            }
            SqlParameter[] paraBus = {  new SqlParameter("OrderID", order.OrderID),
                                            new SqlParameter("CancelDate", order.CancelDate),
                                            new SqlParameter("CancelRemark",order.CancelRemark) 
                                      };
            int re = dbo.ExecuteNonQuery("Update [Order] Set [OrderStatus]=2,CancelDate=@CancelDate,CancelRemark=@CancelRemark  From [Order] Where OrderID=@OrderID ", paraBus);
            if (re == 1)
            {

                //插入营业事件 
                dbo.ExecuteNonQuery("Update Business Set NextPlanDate=null From Business Where CustomerID=@CustomerID ", new SqlParameter("CustomerID", bus.CustomerID));

                string sqlStr = "Insert Into Business(CustomerID,PlanDate, ActualDate,NextPlanDate,BusinessDesc, BusinessEvent,Grade,UpdUser,UpdDate) " +
                            " values (@CustomerID,GetDate(),GetDate(),@NextPlanDate,@BusinessDesc,@BusinessEvent,@Grade,@UpdUser,Getdate())";
                if (cust.Grade == "5")//战败
                { 
                    sqlStr = "Insert Into Business(CustomerID,PlanDate, ActualDate,NextPlanDate,BusinessDesc, BusinessEvent,Grade,UpdUser,UpdDate) " +
                            " values (@CustomerID,GetDate(),GetDate(),Null,@BusinessDesc,@BusinessEvent,@Grade,@UpdUser,Getdate())";
                    SqlParameter[] para_nonPlanDate = {
                                    new SqlParameter("CustomerID", bus.CustomerID),                                    
                                    new SqlParameter("BusinessDesc",bus.BusinessDesc),
                                    new SqlParameter("BusinessEvent",bus.BusinessEvent),
                                    new SqlParameter("Grade",bus.Grade),
                                    new SqlParameter("UpdUser",LastUser)

                    };
                    re = dbo.ExecuteNonQuery(sqlStr, para_nonPlanDate);
                }
                else
                {

                    SqlParameter[] para_PlanDate = {
                                    new SqlParameter("CustomerID", bus.CustomerID),
                                    new SqlParameter("NextPlanDate", bus.NextPlanDate),
                                    new SqlParameter("BusinessDesc",bus.BusinessDesc),
                                    new SqlParameter("BusinessEvent",bus.BusinessEvent),
                                    new SqlParameter("Grade",bus.Grade),
                                    new SqlParameter("UpdUser",LastUser)
                    };
                    re = dbo.ExecuteNonQuery(sqlStr, para_PlanDate);

                }

                if (re==1)
                {
                    SqlParameter[] paracust = {   new SqlParameter("CustomerID", cust.CustomerID),
                                                new SqlParameter("Grade", cust.Grade) };
                    if (cust.Grade == "5")//战败
                    {
                        re = dbo.ExecuteNonQuery("Update Customers Set [Status]=7,GradeID=@Grade From Customers where CustomerID=@CustomerID", paracust);
                    }
                    else
                    {
                        re = dbo.ExecuteNonQuery("Update Customers Set [Status]=4,GradeID=@Grade From Customers where CustomerID=@CustomerID", paracust);
                    } 
                }       
            }
            return re.ToString();
        }
        // api/Customer/
        public bool CheckDuplicate(Customer cust,string CompanaID)
        {
            SQLHelper dbo = new SQLHelper();
            string sqlStr = "Select CustomerID From Customers where CompanyID="+CompanaID+" AND Telphone=" + dbo.DBStr(cust.Telphone);
            if (cust.CustomerID>0)
            {
                sqlStr += " And CustomerID<>" + cust.CustomerID.ToString() ;
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
        public bool CheckOrderStatus(int CustomerID)
        {
            SQLHelper dbo = new SQLHelper();
            string sqlStr = "Select  B.BusinessID From Customers A Inner join Business B ON A.CustomerID=B.CustomerID   Inner join [Order] C on B.BusinessID=C.BusinessID where C.OrderStatus=0 and A.CustomerID=" + CustomerID.ToString();
 
            if (dbo.ReturnInteger(sqlStr, 0) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public int GetOrderStatus(int OrderID)
        {
            SQLHelper dbo = new SQLHelper();
            string sqlStr = "Select [OrderStatus] From [Order] where OrderID=" + OrderID.ToString();
            int status = dbo.ReturnInteger(sqlStr, -1);
            return status;

        }
        /// <summary>
        /// 在指定的字符串列表CnStr中检索符合拼音索引字符串
        /// </summary>
        /// <param name="CnStr">汉字字符串</param>
        /// <returns>相对应的汉语拼音首字母串</returns>
        public string GetSpellCode(string CnStr)
        {

            string strTemp = "";

            int iLen = CnStr.Length;
            if (iLen>0)
            {
                strTemp=GetCharSpellCode(CnStr.Substring(0, 1));
            }
            return strTemp;

        }
        /// <summary>
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母
        /// </summary>
        /// <param name="CnChar">单个汉字</param>
        /// <returns>单个大写字母</returns>
        private static string GetCharSpellCode(string CnChar)
        {
            Encoding gb2312 = Encoding.GetEncoding("GB2312");
            string s = Pinyin.ConvertEncoding(CnChar, Encoding.UTF8, gb2312);
            return Pinyin.GetInitials(s, gb2312);

        }



    }
}