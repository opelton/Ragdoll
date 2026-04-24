using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public class Target : MonoBehaviour
    {
        public enum Team { Neutral, Hostile }
        public Team TeamId = Team.Hostile;
        public UnityEvent<float, GameObject> OnDamaged;
        public void InflictDamage(float damage, GameObject damageSource) => OnDamaged?.Invoke(damage, damageSource);
    }
}