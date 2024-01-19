using System.Threading;
using System.Threading.Tasks;
using MbsCore.StateMachine.Infrastructure;
using UnityEngine;

namespace MbsCore.StateMachine.Example
{
    public sealed class SetupableExampleState : IExampleState, IExitableState, ISetupableState<string>
    {
        public string Value { get; private set; }

        public SetupableExampleState()
        {
            Value = string.Empty;
        }
        
        public void Setup(string value)
        {
            Value = value;
        }
        
        public async Task EnterAsync(CancellationToken token)
        {
            Debug.Log($"Enter to state with value [{Value}]");
        }

        public async Task ExitAsync(CancellationToken token)
        {
            Debug.Log($"Exit from state with value [{Value}]");
        }
    }
}