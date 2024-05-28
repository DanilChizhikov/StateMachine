using System;
using System.Collections.Generic;
using System.Linq;
using MbsCore.Extensions.Runtime;

namespace MbsCore.StateMachine
{
    internal sealed class StatePool<TState> where TState : IState
    {
        private readonly TState[] _states;
        private readonly Dictionary<Type, int> _stateMap;

        public StatePool(IEnumerable<TState> states)
        {
            _states = new TState[states.Count()];
            _stateMap = new Dictionary<Type, int>(_states.Length);
            int index = 0;
            foreach (var state in states)
            {
                _states[index++] = state;
            }
        }
        
        public TState GetStateOrNull<TSearchState>() where TSearchState : TState
        {
            return TryGetState(out TSearchState state) ? state : default;
        }
        
        public TState GetStateOrNull(Type stateType)
        {
            return TryGetState(stateType, out TState state) ? state : default;
        }
        
        public bool TryGetState<TSearchState>(out TSearchState state) where TSearchState : TState
        {
            bool hasState = TryGetState(typeof(TSearchState), out TState foundedState);
            state = hasState ? (TSearchState)foundedState : default;
            return hasState;
        }
        
        public bool TryGetState(Type stateType, out TState state)
        {
            bool hasState = _stateMap.TryGetValue(stateType, out int stateIndex);
            if (!hasState)
            {
                stateIndex = GetStateIndex(stateType);
                hasState = stateIndex >= 0;
                if (hasState)
                {
                    _stateMap.Add(stateType, stateIndex);
                }
            }
            
            state = hasState ? _states[stateIndex] : default;
            return hasState;
        }
        
        private int GetStateIndex(Type stateType)
        {
            int index = -1;
            int smallestWeight = int.MaxValue;
            for (int i = 0; i < _states.Length; i++)
            {
                TState cachedState = _states[i];
                if (!stateType.IsAssignableFrom(cachedState.GetType()))
                {
                    continue;
                }

                int weight = stateType.Comparison(cachedState.GetType());
                if (weight <= smallestWeight)
                {
                    smallestWeight = weight;
                    index = i;
                }
            }
            
            return index;
        }
    }
}