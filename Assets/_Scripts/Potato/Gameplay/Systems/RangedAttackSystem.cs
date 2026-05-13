using System;
using System.Collections.Generic;
using UnityEngine;

namespace Potato.Gameplay
{
    // todo -- utils?
    class RaycastHitDistanceComparer : IComparer<RaycastHit>
    {
        public static readonly RaycastHitDistanceComparer Instance = new();
        public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
    }

    // owner of hitscan, projectiles, and explosions
    [CreateAssetMenu(menuName = "ScriptableObjects/Systems/RangedAttack")]
    public class RangedAttackSystem : ScriptableObject
    {
        const int kRaycastBufferSize = 32;

        [SerializeField] private float maxRaycastRange = 100f; // tf2 uses ~156 in unity meters
        [Tooltip("Layers that block bullet raycasts")]
        [SerializeField] private LayerMask targetLayers;

        public float MaxAttackRange => maxRaycastRange;
        private RaycastHit[] _hitBuffer = new RaycastHit[kRaycastBufferSize];

        public int PreviewAttackRaycast(Vector3 origin, Vector3 direction, ref RaycastHit[] hits, bool sort = false)
        {
            int hitCount = Physics.RaycastNonAlloc(origin, direction, hits, maxRaycastRange, targetLayers, QueryTriggerInteraction.Ignore);
            if (sort)
                Array.Sort(hits, 0, hitCount, RaycastHitDistanceComparer.Instance);
            return hitCount;
        }

        public int DoBulletAttacks(WeaponController owner, Vector3 origin, Vector3 direction, float damage, float spread, int count, ref Vector3[] hits)
        {
            int i = 0;
            while(i < count)
                hits[i++] = DoBulletAttack(owner, origin, direction, damage, spread);

            return i;
        }

        public Vector3 DoBulletAttack(WeaponController owner, Vector3 origin, Vector3 direction, float damage, float spread)
        {
            Vector3 shotDirection = ApplySpread(direction, spread);
            var hitCount = PreviewAttackRaycast(origin, shotDirection, ref _hitBuffer, true);

            if (hitCount >= kRaycastBufferSize * .9)
                Debug.Log($"HitBuffer size {hitCount} is approaching max {kRaycastBufferSize}");

            for (int j = 0; j < hitCount; ++j)
            {
                var hit = _hitBuffer[j];

                // ignore self-hit
                if (hit.collider.gameObject == owner.gameObject)
                    continue;

                if (hit.collider.TryGetComponent(out Target component))
                    component.InflictDamage(new(damage, owner.gameObject, hit.point, shotDirection));

                // return location of the hit
                return hit.point;
            }

            // if no valid hits, return the search raycast
            return origin + maxRaycastRange * shotDirection;
        }

        public void DoProjectileAttack(WeaponController owner, ProjectileBase projectilePrefab, Vector3 origin, Vector3 direction, int count, float spread)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 shotDirection = ApplySpread(direction, spread);
                ProjectileBase newProjectile = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(shotDirection));
                newProjectile.Shoot(owner);
            }
        }

        public void DoExplosiveAttack(WeaponController owner, Vector3 origin, Vector3 direction, int count, float spread)
        {
            Debug.Log($"{count} Shots requested by {owner} from {origin} going {direction} with {spread} spread");
        }

        Vector3 ApplySpread(Vector3 vec, float spreadAngle)
        {
            if(spreadAngle == 0f)
                return vec;
                
            float spreadAngleRatio = spreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(vec, UnityEngine.Random.insideUnitSphere,
                spreadAngleRatio);

            return spreadWorldDirection;
        }
    }
}