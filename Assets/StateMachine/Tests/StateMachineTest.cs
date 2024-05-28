using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MbsCore.StateMachine.Tests
{
    [TestFixture]
    internal sealed class StateMachineTest
    {
        private const string StateValue = "Example";

        private ExampleStateMachine _stateMachine;
        
        [SetUp]
        public void Setup()
        {
            _stateMachine = new ExampleStateMachine(new ExampleState[]
            {
                new SimpleState(),
                new ExitableState()
            });
        }
        
        [UnityTest]
        public IEnumerator When_Enter_Simple_State_Then_Current_State_Equals_Simple_State()
        {
            yield return _stateMachine.EnterAsync<SimpleState>().ToCoroutine();
            
            Assert.AreEqual(typeof(SimpleState), _stateMachine.CurrentState.GetType());
        }

        [UnityTest]
        public IEnumerator When_Enter_Simple_State_And_CurrentState_Is_Exitable_Then_Current_State_Equals_Simple_State_And_Exitable_State_Was_Exited()
        {
            yield return _stateMachine.EnterAsync<ExitableState>().ToCoroutine();

            ExitableState state = _stateMachine.CurrentState as ExitableState;
            
            yield return _stateMachine.EnterAsync<SimpleState>().ToCoroutine();
            
            Assert.AreEqual(typeof(SimpleState), _stateMachine.CurrentState.GetType());
            Assert.IsFalse(state.IsEntered);
        }
    }
}