using UnityEngine;

namespace Potato.Gameplay
{
    // owner of hitscan, projectiles, and explosions
    [CreateAssetMenu(menuName = "ScriptableObjects/Gameplay/Systems/RangedAttack")]
    public class RangedAttackSystem : ScriptableObject
    {
        [SerializeField] private float maxRaycastRange = 1000f;
        [Tooltip("Layers that can block raycasts")]
        [SerializeField] private LayerMask targetLayers;
        // [Tooltip("Layer to assign projectiles")]
        // [SerializeField][LayerIndex] int hitboxLayer;

        public bool IsTargetingEnemy(GameObject owner, Vector3 origin, Vector3 direction)
        {
            var hits = Physics.RaycastAll(origin, direction, maxRaycastRange, targetLayers, QueryTriggerInteraction.Ignore);
            // Debug.Log($"crosshairHitCount: {hits.Length} layerIndex: {hitboxLayers}");

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == owner.gameObject)
                    continue;

                if (hit.collider.GetComponentInParent<Target>() != null)
                    return true;
            }

            return false;
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