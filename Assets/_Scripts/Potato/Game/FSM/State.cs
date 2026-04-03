using System;

namespace Potato.Game
{
    public class State<T>
    {
        private Action _onEnter;
        private Action<float> _onUpdate;
        private Action _onExit;
        public T Identifier { get; private set; }
        public float TimeElapsed { get; private set; }
        public State(T id, Action onEnter = null, Action<float> onUpdate = null, Action onExit = null)
        {
            Identifier = id;
            _onEnter = onEnter;
            _onUpdate = onUpdate;
            _onExit = onExit;
            TimeElapsed = 0;
        }

        public void EnterState()
        {
            TimeElapsed = 0f;
            _onEnter?.Invoke();
        }

        public void UpdateState(float dt)
        {
            TimeElapsed += dt;
            _onUpdate?.Invoke(dt);
        }

        public void ExitState() => _onExit?.Invoke();
    }
}