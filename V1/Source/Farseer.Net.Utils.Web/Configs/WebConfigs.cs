using System;
using FS.Utils;

// ReSharper disable once CheckNamespace
namespace FS.Configs
{
    /// <summary> 网站配置 </summary>
    public class WebConfigs : AbsConfigs<WebConfig> { }

    /// <summary> 网站配置 </summary>
    [Serializable]
    public class WebConfig
    {
        /// <summary> 网站标题 </summary>
        public string WebTitle = "感谢使用Farseer.Net V0.2";
        /// <summary> 重写域名替换(多个用;分隔) </summary>
        public string RewriterDomain = "qyn99.com;";
        /// <summary> Cookies域，不填，则自动当前域 </summary>
        public string CookiesDomain = "";
        /// <summary> 忽略登陆判断地址(多个用;分隔) </summary>
        public string IgnoreLogin = "/Login.aspx;";
        /// <summary> 上传文件的目录 </summary>
        public string UploadDirectory = "/UpLoadFile/";
    }
}