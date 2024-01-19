using System;
using System.Threading.Tasks;

namespace MbsCore.StateMachine.Infrastructure
{
    public interface IStateMachine<TState> where TState : IState
    {
        event Action<TState, TState> OnStateChanged;
        
        TState CurrentState { get; }
        
        Task EnterAsync<TEnterState>() where TEnterState : TState;
        Task EnterAsync<TEnterState, T>(T value) where TEnterState : TState;
    }
}