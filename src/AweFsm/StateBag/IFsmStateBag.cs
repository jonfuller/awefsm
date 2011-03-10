namespace AweFsm.StateBag
{
    public interface IFsmStateBag
    {
        object Get(IStateMachine fsm, string newStateType);
    }
}