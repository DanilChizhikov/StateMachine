using System.Collections.Generic;
using MbsCore.StateMachine.Infrastructure;

namespace MbsCore.StateMachine.Runtime
{
    internal sealed class StateMapBuilder<TState> where TState : IState
    {
        private readonly HashSet<TState> _states;

        public StateMapBuilder()
        {
            _states = new HashSet<TState>();
        }

        public StateMapBuilder<TState> SetState(TState state)
        {
            _states.Add(state);
            return this;
        }

        public StateMapBuilder<TState> SetStates(IEnumerable<TState> states)
        {
            foreach (var state in states)
            {
                SetState(state);
            }

            return this;
        }

        public Dictionary<int, TState> Build()
        {
            var map = new Dictionary<int, TState>();
            foreach (var state in _states)
            {
                map.Add(state.GetType().FullName.GetHashCode(), state);
            }

            return map;
        }

        public void Reset()
        {
            _states.Clear();
        }
    }
}