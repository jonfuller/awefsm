namespace AweFsm.Dispatch
{
    public interface IDispatcher
    {
        object Send(object target, string methodName);
        object Send(object target, string methodName, object[] args);
        bool CanSend(object target, string methodName, params object[] args);
    }
}