using UnityEngine;

namespace Potato.Gameplay
{
    public class RagdollController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Collider[] hitboxes;
        [SerializeField] private Collider[] limbs;

        Rigidbody[] _bodies;

        void Start()
        {
            _bodies = GetComponentsInChildren<Rigidbody>();
        }

        public void SetRagdoll(bool enabled)
        {
            animator.enabled = !enabled;

            foreach (var rb in _bodies)
                rb.isKinematic = !enabled;

            // hitboxes and limb boxes are active at opposite times (for now)
            foreach (var hitbox in hitboxes)
                hitbox.enabled = !enabled;

            foreach (var limb in limbs)
                limb.enabled = enabled;
        }
    }
}