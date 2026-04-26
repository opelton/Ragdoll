using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class RagdollLimb : MonoBehaviour
    {
        [SerializeField] private bool dismembering = true;
        [SerializeField] private float knockbackForce = 1500f;
        public UnityAction onAttacked;
        private Rigidbody _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void SetRagdoll(bool enabled)
        {
            _rb.isKinematic = !enabled;
        }

        public void AttackLimb(float damage, Vector3 point, Vector3 direction, GameObject attacker)
        {
            onAttacked?.Invoke();
            var force = damage * knockbackForce;
            _rb.AddForceAtPosition(direction * force, point);

            if (dismembering)
            {
                if (transform.parent != null)
                    transform.parent.DetachChildren();

                if (TryGetComponent(out CharacterJoint joint))
                {
                    joint.connectedBody = null;
                    Destroy(joint);
                }

                dismembering = false;
            }
        }
    }
}