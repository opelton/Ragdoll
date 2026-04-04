using UnityEngine;
using Potato.Game;

namespace Potato.Gameplay
{
    public class ShotgunController : WeaponController
    {
        enum WeaponState { Ready, Reloading }

        StateMachine<WeaponState> _fsm;

        protected override void Awake()
        {
            base.Awake();
            if (_fsm == null)
            {
                _fsm = new StateMachine<WeaponState>();
                _fsm.AddState(new(WeaponState.Ready));
                _fsm.AddState(new(WeaponState.Reloading,
                onEnter: () =>
                {
                    weaponAnimator.StartReloadAnimation();
                    IsReloading = true;
                },
                onUpdate: dt =>
                {
                    if (_fsm.TimeInState >= ammoReloadDelay)
                    {
                        _currentAmmo += shotsPerReload;
                        if (_currentAmmo >= maxAmmo)
                        {
                            _currentAmmo = maxAmmo;
                            _fsm.SetNextState(WeaponState.Ready);
                        }
                        else
                            _fsm.ResetState();
                    }
                },
                onExit: () => IsReloading = false));
            }

            _fsm.SetNextState(WeaponState.Ready);
        }

        protected override void Update()
        {
            _fsm.Update(Time.deltaTime);
        }

        public override bool HandleWeaponInputs(bool fire1Down, bool fire1Held, bool reloadDown)
        {
            if(reloadDown)
            {
                _fsm.SetNextState(WeaponState.Reloading);
                return false;                
            }

            switch (shootType)
            {
                case ShootType.Manual:
                    if (fire1Down)
                        return TryShoot();
                    return false;

                case ShootType.Automatic:
                    if (fire1Held)
                        return TryShoot();
                    return false;

                default:
                    return false;
            }
        }

        bool TryShoot()
        {
            if(IsReloading)
                _fsm.SetNextState(WeaponState.Ready);

            if(_currentAmmo <= 0)
                _fsm.SetNextState(WeaponState.Reloading);

            else if (_lastShotTime + shotCooldown < Time.time)
            {
                HandleShoot();
                _currentAmmo -= 1;

                return true;
            }

            return false;
        }

        void HandleShoot()
        {
            rats.DoProjectileAttack(this, projectilePrefab, weaponMuzzle.position, weaponMuzzle.forward, bulletsPerShot, bulletSpreadAngle);
            weaponAnimator.OnWeaponFired(weaponMuzzle);
            _lastShotTime = Time.time;
        }
    }
}