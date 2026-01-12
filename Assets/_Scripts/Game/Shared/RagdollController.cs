using UnityEngine;
using Potato.FPS.Editor;

namespace Potato.FPS.Game
{
    public class RagdollController : MonoBehaviour
    {
        [SerializeField] Animator m_Animator;
        [SerializeField] Collider[] m_TerrainColliders;
        [SerializeField] Collider[] m_Hitboxes;
        [SerializeField] Collider[] m_LimbColliders;
        [SerializeField][LayerIndex] int m_RagdollLayer;

        Rigidbody[] m_Rigidbodies;

        void Awake()
        {
            // this assumes all rigidbodies are for the ragdoll, and aren't actually being used for movement
            m_Rigidbodies = GetComponentsInChildren<Rigidbody>();

            // todo injection
            Health health = GetComponentInChildren<Health>();
            health.OnDie += HandleDeath;
        }

        void HandleDeath()
        {
            // todo spice this up
            SetRagdoll(true);
        }

        public void SetRagdoll(bool enabled)
        {
            m_Animator.enabled = !enabled;

            foreach (var rb in m_Rigidbodies)
                rb.isKinematic = !enabled;

            // hitboxes and limb boxes shouldn't be active at the same time
            foreach (var hitbox in m_Hitboxes)
                hitbox.enabled = !enabled;

            foreach (var hitbox in m_LimbColliders)
                hitbox.enabled = enabled;

            if(enabled)
            {
                foreach (var hitbox in m_LimbColliders)
                    hitbox.gameObject.layer = m_RagdollLayer;
            }
        }
    }
}