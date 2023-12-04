namespace MbsCore.StateMachine.Infrastructure
{
    public interface ISetupableState<in T>
    {
        void Setup(T value);
    }
}