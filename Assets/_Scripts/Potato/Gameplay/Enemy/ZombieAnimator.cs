using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public class ZombieAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private RagdollLimb[] _limbs;

        public UnityAction OnLimbAttacked;

        void Start()
        {
            _limbs = GetComponentsInChildren<RagdollLimb>();

            foreach(var limb in _limbs)
                limb.onAttacked += OnAttacked;
        }

        public void EnableRagdoll(bool enabled)
        {
            animator.enabled = !enabled;

            foreach (var limb in _limbs)
                limb.SetRagdoll(enabled);
        }

        void OnAttacked()
        {
            EnableRagdoll(true);

            foreach(var limb in _limbs)
                limb.onAttacked -= OnAttacked;
            
            OnLimbAttacked?.Invoke();
        }
    }
}