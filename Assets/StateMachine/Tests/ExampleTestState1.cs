using System.Threading;
using System.Threading.Tasks;
using MbsCore.StateMachine.Infrastructure;
using UnityEngine;

namespace MbsCore.StateMachine.Tests
{
    internal sealed class ExampleTestState1 : IExitableState
    {
        public async Task EnterAsync(CancellationToken token)
        {
            Debug.Log("Enter to state");
        }

        public async Task ExitAsync(CancellationToken token)
        {
            Debug.Log("Exit from state");
        }
    }
}