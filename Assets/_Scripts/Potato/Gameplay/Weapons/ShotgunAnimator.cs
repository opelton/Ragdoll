using Potato.Game;
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

        [Header("SFX")]
        [SerializeField] private AudioClip sfxTriggerDryfire;
        [SerializeField] private AudioClip sfxTriggerPull;
        [SerializeField] private AudioClip sfxTriggerRelease;
        [SerializeField] private AudioClip sfxExtract;
        [SerializeField] private AudioClip sfxChamber;
        [SerializeField] private AudioClip sfxReload;

        // 0f = default
        // 1f = back
        public void AnimateForendPosition(float lerp)
        {
            forendBone.transform.localPosition = Vector3.Lerp(
                new Vector3(0f, 0f, forendPose_Front),
                 new Vector3(0f, 0f, forendPose_Back),
                 lerp);
        }

        public void Sfx_Extract() => audioSystem.PlayFirstPersonAudio(sfxExtract);
        public void Sfx_Chamber() => audioSystem.PlayFirstPersonAudio(sfxChamber);
        public void Sfx_Reload() => audioSystem.PlayFirstPersonAudio(sfxReload);

        public void AnimateTrigger_Pulled(bool dryfire = false)
        {
            audioSystem.PlayFirstPersonAudio(dryfire ? sfxTriggerDryfire : sfxTriggerPull);
            triggerBone.localRotation = Quaternion.Euler(new Vector3(triggerPose_Pulled, 0f, 0f));
        }

        public void AnimateTrigger_Release()
        {
            audioSystem.PlayFirstPersonAudio(sfxTriggerRelease);
            triggerBone.localRotation = Quaternion.Euler(new Vector3(triggerPose_Neutral, 0f, 0f));
        }
    }
}