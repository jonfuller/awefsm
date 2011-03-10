using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AweFsm
{
    public static class Extensions
    {
        public static bool HasAttribute<TAttr>(this MemberInfo target)
        {
            return target.GetCustomAttributes(true).Any(attr => attr.GetType() == typeof(TAttr));
        }

        public static T Do<T>(this ISynchronizeInvoke target, Func<T> toDo)
        {
            return (T)target.Invoke(toDo, new object[0]);
        }

        public static void Do(this ISynchronizeInvoke target, Action toDo)
        {
            target.Invoke(toDo, new object[0]);
        }

        public static Type[] GetTypes(this object[] args)
        {
            return args.Select(a => a == null ? null : a.GetType()).ToArray();
        }

        public static T IndexOrDefault<T>(this List<T> target, int index)
        {
            if(index < target.Count)
                return target[index];
            return default(T);
        }
    }
}