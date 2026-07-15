using UnityEngine;
using Potato.Game;

namespace Potato.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameRules/PlayerDeath")]
    public class PlayerDeathRule : GameRuleBase
    {
        [SerializeField] private PlayerCharacterControllerReference playerRef;

        public override void StartRule()
        {
            Debug.Log("player death rule started");
            playerRef.Value.IsAlive.OnValueChanged += PlayerAliveStatusChanged;
        }

        public override void EndRule()
        {
            Debug.Log("player death rule ended");
            if(playerRef.Value != null)
                playerRef.Value.IsAlive.OnValueChanged -= PlayerAliveStatusChanged;
        }

        void PlayerAliveStatusChanged(bool alive)
        {
            Debug.Log($"gamerule {name} detected that player.IsAlive = {alive}");
        }
    }
}