using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using FS.Configs;
using FS.Extends;
using FS.Utils.Web.Common;

namespace FS.Utils.Web.Page
{
    /// <summary>
    ///     控制器基类
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        ///     HttpContext.Current.Request
        /// </summary>
        protected new HttpRequest Request = System.Web.HttpContext.Current.Request;

        /// <summary>
        ///     HttpContext.Current.Response
        /// </summary>
        protected new HttpResponse Response = System.Web.HttpContext.Current.Response;

        /// <summary>
        ///     构造
        /// </summary>
        protected BaseController()
        {
            WebTitle = WebConfigs.ConfigEntity.WebTitle;
        }

        #region Request

        /// <summary>
        ///     Request.QueryString
        /// </summary>
        protected string QS(string parmsName, Encoding encoding)
        {
            return Req.QS(parmsName, encoding);
        }

        /// <summary>
        ///     Request.QueryString
        /// </summary>
        protected string QS(string parmsName)
        {
            return Req.QS(parmsName);
        }

        /// <summary>
        ///     Request.QueryString
        /// </summary>
        protected T QS<T>(string parmsName, T defValue)
        {
            return Req.QS(parmsName, defValue);
        }

        /// <summary>
        ///     Request.Form
        /// </summary>
        protected T QF<T>(string parmsName, T defValue)
        {
            return Req.QF(parmsName, defValue);
        }

        /// <summary>
        ///     Request.Form
        /// </summary>
        protected string QF(string parmsName)
        {
            return Req.QF(parmsName);
        }

        /// <summary>
        ///     先QF后QS
        /// </summary>
        /// <param name="parmsName"></param>
        /// <returns></returns>
        protected string QA(string parmsName)
        {
            return Req.QA(parmsName);
        }

        /// <summary>
        ///     先QF后QS
        /// </summary>
        /// <param name="parmsName"></param>
        /// <returns></returns>
        protected T QA<T>(string parmsName, T defValue)
        {
            return Req.QA(parmsName, defValue);
        }

        #endregion

        /// <summary>
        /// return a mvc reqeust object.
        /// </summary>
        protected HttpRequestBase MvcReqeust
        {
            get
            {
                return Request.RequestContext.HttpContext.Request;
            }
        }

        /// <summary>
        ///     网站标题
        /// </summary>
        protected string WebTitle { get; set; }

        /// <summary>
        ///     转到网址
        /// </summary>
        protected void GoToUrl(string url, params object[] args)
        {
            Req.GoToUrl(url, args);
        }

        /// <summary>
        ///     转到网址(默认为最后一次访问)
        /// </summary>
        protected void GoToUrl(string url = "")
        {
            Req.GoToUrl(url);
        }

        /// <summary>
        ///     刷新当前页
        /// </summary>
        protected void Refresh()
        {
            GoToUrl("{0}?{1}", MvcReq.GetPageName(), MvcReq.GetParams());
        }

        /// <summary>
        ///     刷新整页
        /// </summary>
        /// <param name="link"></param>
        protected void RefreshParent(string link)
        {
            Response.Write(
                string.Format("<script type=\"text/javascript\">parent.document.location.href=\"{0}\"</script>", link));
        }

        /// <summary>
        ///     返回连接参数
        /// </summary>
        /// <param name="kic">页面需要用到的参数名称、值</param>
        /// <param name="parmsName">要重新赋值的参数</param>
        /// <param name="value">新的参数值</param>
        protected string Parms(Dictionary<string, object> kic, string parmsName, object value)
        {
            var parms = string.Empty;
            foreach (var kvp in kic)
            {
                parms += string.Format("{0}={1}&", kvp.Key, kvp.Key.IsEquals(parmsName) ? value : kvp.Value);
            }
            return parms.DelEndOf("&");
        }

        /// <summary>
        ///     返回连接参数
        /// </summary>
        /// <param name="kic">页面需要用到的参数名称、值</param>
        /// <param name="parmsName">省略key等于当前参数名称的值</param>
        protected string Parms(Dictionary<string, object> kic, string parmsName)
        {
            var parms = string.Empty;
            foreach (var kvp in kic)
            {
                if (kvp.Key.IsEquals(parmsName))
                {
                    continue;
                }
                parms += string.Format("{0}={1}&", kvp.Key, kvp.Value);
            }
            return parms.DelEndOf("&");
        }
    }
}