namespace FS.Extends
{
    public static class FormExtend
    {
        /// <summary>
        ///     检查是否存在该类型的子窗体
        /// </summary>
        /// <param name="form">Windows窗体对象</param>
        /// <param name="childFormName">窗体名称</param>
        /// <returns>是否存在</returns>
        public static bool IsExist(this System.Windows.Forms.Form form, string childFormName)
        {
            foreach (var frm in form.MdiChildren)
            {
                if (frm.GetType().Name == childFormName)
                {
                    frm.Activate();
                    return true;
                }
            }
            return false;
        }
    }
}
