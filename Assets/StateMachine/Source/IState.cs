using System.Threading;
using System.Threading.Tasks;

namespace MbsCore.StateMachine
{
    public interface IState
    {
        Task EnterAsync(CancellationToken token);
    }
}