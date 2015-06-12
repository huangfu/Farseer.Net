using System.IO;
using System.Xml.Serialization;

namespace FS.Utils
{
    /// <summary>
    /// （反）序例化操作
    /// </summary>
    public static class Serialize
    {
        /// <summary>
        ///     反序列化（从指定路径中读取内容并转换成T）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名称</param>
        public static T Load<T>(string filePath, string fileName) where T : class, new()
        {
            if (!File.Exists(filePath + fileName)) { return default(T); }

            using (var fs = new FileStream(filePath + fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fs);
            }
        }

        /// <summary>
        ///     序列化（将T保存到指定路径中）
        /// </summary>
        /// <param name="t">对象</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名称</param>
        public static void Save<T>(T t, string filePath, string fileName) where T : class, new()
        {
            // 创建目录
            Directory.CreateDirectory(filePath);//filePath.Substring(0, filePath.LastIndexOf("\\", StringComparison.Ordinal))

            using (var fs = new FileStream(filePath + fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                var serializer = new XmlSerializer(t.GetType());
                serializer.Serialize(fs, t);
            }
        }
    }
}
