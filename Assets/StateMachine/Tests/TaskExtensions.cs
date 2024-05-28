using System.Collections;
using System.Threading.Tasks;

namespace MbsCore.StateMachine.Tests
{
    internal static class TaskExtensions
    {
        public static IEnumerator ToCoroutine(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
    }
}