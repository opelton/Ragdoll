using UnityEngine;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Systems/Audio")]
    public class AudioSystem : ScriptableObject
    {
        [SerializeField] private AudioRequestEvent firstPersonAudio;

        public void PlayFirstPersonAudio(AudioClip sfx) => firstPersonAudio.Invoke(sfx, this);        
    }
}