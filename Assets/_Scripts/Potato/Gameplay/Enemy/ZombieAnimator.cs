using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public class ZombieAnimator : MonoBehaviour
    {
        private static readonly int ZombieSpeedHash = Animator.StringToHash("Speed");
        
        [SerializeField] private Animator animator;
        private RagdollLimb[] _limbs;

        public UnityAction OnLimbAttacked;

        void Start()
        {
            _limbs = GetComponentsInChildren<RagdollLimb>();

            foreach(var limb in _limbs)
                limb.onAttacked += OnAttacked;
        }

        public void SetZombieSpeed(float speed)
        {
            animator.SetFloat(ZombieSpeedHash, speed);
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