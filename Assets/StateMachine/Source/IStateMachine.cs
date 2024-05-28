using System;
using System.Threading.Tasks;

namespace MbsCore.StateMachine
{
    public interface IStateMachine<TState> where TState : IState
    {
        event Action<TState, TState> OnStateChanged;
        
        TState CurrentState { get; }

        bool TryGetState<TSearchState>(out TSearchState state) where TSearchState : TState;
        Task EnterAsync<TEnterState>() where TEnterState : TState;
    }
}