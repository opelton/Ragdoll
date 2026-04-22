using UnityEngine;

namespace Potato.Gameplay
{
    // owner of hitscan, projectiles, and explosions
    [CreateAssetMenu(menuName = "ScriptableObjects/Systems/RangedAttack")]
    public class RangedAttackSystem : ScriptableObject
    {
        const int kRaycastBufferSize = 32;

        [SerializeField] private float maxRaycastRange = 1000f; // tf2 uses ~156
        [Tooltip("Layers that can block raycasts")]
        [SerializeField] private LayerMask targetLayers;

        public float MaxAttackRange => maxRaycastRange;
        private RaycastHit[] _hitBuffer = new RaycastHit[kRaycastBufferSize];

        public int PreviewAttackRaycast(Vector3 origin, Vector3 direction, ref RaycastHit[] hits)
        {
            return Physics.RaycastNonAlloc(origin, direction, hits, maxRaycastRange, targetLayers, QueryTriggerInteraction.Ignore);
        }

        public void DoHitscanAttack(WeaponController owner, Vector3 origin, Vector3 direction, float damage, int count, float spread)
        {
            for (int i = 0; i < count; ++i)
            {
                Vector3 shotDirection = ApplySpread(direction, spread);
                var hitCount = PreviewAttackRaycast(origin, shotDirection, ref _hitBuffer);

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
                        component.InflictDamage(damage, owner.gameObject);
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
            float spreadAngleRatio = spreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(vec, Random.insideUnitSphere,
                spreadAngleRatio);

            return spreadWorldDirection;
        }
    }
}