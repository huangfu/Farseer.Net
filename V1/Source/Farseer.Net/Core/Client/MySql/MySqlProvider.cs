using System.Data.Common;
using FS.Core.Data;
using FS.Core.Infrastructure;

namespace FS.Core.Client.MySql
{
    public class MySqlProvider : DbProvider
    {
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("MySql.Data.MySqlClient"); }
        }

        public override string KeywordAegis(string fieldName)
        {
            return string.Format("`{0}`", fieldName);
        }
        public override ISqlBuilder CreateSqlBuilder(BaseQueueManger queueManger, Queue queue)
        {
            return new SqlBuilder(queueManger, queue);
        }

        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            return string.Format("Data Source='{0}';User Id='{1}';Password='{2}';Database='{3}';charset='gbk'", server, userID, passWord, catalog);
        }
    }
}
