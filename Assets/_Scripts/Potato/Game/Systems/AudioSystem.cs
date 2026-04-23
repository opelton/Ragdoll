using UnityEngine;

namespace Potato.Game
{
    // todo -- spatial audio
    [CreateAssetMenu(menuName = "ScriptableObjects/Systems/Audio")]
    public class AudioSystem : ScriptableObject
    {
        [SerializeField] private AudioRequestEvent firstPersonAudio;
        [SerializeField] private AudioRequestEvent firstPersonWeaponAudio;

        public void PlayFirstPersonAudio(AudioClip sfx) => firstPersonAudio.Invoke(sfx, this);
        public void PlayFirstPersonWeaponAudio(AudioClip sfx) => firstPersonWeaponAudio.Invoke(sfx, this);
    }
}