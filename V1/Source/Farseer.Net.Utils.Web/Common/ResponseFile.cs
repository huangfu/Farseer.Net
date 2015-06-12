using System;
using System.IO;
using System.Web;

namespace FS.Utils.Web.Common
{
    /// <summary>
    ///     下载文件
    /// </summary>
    public static class ResponseFile
    {
        /// <summary>
        ///     以指定的文件路径加载后，向客户端输出
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">输出的文件名</param>
        /// <param name="contentType">将文件输出时设置的ContentType</param>
        public static void OutPut(string filePath, string fileName, string contentType = "application/octet-stream")
        {
            // 打开文件
            var iStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            OutPut(iStream, fileName, contentType);
            iStream.Close(); 
        }

        /// <summary>
        ///     以指定的流，向客户端输出
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType">将文件输出时设置的ContentType</param>
        public static void OutPut(FileStream fileStream, string fileName, string contentType = "application/octet-stream")
        {
            var response = HttpContext.Current.Response;
            try
            {
                // 需要读的数据长度
                var dataToRead = fileStream.Length;

                response.ContentType = contentType;
                response.AddHeader("Content-Disposition", "attachment;filename=" + Webs.UrlEncode(fileName).Replace("+", " "));

                // 缓冲区为10k
                var buffer = new Byte[10000];
                while (dataToRead > 0)
                {
                    // 检查客户端是否还处于连接状态
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        var length = fileStream.Read(buffer, 0, 10000);
                        response.OutputStream.Write(buffer, 0, length);
                        response.Flush();
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        // 如果不再连接则跳出死循环
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (fileStream != null) { fileStream.Close(); }
                response.End();
            }
        }
    }
}