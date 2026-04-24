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
        [Tooltip("Layers that can block raycasts")]
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

        public void DoHitscanAttack(WeaponController owner, Vector3 origin, Vector3 direction, float damage, int count, float spread)
        {
            for (int i = 0; i < count; ++i)
            {
                Vector3 shotDirection = ApplySpread(direction, spread);
                var hitCount = PreviewAttackRaycast(origin, shotDirection, ref _hitBuffer, true);

                if (hitCount == 0)
                    break;
                else if (hitCount >= kRaycastBufferSize * .9)
                    Debug.Log($"HitBuffer size {hitCount} is approaching max {kRaycastBufferSize}");

                for (int j = 0; j < hitCount; ++j)
                {
                    var hit = _hitBuffer[j];
                    if (hit.collider.gameObject == owner.gameObject)
                        continue;

                    if (hit.collider.TryGetComponent(out Target component))
                    {
                        component.InflictDamage(damage, hit.point, direction, owner.gameObject);
                        break;
                    }
                }
            }
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

        public void FireTracers(WeaponController owner, TracerProjectile tracerPrefab, Vector3 origin, Vector3 direction, int count, float spread, float speed, float lifetime)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 shotDirection = ApplySpread(direction, spread);
                TracerProjectile tracer = Instantiate(tracerPrefab, origin, Quaternion.LookRotation(shotDirection));
                tracer.Speed = speed;
                tracer.Lifespan = lifetime;
                tracer.Shoot(owner);
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