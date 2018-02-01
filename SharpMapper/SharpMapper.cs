using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace SharpMapper {
    public static class Mapper {

        public static T Map<T>(object from) where T : new() {
            return (T)Map(from, typeof(T));
        }

        public static object Map(object from, Type typeTo) {
            var to = Activator.CreateInstance(typeTo);
            Copy(from, to);
            return to;
        }

        public static void Copy(object from, object to) {
            var fromType = from.GetType();
            var toType = to.GetType();
            var fromTypeInfo = fromType.GetTypeInfo();
            var toTypeInfo = toType.GetTypeInfo();

            if (IsListMapping(fromTypeInfo, toTypeInfo)) {
                MapList((IList)from, (IList)to);
                return;
            }

            if (IsDictionaryMapping(fromTypeInfo, toTypeInfo)) {
                MapDictionary((IDictionary)from, fromTypeInfo, (IDictionary)to, toTypeInfo);
                return;
            }

            var fromProps = fromType.GetRuntimeProperties()
                                    .Where(p => p.CanRead);
            foreach(var fromProp in fromProps) {
                var toProp = toType.GetRuntimeProperty(fromProp.Name);
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
            
            var propValue = fromProp.GetValue(from);
            if (fromPropTypeInfo.IsValueType || 
                fromPropType == typeof(String) ||
                propValue == null) {

                if(propValue == null && fromPropTypeInfo.IsValueType) {
                    var type = Nullable.GetUnderlyingType(fromPropType);
                    propValue = Activator.CreateInstance(type);
                }
                toProp.SetValue(to, propValue);
                return;
            }
            var obj = Activator.CreateInstance(fromPropType);
            Copy(propValue, obj);
            toProp.SetValue(to, obj);
        }

        private static void MapList(IList from, 
                                    IList to) {
            var toItemType = to.GetType()
                               .GetTypeInfo()
                               .GenericTypeArguments.FirstOrDefault();
            foreach (var item in from) {
                var copy = Map(item, toItemType);
                to.Add(copy);
            }
        }

        private static void MapDictionary(IDictionary from, 
                                          TypeInfo fromProp, 
                                          IDictionary to, 
                                          TypeInfo toProp) {
            foreach (var key in from.Keys) {
                var copy = Map(from[key], from[key].GetType());
                to[key] = copy;
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
}
