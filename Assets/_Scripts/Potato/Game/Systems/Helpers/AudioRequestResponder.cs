using UnityEngine;

namespace Potato.Game
{
    public class AudioRequestResponder : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        public void PlayClip(AudioClip sfx) => audioSource.PlayOneShot(sfx);
    }
}