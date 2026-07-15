using UnityEngine;

namespace Potato.Game
{
    public abstract class GameRuleBase : ScriptableObject
    {
        public virtual void StartRule() { }
        public virtual void UpdateRule(float dt) { }
        public virtual void EndRule() { }
    }
}