using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MbsCore.StateMachine.Infrastructure;
using UnityEngine;

namespace MbsCore.StateMachine.Runtime
{
    public sealed class StateMachine : IStateMachine, IDisposable
    {
        public event Action<IState, IState> OnStateChanged;

        private readonly Dictionary<string, IState> _stateMap;

        public IState CurrentState { get; private set; }

        public StateMachine(IEnumerable<IState> states)
        {
            _stateMap = new StateMapBuilder().SetStates(states).Build();
            CurrentState = null;
        }

        public StateMachine(params IState[] states)
        {
            var builder = new StateMapBuilder();
            for (int i = states.Length - 1; i >= 0; i--)
            {
                builder.SetState(states[i]);
            }

            _stateMap = builder.Build();
            CurrentState = null;
        }

        public async Task EnterAsync<TState>() where TState : IState
        {
            await Enter<TState, object>(null);
        }

        public async Task EnterAsync<TState, T>(T value) where TState : IState
        {
            await Enter<TState, T>(value);
        }

        public void Dispose()
        {
            Task.Factory.StartNew(TryExitCachedState);
        }
        
        private bool TryGetState(Type stateType, out IState state)
        {
            if (_stateMap.TryGetValue(stateType.FullName, out state))
            {
                return true;
            }
            
            Debug.LogError($"Could not found state [{stateType.FullName}]");
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

        private async Task Enter<TState, TValue>(TValue value) where TState : IState
        {
            IState lastState = CurrentState;
            await TryExitCachedState();
            if (!TryGetState(typeof(TState), out IState state))
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