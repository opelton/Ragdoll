using UnityEngine;

namespace Potato.Gameplay
{
    public class WeaponAnimator : MonoBehaviour
    {
        const string k_AnimAttackParameter = "Attack";

        [Header("Audio & Visual")]
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private bool unparentMuzzleFlash;
        [SerializeField] private AudioClip shootSfx;
        [SerializeField] private AudioClip changeWeaponSfx;

        AudioSource _shootAudioSource;
        Animator _weaponAnimator;

        void Awake()    // _shootAudioSource null ref if this is Start()
        {
            TryGetComponent(out _shootAudioSource);
            TryGetComponent(out _weaponAnimator);
        }

        public void StartReloadAnimation()
        {
            if(TryGetComponent<Animator>(out var anim))
                anim.SetTrigger("Reload");
        }

        public void OnShowWeapon()
        {
            if(_shootAudioSource != null)
            {
                // todo -- game systems lifecycle should be managed to guarantee things like audio sources are set up ahead of time
                if(_shootAudioSource.enabled == false)
                    _shootAudioSource.enabled = true;

                _shootAudioSource.PlayOneShot(changeWeaponSfx);
            }
        }

        public void OnWeaponFired(Transform muzzleTransform)
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
            if (shootSfx && _shootAudioSource != null)
                _shootAudioSource.PlayOneShot(shootSfx);

            // Trigger attack animation if there is any
            if (_weaponAnimator != null)
                _weaponAnimator.SetTrigger(k_AnimAttackParameter);
        }
    }
}