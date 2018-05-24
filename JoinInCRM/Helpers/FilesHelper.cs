using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JoinInCRM.Helpers
{
    public class FilesHelper
    {

        //写日志


        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="filePath">指定要删除的文件名</param>
        /// <returns>删除结果，成功true，失败false</returns>
        public bool DeleteFile(string filePath)
        {
            bool result = false;
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 创建文件夹路径
        /// </summary>
        /// <param name="path">要创建的路径</param>
        /// <returns>创建结果：true 成功 false 失败</returns>
        public bool CreateFolderPath(string path)
        {
            bool result = false;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    result = true;
                }
                else
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

            return result;

        }

        public void WriteLog(string str)
        {
            try
            {
                if (str!="")
                {
                    str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+": " + str+"\r\n";
                }
                StreamWriter sw = new StreamWriter(@"D:\Projects\wechatlog.txt", true);
                sw.Write(str);
                sw.Close();
            }
            catch
            { 
          
            }
           
        }
    }
}