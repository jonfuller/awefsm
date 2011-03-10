using System;
using System.Threading;
using AweFsm.Dispatch;
using AweFsm.EventArgs;
using AweFsm.StateBag;
using log4net;

namespace AweFsm
{
    public class ThreadedFininiteStateMachine : IStateMachine
    {
        readonly IStateMachine _machine;
        readonly ILog _logger;

        public ThreadedFininiteStateMachine(IStateMachine machine, ILog logger)
        {
            _machine = machine;
            _logger = logger;
        }

        public object State
        {
            get { return _machine.State; }
        }

        public bool IsRunning
        {
            get { return _machine.IsRunning; }
        }

        public IStateMachine Child
        {
            get { return _machine.Child; }
        }

        public void Stop()
        {
            _machine.Stop();
        }

        public void Start()
        {
            new Thread(_ =>
            {
                try
                {
                    _machine.Start();
                }
                catch (Exception e)
                {
                    _logger.Error("Error during synchronization", e);
                }
            }){Name = "StateMachine"}.Start();
        }

        public void Transition<T>(params object[] data)
        {
            _machine.Transition<T>(data);
        }

        public void Transition(Type stateType, params object[] data)
        {
            _machine.Transition(stateType, data);
        }

        public void Handle(string message, params object[] data)
        {
            _machine.Handle(message, data);
        }

        public IStateMachine BuildSubMachine(Func<IDispatcher, IFsmStateBag, ILog, IStateMachine> machineBuilder)
        {
            return _machine.BuildSubMachine(machineBuilder);
        }

        public event Action<object> OnTransition
        {
            add { _machine.OnTransition += value; }
            remove { _machine.OnTransition -= value; }
        }

        public event Action<string> OnUnknownState
        {
            add { _machine.OnUnknownState += value; }
            remove { _machine.OnUnknownState -= value; }
        }
        public event Action<string, object> OnEvent
        {
            add { _machine.OnEvent += value; }
            remove { _machine.OnEvent -= value; }
        }
        public event Action<FsmUnknownEventArgs> OnUnknownEvent
        {
            add { _machine.OnUnknownEvent += value; }
            remove { _machine.OnUnknownEvent -= value; }
        }

        public event Action<FsmExceptionEventArgs> OnException
        {
            add { _machine.OnException += value; }
            remove { _machine.OnException -= value; }
        }

        public event Action Stopped
        {
            add { _machine.Stopped += value; }
            remove { _machine.Stopped -= value; }
        }
    }
}