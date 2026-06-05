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

        float _forendY = 0;
        void Start()
        {
            _forendY = forendBone.transform.localPosition.y;
        }

        // 0f = default
        // 1f = back
        public void AnimateForendPosition(float lerp)
        {
            forendBone.transform.localPosition = Vector3.Lerp(
                new Vector3(0f, _forendY, forendPose_Front),
                 new Vector3(0f, _forendY, forendPose_Back),
                 lerp);
        }

        public void Sfx_Extract() => audioSystem.PlayFirstPersonWeaponAudio(sfxExtract);
        public void Sfx_Chamber() => audioSystem.PlayFirstPersonWeaponAudio(sfxChamber);
        public void Sfx_Reload() => audioSystem.PlayFirstPersonWeaponAudio(sfxReload);

        public void AnimateTrigger_Pulled(bool dryfire = false)
        {
            audioSystem.PlayFirstPersonWeaponAudio(dryfire ? sfxTriggerDryfire : sfxTriggerPull);
            triggerBone.localRotation = Quaternion.Euler(new Vector3(triggerPose_Pulled, 0f, 0f));
        }

        public void AnimateTrigger_Release()
        {
            audioSystem.PlayFirstPersonWeaponAudio(sfxTriggerRelease);
            triggerBone.localRotation = Quaternion.Euler(new Vector3(triggerPose_Neutral, 0f, 0f));
        }
    }
}