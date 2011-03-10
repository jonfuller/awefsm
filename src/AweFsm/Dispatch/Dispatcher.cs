using System;
using System.Reflection;

namespace AweFsm.Dispatch
{
    public class Dispatcher : IDispatcher
    {
        readonly MethodBinder _binder;

        public Dispatcher(MethodBinder binder)
        {
            _binder = binder;
        }

        public object Send(object obj, string methodName)
        {
            return Send(obj, methodName, new object[0]);
        }

        public object Send(object obj, string methodName, object[] args)
        {
            try
            {
                var method = _binder.GetMethod(obj.GetType(), methodName, args.GetTypes());
                var callArgs = _binder.BindArguments(method, args);

                return method.Invoke(obj, callArgs);
            }
            catch (TargetInvocationException e)
            {
                throw new Exception("Error in dispatch", e.InnerException);
            }
        }

        public bool CanSend(object obj, string methodName, params object[] args)
        {
            MethodInfo methodInfo;
            return _binder.TryGetMethod(out methodInfo, obj.GetType(), methodName, args.GetTypes());
        }
    }
}