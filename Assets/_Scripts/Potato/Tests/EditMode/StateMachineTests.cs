using NUnit.Framework;
using Potato.Game;

namespace Potato.Tests.EditMode
{
    public class StateMachineTests
    {
        [Test]
        public void EmptyState()
        {
            var empty = new StateMachine<int>();
            
            // shouldn't crash just because nothing is assigned
            Assert.DoesNotThrow(() => empty.Update(0f));
        }

        [Test]
        public void TimerNullState()
        {
            // TimeInState should be -1 if no current state
            var fsm = new StateMachine<int>();
            Assert.AreEqual(-1f, fsm.TimeInState);
        }

        [Test]
        public void TimerZero()
        {
            // TimeInState starts at zero
            var fsm = new StateMachine<int>();
            fsm.AddState(new State<int>(0));
            fsm.SetNextState(0);
            fsm.Update(1f);

            Assert.AreEqual(0f, fsm.TimeInState);
        }

        [Test]
        public void TimerValue()
        {
            // TimeInState starts at zero
            var fsm = new StateMachine<int>();
            fsm.AddState(new State<int>(0));
            fsm.SetNextState(0);
            fsm.Update(0f);
            fsm.Update(.25f);
            fsm.Update(.25f);
            fsm.Update(.25f);

            Assert.AreEqual(.75f, fsm.TimeInState);
        }

        [Test]
        public void EnterState()
        {
            var fsm = new StateMachine<int>();
            float data = 0f;
            
            // should enter the next state and call onEnter
            fsm.AddState(new State<int>(0, onEnter: () => data += 1f));
            fsm.SetNextState(0);
            fsm.Update(1f);

            Assert.AreEqual(1f, data);
        }

        [Test]
        public void EnterStateOnce()
        {
            var fsm = new StateMachine<int>();
            float data = 0f;

            // onEnter should only be called once
            fsm.AddState(new State<int>(0, onEnter: () => data += 1f));
            fsm.SetNextState(0);
            fsm.Update(1f);
            fsm.Update(1f);
            fsm.Update(1f);

            Assert.AreEqual(1f, data);
        }

        [Test]
        public void UpdateState()
        {
            var fsm = new StateMachine<int>();
            float data = 0f;
            
            // first update should enter the state, next update should call its update
            fsm.AddState(new State<int>(0, onUpdate: value => data += value));
            fsm.SetNextState(0);
            fsm.Update(1f);
            fsm.Update(1f);

            Assert.AreEqual(1f, data);
        }

        [Test]
        public void UpdateStateRepeatedly()
        {
            var fsm = new StateMachine<int>();
            float data = 0f;
            
            // update should be called repeatedly
            fsm.AddState(new State<int>(0, onUpdate: value => data += value));
            fsm.SetNextState(0);
            fsm.Update(1f);
            fsm.Update(1f);
            fsm.Update(1f);
            fsm.Update(1f);

            Assert.AreEqual(3f, data);
        }

        [Test]
        public void ExitState()
        {
            var fsm = new StateMachine<int>();
            float data = 0f;
            
            // should do nothing until transitioning to the next state
            fsm.AddState(new State<int>(0, onExit: () => data += 1f));
            fsm.AddState(new State<int>(1));

            fsm.SetNextState(0);
            fsm.Update(1f);
            Assert.AreEqual(0f, data);

            fsm.SetNextState(1);
            fsm.Update(1f);
            Assert.AreEqual(1f, data);
        }

        [Test]
        public void ExitStateOnce()
        {
            var fsm = new StateMachine<int>();
            float data = 0f;
            
            // onExit should only happen once
            fsm.AddState(new State<int>(0, onExit: () => data += 1f));
            fsm.AddState(new State<int>(1));

            fsm.SetNextState(0);
            fsm.Update(1f);
            Assert.AreEqual(0f, data);

            fsm.SetNextState(1);
            fsm.Update(1f);
            fsm.Update(1f);
            fsm.Update(1f);
            Assert.AreEqual(1f, data);
        }

        [Test]
        public void SetNextState()
        {
            var fsm = new StateMachine<int>();
            float alice = 0f;
            float bob = 0f;
            
            // first update should enter the state, next update should call its update
            fsm.AddState(new State<int>(0, onUpdate: value => alice += value));
            fsm.AddState(new State<int>(1, onUpdate: value => bob += value));

            // next state is applied on next update
            fsm.SetNextState(0);

            // on enter/exit triggered on first update, update hasn't happened yet
            fsm.Update(1f);

            // onUpdate hasn't been called yet
            Assert.AreEqual(0f, alice);
            Assert.AreEqual(0f, bob);

            // first update after entering state, onUpdate is called
            fsm.Update(1f);

            // alice was updated once, bob hasn't gone yet
            Assert.AreEqual(1f, alice);
            Assert.AreEqual(0f, bob);

            // bob's turn, update twice to enter state and then update once
            fsm.SetNextState(1);
            fsm.Update(1f);
            fsm.Update(1f);

            // alice stopped updating, bob has updated once
            Assert.AreEqual(1f, bob);
            Assert.AreEqual(1f, alice);
        }

        [Test]
        public void SetCurrentState()
        {
            var fsm = new StateMachine<int>();
            int alice = 0;
            int bob = 0;
            
            // first update should enter the state, next update should call its update
            fsm.AddState(new State<int>(0, onEnter: () => alice += 1));
            fsm.AddState(new State<int>(1, onEnter: () => bob += 1));

            // no state entered yet
            Assert.AreEqual(0, alice);
            Assert.AreEqual(0, bob);

            // on enter should be called immediately, without waiting for update
            fsm.SetCurrentState(0);

            Assert.AreEqual(1, alice);
            Assert.AreEqual(0, bob);

            fsm.SetCurrentState(1);
            Assert.AreEqual(1, alice);
            Assert.AreEqual(1, bob);
        }

        [Test]
        public void ResetState()
        {
            var fsm = new StateMachine<int>();
            int enters = 0;
            
            // resetting state should set state duration back to 0 and call OnEnter again
            fsm.AddState(new State<int>(0, onEnter: () => ++enters));
            fsm.SetNextState(0);
            fsm.Update(1f);
            fsm.Update(1f);
            fsm.Update(1f);
            fsm.Update(1f);

            // state has aged, onEnter has been called
            Assert.AreEqual(3f, fsm.TimeInState);
            Assert.AreEqual(1, enters);

            // time is reset, onEnter was called again
            fsm.ResetState();
            Assert.AreEqual(0f, fsm.TimeInState);
            Assert.AreEqual(2, enters);
        }
    }
}