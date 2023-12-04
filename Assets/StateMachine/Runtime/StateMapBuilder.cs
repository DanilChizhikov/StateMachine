using System.Collections.Generic;
using MbsCore.StateMachine.Infrastructure;

namespace MbsCore.StateMachine.Runtime
{
    internal sealed class StateMapBuilder
    {
        private readonly HashSet<IState> _states;

        public StateMapBuilder()
        {
            _states = new HashSet<IState>();
        }

        public StateMapBuilder SetState(IState state)
        {
            _states.Add(state);
            return this;
        }

        public StateMapBuilder SetStates(IEnumerable<IState> states)
        {
            foreach (var state in states)
            {
                SetState(state);
            }

            return this;
        }

        public Dictionary<string, IState> Build()
        {
            var map = new Dictionary<string, IState>();
            foreach (var state in _states)
            {
                map.Add(state.GetType().FullName, state);
            }

            return map;
        }

        public void Reset()
        {
            _states.Clear();
        }
    }
}