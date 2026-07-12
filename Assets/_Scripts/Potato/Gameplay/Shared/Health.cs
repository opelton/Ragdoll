using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public class Health : MonoBehaviour
    {
        [SerializeField] protected int maxHp;
        public UnityEvent<int> OnHealthLostEvent;
        public UnityEvent<int> OnHealthGainedEvent;
        public UnityEvent<AttackInfo> OnAttackedEvent;
        public UnityEvent OnKilledEvent;

        public int CurrentHp { get; protected set; }
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

        public virtual void InflictDamage(int damage)
        {
            CurrentHp -= damage;
            OnHealthLostEvent?.Invoke(damage);
            if (CurrentHp <= 0)
                HandleDeath();
        }

        public virtual void Heal(int healing)
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