
using UnityEngine;
using Potato.Core;

namespace Potato.Gameplay
{
    public class PlayerHealth : Health
    {
        [SerializeField] private IntReference playerHealth;

        protected override void Start()
        {
            base.Start();
            UpdateData();
        }

        public override void InflictDamage(int damage)
        {
            base.InflictDamage(damage);
            UpdateData();
        }

        public override void Heal(int healing)
        {
            base.Heal(healing);
            UpdateData();
        }

        void UpdateData() => playerHealth.Value = CurrentHp;
    }
}