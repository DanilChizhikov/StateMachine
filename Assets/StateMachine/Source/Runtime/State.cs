using System;
using System.Threading;
using System.Threading.Tasks;

namespace MbsCore.StateMachine
{
    public abstract class State : IState
    {
        public event Func<Type, Task> OnNext;
        public event Func<Type, IState> OnGetState; 
        
        protected LogBuilder LogBuilder { get; }

        public State()
        {
            LogBuilder = new LogBuilder(GetType().Name);
        }
        
        public abstract Task EnterAsync(CancellationToken token);

        protected bool TryGetState<TState>(out TState state)
        {
            bool hasState = false;
            state = default;
            IState foundedState = OnGetState?.Invoke(typeof(TState));
            if (foundedState is TState genericState)
            {
                state = genericState;
                hasState = true;
            }
            
            return hasState;
        }

        protected async Task SetNext<TState>() where TState : State
        {
            if(OnNext == null)
            {
                return;
            }
            
            await OnNext.Invoke(typeof(TState));
        }
    }
}