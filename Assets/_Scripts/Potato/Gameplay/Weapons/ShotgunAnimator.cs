using UnityEngine;

namespace Potato.Gameplay
{
    public class ShotgunAnimator : WeaponAnimator
    {
        [Header("Armature Bones")]
        [SerializeField] private Transform forendBone;
        [SerializeField] private Transform triggerBone;

        [Header("Animation Params")]
        [SerializeField] private float forendPose_Front;
        [SerializeField] private float forendPose_Back;
        [SerializeField] private float triggerPose_Pulled;
        [SerializeField] private float triggerPose_Neutral;

        public override void StartReloadAnimation()
        {
            base.StartReloadAnimation();

        }



        public override void AnimateWeaponAttack(Transform muzzleTransform)
        {
            base.AnimateWeaponAttack(muzzleTransform);
        }
    }
}