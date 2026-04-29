using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public class Health : MonoBehaviour
    {
        [SerializeField] protected float maxHp;
        public UnityEvent<int> OnHealthLostEvent;
        public UnityEvent OnKilledEvent;

        public float CurrentHp { get; protected set; }
        public bool IsAlive { get; protected set; } = true;

        protected virtual void Start()
        {
            CurrentHp = maxHp;
        }

        // matches target onDamaged event signature
        public virtual void OnTargetDamaged(float damage, Vector3 hitPoint, Vector3 hitDirection, GameObject damageSource)
        {
            InflictDamage(damage);
        }

        public virtual void InflictDamage(float damage)
        {
            CurrentHp -= damage;
            if (CurrentHp <= 0)
                HandleDeath();
        }

        public virtual void Heal(float healing)
        {
            CurrentHp = Mathf.Min(CurrentHp + healing, maxHp);
        }

        void HandleDeath()
        {
            IsAlive = false;
            OnKilledEvent?.Invoke();
        }
    }
}