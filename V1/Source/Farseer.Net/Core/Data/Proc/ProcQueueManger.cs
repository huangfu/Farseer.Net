using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using FS.Core.Infrastructure;
using FS.Mapping.Context;

namespace FS.Core.Data.Proc
{
    /// <summary>
    /// 队列管理
    /// </summary>
    public class ProcQueueManger : BaseQueueManger
    {
        /// <summary>
        /// 当前所有持久化列表
        /// </summary>
        private readonly List<Queue> _groupQueueList;
        /// <summary>
        /// 所有队列的参数
        /// </summary>
        public override List<DbParameter> Param
        {
            get
            {
                var lst = new List<DbParameter>();
                _groupQueueList.Where(o => o.Param != null).Select(o => o.Param).ToList().ForEach(lst.AddRange);
                return lst;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="database">数据库操作</param>
        /// <param name="contextMap">映射关系</param>
        public ProcQueueManger(DbExecutor database, ContextMap contextMap)
            : base(database, contextMap)
        {
            _groupQueueList = new List<Queue>();
        }

        /// <summary>
        /// 获取当前队列（不存在，则创建）
        /// </summary>
        /// <param name="map">字段映射</param>
        /// <param name="name">表名称</param>
        public override Queue CreateQueue(string name, FieldMap map)
        {
            return Queue ?? (Queue = new Queue(_groupQueueList.Count, name, map, this));
        }

        /// <summary>
        /// 延迟执行数据库交互，并提交到队列
        /// </summary>
        /// <param name="act">要延迟操作的委托</param>
        /// <param name="map">字段映射</param>
        /// <param name="name">表名称</param>
        /// <param name="isExecute">是否立即执行</param>
        public override void AppendLazy(string name, FieldMap map, Action<Queue> act, bool isExecute)
        {
            try
            {
                CreateQueue(name, map);
                if (isExecute) { act(Queue); return; }
                Queue.LazyAct = act;
                if (Queue != null) { _groupQueueList.Add(Queue); }
            }
            finally
            {
                Clear();
            }
        }

        /// <summary>
        /// 提交所有GetQueue，完成数据库交互
        /// </summary>
        public int Commit()
        {
            try
            {
                foreach (var queryQueue in _groupQueueList)
                {
                    // 查看是否延迟加载
                    if (queryQueue.LazyAct != null) { queryQueue.LazyAct(queryQueue); }
                    queryQueue.Dispose();
                }
            }
            finally
            {
                // 清除队列
                _groupQueueList.Clear();
                Clear();
            }
            return 0;
        }
    }
}