using System.Threading;
using System.Threading.Tasks;

namespace MbsCore.StateMachine.Tests
{
    internal sealed class ExitableState : ExampleState, IExitableState
    {
        public bool IsEntered { get; private set; }
        
        public override Task EnterAsync(CancellationToken token)
        {
            IsEntered = true;
            return Task.CompletedTask;
        }

        public Task ExitAsync(CancellationToken token)
        {
            IsEntered = false;
            return Task.CompletedTask;
        }
    }
}