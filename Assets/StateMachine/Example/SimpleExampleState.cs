using System.Threading;
using System.Threading.Tasks;
using MbsCore.StateMachine.Infrastructure;
using UnityEngine;

namespace MbsCore.StateMachine.Example
{
    public sealed class SimpleExampleState : IExampleState, IExitableState
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