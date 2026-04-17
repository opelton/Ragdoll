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

        public override void AnimateWeaponAttack(Transform muzzleTransform)
        {
            base.AnimateWeaponAttack(muzzleTransform);
            AnimateTrigger_Pulled();
        }

        // 0f = default
        // 1f = back
        public void AnimateForendPosition(float lerp)
        {
            forendBone.transform.localPosition = Vector3.Lerp(new Vector3(0f, 0f, forendPose_Front), new Vector3(0f, 0f, forendPose_Back), lerp);
        }

        public void AnimateTrigger_Pulled() => triggerBone.localRotation = Quaternion.Euler(new Vector3(triggerPose_Pulled, 0f, 0f));
        public void AnimateTrigger_Release() => triggerBone.localRotation = Quaternion.Euler(new Vector3(triggerPose_Neutral, 0f, 0f));
    }
}