using System;
using System.ComponentModel;

namespace AweFsm.Dispatch
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class InvokeRequiredAttribute : Attribute
    {
    }

    public class InvokingDispatcher : IDispatcher
    {
        readonly IDispatcher _baseDispatcher;
        readonly MethodBinder _binder;
        readonly ISynchronizeInvoke _invoker;

        public InvokingDispatcher(IDispatcher baseDispatcher, MethodBinder binder, ISynchronizeInvoke invoker)
        {
            _baseDispatcher = baseDispatcher;
            _binder = binder;
            _invoker = invoker;
        }

        public object Send(object target, string methodName)
        {
            return Send(target, methodName, new object[0]);
        }

        public object Send(object target, string methodName, object[] args)
        {
            var method = _binder.GetMethod(target.GetType(), methodName, args.GetTypes());

            if(method.HasAttribute<InvokeRequiredAttribute>())
                return _invoker.Do(() => _baseDispatcher.Send(target, methodName, args));
            return _baseDispatcher.Send(target, methodName, args);
        }

        public bool CanSend(object target, string methodName, params object[] args)
        {
            return _baseDispatcher.CanSend(target, methodName, args);
        }
    }
}