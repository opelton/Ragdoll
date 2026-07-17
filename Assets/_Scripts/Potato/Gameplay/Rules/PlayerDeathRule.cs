using UnityEngine;
using Potato.Game;
using System.Collections;

namespace Potato.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/GameRules/PlayerDeath")]
    public class PlayerDeathRule : GameRuleBase
    {
        [SerializeField] private PlayerCharacterControllerReference playerRef;
        [SerializeField] private GameFlowSystem gameFlow;
        [SerializeField] private float onDeathReloadDelay = 3f;

        Coroutine _playerDeathSequence = null;

        public override void StartRule()
        {
            playerRef.Value.IsAlive.OnValueChanged += PlayerAliveStatusChanged;
        }

        public override void EndRule()
        {
            if(playerRef.Value != null)
                playerRef.Value.IsAlive.OnValueChanged -= PlayerAliveStatusChanged;
            
            if(_playerDeathSequence != null)
                _owner.StopCoroutine(PlayerDeathSequence());
        }

        void PlayerAliveStatusChanged(bool alive)
        {
            if(alive == false)
                _playerDeathSequence = _owner.StartCoroutine(PlayerDeathSequence());
        }

        IEnumerator PlayerDeathSequence()
        {
            yield return new WaitForSeconds(onDeathReloadDelay);
            _playerDeathSequence = null;
            gameFlow.ReloadCurrentScene();
        }
    }
}