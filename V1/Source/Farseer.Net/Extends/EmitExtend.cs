using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FS.Core;
using FS.Mapping.Context.Attribute;
using FS.Utils;

namespace FS.Extends
{
    /// <summary>
    /// DataReader Extensions
    /// </summary>
    public static class EmitExtend
    {
        #region Static Readonly Fields
        private static readonly MethodInfo DataRecord_ItemGetter_String = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(string) });
        private static readonly MethodInfo DataRecord_ItemGetter_Int = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo DataRecord_GetOrdinal = typeof(IDataRecord).GetMethod("GetOrdinal");
        private static readonly MethodInfo DataReader_Read = typeof(IDataReader).GetMethod("Read");
        private static readonly MethodInfo Convert_IsDBNull = typeof(Convert).GetMethod("IsDBNull");
        private static readonly MethodInfo DataRecord_GetDateTime = typeof(IDataRecord).GetMethod("GetDateTime");
        private static readonly MethodInfo DataRecord_GetDecimal = typeof(IDataRecord).GetMethod("GetDecimal");
        private static readonly MethodInfo DataRecord_GetDouble = typeof(IDataRecord).GetMethod("GetDouble");
        private static readonly MethodInfo DataRecord_GetInt32 = typeof(IDataRecord).GetMethod("GetInt32");
        private static readonly MethodInfo DataRecord_GetInt64 = typeof(IDataRecord).GetMethod("GetInt64");
        private static readonly MethodInfo DataRecord_GetString = typeof(IDataRecord).GetMethod("GetString");
        private static readonly MethodInfo DataRecord_IsDBNull = typeof(IDataRecord).GetMethod("IsDBNull");
        #endregion

        #region Public Static Methods

        /// <summary>
        /// 把结果集流转换成数据实体列表
        /// </summary>
        /// <typeparam name="TEntity">数据实体类型</typeparam>
        /// <param name="reader">结果集流</param>
        /// <returns>数据实体列表</returns>
        public static TEntity ToEntity<TEntity>(this IDataReader reader) where TEntity : class, new()
        {
            Check.NotNull(reader, "reader参数不能为空");
            return EntityConverter<TEntity>.Select(reader).FirstOrDefault();
        }

        /// <summary>
        /// 把结果集流转换成数据实体列表
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="reader">结果集流</param>
        /// <returns>数据实体列表</returns>
        public static List<T> ToList<T>(this IDataReader reader) where T : class, new()
        {
            Check.NotNull(reader, "reader参数不能为空");
            return EntityConverter<T>.Select(reader);
        }

        /// <summary>
        /// 把结果集流转换成数据实体序列（延迟）
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        /// <param name="reader">结果集流</param>
        /// <returns>数据实体序列（延迟）</returns>
        public static IEnumerable<T> ToListLazy<T>(this IDataReader reader) where T : class, new()
        {
            Check.NotNull(reader, "reader参数不能为空");
            return EntityConverter<T>.SelectDelay(reader);
        }

        ///// <summary>
        ///// 把结果集流转换成数据实体列表
        ///// </summary>
        ///// <typeparam name="T">数据实体类型</typeparam>
        ///// <param name="dt">结果集流</param>
        ///// <returns>数据实体列表</returns>
        //public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        //{
        //    Check.NotNull(dt, "reader参数不能为空");
        //    return EntityConverter<T>.Select(dt);
        //}

        ///// <summary>
        ///// 把结果集流转换成数据实体序列（延迟）
        ///// </summary>
        ///// <typeparam name="T">数据实体类型</typeparam>
        ///// <param name="dt">结果集流</param>
        ///// <returns>数据实体序列（延迟）</returns>
        //public static IEnumerable<T> ToListLazy<T>(this DataTable dt) where T : class, new()
        //{
        //    Check.NotNull(dt, "reader参数不能为空");
        //    return EntityConverter<T>.SelectDelay(dt);
        //}

        #endregion

        #region Class: EntityConverter<T>

        /// <summary>
        /// 实体转换器
        /// </summary>
        /// <typeparam name="T">数据实体类型</typeparam>
        private class EntityConverter<T> where T : class, new()
        {
            #region Struct: DbColumnInfo
            private struct DbColumnInfo
            {
                public readonly string ColumnName;
                public readonly Type Type;
                public readonly MethodInfo SetMethod;
                public readonly bool IsOptional;

                public DbColumnInfo(PropertyInfo prop, FieldAttribute attr)
                {
                    ColumnName = attr.Name;
                    Type = prop.PropertyType;
                    SetMethod = prop.GetSetMethod(false);
                    IsOptional = true;
                }
            }
            #endregion

            #region Fields
            private static Converter<IDataReader, T> _dataReaderLoader;
            private static Converter<IDataReader, List<T>> _batchDataReaderLoader;

            private static Converter<DataRow, T> _dataLoader;
            private static Converter<DataTable, List<T>> _batchDataLoader;
            #endregion

            #region Properties

            private static Converter<IDataReader, T> DataReaderLoader
            {
                get { return _dataReaderLoader ?? (_dataReaderLoader = CreateDataReaderLoader(new List<DbColumnInfo>(GetProperties()))); }
            }

            private static Converter<IDataReader, List<T>> BatchDataReaderLoader
            {
                get { return _batchDataReaderLoader ?? (_batchDataReaderLoader = CreateBatchDataReaderLoader(new List<DbColumnInfo>(GetProperties()))); }
            }
            private static Converter<DataRow, T> DataRowLoader
            {
                get { return _dataLoader ?? (_dataLoader = CreateDataRowLoader(new List<DbColumnInfo>(GetProperties()))); }
            }

            private static Converter<DataTable, List<T>> BatchDataRowLoader
            {
                get { return _batchDataLoader ?? (_batchDataLoader = CreateBatchDataLoader(new List<DbColumnInfo>(GetProperties()))); }
            }
            #endregion

            #region Init Methods

            private static IEnumerable<DbColumnInfo> GetProperties()
            {
                return from fieldState in CacheManger.GetFieldMap(typeof(T)).MapList where fieldState.Value.FieldAtt.IsMap select new DbColumnInfo(fieldState.Key, fieldState.Value.FieldAtt);
            }

            private static Converter<IDataReader, T> CreateDataReaderLoader(List<DbColumnInfo> columnInfoes)
            {
                var dm = new DynamicMethod(string.Empty, typeof(T), new Type[] { typeof(IDataReader) }, typeof(EntityConverter<T>));
                var il = dm.GetILGenerator();
                var item = il.DeclareLocal(typeof(T));
                // [ int %index% = arg.GetOrdinal(%ColumnName%); ]
                var colIndices = GetColumnIndices(il, columnInfoes);
                // T item = new T { %Property% = ... };
                BuildItem(il, columnInfoes, item, colIndices);
                // return item;
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ret);
                return (Converter<IDataReader, T>)dm.CreateDelegate(typeof(Converter<IDataReader, T>));
            }

            private static Converter<IDataReader, List<T>> CreateBatchDataReaderLoader(List<DbColumnInfo> columnInfoes)
            {
                var dm = new DynamicMethod(string.Empty, typeof(List<T>), new Type[] { typeof(IDataReader) }, typeof(EntityConverter<T>));
                var il = dm.GetILGenerator();
                var list = il.DeclareLocal(typeof(List<T>));
                var item = il.DeclareLocal(typeof(T));
                var exit = il.DefineLabel();
                var loop = il.DefineLabel();
                // List<T> list = new List<T>();
                il.Emit(OpCodes.Newobj, typeof(List<T>).GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, list);
                // [ int %index% = arg.GetOrdinal(%ColumnName%); ]
                var colIndices = GetColumnIndices(il, columnInfoes);
                // while (arg.Read()) {
                il.MarkLabel(loop);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, DataReader_Read);
                il.Emit(OpCodes.Brfalse, exit);
                //      T item = new T { %Property% = ... };
                BuildItem(il, columnInfoes, item, colIndices);
                //      list.Add(item);
                il.Emit(OpCodes.Ldloc_S, list);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Callvirt, typeof(List<T>).GetMethod("Add"));
                // }
                il.Emit(OpCodes.Br, loop);
                il.MarkLabel(exit);
                // return list;
                il.Emit(OpCodes.Ldloc_S, list);
                il.Emit(OpCodes.Ret);
                return (Converter<IDataReader, List<T>>)dm.CreateDelegate(typeof(Converter<IDataReader, List<T>>));
            }

            private static Converter<DataRow, T> CreateDataRowLoader(List<DbColumnInfo> columnInfoes)
            {
                var dm = new DynamicMethod(string.Empty, typeof(T), new Type[] { typeof(DataRow) }, typeof(EntityConverter<T>));
                var il = dm.GetILGenerator();
                var item = il.DeclareLocal(typeof(T));
                // [ int %index% = arg.GetOrdinal(%ColumnName%); ]
                var colIndices = GetColumnIndices(il, columnInfoes);
                // T item = new T { %Property% = ... };
                BuildItem(il, columnInfoes, item, colIndices);
                // return item;
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ret);
                return (Converter<DataRow, T>)dm.CreateDelegate(typeof(Converter<DataRow, T>));
            }

            private static Converter<DataTable, List<T>> CreateBatchDataLoader(List<DbColumnInfo> columnInfoes)
            {
                var dm = new DynamicMethod(string.Empty, typeof(List<T>), new Type[] { typeof(DataTable) }, typeof(EntityConverter<T>));
                var il = dm.GetILGenerator();
                var list = il.DeclareLocal(typeof(List<T>));
                var item = il.DeclareLocal(typeof(T));
                var exit = il.DefineLabel();
                var loop = il.DefineLabel();
                // List<T> list = new List<T>();
                il.Emit(OpCodes.Newobj, typeof(List<T>).GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, list);
                // [ int %index% = arg.GetOrdinal(%ColumnName%); ]
                var colIndices = GetColumnIndices(il, columnInfoes);
                // while (arg.Read()) {
                il.MarkLabel(loop);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, DataReader_Read);
                il.Emit(OpCodes.Brfalse, exit);
                //      T item = new T { %Property% = ... };
                BuildItem(il, columnInfoes, item, colIndices);
                //      list.Add(item);
                il.Emit(OpCodes.Ldloc_S, list);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Callvirt, typeof(List<T>).GetMethod("Add"));
                // }
                il.Emit(OpCodes.Br, loop);
                il.MarkLabel(exit);
                // return list;
                il.Emit(OpCodes.Ldloc_S, list);
                il.Emit(OpCodes.Ret);
                return (Converter<DataTable, List<T>>)dm.CreateDelegate(typeof(Converter<DataTable, List<T>>));
            }

            private static LocalBuilder[] GetColumnIndices(ILGenerator il, List<DbColumnInfo> columnInfoes)
            {
                var colIndices = new LocalBuilder[columnInfoes.Count];
                for (var i = 0; i < colIndices.Length; i++)
                {
                    // int %index% = arg.GetOrdinal(%ColumnName%);
                    colIndices[i] = il.DeclareLocal(typeof(int));
                    if (columnInfoes[i].IsOptional)
                    {
                        // try {
                        il.BeginExceptionBlock();
                    }
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldstr, columnInfoes[i].ColumnName);
                    il.Emit(OpCodes.Callvirt, DataRecord_GetOrdinal);
                    il.Emit(OpCodes.Stloc_S, colIndices[i]);
                    if (columnInfoes[i].IsOptional)
                    {
                        var exit = il.DefineLabel();
                        il.Emit(OpCodes.Leave_S, exit);
                        // } catch (IndexOutOfRangeException) {
                        il.BeginCatchBlock(typeof(IndexOutOfRangeException));
                        // //forget the exception
                        il.Emit(OpCodes.Pop);
                        // int %index% = -1; // if not found, -1
                        il.Emit(OpCodes.Ldc_I4_M1);
                        il.Emit(OpCodes.Stloc_S, colIndices[i]);
                        il.Emit(OpCodes.Leave_S, exit);
                        // } catch (ArgumentException) {
                        il.BeginCatchBlock(typeof(ArgumentException));
                        // forget the exception
                        il.Emit(OpCodes.Pop);
                        // int %index% = -1; // if not found, -1
                        il.Emit(OpCodes.Ldc_I4_M1);
                        il.Emit(OpCodes.Stloc_S, colIndices[i]);
                        il.Emit(OpCodes.Leave_S, exit);
                        // }
                        il.EndExceptionBlock();
                        il.MarkLabel(exit);
                    }
                }
                return colIndices;
            }

            private static void BuildItem(ILGenerator il, List<DbColumnInfo> columnInfoes, LocalBuilder item, LocalBuilder[] colIndices)
            {
                // T item = new T();
                il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, item);
                var skip = new Label();
                for (var i = 0; i < colIndices.Length; i++)
                {
                    if (columnInfoes[i].IsOptional)
                    {
                        // if %index% == -1 then goto skip;
                        skip = il.DefineLabel();
                        il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                        il.Emit(OpCodes.Ldc_I4_M1);
                        il.Emit(OpCodes.Beq, skip);
                    }
                    if (IsCompatibleType(columnInfoes[i].Type, typeof(int)))
                    {
                        // item.%Property% = arg.GetInt32(%index%);
                        ReadInt32(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(int?)))
                    {
                        // item.%Property% = arg.IsDBNull ? default(int?) : (int?)arg.GetInt32(%index%);
                        ReadNullableInt32(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(long)))
                    {
                        // item.%Property% = arg.GetInt64(%index%);
                        ReadInt64(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(long?)))
                    {
                        // item.%Property% = arg.IsDBNull ? default(long?) : (long?)arg.GetInt64(%index%);
                        ReadNullableInt64(il, item, columnInfoes, colIndices, i);
                    }
                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(decimal)))
                    {
                        // item.%Property% = arg.GetDecimal(%index%);
                        ReadDecimal(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else if (columnInfoes[i].Type == typeof(decimal?))
                    {
                        // item.%Property% = arg.IsDBNull ? default(decimal?) : (int?)arg.GetDecimal(%index%);
                        ReadNullableDecimal(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else if (columnInfoes[i].Type == typeof(DateTime))
                    {
                        // item.%Property% = arg.GetDateTime(%index%);
                        ReadDateTime(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else if (columnInfoes[i].Type == typeof(DateTime?))
                    {
                        // item.%Property% = arg.IsDBNull ? default(DateTime?) : (int?)arg.GetDateTime(%index%);
                        ReadNullableDateTime(il, item, columnInfoes[i].SetMethod, colIndices[i]);
                    }
                    else
                    {
                        // item.%Property% = (%PropertyType%)arg[%index%];
                        ReadObject(il, item, columnInfoes, colIndices, i);
                    }
                    if (columnInfoes[i].IsOptional)
                    {
                        // :skip
                        il.MarkLabel(skip);
                    }
                }
            }

            private static bool IsCompatibleType(Type t1, Type t2)
            {
                if (t1 == t2) return true;
                if (t1.IsEnum && Enum.GetUnderlyingType(t1) == t2) return true;
                var u1 = Nullable.GetUnderlyingType(t1);
                var u2 = Nullable.GetUnderlyingType(t2);
                if (u1 != null && u2 != null) return IsCompatibleType(u1, u2);
                return false;
            }

            private static void ReadInt32(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_GetInt32);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadNullableInt32(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var local = il.DeclareLocal(columnInfoes[i].Type);
                Label intNull = il.DefineLabel();
                Label intCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
                il.Emit(OpCodes.Brtrue_S, intNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_GetInt32);
                il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
                il.Emit(OpCodes.Br_S, intCommon);
                il.MarkLabel(intNull);
                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
                il.MarkLabel(intCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadInt64(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_GetInt64);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadNullableInt64(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                var local = il.DeclareLocal(columnInfoes[i].Type);
                Label intNull = il.DefineLabel();
                Label intCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
                il.Emit(OpCodes.Brtrue_S, intNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_GetInt64);
                il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
                il.Emit(OpCodes.Br_S, intCommon);
                il.MarkLabel(intNull);
                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
                il.MarkLabel(intCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            private static void ReadDecimal(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_GetDecimal);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadNullableDecimal(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                var local = il.DeclareLocal(typeof(decimal?));
                Label decimalNull = il.DefineLabel();
                Label decimalCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
                il.Emit(OpCodes.Brtrue_S, decimalNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_GetDecimal);
                il.Emit(OpCodes.Call, typeof(decimal?).GetConstructor(new Type[] { typeof(decimal) }));
                il.Emit(OpCodes.Br_S, decimalCommon);
                il.MarkLabel(decimalNull);
                il.Emit(OpCodes.Initobj, typeof(decimal?));
                il.MarkLabel(decimalCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadDateTime(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_GetDateTime);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadNullableDateTime(ILGenerator il, LocalBuilder item, MethodInfo setMethod, LocalBuilder colIndex)
            {
                var local = il.DeclareLocal(typeof(DateTime?));
                Label dtNull = il.DefineLabel();
                Label dtCommon = il.DefineLabel();
                il.Emit(OpCodes.Ldloca, local);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
                il.Emit(OpCodes.Brtrue_S, dtNull);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndex);
                il.Emit(OpCodes.Callvirt, DataRecord_GetDateTime);
                il.Emit(OpCodes.Call, typeof(DateTime?).GetConstructor(new Type[] { typeof(DateTime) }));
                il.Emit(OpCodes.Br_S, dtCommon);
                il.MarkLabel(dtNull);
                il.Emit(OpCodes.Initobj, typeof(DateTime?));
                il.MarkLabel(dtCommon);
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldloc, local);
                il.Emit(OpCodes.Callvirt, setMethod);
            }

            private static void ReadObject(ILGenerator il, LocalBuilder item, List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
            {
                Label common = il.DefineLabel();
                il.Emit(OpCodes.Ldloc_S, item);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
                il.Emit(OpCodes.Callvirt, DataRecord_ItemGetter_Int);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Call, Convert_IsDBNull);
                il.Emit(OpCodes.Brfalse_S, common);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldnull);
                il.MarkLabel(common);
                il.Emit(OpCodes.Unbox_Any, columnInfoes[i].Type);
                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
            }

            #endregion

            #region Internal Methods

            internal static IEnumerable<T> SelectDelay(IDataReader reader)
            {
                while (reader.Read()) yield return DataReaderLoader(reader);
            }
            internal static List<T> Select(IDataReader reader)
            {
                return BatchDataReaderLoader(reader);
            }

            internal static IEnumerable<T> SelectDelay(DataTable reader)
            {
                return from DataRow row in reader.Rows select DataRowLoader(row);
            }
            internal static List<T> Select(DataTable reader)
            {
                return BatchDataRowLoader(reader);
            }

            #endregion

        }

        #endregion

    }
}
