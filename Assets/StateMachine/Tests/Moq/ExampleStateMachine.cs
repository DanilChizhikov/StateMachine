using System.Collections.Generic;

namespace MbsCore.StateMachine.Tests
{
    internal sealed class ExampleStateMachine : StateMachine<ExampleState>
    {
        internal ExampleStateMachine(IEnumerable<ExampleState> states) : base(states)
        {
        }
    }
}