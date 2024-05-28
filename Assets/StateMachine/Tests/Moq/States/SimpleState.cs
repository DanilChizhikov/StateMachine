using System.Threading;
using System.Threading.Tasks;

namespace MbsCore.StateMachine.Tests
{
    internal sealed class SimpleState : ExampleState
    {
        public override Task EnterAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}