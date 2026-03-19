using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public class Target : MonoBehaviour
    {
        public UnityEvent<float, GameObject> OnDamaged;
        public void InflictDamage(float damage, GameObject damageSource) => OnDamaged?.Invoke(damage, damageSource);
    }
}