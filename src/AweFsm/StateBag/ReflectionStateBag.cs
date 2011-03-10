using System;

namespace AweFsm.StateBag
{
    public class ReflectionStateBag : IFsmStateBag
    {
        readonly IStateMachine _machine;

        public ReflectionStateBag(IStateMachine machine)
        {
            _machine = machine;
        }

        public object Get(IStateMachine fsm, string newStateType)
        {
            return Activator.CreateInstance(Type.GetType(newStateType), _machine);
        }
    }
}