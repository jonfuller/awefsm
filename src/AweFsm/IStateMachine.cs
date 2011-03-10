using System;
using AweFsm.Dispatch;
using AweFsm.EventArgs;
using AweFsm.StateBag;
using log4net;

namespace AweFsm
{
    public interface IStateMachine
    {
        object State { get; }
        void Stop();
        void Start();
        void Transition<T>(params object[] data);
        void Transition(Type stateType, params object[] data);
        void Handle(string message, params object[] data);
        bool IsRunning { get; }
        IStateMachine Child { get; }
        IStateMachine BuildSubMachine(Func<IDispatcher, IFsmStateBag, ILog, IStateMachine> machineBuilder);

        event Action<object> OnTransition;
        event Action<string> OnUnknownState;
        event Action<string, object> OnEvent;
        event Action<FsmUnknownEventArgs> OnUnknownEvent;
        event Action<FsmExceptionEventArgs> OnException;
        event Action Stopped;
    }
}