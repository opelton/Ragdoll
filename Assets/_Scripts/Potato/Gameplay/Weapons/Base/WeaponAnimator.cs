using Potato.Game;
using UnityEngine;

namespace Potato.Gameplay
{
    public class WeaponAnimator : MonoBehaviour
    {
        [SerializeField] protected AudioSystem audioSystem;

        [Header("Audio & Visual")]
        [SerializeField] protected GameObject muzzleFlashPrefab;
        [SerializeField] protected bool unparentMuzzleFlash;
        [SerializeField] protected AudioClip shootSfx;
        [SerializeField] protected AudioClip changeWeaponSfx;

        public virtual void StartReloadAnimation()
        {

        }

        public virtual void AnimateShowWeapon()
        {
            audioSystem.PlayFirstPersonAudio(changeWeaponSfx);
        }

        public virtual void AnimateWeaponAttack(Transform muzzleTransform)
        {
            if (muzzleFlashPrefab != null)
            {
                GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, muzzleTransform.position,
                    muzzleTransform.rotation, muzzleTransform);

                if (unparentMuzzleFlash)
                    muzzleFlashInstance.transform.SetParent(null);

                // todo -- fx system
                Destroy(muzzleFlashInstance, 2f);
            }

            // play shoot SFX
            if (shootSfx)
                audioSystem.PlayFirstPersonAudio(shootSfx);
        }
    }
}