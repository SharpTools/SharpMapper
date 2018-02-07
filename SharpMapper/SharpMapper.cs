using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpMapper {
    public class Mapper {

        public static T Map<T>(object from) where T : new() {
            return (T)Map(from, typeof(T));
        }

        public static object Map(object from, Type typeTo) {
            var to = Activator.CreateInstance(typeTo);
            Copy(from, to);
            return to;
        }

        public static void Copy(object from, object to) {
            var mapper = new Mapper();
            mapper._cache.Add(from.GetHashCode(), to);
            mapper.CopyInternal(from, to);
        }

        private Dictionary<int, object> _cache = new Dictionary<int, object>();

        private Mapper() {

        }

        private void CopyInternal(object from, object to) {
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
            foreach (var fromProp in fromProps) {
                var toProp = toType.GetRuntimeProperty(fromProp.Name);
                MapProp(from, fromProp, to, toProp);
            }
        }

        private void MapProp(object from,    
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

            var fromValue = fromProp.GetValue(from);

            if (IsNotAnObject(fromPropType, fromPropTypeInfo, fromValue)) {
                if (IsNullableType(fromPropTypeInfo, fromValue)) {
                    var type = Nullable.GetUnderlyingType(fromPropType);
                    fromValue = Activator.CreateInstance(type);
                }
                else if(fromPropTypeInfo.IsEnum && toPropType == typeof(String)) {
                    fromValue = fromValue.ToString();
                }
                else if(toPropTypeInfo.IsEnum && fromPropType == typeof(String)) {
                    var name = Enum.GetNames(toPropType)
                                   .FirstOrDefault(p => p.ToLowerInvariant() == fromValue?.ToString().ToLowerInvariant());
                    if(name != null) {
                        foreach(var v in Enum.GetValues(toPropType)) {
                            if(v.ToString() == name) {
                                fromValue = v;
                            }
                        }
                    }
                    else {
                        fromValue = Activator.CreateInstance(toPropType);
                    }
                }
                toProp.SetValue(to, fromValue);
                return;
            }

            var toValue = CreateObjectFor(fromValue, toPropType);
            toProp.SetValue(to, toValue);
        }
        
        private static bool IsNotAnObject(Type fromPropType, TypeInfo fromPropTypeInfo, object fromValue) {
            return fromPropTypeInfo.IsValueType ||
                            fromPropType == typeof(String) ||
                            fromValue == null;
        }
        private static bool IsNullableType(TypeInfo fromPropTypeInfo, object fromValue) {
            return fromValue == null && fromPropTypeInfo.IsValueType;
        }

        private void MapList(IList from, 
                             IList to) {
            var toItemType = to.GetType()
                               .GetTypeInfo()
                               .GenericTypeArguments.FirstOrDefault();
            foreach (var item in from) {
                var copy = CreateObjectFor(item, toItemType);
                to.Add(copy);
            }
        }

        private void MapDictionary(IDictionary from, 
                                   TypeInfo fromProp, 
                                   IDictionary to, 
                                   TypeInfo toProp) {
            foreach (var key in from.Keys) {
                var copy = CreateObjectFor(from[key], from[key].GetType());
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

        private object CreateObjectFor(object fromValue, Type toType) {
            object toValue;
            if (_cache.ContainsKey(fromValue.GetHashCode())) {
                toValue = _cache[fromValue.GetHashCode()];
            }
            else {
                toValue = Activator.CreateInstance(toType);
                CopyInternal(fromValue, toValue);
                _cache.Add(fromValue.GetHashCode(), toValue);
            }
            return toValue;
        }
    }
}
