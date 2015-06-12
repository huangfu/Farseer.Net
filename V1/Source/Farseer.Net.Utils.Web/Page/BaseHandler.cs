using System.Text;
using System.Web.SessionState;
using FS.Utils.Web.Common;

namespace FS.Utils.Web.Page
{
    /// <summary>
    /// Handler基类
    /// </summary>
    public abstract class BaseHandler : IRequiresSessionState
    {
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
    }
}
