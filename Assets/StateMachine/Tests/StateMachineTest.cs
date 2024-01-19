using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MbsCore.StateMachine.Example;
using MbsCore.StateMachine.Infrastructure;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MbsCore.StateMachine.Tests
{
    [TestFixture]
    internal sealed class StateMachineTest
    {
        private const int SecondsForEnterToState = 5;
        private const string StateValue = "Example";

        private IExampleStateMachine _stateMachine;
        
        [SetUp]
        public void Setup()
        {
            _stateMachine = new ExampleStateMachine(new SimpleExampleState(), new SetupableExampleState());
        }

        [UnityTest]
        public IEnumerator SimpleEnterToState()
        {
            TaskAwaiter awaiter = _stateMachine.EnterAsync<SimpleExampleState>().GetAwaiter();
            DateTime beginTime = DateTime.UtcNow;
            TimeSpan timeout = beginTime.AddSeconds(SecondsForEnterToState) - beginTime;
            
            yield return new WaitUntil(() => EnterStatePredicate(awaiter, beginTime, timeout));

            if (IsTimeout(beginTime, timeout))
            {
                Assert.False(false, "Enter to state timeouted!");
            }
            else
            {
                bool isDesiredState = typeof(SimpleExampleState).Equals(_stateMachine.CurrentState.GetType());
                Assert.AreEqual(isDesiredState, true);
            }
        }

        [UnityTest]
        public IEnumerator EnterToStateWithValue()
        {
            TaskAwaiter awaiter = _stateMachine.EnterAsync<SetupableExampleState, string>(StateValue).GetAwaiter();
            DateTime beginTime = DateTime.UtcNow;
            TimeSpan timeout = beginTime.AddSeconds(SecondsForEnterToState) - beginTime;
            
            yield return new WaitUntil(() => EnterStatePredicate(awaiter, beginTime, timeout));
            
            if (IsTimeout(beginTime, timeout))
            {
                Assert.False(false, "Enter to state timeouted!");
            }
            else
            {
                bool isDesiredValue = _stateMachine.CurrentState is SetupableExampleState state &&
                                      state.Value.Equals(StateValue);
                
                Assert.AreEqual(isDesiredValue, true);
            }
        }

        private bool EnterStatePredicate(TaskAwaiter awaiter, DateTime beginTime, TimeSpan timeout)
        {
            return awaiter.IsCompleted || IsTimeout(beginTime, timeout);
        }

        private bool IsTimeout(DateTime beginTime, TimeSpan timeout)
        {
            return (DateTime.UtcNow - beginTime).TotalSeconds >= timeout.TotalSeconds;
        }
    }
}