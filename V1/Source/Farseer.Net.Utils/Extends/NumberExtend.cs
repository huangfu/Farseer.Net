using System;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    /// <summary>
    ///     格式化变量
    /// </summary>
    public static class NumberExtend
    {
        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this int number, bool isHaveTag = true, int len = 2)
        {
            var str = String.Empty;
            if (isHaveTag)
            {
                str = "￥";
            }
            return str + number.ToString(String.Format("n{0}", len));
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this int? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this uint number, bool isHaveTag = true, int len = 2)
        {
            var str = String.Empty;
            if (isHaveTag)
            {
                str = "￥";
            }
            return str + number.ToString(String.Format("n{0}", len));
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this uint? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this decimal number, bool isHaveTag = true, int len = 2)
        {
            var str = String.Empty;
            if (isHaveTag)
            {
                str = "￥";
            }
            return str + number.ToString(String.Format("n{0}", len));
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this decimal? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this double number, bool isHaveTag = true, int len = 2)
        {
            var str = String.Empty;
            if (isHaveTag)
            {
                str = "￥";
            }
            return str + number.ToString(String.Format("n{0}", len));
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this double? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     格式化字节数字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatBytesStr(int bytes)
        {
            if (bytes > 1073741824)
            {
                return ((double)(bytes / 1073741824)).ToString("0") + "G";
            }
            if (bytes > 1048576)
            {
                return ((double)(bytes / 1048576)).ToString("0") + "M";
            }
            if (bytes > 1024)
            {
                return ((double)(bytes / 1024)).ToString("0") + "K";
            }
            return bytes.ToString() + "Bytes";
        }

        /// <summary>
        ///     当NullOrEmpty，用新的字符串代替，否则用原来的。
        /// </summary>
        /// <param name="obj">要检测的值</param>
        /// <param name="newString">要替换的新字符串</param>
        public static string IsNullOrEmpty<T>(this T? obj, string newString) where T : struct
        {
            return (obj == null || obj.ToString().IsNullOrEmpty()) ? newString : obj.ToString();
        }

        /// <summary>
        ///     当不为NullOrEmpty，用新的字符串代替，否则用原来的。
        /// </summary>
        /// <param name="obj">要检测的值</param>
        /// <param name="newString">要替换的新字符串</param>
        public static string IsNotNullOrEmpty<T>(this T? obj, string newString) where T : struct
        {
            return (obj == null || obj.ToString().IsNullOrEmpty()) ? obj.ToString() : newString;
        }
    }
}