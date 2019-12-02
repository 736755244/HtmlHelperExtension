using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;

namespace System.Web.Mvc 
{
    /// <summary>
    /// created by meng.zhu
    /// 2019.12.02
    /// </summary>
    public static class HtmlHelperExtension
    {
        /// <summary>
        /// 自动为 Js 文件添加版本号
        /// </summary>
        /// <param name="html"></param>
        /// <param name="contentPath"></param>
        /// <returns></returns>
        public static MvcHtmlString Script(this HtmlHelper html, string contentPath)
        {
            return VersionContent(html, "<script src=\"{0}\" type=\"text/javascript\"></script>", contentPath);
        }
        /// <summary>
        /// 自动为 css 文件添加版本号
        /// </summary>
        /// <param name="html"></param>
        /// <param name="contentPath"></param>
        /// <returns></returns>
        public static MvcHtmlString Style(this HtmlHelper html, string contentPath)
        {
            return VersionContent(html, "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\">", contentPath);
        }

        private static MvcHtmlString VersionContent(this HtmlHelper html, string template, string contentPath)
        {
            var httpContenxt = html.ViewContext.HttpContext;
            //获取版本号
            string Value = VersionUtils.GetFileVersion(httpContenxt.Server.MapPath(contentPath));
            contentPath = UrlHelper.GenerateContentUrl(contentPath, httpContenxt) + "?v=" + Value;
            return MvcHtmlString.Create(string.Format(template, contentPath));
        }

    }
    public static class VersionUtils
    {
        public static Dictionary<string, string> FileHashDic = new Dictionary<string, string>();
        public static string GetFileVersion(string filePath)
        {
            /*
             * 生成版本号有三种方式
             * 1. 将文件的将最后一次写入时间作为版本号
             * 2. 从配置文件中读取预先设定版本号
             * 3. 计算文件的 hash 值  
             */

            //1、直接读取修改时间作为版本号
            return File.GetLastWriteTime(filePath).ToString("yyyyMMddHHmmss");

            //2、从配置文件中读取预先设定版本号
            //return ConfigurationManager.AppSettings["JsCssVersion"];

            //3、文件hash
            //string fileName = Path.GetFileName(filePath);
            //// 验证是否已计算过文件的Hash值，避免重复计算
            //if (FileHashDic.ContainsKey(fileName))
            //{
            //    return FileHashDic[fileName];
            //}
            //else
            //{
            //    string hashvalue = GetFileShaHash(filePath); //计算文件的hash值
            //    FileHashDic.Add(fileName, hashvalue);
            //    return hashvalue;
            //}
        }

        private static string GetFileShaHash(string filePath)
        {
            string hashSHA1 = String.Empty;
            //检查文件是否存在，如果文件存在则进行计算，否则返回空值
            if (System.IO.File.Exists(filePath))
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    //计算文件的SHA1值
                    System.Security.Cryptography.SHA1 calculator = System.Security.Cryptography.SHA1.Create();
                    Byte[] buffer = calculator.ComputeHash(fs);
                    calculator.Clear();
                    //将字节数组转换成十六进制的字符串形式
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        stringBuilder.Append(buffer[i].ToString("x2"));
                    }
                    hashSHA1 = stringBuilder.ToString();
                }//关闭文件流
            }
            return hashSHA1;
        }
    }
}