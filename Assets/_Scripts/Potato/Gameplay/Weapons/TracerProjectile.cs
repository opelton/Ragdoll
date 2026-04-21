using UnityEngine;

namespace Potato.Gameplay
{
    public class TracerProjectile : ProjectileBase
    {
        public float Lifespan = .01f;
        public float Speed = 300f;
        protected override void HandleOnShoot()
        {
            Destroy(gameObject, Lifespan);
        }

        void Update()
        {
            transform.position += Speed * Time.deltaTime * InitialDirection;
        }
    }
}