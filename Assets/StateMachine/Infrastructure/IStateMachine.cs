using System;
using System.Threading.Tasks;

namespace MbsCore.StateMachine.Infrastructure
{
    public interface IStateMachine
    {
        event Action<IState, IState> OnStateChanged;
        
        IState CurrentState { get; }
        
        Task EnterAsync<TState>() where TState : IState;
        Task EnterAsync<TState, T>(T value) where TState : IState;
    }
}