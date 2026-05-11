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
        bool _triggerPulled = false;

        // shotgun state control
        StateMachine<WeaponState> _fsm;

        protected override void Awake()
        {
            base.Awake();
            if(_fsm == null)
                InitializeStateMachine();

            _fsm.SetCurrentState(WeaponState.Neutral);
        }

        void OnEnable()
        {
            if(_currentAmmo != 0 && _chamberState != ChamberState.Ready)
            {
                if(_chamberState == ChamberState.Empty && _receiverLoaded)
                    _fsm.SetCurrentState(WeaponState.Chambering);
                else
                    _fsm.SetCurrentState(WeaponState.Extracting);
            }
            else
                _fsm.SetCurrentState(WeaponState.Neutral);
        }

        void Update()
        {
            _fsm.Update(Time.deltaTime);
        }

        public override bool HandleWeaponInputs(bool fire1Down, bool fire1Held, bool reloadDown, bool isAiming)
        {
            IsAiming = isAiming;

            if(_triggerPulled != fire1Held)
            {
                if(fire1Held)
                    Animator.AnimateTrigger_Pulled(CurrentAmmo == 0);
                else
                    Animator.AnimateTrigger_Release();
            }
            _triggerPulled = fire1Held;

            if(reloadDown)
                RequestReload();
            else if(fire1Held)
                return TryShoot();

            return false;
        }

        bool TryShoot()
        {
            if(CurrentAmmo > 0 && _lastShotTime + shotCooldown < Time.time)
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

        void HandleShooting()
        {
            FireWeapon();
            _currentAmmo -= 1;
            _chamberState = ChamberState.Fired;
        }

        void RequestReload()
        {
            // if ammo can be refilled, and isn't already reloading, trigger reload
            if(CurrentAmmo < MaxAmmo)
            {
                if(!IsReloading)
                    _fsm.SetCurrentState(WeaponState.Reloading);
            }
            // if full ammo, but chamber isn't ready to fire, make it ready
            else if(_chamberState != ChamberState.Ready)
            {
                if(_chamberState == ChamberState.Empty && _receiverLoaded)
                    _fsm.SetCurrentState(WeaponState.Chambering);
                else
                    _fsm.SetCurrentState(WeaponState.Extracting);
            }
            // if the previous conditions are cleared, gun is ready to fire
            else
                _fsm.SetCurrentState(WeaponState.Neutral);
        }

        void InitializeStateMachine()
        {
            _fsm = new StateMachine<WeaponState>();

            _fsm.AddState(new(WeaponState.Neutral));

            _fsm.AddState(new(WeaponState.Firing,
                onEnter: () =>
                {
                    HandleShooting();
                },
                onUpdate: _ =>
                {
                    // delay before pump = 20-30% shot cd
                    // lastShotTime instead of timeInState because interrupts (todo) shouldn't stop cooldown
                    if(CurrentAmmo != 0 && _lastShotTime + (shotCooldown * .25f) <= Time.time)
                        _fsm.SetCurrentState(WeaponState.Extracting);
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
                        Animator.Sfx_Reload();

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
                    Animator.Sfx_Extract();
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
                    Animator.AnimateForendPosition(1f);
                    Animator.AnimateShellEject();
                    _chamberState = ChamberState.Empty;
                    _receiverLoaded = CurrentAmmo > 0;
                }
            ));

            _fsm.AddState(new(WeaponState.Chambering,
                onEnter: () =>
                {
                    Animator.AnimateForendPosition(1f);
                    Animator.Sfx_Chamber();
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
                    _chamberState = _receiverLoaded ? ChamberState.Ready : ChamberState.Empty;
                    _receiverLoaded = false;
                }
            ));
        }
    }
}