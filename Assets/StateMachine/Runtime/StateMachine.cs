using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MbsCore.Extensions.Runtime;
using MbsCore.StateMachine.Infrastructure;
using UnityEngine;

namespace MbsCore.StateMachine.Runtime
{
    public abstract class StateMachine<TState> : IStateMachine<TState>, IDisposable where TState : IState
    {
        public event Action<TState, TState> OnStateChanged;

        private readonly List<TState> _states;
        private readonly Dictionary<Type, TState> _stateMap;

        public TState CurrentState { get; private set; }

        public StateMachine(IEnumerable<TState> states)
        {
            _states = new List<TState>(states);
            _stateMap = new Dictionary<Type, TState>(_states.Count);
            CurrentState = default;
        }

        public StateMachine(params TState[] states)
        {
            _states = new List<TState>(states);
            _stateMap = new Dictionary<Type, TState>(_states.Count);
            CurrentState = default;
        }

        public async Task EnterAsync<TEnterState>() where TEnterState : TState
        {
            await Enter<TEnterState, object>(null);
        }

        public async Task EnterAsync<TEnterState, T>(T value) where TEnterState : TState
        {
            await Enter<TEnterState, T>(value);
        }

        public void Dispose()
        {
            Task.Factory.StartNew(TryExitCachedState);
        }
        
        private bool TryGetState(Type stateType, out TState state)
        {
            if (_stateMap.TryGetValue(stateType, out state))
            {
                return true;
            }

            int smallestWeight = int.MaxValue;
            bool found = false;
            for (int i = _states.Count - 1; i >= 0; i--)
            {
                TState cachedState = _states[i];
                if (!cachedState.GetType().IsAssignableFrom(stateType))
                {
                    continue;
                }

                int weight = cachedState.GetType().Comparison(stateType);
                if (weight <= smallestWeight)
                {
                    smallestWeight = weight;
                    state = cachedState;
                    found = true;
                }
            }

            if (!found)
            {
                Debug.LogError($"Could not found state [{stateType.FullName}]");
                return false;
            }
            
            _stateMap.Add(stateType, state);
            return false;
        }

        private async Task TryExitCachedState()
        {
            if (CurrentState == null ||
                CurrentState is not IExitableState exitableState)
            {
                return;
            }
            
            var exitableTokenSource = new CancellationTokenSource();
            Debug.Log($"Begin exit from state [{exitableState.GetType().FullName}]");
            await exitableState.ExitAsync(exitableTokenSource.Token);
            if (exitableTokenSource.IsCancellationRequested)
            {
                Debug.LogError($"Could not exit from state [{exitableState.GetType().FullName}]");
            }
            else
            {
                Debug.Log($"End exit form state [{exitableState.GetType().FullName}]");
            }
        }

        private void TrySetupState<TValue>(IState state, TValue value)
        {
            if (state is not ISetupableState<TValue> setupable)
            {
                return;
            }
            
            setupable.Setup(value);
        }

        private async Task Enter<TEnterState, TValue>(TValue value) where TEnterState : TState
        {
            TState lastState = CurrentState;
            await TryExitCachedState();
            if (!TryGetState(typeof(TEnterState), out TState state))
            {
                return;
            }
            
            TrySetupState(state, value);
            Debug.Log($"Begin enter to state [{state.GetType().FullName}]");
            var enterTokenSource = new CancellationTokenSource();
            await state.EnterAsync(enterTokenSource.Token);
            if (enterTokenSource.IsCancellationRequested)
            {
                Debug.LogError($"Could not enter to state [{state.GetType().FullName}]");
            }
            else
            {
                CurrentState = state;
                OnStateChanged?.Invoke(lastState, CurrentState);
                Debug.Log($"End enter to state [{state.GetType().FullName}]");
            }
        }
    }
}