using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Potato.Game
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] Image fullscreenFade;
        [SerializeField] Color fullscreenFadeColor = Color.black;
        [SerializeField] RawImage logoTarget;
        [SerializeField] Texture[] splashLogos;
        [SerializeField] float logoDuration = 3f;
        [SerializeField] float fadeTime = 1f;
        [SerializeField] float downTime = 1f;

        private Coroutine _splashCoroutine;

        void Start()
        {
            _splashCoroutine = StartCoroutine(PlaySplashSequence());
        }

        void Update()
        {
            // todo use input system
            if(Input.anyKeyDown)
                CancelSplashSequence();
        }

        IEnumerator PlaySplashSequence()
        {
            fullscreenFade.color = fullscreenFadeColor;

            yield return new WaitForSecondsRealtime(downTime);

            foreach (var logo in splashLogos)
            {
                logoTarget.texture = logo;

                // lerp fullscreen fade image
                yield return LerpFullscreenFade(1f, 0f, fadeTime);

                // Hold logo
                yield return new WaitForSecondsRealtime(logoDuration);

                // lerp fullscreen fade image
                yield return LerpFullscreenFade(0f, 1f, fadeTime);

                yield return new WaitForSecondsRealtime(downTime);
            }
        }

        IEnumerator LerpFullscreenFade(float from, float to, float duration)
        {
            float elapsed = 0f;
            Color c = fullscreenFade.color;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                c.a = Mathf.Lerp(from, to, t);
                fullscreenFade.color = c;
                yield return null;
            }

            c.a = to;
            fullscreenFade.color = c;
        }

        void CancelSplashSequence()
        {
            StopCoroutine(_splashCoroutine);
            logoTarget.texture = null;
            fullscreenFade.color = Color.clear;
        }
    }
}