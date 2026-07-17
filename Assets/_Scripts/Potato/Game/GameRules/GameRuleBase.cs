using UnityEngine;

namespace Potato.Game
{
    public abstract class GameRuleBase : ScriptableObject
    {
        protected MonoBehaviour _owner = null;
        public void InitializeRule(MonoBehaviour rulesManager)
        {
            _owner = rulesManager;
            StartRule();
        }

        public virtual void StartRule() { }
        public virtual void UpdateRule(float dt) { }
        public virtual void EndRule() { }
    }
}