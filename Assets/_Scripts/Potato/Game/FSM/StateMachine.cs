using System.Collections.Generic;

namespace Potato.Game
{
    public class StateMachine<T>
    {
        private Dictionary<T, State<T>> _states;
        private State<T> _currentState;
        private State<T> _nextState;
        public StateMachine()
        {
            _states = new Dictionary<T, State<T>>();
        }

        public float TimeInState => _currentState != null ? _currentState.TimeElapsed : -1f;

        public bool AddState(State<T> state) => _states.TryAdd(state.Identifier, state);

        public void SetNextState(T identifier)
        {
            if(_states.TryGetValue(identifier, out State<T> state))
                _nextState = state;
        }

        public void Update(float dt)
        {
            if(_nextState != null && _currentState != _nextState)
            {
                _currentState?.ExitState();
                _nextState.EnterState();
                _currentState = _nextState;
                _nextState = null;
                return;
            }

            _currentState?.UpdateState(dt);
        }

        public void ResetState() => _currentState?.EnterState();
    }
}