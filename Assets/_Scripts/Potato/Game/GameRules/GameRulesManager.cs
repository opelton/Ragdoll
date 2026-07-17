using UnityEngine;

namespace Potato.Game
{
    public class GameRulesManager : MonoBehaviour
    {
        [SerializeField] private GameRuleBase[] gameRules;

        void OnEnable()
        {
            foreach(var rule in gameRules)
                rule.InitializeRule(this);
        }

        void Update()
        {
            float dt = Time.deltaTime;
            foreach(var rule in gameRules)
                rule.UpdateRule(dt);
        }

        void OnDisable()
        {
            foreach(var rule in gameRules)
                rule.EndRule();
        }
    }
}