using UnityEngine;

namespace Potato.Gameplay
{
    public class MeleeAttack : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<Target>(out var target))
            {
                var attackInfo = new AttackInfo(50, gameObject, transform.position, target.transform.position - transform.position);
                target.InflictDamage(attackInfo);
            }
        }
    }
}