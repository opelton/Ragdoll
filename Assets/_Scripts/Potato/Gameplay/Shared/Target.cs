using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public class Target : MonoBehaviour
    {
        public enum Team { Neutral, Hostile }
        public Team TeamId = Team.Hostile;
        public UnityEvent<float, Vector3, Vector3, GameObject> OnDamaged;
        public void InflictDamage(float damage, Vector3 hitPoint, Vector3 hitDirection, GameObject damageSource)
            => OnDamaged?.Invoke(damage, hitPoint, hitDirection, damageSource);
    }
}