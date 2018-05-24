using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Transactions;

namespace JoinInCRM.Helpers
{
    public class SQLHelper
    {

        private SqlConnection con; //创建连接对象

        #region basic operation function
        //打开数据库连接
        private void Open()
        {
            //打开数据库连接
            if (con == null)
            {
                //实例化SqlConnection对象
                con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            }
            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }
        }

        //关闭连接
        public void Close()
        {
            if (con != null)
                con.Close();
        }
        //释放数据库连接资源
        public void Dispose()
        {
            if (con != null)
            {
                con.Dispose();
                con = null;
            }
        }

        #region create datatable object
        /// <summary>
        /// create datatable object,select and return a datatable
        /// </summary>
        /// <param name="sqlStr">sql</param>
        /// <returns></returns>
        public DataTable getsqlDatable(string sqlStr)
        {
            DataTable mydt = new DataTable();
            try
            {
                this.Open();
                SqlDataAdapter sqlda = new SqlDataAdapter(sqlStr, con);

                sqlda.Fill(mydt);
                return mydt;
            }
            catch
            {
                mydt.Rows.Clear();
                return mydt;
            }
            finally
            {
                this.Close();
            }
        }

        //执行查询命令文本（返回dataset）
        public DataSet getsqlDataSet(string sqlStr)
        {
            DataSet myds = new DataSet();
            try
            {
                this.Open();
                SqlDataAdapter sqlda = new SqlDataAdapter(sqlStr, con);
                sqlda.Fill(myds);
                this.Close();
                return myds;
            }
            catch
            {
                myds.Tables.Clear();
                return myds;
            }
            finally
            {
                this.Close();
            }

        }
        #endregion

        #region execute SqlCommand
        /// <summary>
        /// execute sqlcommand,insert/update/delete 
        /// </summary>
        /// <param name="sqlStr">sql</param>
        public bool exeSQL(string sqlStr)
        {
            bool result = false;
            try
            {
                this.Open();
                SqlCommand sqlcom = new SqlCommand(sqlStr, con);
                sqlcom.ExecuteNonQuery();
                sqlcom.Dispose();
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                this.Close();
            }
            return result;

        }

        public string exeReturnSQL(string sqlStr)
        {
            string result = null;
            try
            {
                this.Open();
                SqlCommand sqlcom = new SqlCommand(sqlStr, con);
                sqlcom.ExecuteNonQuery();
                sqlcom.Dispose();
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            finally
            {
                this.Close();
            }

            return result;
        }
        #endregion

        public string ReturnStringSplit(string sqlstr)
        {

            try
            {
                this.Open();
                string re = string.Empty;
                SqlCommand sqlcom = new SqlCommand(sqlstr, con);
                SqlDataReader rdr = sqlcom.ExecuteReader();
                try
                {
                    while (rdr.Read())
                    {
                        for (int i = 0; i < rdr.FieldCount; i++)
                        {
                            re = re + rdr[i].ToString() + ",";
                        }
                    }
                }
                finally { rdr.Close(); }
                if (re != "") re = re.Substring(0, re.Length - 1);
                return re;
            }
            catch (Exception)
            { return string.Empty; }
            finally { this.Close(); }
        }

        //传入参数
        public SqlParameter MakeInParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        //初始化参数值并且转换为SqlParameter类型
        public SqlParameter MakeParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            SqlParameter param;
            if (Size > 0)
                param = new SqlParameter(ParamName, DbType, Size);
            else
                param = new SqlParameter(ParamName, DbType);
            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
                param.Value = Value;
            return param;
        }

        //将命令文本添加到SqlCommand
        private SqlCommand CreateCommand(string procName, SqlParameter[] prams)
        {
            //确认打开连接；


            this.Open();
            SqlCommand cmd = new SqlCommand(procName, con);
            cmd.CommandType = CommandType.Text;
            if (prams != null)
            {
                foreach (SqlParameter p in prams)
                    cmd.Parameters.Add(p);
            }
            return cmd;
        }

        //执行参数命令文本（无数据库中数据返回，实现增删改功能）


        public int RunProc(string procName, SqlParameter[] prams)
        {
            SqlCommand com = CreateCommand(procName, prams);
            try
            {
                com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                com.Dispose();
                this.Close();
            }
            return 1;
        }

        //直接执行SQL语句（数据库备份或恢复）
        public int RunProc(string procName)
        {
            try
            {
                this.Open();
                SqlCommand com = new SqlCommand(procName, con);
                com.ExecuteNonQuery();
                com.Dispose();
            }
            catch (Exception ex)
            {
                return -1;
            }
            finally
            {
                this.Close();
            }
            return 1;
        }

        //将命令文本添加到SqlDataAdapter
        private SqlDataAdapter CreateAdapter(string procName, SqlParameter[] prams)
        {
            this.Open();
            SqlDataAdapter dap = new SqlDataAdapter(procName, con);
            dap.SelectCommand.CommandType = CommandType.Text;  //执行类型：命令文本
            if (prams != null)
            {
                foreach (SqlParameter p in prams)
                    dap.SelectCommand.Parameters.Add(p);
            }
            return dap;
        }

        //执行查询命令文本（有返回值）
        public DataSet RunProcReturn(string procName, SqlParameter[] prams, string tbName)
        {
            SqlDataAdapter dap = CreateAdapter(procName, prams);
            DataSet ds = new DataSet();
            dap.Fill(ds, tbName);
            this.Close();
            return ds;
        }

        public string DoValidate(string procName, string userName, string password)
        {
            string result;
            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(procName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", userName);
                cmd.Parameters.AddWithValue("@Password", password);
                result = Convert.ToString(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            finally
            {
                this.Close();
            }
            return result;

        }

        //执行命令文本(查找所有)
        public DataSet RunProcReturn(string procName, string tbName)
        {
            SqlDataAdapter dap = CreateAdapter(procName, null);
            DataSet ds = new DataSet();
            dap.Fill(ds, tbName);
            this.Close();
            return ds;
        }

        public string ReturnString(string sqlstr)
        {
            try
            {
                this.Open();
                string re = "-1";
                SqlCommand cmd = new SqlCommand(sqlstr, con);
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    re = obj.ToString();
                }

                return re;
            }
            catch
            {
                return "-1";
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>Execute Sqlscript And return the number of rows affected
        /// </summary>
        /// <param name="sqlStr">transact-SQL stament</param>
        /// <param name="spara">Sqlparameter array</param>
        /// <returns>number of affected rows,if error,return -1</returns>
        public int ExecuteNonQuery(string sqlStr, SqlParameter[] spara)
        {
            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.Parameters.AddRange(spara);
                return cmd.ExecuteNonQuery();

            }
            catch(SqlException e)
            {
                return -1;
            }
            catch(Exception syse)
            {
                return -1;
            }
            finally
            {
                this.Close();
            }
        }
        /// <summary>Execute Sqlscript And return the number of rows affected
        /// </summary>
        /// <param name="sqlStr">transact-SQL stament</param>
        /// <param name="spara">Sqlparameter</param>
        /// <returns>number of affected rows,if error,return -1</returns>
        public int ExecuteNonQuery(string sqlStr, SqlParameter spara)
        {

            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.Parameters.Add(spara);
                return cmd.ExecuteNonQuery();
            }
            catch
            {
                return -1;
            }
            finally
            {
                this.Close();
            }
        }
        /// <summary> return string result for query
        /// </summary>
        /// <param name="SQL">string query sqlscript</param>
        /// <param name="ReturnThisIfEmpty">If there is no item ,just return this string value specified by user</param>
        /// <returns></returns>
        public string ReturnString(string SQL, string ReturnThisIfEmpty)
        {
            try
            {
                Open();
                string re = ReturnThisIfEmpty;
                SqlCommand cmd = new SqlCommand(SQL, con);
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    re = obj.ToString();
                }

                return re;
            }
            catch
            {
                return ReturnThisIfEmpty;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>return int result for query
        /// </summary>
        /// <param name="SQL">string query sqlscript</param>
        /// <param name="ReturnThisIfEmpty">If there is no item ,just return this Integer</param>
        /// <returns></returns>
        public int ReturnInteger(string SQL, int ReturnThisIfEmpty)
        {
            try
            {
                Open();

                SqlCommand cmd = new SqlCommand(SQL, con);
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    return int.Parse(obj.ToString());
                }
                else
                {
                    return ReturnThisIfEmpty;
                }

            }
            catch
            {
                return ReturnThisIfEmpty;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>Execute Query Return String Result</summary>
        /// <param name="sqlStr">sql script for execute</param>
        /// <param name="paras"> sqlparameters array</param>
        /// <param name="returnThisValueIfNoEffective">if exec sqlstr and no affect row,then return this value specifed by user,default=""
        /// </param>
        /// <returns>string:returns the first column of first row in the result set returned by the query </returns>
        public string ExecuteQueryReturnString(string sqlStr, SqlParameter[] paras, string returnThisValueIfNullOrNoData = "")
        {
            string result = returnThisValueIfNullOrNoData;

            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.Parameters.AddRange(paras);
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    result = obj.ToString();
                }
            }
            catch
            {
                result = returnThisValueIfNullOrNoData;
            }
            finally
            {
                this.Close();
            }

            return result;
        }

        /// <summary>Execute query and return string result</summary>
        /// <param name="sqlStr">sql script for execute</param>
        /// <param name="paras"> sqlparameter</param>
        /// <param name="returnThisValueIfNoAffect">if exec sqlstr and no affect row,then return this value specifed by user,default=""
        /// </param>
        /// <returns>returns the first column of first row in the result set returned by the query </returns>
        public string ExecuteQueryReturnString(string sqlStr, SqlParameter paras, string returnThisValueIfNullOrNoData = "")
        {
            string result = returnThisValueIfNullOrNoData;
            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.Parameters.Add(paras);
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    result = obj.ToString();
                }
            }
            catch
            {
                result = returnThisValueIfNullOrNoData;
            }
            finally
            {
                this.Close();
            }

            return result;
        }

        /// <summary>Execute query and return int result,
        /// use for single row single col return,
        /// "" or null or no data:return 0; exec error: return -1
        /// </summary>
        /// <param name="sqlStr">sql script for execute</param>
        /// <param name="paras"> sqlparameter array</param>
        /// <param name="returnThisValueIfNullOrNoData">if exec sqlstr and no result,then return this value specifed by user,default="0"
        /// </param>
        /// <returns>returns the first column of first row in the result set returned by the query </returns>
        public int ExecuteQueryReturnInt(string sqlStr, SqlParameter[] paras, int returnThisValueIfNullOrNoData = 0)
        {
            int result = returnThisValueIfNullOrNoData;
            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.Parameters.AddRange(paras);
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    if (obj.ToString() == "")
                    {
                        result = returnThisValueIfNullOrNoData;
                    }
                    else
                    {
                        result = int.Parse(obj.ToString());
                    }
                }
                else
                {
                    result = returnThisValueIfNullOrNoData;
                }
            }
            catch
            {
                result = -1;
            }
            finally
            {
                this.Close();
            }

            return result;
        }


        /// <summary>Execute query and return int result,
        /// use for single row single col return,
        /// "" or null or no data:return 0; exec error: return -1
        /// </summary>
        /// <param name="sqlStr">sql script for execute</param>
        /// <param name="paras"> sqlparameter</param>
        /// <param name="returnThisValueIfNullOrNoData">if exec sqlstr and no result,then return this value specifed by user,default="0"
        /// </param>
        /// <returns>returns the first column of first row in the result set returned by the query </returns>
        public int ExecuteQueryReturnInt(string sqlStr, SqlParameter paras, int returnThisValueIfNullOrNoData = 0)
        {
            int result = returnThisValueIfNullOrNoData;
            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.Parameters.Add(paras);
                object obj = cmd.ExecuteScalar();
                if (obj != null)
                {
                    if (obj.ToString() == "")
                    {
                        result = returnThisValueIfNullOrNoData;
                    }
                    else
                    {
                        result = int.Parse(obj.ToString());
                    }
                }
                else
                {
                    result = returnThisValueIfNullOrNoData;
                }
            }
            catch
            {
                result = -1;
            }
            finally
            {
                this.Close();
            }

            return result;
        }

        /// <summary>Execute query and return dataset result
        /// </summary>
        /// <param name="sqlStr">transact-SQL stament</param>
        /// <param name="para">sqlparameter</param>
        /// <returns>dataset</returns>
        public DataSet getsqlDataSet(string sqlStr, SqlParameter para)
        {
            DataSet myds = new DataSet();

            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.Parameters.Add(para);
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                sqlda.Fill(myds);
                this.Close();
            }
            catch
            {
                myds.Tables.Clear();
            }
            finally
            {
                this.Close();
            }
            return myds;
        }
        /// <summary>Execute query and return dataset result
        /// </summary>
        /// <param name="sqlStr">transact-SQL stament</param>
        /// <param name="para">sqlparameter array</param>
        /// <returns>dataset</returns>
        public DataSet getsqlDataSet(string sqlStr, SqlParameter[] para)
        {
            DataSet myds = new DataSet();

            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.Parameters.AddRange(para);
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                sqlda.Fill(myds);
                this.Close();
            }
            catch
            {
                myds.Tables.Clear();
            }
            finally
            {
                this.Close();
            }
            return myds;
        }

        /// <summary>Execute query and return a datatable
        /// </summary>
        /// <param name="sqlStr">transact-SQL stament</param>
        /// <param name="para">sqlparameter</param>
        /// <returns>datatable</returns>
        public DataTable getsqlDatable(string sqlStr, SqlParameter para)
        {
            DataTable mydt = new DataTable();
            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.Parameters.Add(para);
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                sqlda.Fill(mydt);
            }
            catch
            {
                mydt.Rows.Clear();
            }
            finally
            {
                this.Close();
            }
            return mydt;
        }

        /// <summary>Execute query and return a datatable
        /// </summary>
        /// <param name="sqlStr">transact-SQL stament</param>
        /// <param name="para">sqlparameter array</param>
        /// <returns>datatable</returns>
        public DataTable getsqlDatable(string sqlStr, SqlParameter[] para)
        {
            DataTable mydt = new DataTable();
            try
            {
                this.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, con);
                cmd.Parameters.AddRange(para);
                SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                sqlda.Fill(mydt);
            }
            catch
            {
                mydt.Rows.Clear();
            }
            finally
            {
                this.Close();
            }
            return mydt;
        }

        #region batch insert by SqlBulkCopy
        public string ExecuteTransactionScopeInsert(DataTable dt, string tableName)
        {
            int count = dt.Rows.Count;
            int copyTimeout = 600;
            string strReturn = "";
            Open();

            try
            {

                using (TransactionScope scope = new TransactionScope())
                {
                    //con.Open();
                    using (SqlBulkCopy sbc = new SqlBulkCopy(con))
                    {
                        //destination table   
                        sbc.DestinationTableName = tableName;
                        sbc.BatchSize = dt.Rows.Count;
                        sbc.BulkCopyTimeout = copyTimeout;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            //match relationship   
                            sbc.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }

                        sbc.WriteToServer(dt);

                        scope.Complete();
                    }
                }

            }
            catch (Exception ex)
            {
                //throw ex;
                strReturn = "Batch Insert Issue:" + ex.Message;
            }
            finally
            {
                try
                {
                    Close();
                }
                catch
                { }
            }

            return strReturn;

        }
        #endregion

        #endregion


        #region Assist Method
        /// <summary>
        /// return the string which could use in SQL .
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public string DBStr(string strSource)
        {
            string strResult = strSource.Replace("'", "''");

            strResult = "'" + strResult + "'";

            return strResult;
        }

        /// <summary>
        /// 将 查询 加入行号，以便进一步分页处理
        /// </summary>
        /// <param name="querySqlStr">查询语句</param>
        /// <param name="orderByStr">排序语句，Order By Field1,Field2... asc/desc,不要带如A.Field的别名</param>
        /// <returns></returns>

        public string GetPageListSql(string querySqlStr, string orderByStr)
        {
            string strSql = @"SELECT * FROM ( SELECT ROW_NUMBER() OVER (" + orderByStr + ") AS [Row],T.*" +
                           " FROM (" + querySqlStr +
                           " ) T) TT " +
                           " ";
            return strSql;
        }
        #endregion


    }
}