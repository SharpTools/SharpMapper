using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace SharpMapper {
    public static class SharpMapper {

        public static T Map<T>(object from) where T : new() {
            return (T)Map(from, typeof(T));
        }

        public static object Map(object from, Type typeTo) {
            var to = Activator.CreateInstance(typeTo);
            Copy(from, to);
            return to;
        }

        public static void Copy(object from, object to) {
            var fromType = from.GetType().GetTypeInfo();
            var toType = to.GetType().GetTypeInfo();

            var fromProps = fromType.DeclaredProperties
                                    .Where(p => p.CanRead);
            foreach(var fromProp in fromProps) {
                var toProp = toType.GetDeclaredProperty(fromProp.Name);
                MapProp(from, fromProp, to, toProp);
            }
        }

        private static void MapProp(object from,    
                                    PropertyInfo fromProp, 
                                    object to, 
                                    PropertyInfo toProp) {
            if (toProp == null || !toProp.CanWrite) {
                return;
            }
            var fromPropType = fromProp.PropertyType;
            var fromPropTypeInfo = fromPropType.GetTypeInfo();
            var toPropType = toProp.PropertyType;
            var toPropTypeInfo = toPropType.GetTypeInfo();

            if (IsListMapping(fromPropTypeInfo, toPropTypeInfo)) {
                MapList(from, fromProp, to, toProp, toPropType);
                return;
            }

            if(IsDictionaryMapping(fromPropTypeInfo, toPropTypeInfo)) {
                MapDictionary(from, fromProp, to, toProp, toPropType);
                return;
            }

            var propValue = fromProp.GetValue(from);
            if (fromPropTypeInfo.IsValueType || fromPropType == typeof(String)) {
                toProp.SetValue(to, propValue);
                return;
            }
            var obj = Activator.CreateInstance(fromPropType);
            Copy(propValue, obj);
            toProp.SetValue(to, obj);
        }

        private static void MapList(object from, PropertyInfo fromProp, object to, PropertyInfo toProp, Type toPropType) {
            var fromList = (IList)fromProp.GetValue(from);
            var toList = (IList)toProp.GetValue(to);
            if(toList == null) {
                toList = (IList) Activator.CreateInstance(toPropType);
                toProp.SetValue(to, toList);
            }
            foreach (var item in fromList) {
                var copy = Map(item, item.GetType());
                toList.Add(copy);
            }
        }

        private static void MapDictionary(object from, PropertyInfo fromProp, object to, PropertyInfo toProp, Type toPropType) {
            var fromDic = (IDictionary)fromProp.GetValue(from);
            var toDic = (IDictionary)toProp.GetValue(to);
            if (toDic == null) {
                toDic = (IDictionary)Activator.CreateInstance(toPropType);
                toProp.SetValue(to, toDic);
            }
            foreach (var key in fromDic.Keys) {
                var copy = Map(fromDic[key], fromDic[key].GetType());
                toDic[key] = copy;
            }
        }

        private static bool IsListMapping(TypeInfo fromPropType, TypeInfo toPropType) {
            return typeof(IList).GetTypeInfo().IsAssignableFrom(toPropType) &&
                   typeof(IList).GetTypeInfo().IsAssignableFrom(fromPropType);
        }

        private static bool IsDictionaryMapping(TypeInfo fromPropType, TypeInfo toPropType) {
            return _dicTypeInfo.IsAssignableFrom(toPropType) &&
                   _dicTypeInfo.IsAssignableFrom(fromPropType);
        }

        private static TypeInfo _dicTypeInfo = typeof(IDictionary).GetTypeInfo();
        private static TypeInfo _listTypeInfo = typeof(IList).GetTypeInfo();
    }


        //public static List<T> Map<T>(this ResultSet res) where T : new() {
        //    var list = new List<T>();
        //    Type type = typeof(T);
        //    var columns = res.GetColumnNames();
        //    var columnProps =
        //        columns.Select(x => type.GetProperty(x, ReflectionHelper.NoRestrictions | BindingFlags.IgnoreCase))
        //            .ToList();
        //    foreach (var row in res) {
        //        var obj = new T();
        //        for (int i = 0; i < columnProps.Count; i++) {
        //            var propertyInfo = columnProps[i];
        //            if (propertyInfo == null) {
        //                continue;
        //            }
        //            object value = row[i];
        //            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

        //            if (value != null && value.GetType() != targetType) {
        //                value = Convert.ChangeType(value, targetType);
        //            }
        //            propertyInfo.SetValue(obj, value, null);
        //        }
        //        list.Add(obj);

        //    }
        //    return list;
        //}

        //private static bool IsNullableType(Type type) {
        //    var typeInfo = type.GetTypeInfo();
        //    return typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        //}
}
