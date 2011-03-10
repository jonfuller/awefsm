using System;
using System.Collections.Generic;
using System.Threading;
using AweFsm.Dispatch;
using AweFsm.EventArgs;
using AweFsm.StateBag;
using log4net;

namespace AweFsm
{
    // Sorry, Zed, for corrupting your JS FSM in this way.
    public class FiniteStateMachine : IStateMachine
    {
        public event Action<object> OnTransition = delegate { };
        public event Action<string> OnUnknownState = delegate { };
        public event Action<string, object> OnEvent = delegate { };
        public event Action<FsmUnknownEventArgs> OnUnknownEvent = delegate { };
        public event Action<FsmExceptionEventArgs> OnException = delegate { };
        public event Action Stopped = delegate { };

        readonly Type _initialStateType;
        readonly IStateMachine _parentMachine;
        readonly IFsmStateBag _stateBag;
        readonly ILog _logger;
        readonly IDispatcher _dispatcher;

        readonly Queue<Action> _pendingActions = new Queue<Action>();

        public object State { get; private set; }
        public bool IsRunning { get; private set; }

        public FiniteStateMachine(Type initialStateType, IFsmStateBag stateBag, ILog logger, IDispatcher dispatcher)
            : this(initialStateType, null, stateBag, logger, dispatcher)
        {
        }

        public FiniteStateMachine(Type initialStateType, IStateMachine parent, IFsmStateBag stateBag, ILog logger, IDispatcher dispatcher)
        {
            _initialStateType = initialStateType;
            _parentMachine = parent;
            _stateBag = stateBag;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public IStateMachine Child { get; private set; }

        public IStateMachine BuildSubMachine(Func<IDispatcher, IFsmStateBag, ILog, IStateMachine> machineBuilder)
        {
            var child = machineBuilder(_dispatcher, _stateBag, _logger);
            Child = child;
            child.Stopped += () => Child = null;
            return child;
        }

        public void Stop()
        {
            IsRunning = false;
            _logger.Info("Preparing to stop");
        }

        public void Start()
        {
            State = null;
            IsRunning = true;
            _logger.Debug("Start FSM");
            Transition(_initialStateType);

            while (IsRunning)
            {
                Action toProcess = null;
                lock (_pendingActions)
                {
                    if (_pendingActions.Count > 0)
                        toProcess = _pendingActions.Dequeue();
                }

                if (toProcess != null)
                    toProcess();

                // HACK!
                Thread.Sleep(10);
            }
            Stopped();
            _logger.Info("Stopped");
        }

        public void Transition<T>(params object[] data)
        {
            Transition(typeof(T), data);
        }

        public void Transition(Type stateType, params object[] data)
        {
            lock (_pendingActions)
            {
                _pendingActions.Enqueue(() => ProcessTransition(stateType, data));
            }
        }

        public void Handle(string message, params object[] data)
        {
            lock (_pendingActions)
            {
                _pendingActions.Enqueue(() => ProcessHandle(message, data));
            }
        }

        private void ProcessTransition(Type stateType, object[] data)
        {
            var newStateName = stateType.Namespace + "." + stateType.Name;

            _logger.Info("Transition: " + newStateName);
            var newState = _stateBag.Get(this, newStateName);
            if (newState == null)
            {
                _logger.Warn(newStateName + ": Unknown state!");
                OnUnknownState(newStateName);
                return;
            }
            
            OnTransition(newState);

            if (State != null && _dispatcher.CanSend(State, "StateExit"))
            {
                _dispatcher.Send(State, "StateExit");
            }

            if (_dispatcher.CanSend(newState, "StateEnter", this))
            {
                var args = new List<object>();
                args.Add(this);
                if (data != null)
                    args.AddRange(data);

                _dispatcher.Send(newState, "StateEnter", args.ToArray());
            }
            State = newState;
        }

        private void ProcessHandle(string message, object[] data)
        {
            _logger.Info(State.GetType().FullName + ": Handle(" + message + ")");
            if(!_dispatcher.CanSend(State, message, this, data))
            {
                _logger.Warn(State.GetType().FullName + ": doesn't handle message " + message + "!");

                if (_parentMachine != null)
                {
                    _logger.InfoFormat("Sending message to parent machine: {0}", message);
                    _parentMachine.Handle(message, data);
                }

                OnUnknownEvent(new FsmUnknownEventArgs(message, data));
                return;
            }

            OnEvent(message, data);
            try
            {
                var args = new List<object>();
                args.Add(this);
                if (data != null)
                    args.AddRange(data);

                _dispatcher.Send(State, message, args.ToArray());
            }
            catch (Exception e)
            {
                _logger.WarnFormat("Error during FSM message {0} on state {1}.  Exception: {2}", message, State.GetType().FullName, e);
                var x = new FsmExceptionEventArgs(message, data, e);
                OnException(x);
                if(!x.Handled)
                    throw;
            }
        }
    }
}