using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    public class PlayerAimManager : MonoBehaviour
    {
        const int kRaycastBufferSize = 32;

        [SerializeField] private PlayerCamerasController playerCams;
        [SerializeField] private RangedAttackSystem rats;
        [SerializeField] private BoolReference isAimingAtEnemy;
        [SerializeField] private Vector3Reference playerAimPoint;

        private RaycastHit[] _hitBuffer = new RaycastHit[kRaycastBufferSize];

        void Update()
        {
            //var targetingHostile = rats.IsTargetingEnemy(gameObject, playerCams.AimPos, playerCams.AimDir);
            var targetingHostile = false;
            var hitCount = rats.PreviewAttackRaycast(playerCams.AimPos, playerCams.AimDir, ref _hitBuffer);

            if(hitCount >= kRaycastBufferSize * .9)
                Debug.Log($"HitBuffer size {hitCount} is approaching max {kRaycastBufferSize}");

            for (int i = 0; i < hitCount; ++i)
            {
                var hit = _hitBuffer[i];
                if (hit.collider.gameObject == gameObject)
                    continue;

                if (hit.collider.GetComponentInParent<Target>() != null)
                {
                    playerAimPoint.Value = hit.point;
                    targetingHostile = true;
                    break;
                }
            }

            if(!targetingHostile)
                SetAimPointNoTarget();
            
            // avoid firing an onChanged event unless it changed
            if(isAimingAtEnemy.Value != targetingHostile)
                isAimingAtEnemy.Value = targetingHostile;
        }

        void SetAimPointNoTarget() => playerAimPoint.Value = playerCams.AimPos + rats.MaxAttackRange * playerCams.AimDir;
    }
}