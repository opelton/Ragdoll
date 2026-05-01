using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    public class RagdollController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField][LayerIndex] private int neutralLayer;
        [SerializeField][LayerIndex] private int ragdollLayer;

        private Target[] _hitboxes;
        private RagdollLimb[] _limbs;
        void Start()
        {
            _hitboxes = GetComponentsInChildren<Target>();
            _limbs = GetComponentsInChildren<RagdollLimb>();

            foreach(var limb in _limbs)
                limb.onAttacked += OnAttacked;
        }

        public void SetRagdoll(bool enabled)
        {
            animator.enabled = !enabled;

            foreach (var limb in _limbs)
            {
                limb.gameObject.layer = enabled ? ragdollLayer : neutralLayer;
                limb.SetRagdoll(enabled);
            }

            foreach(var hitbox in _hitboxes)
                hitbox.TeamId = enabled ? Target.Team.Neutral : Target.Team.Hostile;
        }

        void OnAttacked()
        {
            SetRagdoll(true);

            foreach(var limb in _limbs)
                limb.onAttacked -= OnAttacked;
        }
    }
}