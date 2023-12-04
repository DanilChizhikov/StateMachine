using System.Threading;
using System.Threading.Tasks;

namespace MbsCore.StateMachine.Infrastructure
{
    public interface IExitableState : IState
    {
        Task ExitAsync(CancellationToken token);
    }
}