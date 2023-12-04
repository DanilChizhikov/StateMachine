using System.Threading;
using System.Threading.Tasks;

namespace MbsCore.StateMachine.Infrastructure
{
    public interface IState
    {
        Task EnterAsync(CancellationToken token);
    }
}