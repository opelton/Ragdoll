using UnityEngine;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(TrailRenderer))]
    public class TracerProjectile : ProjectileBase
    {

        public float Lifespan = .01f;
        public float Speed = 300f;
        float _age = 0f;
        bool _alive = true;
        TrailRenderer _tail;

        void Awake()
        {
            _tail = GetComponent<TrailRenderer>();
        }

        void Update()
        {
            if (_age < Lifespan)
            {
                transform.position += Speed * Time.deltaTime * transform.forward;
                _age += Time.deltaTime;
            }
            else if(_alive)
            {
                _alive = false;
                _tail.emitting = false;
                Destroy(gameObject, _tail.time);
            }
        }
    }
}