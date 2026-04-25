using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class RagdollLimb : MonoBehaviour
    {
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
            var force = 1500;
            _rb.AddForceAtPosition(direction * force, point);
        }
    }
}