using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AweFsm.Dispatch
{
    public class MethodBinder
    {
        public MethodInfo GetMethod(Type t, string methodName, Type[] argTypes)
        {
            MethodInfo methodInfo;
            if(!TryGetMethod(out methodInfo, t, methodName, argTypes))
                throw new Exception("No method " + t.FullName + "#" + methodName + " (with the right arguments).");
            return methodInfo;
        }

        public bool TryGetMethod(out MethodInfo methodInfo, Type type, string methodName, Type[] argTypes)
        {
            var methodsWithCorrectName = type.GetMethods()
                .Where(m => m.Name == methodName)
                .Select(m => new { Method = m, NumParams = m.GetParameters().Length })
                .OrderBy(m => m.NumParams);
            var x = methodsWithCorrectName.FirstOrDefault(m => m.NumParams == argTypes.Length);
            if(x == null)
                x = methodsWithCorrectName.FirstOrDefault(m => m.NumParams > argTypes.Length);
            if(x == null)
                x = methodsWithCorrectName.LastOrDefault();
            methodInfo = x == null ? null : x.Method;
            return methodInfo != null;
        }

        public object[] BindArguments(MethodInfo method, object[] args)
        {
            var methodParameters = method.GetParameters();
            var paramsGroup = methodParameters.GroupBy(p => p.ParameterType, p => p);

            var paramValues = new Dictionary<string, object>();

            foreach (var group in paramsGroup)
            {
                var argsForType = args
                    .Where(a => a != null)
                    .Where(a => group.Key.IsAssignableFrom(a.GetType()))
                    .ToList();

                var paramsForType = group.ToList();

                for (int i = 0; i < paramsForType.Count; i++)
                {
                    paramValues.Add(paramsForType[i].Name, argsForType.IndexOrDefault(i));
                }
            }

            var newArgs = new List<object>();
            foreach (var param in methodParameters)
                newArgs.Add(paramValues[param.Name]);
            return newArgs.ToArray();
        }
    }
}