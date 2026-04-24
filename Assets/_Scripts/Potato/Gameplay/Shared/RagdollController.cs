using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    public class RagdollController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField][LayerIndex] private int neutralLayer;
        [SerializeField][LayerIndex] private int ragdollLayer;
        [SerializeField] private RagdollLimb[] limbs;

        private Target[] _hitboxes;

        void Start()
        {
            _hitboxes = GetComponentsInChildren<Target>();

            foreach(var limb in limbs)
                limb.onAttacked += OnAttacked;
        }

        public void SetRagdoll(bool enabled)
        {
            animator.enabled = !enabled;

            foreach (var limb in limbs)
            {
                limb.gameObject.layer = enabled ? ragdollLayer : neutralLayer;
                limb.SetRagdoll(enabled);
            }

            foreach(var hitbox in _hitboxes)
                hitbox.TeamId = enabled ? Target.Team.Neutral : Target.Team.Hostile;
        }

        void OnAttacked() => SetRagdoll(true);
    }
}