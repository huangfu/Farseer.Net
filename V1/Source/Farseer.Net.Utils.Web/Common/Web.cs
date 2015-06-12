using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using FS.Extends;
using FS.Utils.Common;

namespace FS.Utils.Web.Common
{
    /// <summary>
    ///     文件工具
    /// </summary>
    public static class Webs
    {
        /// <summary>
        ///     获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        public static string GetMapPath(string strPath)
        {
            try
            {
                return Files.ConvertPath(HttpContext.Current != null ? HttpContext.Current.Server.MapPath(strPath) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath));
            }
            catch
            {
                return Files.ConvertPath(strPath);
            }
        }
        /// <summary>
        ///     对Url字符，进去编码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        ///     对Url字符，进去解码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        ///     参数编码
        /// </summary>
        public static string ParmsEncode(string parms)
        {
            var lstParms = new List<string>();
            foreach (var strs in parms.Split('&'))
            {
                var index = strs.IndexOf('=');
                if (index > -1)
                {
                    lstParms.Add(strs.SubString(0, index + 1) + UrlEncode(strs.SubString(index + 1, -1)));
                }
            }
            return lstParms.ToString("&");
        }
    }
}