using System;
using System.Collections.Generic;

namespace AweFsm.StateBag
{
    public class HashTableStateBag : IFsmStateBag
    {
        readonly Dictionary<Type, object> _states;

        public HashTableStateBag()
        {
            _states = new Dictionary<Type, object>();
        }

        public void AddState<T>(T state)
        {
            if (_states.ContainsKey(typeof(T)))
                _states.Remove(typeof (T));
            _states.Add(typeof (T), state);
        }

        public object Get(IStateMachine fsm, string newStateType)
        {
            return _states[Type.GetType(newStateType)];
        }
    }
}