using UnityEngine;

namespace Potato.Gameplay
{
    // owner of hitscan, projectiles, and explosions
    [CreateAssetMenu(menuName = "ScriptableObjects/Systems/RangedAttack")]
    public class RangedAttackSystem : ScriptableObject
    {
        [SerializeField] private float maxRaycastRange = 1000f; // tf2 uses ~156
        [Tooltip("Layers that can block raycasts")]
        [SerializeField] private LayerMask targetLayers;
        // [Tooltip("Layer to assign projectiles")]
        // [SerializeField][LayerIndex] int hitboxLayer;

        public float MaxAttackRange => maxRaycastRange;

        // raycast on layer for enemies + surfaces
        // if no hits, targeting nothing
        // else if enemy component, targeting enemy
        // else, targeting wall
        public int PreviewAttackRaycast(Vector3 origin, Vector3 direction, ref RaycastHit[] hits)
        {
            return Physics.RaycastNonAlloc(origin, direction, hits, maxRaycastRange, targetLayers, QueryTriggerInteraction.Ignore);
        }

        public void DoHitscanAttack(WeaponController owner, Vector3 origin, Vector3 direction, int count, float spread)
        {
            Debug.Log($"{count} Shots requested by {owner} from {origin} going {direction} with {spread} spread");
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
            float spreadAngleRatio = spreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(vec, Random.insideUnitSphere,
                spreadAngleRatio);

            return spreadWorldDirection;
        }
    }
}