using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Potato.Gameplay.UI
{
    public class DeathSplashUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject deathSplashRoot;
        [SerializeField] Image splashImage;
        [SerializeField] float fadeTime = 2f;
        [SerializeField] float fadeDelay = 2f;

        private Coroutine _splashCoroutine;

        public void StartDeathSequence() => StartDeathSplash();

        void OnEnable() => deathSplashRoot.SetActive(false);

        void OnDisable()
        {
            if (_splashCoroutine != null)
            {
                StopCoroutine(_splashCoroutine);
                _splashCoroutine = null;
            }
            deathSplashRoot.SetActive(false);
        }

        void StartDeathSplash()
        {
            deathSplashRoot.SetActive(true);
            Color targetColor = splashImage.color;
            targetColor.a = 0f;
            splashImage.color = targetColor;
            _splashCoroutine = StartCoroutine(PlaySplashSequence());
        }

        IEnumerator PlaySplashSequence()
        {
            Color targetColor = splashImage.color;
            targetColor.a = 0f;

            splashImage.color = targetColor;

            if (fadeDelay > 0f)
                yield return new WaitForSecondsRealtime(fadeDelay);

            yield return LerpLogoAlpha(0f, 1f, fadeTime);

            _splashCoroutine = null;
        }

        IEnumerator LerpLogoAlpha(float from, float to, float duration)
        {
            float elapsed = 0f;
            Color c = splashImage.color;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                c.a = Mathf.Lerp(from, to, t);
                splashImage.color = c;
                yield return null;
            }

            c.a = to;
            splashImage.color = c;
        }
    }
}