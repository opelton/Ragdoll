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
        [SerializeField] RawImage logoTarget;
        [SerializeField] Texture[] splashLogos;
        [SerializeField] float logoDuration = 3f;
        [SerializeField] float fadeTime = 1f;
        [SerializeField] float downTime = 1f;

        private Coroutine _splashCoroutine;
        private int _logoIndex;

        void OnEnable() => StartSplashSequence(0, fadeTime);

        // enables hacky single-logo skip
        void StartSplashSequence(int startIndex, float wakeupTime)
        {
            _logoIndex = startIndex;
            _splashCoroutine = StartCoroutine(PlaySplashSequence(wakeupTime));
        }

        IEnumerator PlaySplashSequence(float wakeupTime)
        {
            Color targetColor = logoTarget.color;
            targetColor.a = 0f;

            logoTarget.color = targetColor;

            if(wakeupTime > 0f)
                yield return new WaitForSecondsRealtime(wakeupTime);

            while (_logoIndex < splashLogos.Length)
            {
                logoTarget.texture = splashLogos[_logoIndex];

                // Show logo
                yield return LerpLogoAlpha(0f, 1f, fadeTime);

                // Hold logo
                yield return new WaitForSecondsRealtime(logoDuration);

                // Hide logo
                yield return LerpLogoAlpha(1f, 0f, fadeTime);

                // Breathe
                yield return new WaitForSecondsRealtime(downTime);
                ++_logoIndex;
            }

            _splashCoroutine = null;
            NotifySplashSequenceFinished.Invoke(this);
        }

        IEnumerator LerpLogoAlpha(float from, float to, float duration)
        {
            float elapsed = 0f;
            Color c = logoTarget.color;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                c.a = Mathf.Lerp(from, to, t);
                logoTarget.color = c;
                yield return null;
            }

            c.a = to;
            logoTarget.color = c;
        }

        void StopSplashSequence()
        {
            if (_splashCoroutine != null)
            {
                StopCoroutine(_splashCoroutine);
                logoTarget.texture = null;

                Color c = logoTarget.color;
                c.a = 0f;
                logoTarget.color = c;

                _splashCoroutine = null;
                _logoIndex = 0;
            }
        }

        public void SkipSplashSequence()
        {
            StopSplashSequence();
            NotifySplashSequenceFinished.Invoke(this);            
        }

        public void SkipSingleLogo()
        {
            int savedNextIndex = _logoIndex + 1;
            StopSplashSequence();
            StartSplashSequence(savedNextIndex, 0f);
        }
    }
}