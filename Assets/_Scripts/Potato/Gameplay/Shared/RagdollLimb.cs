using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class RagdollLimb : MonoBehaviour
    {
        static readonly float kHitForce = 1500f;
        [Header("Basic response")]
        [SerializeField] [Range(0f, 2f)] private float hitForceModifier = 1f;
        [SerializeField] [Range(0f, 1f)] private float severVelocityModifier = .5f;
        [SerializeField] private bool dismembering = true;

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

        IEnumerator BeginSevering()
        {
            // wait one physics update so forces apply before detaching
            yield return new WaitForFixedUpdate();

            if (transform.parent != null)
                transform.parent.DetachChildren();

            if (TryGetComponent(out CharacterJoint joint))
                Destroy(joint);

            _rb.velocity *= severVelocityModifier;
            dismembering = false;
        }

        public void AttackLimb(AttackInfo data)
        {
            onAttacked?.Invoke();
            var force = data.Damage * kHitForce * hitForceModifier;
            _rb.AddForceAtPosition(data.HitDirection * force, data.HitPoint);

            if (dismembering)
                StartCoroutine(BeginSevering());
        }
    }
}