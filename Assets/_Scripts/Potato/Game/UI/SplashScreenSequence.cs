using System.Collections;
using Potato.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Potato.Game.UI
{
    public class SplashScreenSequence : MonoBehaviour
    {
        [Header("Out Events")]
        [SerializeField] GameEvent NotifySplashSequenceFinished;

        [Header("Splash Sequence")]
        [SerializeField] Image fullscreenFade;
        [SerializeField] Color fullscreenFadeColor = Color.black;
        [SerializeField] RawImage logoTarget;
        [SerializeField] Texture[] splashLogos;
        [SerializeField] float logoDuration = 3f;
        [SerializeField] float fadeTime = 1f;
        [SerializeField] float downTime = 1f;

        private Coroutine _splashCoroutine;

        void Start() => _splashCoroutine = StartCoroutine(PlaySplashSequence());

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

            _splashCoroutine = null;
            NotifySplashSequenceFinished.Invoke(this);
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

        public void CancelSplashSequence()
        {
            if (_splashCoroutine != null)
            {
                StopCoroutine(_splashCoroutine);
                logoTarget.texture = null;
                fullscreenFade.color = Color.clear;
                
                _splashCoroutine = null;
                NotifySplashSequenceFinished.Invoke(this);
            }
        }
    }
}