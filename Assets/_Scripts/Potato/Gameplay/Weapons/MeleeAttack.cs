using UnityEngine;

namespace Potato.Gameplay
{
    public class MeleeAttack : MonoBehaviour
    {
        [SerializeField] private int damage = 100;
        void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<Target>(out var target))
            {
                var attackInfo = new AttackInfo(damage, gameObject, transform.position, target.transform.position - transform.position);
                target.InflictDamage(attackInfo);
            }
        }
    }
}