using System.Threading;
using System.Threading.Tasks;

namespace MbsCore.StateMachine
{
    public interface IExitableState : IState
    {
        Task ExitAsync(CancellationToken token);
    }
}