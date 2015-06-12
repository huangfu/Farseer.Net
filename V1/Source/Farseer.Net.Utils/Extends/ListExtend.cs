namespace FS.Extends
{
    public static class ListHelper
    {
        ///// <summary>
        /////     关联两个实体
        ///// </summary>
        ///// <typeparam name="T1">主实体</typeparam>
        ///// <typeparam name="T2">要附加关联的实体</typeparam>
        ///// <param name="lst">主列表</param>
        ///// <param name="JoinModule">要关联的子实体</param>
        ///// <param name="JoinModuleSelect">要附加关联的子实体的字段筛选</param>
        ///// <param name="JoinModuleID">主表关系字段</param>
        ///// <param name="defJoinModule">为空时如何处理？</param>
        ///// <param name="db">事务</param>
        //public static List<T1> Join<T1, T2>(this List<T1> lst, Expression<Func<T1, T2>> JoinModule,
        //                                    Func<T1, int?> JoinModuleID = null,
        //                                    Expression<Func<T2, object>> JoinModuleSelect = null,
        //                                    T2 defJoinModule = null, DbExecutor db = null)
        //    where T1 : IEntity, new()
        //    where T2 : IEntity, new()
        //{
        //    if (lst == null || lst.Count == 0) { return lst; }
        //    if (JoinModuleID == null) { JoinModuleID = o => o.ID; }

        //    #region 获取实际类型

        //    var memberExpression = JoinModule.Body as MemberExpression;
        //    // 获取属性类型
        //    var propertyType = (PropertyInfo)memberExpression.Member;

        //    var lstPropery = new List<PropertyInfo>();
        //    while (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
        //    {
        //        memberExpression = memberExpression.Expression as MemberExpression;
        //        lstPropery.Add((PropertyInfo)memberExpression.Member);
        //    }
        //    lstPropery.Reverse();

        //    #endregion

        //    // 内容ID
        //    var lstIDs = lst.Select(JoinModuleID).ToList().Select(o => o.GetValueOrDefault()).ToList();
        //    // 详细资料
        //    var lstSub = (new T2()) is BaseCacheModel<T2>
        //                          ? BaseCacheModel<T2>.Cache(db).ToList(lstIDs)
        //                          : BaseModel<T2>.Data.Where(o => lstIDs.Contains(o.ID))
        //                                         .Select(JoinModuleSelect)
        //                                         .Select(o => o.ID)
        //                                         .ToList(db);

        //    foreach (var item in lst)
        //    {
        //        var subInfo = lstSub.FirstOrDefault(o => o.ID == JoinModuleID.Invoke(item)) ?? defJoinModule;

        //        object value = item;
        //        foreach (var propery in lstPropery)
        //        {
        //            value = propery.GetValue(value, null);
        //        }
        //        propertyType.SetValue(value, subInfo, null);
        //    }

        //    return lst;
        //}

    }
}