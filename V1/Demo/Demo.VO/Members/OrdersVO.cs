using System;
using Demo.Common;
using FS.Core.Infrastructure;
using FS.Mapping.Context.Attribute;

namespace Demo.VO.Members
{
    /// <summary>
    /// 订单
    /// </summary>
    public class OrdersVO : IEntity<Guid>
    {
        /// <summary> 订单ID </summary>
        [Field(UpdateStatusType = StatusType.ReadCondition)]
        public Guid ID { get; set; }

        /// <summary> 订单编号 </summary>
        [Field(UpdateStatusType = StatusType.ReadOnly)]
        public string OrderNo { get; set; }

        /// <summary> 订单总价 </summary>
        [Field(UpdateStatusType = StatusType.ReadOnly)]
        public decimal Price { get; set; }

        /// <summary> 创建人ID </summary>
        [Field(UpdateStatusType = StatusType.ReadOnly)]
        public int? CreateID { get; set; }

        /// <summary> 创建人名称 </summary>
        [Field(UpdateStatusType = StatusType.ReadOnly)]
        public string CreateName { get; set; }

        /// <summary> 创建时间 </summary>
        [Field(UpdateStatusType = StatusType.ReadOnly)]
        public DateTime? CreateAt { get; set; }
    }
}
