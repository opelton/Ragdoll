using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Potato.Gameplay
{
    public class BulletDecal : MonoBehaviour
    {
        [SerializeField] private float coolingTime = 2f;
        [SerializeField] private float coolingTimeRandomness = .25f;
        [SerializeField] private AnimationCurve coolingCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        [SerializeField] private DecalProjector decalProjector;
        private float _age = 0f;
        private float _coolingTime;

        void Start()
        {
            _coolingTime = Random.Range(coolingTime - coolingTimeRandomness, coolingTime + coolingTimeRandomness);
            decalProjector.material = Instantiate(decalProjector.material);
            decalProjector.material.SetFloat("_Heat", coolingCurve.Evaluate(_age));
        }

        void Update()
        {
            _age += Time.deltaTime;
            var cooling = Mathf.Clamp01(_age / _coolingTime);
            decalProjector.material.SetFloat("_Heat", coolingCurve.Evaluate(cooling));

            if(cooling >= 1f)
                enabled = false;
        }
    }
}