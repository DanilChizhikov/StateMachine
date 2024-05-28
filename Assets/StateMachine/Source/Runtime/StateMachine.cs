using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MbsCore.StateMachine
{
    public abstract class StateMachine<TState> : IStateMachine<TState>, IAsyncDisposable
        where TState : State
    {
        public event Action<TState, TState> OnStateChanged;

        private readonly StatePool<TState> _statePool;
        private readonly LogBuilder _logBuilder;

        public TState CurrentState { get; private set; }

        public StateMachine(IEnumerable<TState> states)
        {
            _statePool = new StatePool<TState>(states);
            _logBuilder = new LogBuilder(GetType().Name);
            CurrentState = default;
        }

        public bool TryGetState<TSearchState>(out TSearchState state) where TSearchState : TState
        {
            return _statePool.TryGetState(out state);
        }

        public async Task EnterAsync<TEnterState>() where TEnterState : TState
        {
            await Enter(typeof(TEnterState));
        }
        
        public async ValueTask DisposeAsync()
        {
            await TryExitCachedState();
        }
        
        private async Task TryExitCachedState()
        {
            if (CurrentState == null ||
                CurrentState is not IExitableState exitableState)
            {
                return;
            }
            
            _logBuilder.AppendFormat("Begin exit from state [{0}]", exitableState.GetType().FullName)
                .Log();
            CurrentState.OnNext -= Enter;
            CurrentState.OnGetState -= _statePool.GetStateOrNull;
            var exitableTokenSource = new CancellationTokenSource();
            await exitableState.ExitAsync(exitableTokenSource.Token);
            if (exitableTokenSource.IsCancellationRequested)
            {
                _logBuilder.AppendFormat("Could not exit from state [{0}]", exitableState.GetType().FullName)
                    .Error();
            }
            else
            {
                _logBuilder.AppendFormat("End exit form state [{0}]", exitableState.GetType().FullName)
                    .Log();
            }
        }
        
        private async Task Enter(Type stateType)
        {
            if (!_statePool.TryGetState(stateType, out TState state))
            {
                return;
            }
            
            TState lastState = CurrentState;
            await TryExitCachedState();
            _logBuilder.AppendFormat("Begin enter to state [{0}]", state.GetType().FullName)
                .Log();
            var enterTokenSource = new CancellationTokenSource();
            await state.EnterAsync(enterTokenSource.Token);
            if (enterTokenSource.IsCancellationRequested)
            {
                _logBuilder.AppendFormat("Could not enter to state [{0}]", state.GetType().FullName)
                    .Error();
            }
            else
            {
                CurrentState = state;
                CurrentState.OnNext += Enter;
                CurrentState.OnGetState += _statePool.GetStateOrNull;
                OnStateChanged?.Invoke(lastState, CurrentState);
                _logBuilder.AppendFormat("End enter to state [{0}]", state.GetType().FullName)
                    .Log();
            }
        }
    }
}