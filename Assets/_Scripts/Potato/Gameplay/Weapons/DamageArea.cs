using System.Collections.Generic;
using UnityEngine;

namespace Potato.Gameplay
{
    public class DamageArea : MonoBehaviour
    {
        [Tooltip("Area of damage when the projectile hits something")]
        public float AreaOfEffectDistance = 5f;

        [Tooltip("Damage multiplier over distance for area of effect")]
        public AnimationCurve DamageRatioOverDistance;

        [Header("Debug")]
        [Tooltip("Color of the area of effect radius")]
        public Color AreaOfEffectColor = Color.red * 0.5f;

        public void InflictDamageInArea(float damage, Vector3 center, LayerMask layers,
            QueryTriggerInteraction interaction, GameObject owner)
        {
            var uniqueTargets = new HashSet<Target>();

            // Create a collection of unique health components that would be damaged in the area of effect (in order to avoid damaging a same entity multiple times)
            Collider[] affectedColliders = Physics.OverlapSphere(center, AreaOfEffectDistance, layers, interaction);
            foreach (var collider in affectedColliders)
            {
                collider.TryGetComponent(out Target target);
                if (target != null)
                    uniqueTargets.Add(target);
            }

            // Apply damages with distance falloff
            foreach (Target uniqueTarget in uniqueTargets)
            {
                float distance = Vector3.Distance(uniqueTarget.transform.position, transform.position);
                uniqueTarget.InflictDamage(
                    damage * DamageRatioOverDistance.Evaluate(distance / AreaOfEffectDistance), owner);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = AreaOfEffectColor;
            Gizmos.DrawSphere(transform.position, AreaOfEffectDistance);
        }
    }
}