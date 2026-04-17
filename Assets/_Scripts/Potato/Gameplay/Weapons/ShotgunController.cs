using System;
using UnityEngine;
using Potato.Game;

namespace Potato.Gameplay
{
    public class ShotgunController : WeaponController
    {
        enum WeaponState { Neutral, Firing, Reloading, Extracting, Chambering }
        enum ChamberState { Empty, Ready, Fired }
        ShotgunAnimator Animator => (ShotgunAnimator)weaponAnimator;

        // control vars
        ChamberState _chamberState = ChamberState.Ready;
        bool _receiverLoaded;   // a shell is ready to be chambered

        // shotgun state control
        StateMachine<WeaponState> _fsm;

        protected override void Awake()
        {
            base.Awake();
            if (_fsm == null)
                InitializeStateMachine();

            _fsm.SetNextState(WeaponState.Neutral);
        }

        protected override void Update()
        {
            _fsm.Update(Time.deltaTime);
        }

        public override bool HandleWeaponInputs(bool fire1Down, bool fire1Held, bool reloadDown)
        {
            if(reloadDown)
            {
                RequestReload();
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

        // todo -- state transition control is busted
        // if chamber is ready, fire
        // else if chamber is fired, extract
        // else if chamber is empty
            // if receiver is loaded, chamber+shoot (slamfire)
            // if receiver is empty
                // if tube has ammo, extract
                // else, reload
        bool TryShoot()
        {
            if(_currentAmmo <= 0)
                RequestReload();

            else if (_lastShotTime + shotCooldown < Time.time)
            {
                if(_chamberState == ChamberState.Ready)
                {
                    _fsm.SetCurrentState(WeaponState.Firing);
                    return true;
                }

                if(_chamberState == ChamberState.Fired)
                    _fsm.SetCurrentState(WeaponState.Extracting);
                else if(_receiverLoaded)
                    _fsm.SetCurrentState(WeaponState.Chambering);

            }

            return false;
        }

        void RequestReload()
        {
            if(CurrentAmmo < MaxAmmo)
            {
                if(!IsReloading)
                    _fsm.SetCurrentState(WeaponState.Reloading);
            }
            else if(_chamberState != ChamberState.Ready)
            {
                if(_chamberState == ChamberState.Empty && _receiverLoaded)
                    _fsm.SetCurrentState(WeaponState.Chambering);
                else
                    _fsm.SetCurrentState(WeaponState.Extracting);
            }
        }

        void InitializeStateMachine()
        {
            _fsm = new StateMachine<WeaponState>();

            _fsm.AddState(new(WeaponState.Neutral));

            _fsm.AddState(new(WeaponState.Firing,
                onEnter: () =>
                {
                    FireWeapon();
                    _currentAmmo -= 1;
                    _chamberState = ChamberState.Fired;
                },
                onUpdate: _ =>
                {
                    // delay before pump = 20-30% shot cd
                    // lastShotTime instead of timeInState because interrupts (todo) shouldn't stop cooldown
                    if(CurrentAmmo != 0 && _lastShotTime + (shotCooldown * .25f) <= Time.time)
                        _fsm.SetCurrentState(WeaponState.Extracting);
                },
                onExit: () =>
                {
                    Animator.AnimateTrigger_Release();
                }
            ));

            _fsm.AddState(new(WeaponState.Reloading,
                onEnter: () =>
                {
                    IsReloading = true;
                },
                onUpdate: _ =>
                {
                    if(_fsm.TimeInState >= ammoReloadDelay)
                    {
                        _currentAmmo = Math.Min(_currentAmmo + shotsPerReload, MaxAmmo);
                        if(CurrentAmmo != MaxAmmo)
                            _fsm.ResetState();
                        else
                            RequestReload();
                    }
                },
                onExit: () =>
                {
                    IsReloading = false;
                }
            ));

            _fsm.AddState(new(WeaponState.Extracting,
                onEnter: () =>
                {
                    Animator.AnimateForendPosition(0f);
                },
                onUpdate: _ =>
                {
                    float extractLerp = _fsm.TimeInState / (shotCooldown * .30f);
                    Animator.AnimateForendPosition(extractLerp);

                    // extract time = 30-35% shot cd
                    if(_fsm.TimeInState >= shotCooldown * .30f)
                        _fsm.SetCurrentState(WeaponState.Chambering);
                },
                onExit: () =>
                {
                    Animator.AnimateShellEject();
                    _chamberState = ChamberState.Empty;
                    _receiverLoaded = true;
                }
            ));

            _fsm.AddState(new(WeaponState.Chambering,
                onEnter: () =>
                {
                    Animator.AnimateForendPosition(1f);
                },
                onUpdate: _ =>
                {
                    float extractLerp = 1f - (_fsm.TimeInState / (shotCooldown * .45f));
                    Animator.AnimateForendPosition(extractLerp);

                    // chamber time = 40-45% shot cd
                    if(_fsm.TimeInState >= shotCooldown * .45f)
                        _fsm.SetCurrentState(WeaponState.Neutral);
                },
                onExit: () =>
                {
                    Animator.AnimateForendPosition(0f);
                    _receiverLoaded = false;
                    _chamberState = ChamberState.Ready;
                }
            ));
        }
    }
}