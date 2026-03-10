using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Potato.Core;

namespace Potato.Game.UI
{
    public class LoadScreenController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup loadingScreen;
        [SerializeField] private Image loadingBar;

        [Header("Shared Data")]
        [SerializeField] private FloatReference loadProgressRef;
        [SerializeField] private BoolReference showLoadScreenRef;

        [Header("Tuning")]
        [SerializeField] private float alphaInterpDuration = .25f;
        [SerializeField] private float lingerDuration = .5f;

        private Coroutine _loadScreenCoroutine = null;

        void Awake() => loadingScreen.alpha = 0f;
        void Update() => loadingBar.fillAmount = loadProgressRef.Value;

        public void OnSceneLoadStarted()
        {
            if(_loadScreenCoroutine != null)
                StopCoroutine(_loadScreenCoroutine);
            
            _loadScreenCoroutine = StartCoroutine(LoadingScreenFade(0f, 1f, alphaInterpDuration));
        }

        public void OnSceneLoadFinished() => StartCoroutine(LoadFinishedSequence());

        private IEnumerator LoadFinishedSequence()
        {
            // if the load finished faster than the screen-fade, wait for the screen-fade
            if(_loadScreenCoroutine != null)
                yield return _loadScreenCoroutine;

            loadingScreen.alpha = 1f;
            yield return new WaitForSecondsRealtime(lingerDuration);
            yield return LoadingScreenFade(1f, 0f, alphaInterpDuration);
            showLoadScreenRef.Value = false;
        }

        private IEnumerator LoadingScreenFade(float start, float end, float duration)
        {
            float t = 0f;
            loadingScreen.alpha = 0f;

            while(t < duration)
            {
                t += Time.deltaTime;
                float a = Mathf.Clamp01(t / duration);
                loadingScreen.alpha = Mathf.Lerp(start, end, a);
                yield return null;
            }
        }
    }
}