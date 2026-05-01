using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public class AttackInfo
    {
        public float Damage;
        public GameObject DamageSource;
        public Vector3 HitPoint;
        public Vector3 HitDirection;

        public AttackInfo(float damage, GameObject damageSource, Vector3 hitPoint, Vector3 hitDirection)
        {
            Damage = damage;
            DamageSource = damageSource;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
        }
    }

    public class Target : MonoBehaviour
    {
        public enum Team { Neutral, Hostile }
        public Team TeamId = Team.Hostile;
        public UnityEvent<AttackInfo> OnDamaged;
        public void InflictDamage(AttackInfo data)
            => OnDamaged?.Invoke(data);
    }
}