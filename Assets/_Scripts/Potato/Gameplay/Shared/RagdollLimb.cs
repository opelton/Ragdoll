using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class RagdollLimb : MonoBehaviour
    {
        public UnityAction onAttacked;

        private Rigidbody _rb;
        private Collider _collider;

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void SetRagdoll(bool enabled)
        {
            _rb.isKinematic = !enabled;
            _collider.enabled = enabled;
        }

        public void AttackLimb(float damage, Vector3 point, Vector3 direction, GameObject attacker)
        {
            onAttacked?.Invoke();
            var force = 1000f;
            _rb.AddForceAtPosition(direction * force, point);
        }
    }
}