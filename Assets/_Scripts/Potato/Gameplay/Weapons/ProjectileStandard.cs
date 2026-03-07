using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Potato.Gameplay
{
    public class ProjectileStandard : ProjectileBase
    {
        [Header("General")]
        public float hitRadius = 0.01f;
        public Transform projectileRoot;
        public Transform projectileTip;
        public float maxLifetime = 5f;
        public GameObject impactVfx;
        public float impactVfxDuration = 5f;
        public float impactVfxSpawnOffset = 0.1f;
        public AudioClip impactSxfClip;
        public float projectileSpeed = 20f;
        public float projectileGravityForce = 0f;

        [Tooltip(
            "Distance over which the projectile will correct its course to fit the intended trajectory (used to drift projectiles towards center of screen in First Person view). At values under 0, there is no correction")]
        public float trajectoryCorrectionDistance = -1;
        public bool inheritWeaponVelocity = false;

        [Header("damage")]
        public float damage = 40f;
        public DamageArea damageArea;
        public LayerMask hitLayers;

        [Header("Debug")]
        public Color debugDrawColor = Color.cyan * 0.2f;

        ProjectileBase _projectile;
        Vector3 _previousPosition;
        Vector3 _velocity;
        bool _hasTrajectoryOverride;
        Vector3 _trajectoryCorrectionVector;
        Vector3 _consumedTrajectoryCorrectionVector;
        List<Collider> _ignoredColliders;


        const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

        void OnEnable()
        {
            _projectile = GetComponent<ProjectileBase>();
            // DebugUtility.HandleErrorIfNullGetComponent<ProjectileBase, ProjectileStandard>(_projectile, this,
            //     gameObject);

            _projectile.OnShoot += OnShoot;

            Destroy(gameObject, maxLifetime);
        }

        // void Start()
        // {
        //     hitLayers = LayerHelper.GetCollisionMaskForLayer(gameObject.layer);
        // }

        new void OnShoot()
        {
            _previousPosition = projectileRoot.position;
            _velocity = transform.forward * projectileSpeed;
            _ignoredColliders = new List<Collider>();
            transform.position += _projectile.InheritedMuzzleVelocity * Time.deltaTime;

            // Ignore colliders of owner
            Collider[] ownerColliders = _projectile.Owner.GetComponentsInChildren<Collider>();
            _ignoredColliders.AddRange(ownerColliders);

            // Handle case of player shooting (make projectiles not go through walls, and remember center-of-screen trajectory)
            PlayerWeaponsManager playerWeaponsManager = _projectile.Owner.GetComponent<PlayerWeaponsManager>();
            if (playerWeaponsManager)
            {
                _hasTrajectoryOverride = true;

                Vector3 cameraToMuzzle = _projectile.InitialPosition -
                                          playerWeaponsManager.weaponCamera.transform.position;

                _trajectoryCorrectionVector = Vector3.ProjectOnPlane(-cameraToMuzzle,
                    playerWeaponsManager.weaponCamera.transform.forward);
                if (trajectoryCorrectionDistance == 0)
                {
                    transform.position += _trajectoryCorrectionVector;
                    _consumedTrajectoryCorrectionVector = _trajectoryCorrectionVector;
                }
                else if (trajectoryCorrectionDistance < 0)
                {
                    _hasTrajectoryOverride = false;
                }

                if (Physics.Raycast(playerWeaponsManager.weaponCamera.transform.position, cameraToMuzzle.normalized,
                    out RaycastHit hit, cameraToMuzzle.magnitude, hitLayers, k_TriggerInteraction))
                {
                    if (IsHitValid(hit))
                    {
                        OnHit(hit.point, hit.normal, hit.collider);
                    }
                }
            }
        }

        void Update()
        {
            // Move
            transform.position += _velocity * Time.deltaTime;
            if (inheritWeaponVelocity)
            {
                transform.position += _projectile.InheritedMuzzleVelocity * Time.deltaTime;
            }

            // Drift towards trajectory override (this is so that projectiles can be centered 
            // with the camera center even though the actual weapon is offset)
            if (_hasTrajectoryOverride && _consumedTrajectoryCorrectionVector.sqrMagnitude <
                _trajectoryCorrectionVector.sqrMagnitude)
            {
                Vector3 correctionLeft = _trajectoryCorrectionVector - _consumedTrajectoryCorrectionVector;
                float distanceThisFrame = (projectileRoot.position - _previousPosition).magnitude;
                Vector3 correctionThisFrame =
                    (distanceThisFrame / trajectoryCorrectionDistance) * _trajectoryCorrectionVector;
                correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
                _consumedTrajectoryCorrectionVector += correctionThisFrame;

                // Detect end of correction
                if (_consumedTrajectoryCorrectionVector.sqrMagnitude == _trajectoryCorrectionVector.sqrMagnitude)
                {
                    _hasTrajectoryOverride = false;
                }

                transform.position += correctionThisFrame;
            }

            // Orient towards velocity
            transform.forward = _velocity.normalized;

            // Gravity
            if (projectileGravityForce > 0)
            {
                // add gravity to the projectile velocity for ballistic effect
                _velocity += Vector3.down * projectileGravityForce * Time.deltaTime;
            }

            // Hit detection
            {
                RaycastHit closestHit = new RaycastHit();
                closestHit.distance = Mathf.Infinity;
                bool foundHit = false;

                // Sphere cast
                Vector3 displacementSinceLastFrame = projectileTip.position - _previousPosition;
                RaycastHit[] hits = Physics.SphereCastAll(_previousPosition, hitRadius,
                    displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, hitLayers,
                    k_TriggerInteraction);
                foreach (var hit in hits)
                {
                    if (IsHitValid(hit) && hit.distance < closestHit.distance)
                    {
                        foundHit = true;
                        closestHit = hit;
                    }
                }

                if (foundHit)
                {
                    // Handle case of casting while already inside a collider
                    if (closestHit.distance <= 0f)
                    {
                        closestHit.point = projectileRoot.position;
                        closestHit.normal = -transform.forward;
                    }

                    OnHit(closestHit.point, closestHit.normal, closestHit.collider);
                }
            }

            _previousPosition = projectileRoot.position;
        }

        bool IsHitValid(RaycastHit hit)
        {
            // // ignore hits with an ignore component
            // if (hit.collider.GetComponent<IgnoreHitDetection>())
            // {
            //     return false;
            // }

            // // ignore hits with triggers that don't have a Damageable component
            // if (hit.collider.isTrigger && hit.collider.GetComponent<Damageable>() == null)
            // {
            //     return false;
            // }

            // ignore hits with specific ignored colliders (self colliders, by default)
            if (_ignoredColliders != null && _ignoredColliders.Contains(hit.collider))
            {
                return false;
            }

            return true;
        }

        void OnHit(Vector3 point, Vector3 normal, Collider collider)
        {
            // damage
            if (damageArea)
            {
                // // area damage
                // damageArea.InflictDamageInArea(damage, point, hitLayers, k_TriggerInteraction,
                //     _projectile.Owner);
            }
            else
            {
                // // point damage
                // Damageable damageable = collider.GetComponent<Damageable>();
                // if (damageable)
                // {
                //     damageable.InflictDamage(damage, false, _projectile.Owner);
                // }
            }

            // impact vfx
            if (impactVfx)
            {
                GameObject impactVfxInstance = Instantiate(impactVfx, point + (normal * impactVfxSpawnOffset),
                    Quaternion.LookRotation(normal));
                if (impactVfxDuration > 0)
                {
                    Destroy(impactVfxInstance.gameObject, impactVfxDuration);
                }
            }

            // // impact sfx
            // if (impactSxfClip)
            // {
            //     AudioUtility.CreateSFX(impactSxfClip, point, AudioUtility.AudioGroups.Impact, 1f, 3f);
            // }

            // Self Destruct
            Destroy(this.gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = debugDrawColor;
            Gizmos.DrawSphere(transform.position, hitRadius);
        }
    }
}