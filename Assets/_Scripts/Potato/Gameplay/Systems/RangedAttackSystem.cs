using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Potato.Gameplay
{
    public class HitInfo
    {
        public Vector3 Point;
        public Vector3 Normal;
        public GameObject StruckObject;
        public bool StruckEnemy;
        public bool StruckSurface;
    }

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

        public int DoBulletAttacks(WeaponController owner, float damage, float spread, int count, ref HitInfo[] hits)
        {
            int i = 0;
            while(i < count)
                hits[i++] = DoBulletAttack(owner, damage, spread);

            return i;
        }

        public int DoBulletAttacks(WeaponController owner, float damage, float xSpread, float ySpread, int count, ref HitInfo[] hits)
        {
            int i = 0;
            while(i < count)
                hits[i++] = DoBulletAttack(owner, damage, xSpread, ySpread);

            return i;
        }

        public HitInfo DoBulletAttack(WeaponController owner, float damage, float spread)
        {
            Vector3 shotDirection = ApplyCircleSpread(owner.Owner.AimCams.AimTransform, spread);
            return ProcessShot(owner, shotDirection, damage);
        }

        public HitInfo DoBulletAttack(WeaponController owner, float damage, float xSpread, float ySpread)
        {
            Vector3 shotDirection = ApplyBoxSpread(owner.Owner.AimCams.AimTransform, xSpread, ySpread);
            return ProcessShot(owner, shotDirection, damage);
        }

        HitInfo ProcessShot(WeaponController owner, Vector3 adjustedShotDirection, float damage)
        {
            HitInfo info = new()
            {
                StruckEnemy = false,
                StruckSurface = true
            };

            var hitCount = PreviewAttackRaycast(owner.Owner.AimCams.AimPos, adjustedShotDirection, ref _hitBuffer, true);

            if (hitCount >= kRaycastBufferSize * .9)
                Debug.Log($"HitBuffer size {hitCount} is approaching max {kRaycastBufferSize}");

            for (int j = 0; j < hitCount; ++j)
            {
                var hit = _hitBuffer[j];
                info.StruckObject = hit.collider.gameObject;

                // ignore self-hit
                if (hit.collider.gameObject == owner.gameObject)
                    continue;

                if (hit.collider.TryGetComponent(out Target component))
                {
                    info.StruckEnemy = true;
                    component.InflictDamage(new(damage, owner.gameObject, hit.point, adjustedShotDirection));
                }

                // return location of the hit
                info.Point = hit.point;
                info.Normal = hit.normal;
                return info;
            }

            // if no valid hits, return the search raycast
            info.Point = owner.Owner.AimCams.AimPos + maxRaycastRange * adjustedShotDirection;
            info.Normal = owner.Owner.AimCams.AimPos - info.Point;
            info.StruckSurface = false;
            return info;
        }

        public void DoProjectileAttack(WeaponController owner, ProjectileBase projectilePrefab, int count, float spread)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 shotDirection = ApplyCircleSpread(owner.Owner.AimCams.AimTransform, spread);
                ProjectileBase newProjectile = Instantiate(projectilePrefab, owner.Owner.AimCams.AimPos, Quaternion.LookRotation(shotDirection));
                newProjectile.Shoot(owner);
            }
        }

        public void DoExplosiveAttack(WeaponController owner, int count, float spread)
        {
            Debug.Log($"{count} Shots requested by {owner} from {owner.Owner.AimCams.AimPos} going {owner.Owner.AimCams.AimDir} with {spread} spread");
        }

        // single spread angle for circle-shaped spread pattern
        Vector3 ApplyCircleSpread(Transform origin, float spreadAngle)
        {
            if(spreadAngle == 0f)
                return origin.forward;
                
            float spreadAngleRatio = spreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(origin.forward, Random.insideUnitSphere,
                spreadAngleRatio);

            return spreadWorldDirection;
        }

        // separate x and y axes for box-shaped spread pattern
        Vector3 ApplyBoxSpread(Transform origin, float xAngle, float yAngle)
        {
            if(xAngle == 0f && yAngle == 0f)
                return origin.forward;

            return Quaternion.AngleAxis(Random.Range(-xAngle / 2, xAngle / 2), origin.up) *
                Quaternion.AngleAxis(Random.Range(-yAngle / 2, yAngle / 2), origin.right) *
                origin.forward;
        }
    }
}