using System.Collections;
using UnityEngine;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(LineRenderer))]
    public class TracerProjectile : MonoBehaviour
    {
        private Vector3 _impact;
        private Vector3 _origin;
        LineRenderer _line;

        public void Fire(Vector3 impact, Vector3 origin, float speed)
        {
            _impact = impact;
            _origin = origin;

            _line = GetComponent<LineRenderer>();
            _line.SetPosition(0, _impact);
            _line.SetPosition(1, _origin);

            var duration = (_impact - _origin).magnitude / speed;
            StartCoroutine(AnimateTracer(duration));
        }

        IEnumerator AnimateTracer(float duration)
        {
            float age = 0f;
            while(age < duration)
            {
                float t = age / duration;
                _line.SetPosition(1, Vector3.Lerp(_origin, _impact, t));

                // update age last so there's always at least 1 frame
                age += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}