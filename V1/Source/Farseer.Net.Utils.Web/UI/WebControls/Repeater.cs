using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using FS.Utils.Web.Common;

[assembly: TagPrefix("FS.UI.WebControls", "FS")]
namespace FS.Utils.Web.UI.WebControls
{
    /// <summary>
    ///     带分页
    /// </summary>
    [ToolboxData("<{0}:Repeater ID=\"rptList\" runat=server><ItemTemplate><tr><td><%# ((string)Container.DataItem)%></td></tr></ItemTemplate><PaginationHtml><tr class=\"tdbg\" align=\"center\" style=\"height: 28px;\"><td colspan=\"12\"><Pagination /></td></tr></PaginationHtml></{0}:Repeater>")]
    public class Repeater : System.Web.UI.WebControls.Repeater
    {
        public Repeater()
        {
            IsShowNotEnough = true;
            IsShowRecordCount = true;
        }
        /// <summary>
        ///     语言类型
        /// </summary>
        public enum LanguageType
        {
            /// <summary>
            ///     中文
            /// </summary>
            Chinease = 0,

            /// <summary>
            ///     英文
            /// </summary>
            English = 1
        }

        /// <summary>
        ///     分页显示方式
        /// </summary>
        public enum eumPageType
        {
            /// <summary>
            ///     Aspx
            /// </summary>
            Aspx = 0,

            /// <summary>
            ///     Html
            /// </summary>
            Html,

            /// <summary>
            ///     Ajax
            /// </summary>
            Ajax
        }

        private string m_PaginationHtml = "";

        /// <summary>
        ///     语言类型
        /// </summary>
        [Bindable(true),
         Category("语言设置"),
         DefaultValue(LanguageType.Chinease),
         Description("设置分页显示的语言")]
        public LanguageType Languange { get; set; }

        /// <summary>
        ///     显示方式
        /// </summary>
        [Bindable(true),
         Category("显示方式"),
         DefaultValue(eumPageType.Aspx),
         Description("设置分页显示方式")]
        public eumPageType PageType { get; set; }

        /// <summary>
        ///     显示方式
        /// </summary>
        [Browsable(false)]
        [Category("显示方式")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Description("设置分页的开始html标记")]
        [TemplateContainer(typeof(RepeaterItem))]
        public string PaginationHtml
        {
            get { return m_PaginationHtml; }
            set { m_PaginationHtml = value; }
        }

        #region 分页控件属性

        private string m_NoSelected = "Pagination";
        private int m_PageCount = 0;
        private int m_PageIndex = 1;
        private int m_PageSize = 10;
        private string m_PageUrl = "";
        private string m_Selected = "PaginationSelected";
        private int m_TotalPage = 1;
        private string m_UrlParms = "";

        /// <summary>
        ///     每页显示记录数
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(10),
            Description("分页大小")
        ]
        public int PageSize
        {
            get
            {
                if (ChangePageSize && Context != null)
                {
                    m_PageSize = Req.QS("PageSize", m_PageSize);
                    if (m_PageSize < 1)
                    {
                        m_PageSize = 10;
                    }
                }
                return m_PageSize;
            }
            set { m_PageSize = value; }
        }

        /// <summary>
        ///     总记录数
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(0),
            Description("总记录数")
        ]
        public int PageCount
        {
            get { return m_PageCount; }
            set { m_PageCount = value; }
        }

        /// <summary>
        ///     当前页码
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(1),
            Description("当前页码"),
            Browsable(false)
        ]
        public int PageIndex
        {
            get
            {
                if (Context != null)
                {
                    m_PageIndex = Req.QS("PageIndex", 1);
                    if (m_PageIndex < 1)
                    {
                        m_PageIndex = 1;
                    }
                }
                return m_PageIndex;
            }
            set { m_PageIndex = value; }
        }

        /// <summary>
        ///     总记录数
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(0),
            Description("总页数")
        ]
        public int TotalPage
        {
            get
            {
                m_TotalPage = (PageCount / PageSize);
                m_TotalPage = ((PageCount % PageSize) != 0 ? m_TotalPage + 1 : m_TotalPage);
                m_TotalPage = (m_TotalPage == 0 ? 1 : m_TotalPage);
                return m_TotalPage;
            }
        }

        /// <summary>
        ///     页面地址
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(""),
            Description("页面地址")
        ]
        public string PageUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(m_PageUrl) && Context != null)
                {
                    m_PageUrl = Req.GetPageName();
                }
                return m_PageUrl;
            }
            set { m_PageUrl = value; }
        }

        private string UrlParms
        {
            get
            {
                if (Context != null && PageType == eumPageType.Aspx)
                {
                    m_UrlParms = Req.GetParams();

                    var parms = m_UrlParms.Split('&');
                    string[] subParms;
                    m_UrlParms = "";
                    foreach (var parm in parms)
                    {
                        subParms = parm.Split('=');

                        if (
                            subParms.Length == 2 &&
                            string.Compare(subParms[0], "PageIndex", true) != 0 &&
                            string.Compare(subParms[0], "PageSize", true) != 0)
                        {
                            m_UrlParms += string.Format("&{0}={1}", subParms[0], subParms[1]);
                        }
                    }
                    if (!string.IsNullOrEmpty(m_UrlParms))
                    {
                        m_UrlParms = string.Format("?{0}", m_UrlParms.Substring(1));
                    }
                }
                return m_UrlParms;
            }
        }

        /// <summary>
        ///     选中项的CSS显示名称
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(""),
            Description("选中项的CSS显示名称")
        ]
        public string Selected
        {
            get { return m_Selected; }
            set { m_Selected = value; }
        }

        /// <summary>
        ///     未选中项的CSS显示名称
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(""),
            Description("未选中项的CSS显示名称")
        ]
        public string NoSelected
        {
            get { return m_NoSelected; }
            set { m_NoSelected = value; }
        }

        /// <summary>
        ///     当前记录小于总记录时，是否显示控件
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(true),
            Description("当前记录小于总记录时，是否显示控件")
        ]
        public bool IsShowNotEnough { get; set; }

        /// <summary>
        ///     是否允许用户更改PageSize
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(""),
            Description("是否允许用户更改PageSize")
        ]
        public bool ChangePageSize { get; set; }

        /// <summary>
        ///     是否显示跳转功能
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(""),
            Description("是否显示跳转功能")
        ]
        public bool IsShowJump { get; set; }

        /// <summary>
        ///     是否详细显示记录数
        /// </summary>
        [
            Bindable(true),
            Category("分页属性"),
            DefaultValue(""),
            Description("是否详细显示记录数")
        ]
        public bool IsShowRecordCount { get; set; }

        #endregion

        /// <summary>
        ///     输出Html
        /// </summary>
        /// <param name="writer">HtmlTextWriter</param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (!IsShowNotEnough && m_PageCount < 2)
            {
                return;
            }

            var HtmlSplit = string.Empty;
            switch (PageType)
            {
                case eumPageType.Html:
                    HtmlSplit = PageSplit.HtmlSplit(PageCount, PageSize, PageIndex, IsShowRecordCount, IsShowJump,
                                                    PageUrl + UrlParms, Selected, NoSelected);
                    break;
                case eumPageType.Ajax:
                    HtmlSplit = PageSplit.AjaxSplit(PageCount, PageSize, PageIndex, IsShowRecordCount, IsShowJump,
                                                    PageUrl + UrlParms, Selected, NoSelected);
                    break;
                default:
                    HtmlSplit = PageSplit.AspxSplit(PageCount, PageSize, PageIndex, IsShowRecordCount, IsShowJump,
                                                    PageUrl + UrlParms, Selected, NoSelected);
                    break;
            }
            if (!ChangePageSize)
            {
                HtmlSplit = Regex.Replace(HtmlSplit, string.Format("pageSize={0}&", PageSize), "", RegexOptions.None);
                HtmlSplit = Regex.Replace(HtmlSplit, string.Format("/?pageSize={0}", PageSize), "", RegexOptions.None);
                HtmlSplit = Regex.Replace(HtmlSplit, string.Format("pageSize={0}", PageSize), "", RegexOptions.None);
            }

            if (Languange == LanguageType.English)
            {
                HtmlSplit = HtmlSplit.Replace("条记录", "RecordCount")
                                     .Replace("上一页", "Previous")
                                     .Replace("下一页", "Next")
                                     .Replace("首页", "First")
                                     .Replace("尾页", "End")
                                     .Replace("跳转", "Jump")
                                     .Replace("页", "Page");
            }
            writer.WriteLine(PaginationHtml.Replace("<Pagination />", HtmlSplit).Replace("<pagination />", HtmlSplit));
        }
    }
}