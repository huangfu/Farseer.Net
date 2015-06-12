using System;
using FS.Utils;

// ReSharper disable once CheckNamespace
namespace FS.Configs
{
    /// <summary> 网站系统配置文件 </summary>
    public class WebSystemConfigs : AbsConfigs<WebSystemConfig> { }

    /// <summary> 网站系统配置文件 </summary>
    [Serializable]
    public class WebSystemConfig
    {
        /// <summary> 管理员登陆验证码的 </summary>
        public string Cookies_Admin_VerifyCode = "Cookies_Admin_VerifyCode";
        /// <summary> Cookies前缀 </summary>
        public string Cookies_Prefix = "Farseer.Net.V1.x";
        /// <summary> Cookies超时时间(分钟) </summary>
        public int Cookies_TimeOut = 20;
        /// <summary> 用户名 </summary>
        public string Cookies_User_Name = "Cookies_User_Name";
        /// <summary> 用户登陆验证码的 </summary>
        public string Cookies_User_VerifyCode = "Cookies_User_VerifyCode";
        /// <summary> 会员类型 </summary>
        public string Cookies_User_Role = "Cookies_User_Role";
        /// <summary> 记住最后访问地址 </summary>
        public string Cookies_CallBack_Url = "Cookies_CallBack_Url";

        /// <summary> 管理员ID的Cookies </summary>
        public string Session_Admin = "Session_Admin";
        /// <summary> 管理员ID的Cookies </summary>
        public string Session_User = "Session_User";
        /// <summary> 用户ID </summary>
        public string Session_User_ID = "Session_User_ID";
        /// <summary> Session前缀 </summary>
        public string Session_Prefix = "Farseer.Net.V1.x";
        /// <summary> Session超时时间(分钟) </summary>
        public int Session_TimeOut = 20;
    }
}