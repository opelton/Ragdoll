using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public class Health : MonoBehaviour
    {
        [SerializeField] protected float maxHp;
        public UnityEvent<float> OnHealthLostEvent;
        public UnityEvent<float> OnHealthGainedEvent;
        public UnityEvent<AttackInfo> OnAttackedEvent;
        public UnityEvent OnKilledEvent;

        public float CurrentHp { get; protected set; }
        public bool IsAlive => CurrentHp > 0;

        protected virtual void Start()
        {
            CurrentHp = maxHp;
        }

        public virtual void OnTargetDamaged(AttackInfo data)
        {
            InflictDamage(data.Damage);
            OnAttackedEvent?.Invoke(data);
        }

        public virtual void InflictDamage(float damage)
        {
            CurrentHp -= damage;
            OnHealthLostEvent?.Invoke(damage);
            if (CurrentHp <= 0)
                HandleDeath();
        }

        public virtual void Heal(float healing)
        {
            var oldHp = CurrentHp;
            CurrentHp = Mathf.Min(CurrentHp + healing, maxHp);
            var healed = CurrentHp - oldHp;
            if(healed > 0f)
                OnHealthGainedEvent?.Invoke(healed);
        }

        protected virtual void HandleDeath()
        {
            OnKilledEvent?.Invoke();
        }
    }
}