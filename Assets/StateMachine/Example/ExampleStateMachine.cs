using System.Collections.Generic;
using MbsCore.StateMachine.Runtime;

namespace MbsCore.StateMachine.Example
{
    public sealed class ExampleStateMachine : StateMachine<IExampleState>, IExampleStateMachine
    {
        public ExampleStateMachine(IEnumerable<IExampleState> states) : base(states) { }

        public ExampleStateMachine(params IExampleState[] states) : base(states) { }
    }
}