using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        private readonly HashSet<IState> _states = new();
        
        [SetUp]
        public void Setup()
        {
            _states.Clear();
            _states.Add(new ExampleTestState1());
            _states.Add(new ExampleTestState2());
        }

        [UnityTest]
        public IEnumerator SimpleEnterToState()
        {
            IStateMachine machine = new Runtime.StateMachine(_states);
            TaskAwaiter awaiter = machine.EnterAsync<ExampleTestState1>().GetAwaiter();
            DateTime beginTime = DateTime.UtcNow;
            TimeSpan timeout = beginTime.AddSeconds(SecondsForEnterToState) - beginTime;
            
            yield return new WaitUntil(() => EnterStatePredicate(awaiter, beginTime, timeout));

            if (IsTimeout(beginTime, timeout))
            {
                Assert.False(false, "Enter to state timeouted!");
            }
            else
            {
                bool isDesiredState = typeof(ExampleTestState1) == machine.CurrentState.GetType();
                Assert.AreEqual(isDesiredState, true);
            }
        }

        [UnityTest]
        public IEnumerator EnterToStateWithValue()
        {
            IStateMachine machine = new Runtime.StateMachine(_states);
            TaskAwaiter awaiter = machine.EnterAsync<ExampleTestState2, string>(StateValue).GetAwaiter();
            DateTime beginTime = DateTime.UtcNow;
            TimeSpan timeout = beginTime.AddSeconds(SecondsForEnterToState) - beginTime;
            
            yield return new WaitUntil(() => EnterStatePredicate(awaiter, beginTime, timeout));
            
            if (IsTimeout(beginTime, timeout))
            {
                Assert.False(false, "Enter to state timeouted!");
            }
            else
            {
                bool isDesiredState = typeof(ExampleTestState2) == machine.CurrentState.GetType();
                bool isDesiredValue = isDesiredState &&
                                      machine.CurrentState is ExampleTestState2 state &&
                                      state.Value.Equals(StateValue);
                
                Assert.AreEqual(isDesiredState && isDesiredValue, true);
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