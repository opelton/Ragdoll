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
        public bool inheritWeaponVelocity = false;

        [Header("damage")]
        public int damage = 40;
        public DamageArea damageArea;
        public LayerMask hitLayers;

        [Header("Debug")]
        public Color debugDrawColor = Color.cyan * 0.2f;

        Vector3 _previousPosition;
        Vector3 _velocity;
        List<Collider> _ignoredColliders;


        const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

        void OnEnable()
        {
            Destroy(gameObject, maxLifetime);
        }

        protected override void HandleOnShoot()
        {
            _previousPosition = projectileRoot.position;
            _velocity = InitialDirection * projectileSpeed;
            _ignoredColliders = new List<Collider>();

            // Ignore colliders of owner
            Collider[] ownerColliders = Owner.GetComponentsInChildren<Collider>();
            _ignoredColliders.AddRange(ownerColliders);

            // todo -- this shouldn't be the projectile's job
            // Handle case of player shooting (make projectiles not go through walls, and remember center-of-screen trajectory)
            PlayerWeaponsManager playerWeaponsManager = Owner.GetComponent<PlayerWeaponsManager>();
            if (playerWeaponsManager)
            {
                Vector3 cameraToMuzzle = InitialPosition -
                                          playerWeaponsManager.AimCams.AimPos;

                if (Physics.Raycast(playerWeaponsManager.AimCams.AimPos, cameraToMuzzle.normalized,
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
                transform.position += InheritedMuzzleVelocity * Time.deltaTime;
            }

            // Gravity
            if (projectileGravityForce != 0)
            {
                // Orient towards velocity
                transform.forward = _velocity.normalized;

                // add gravity to the projectile velocity for ballistic effect
                _velocity += projectileGravityForce * Time.deltaTime * Vector3.down;
            }

            // Hit detection
            {
                RaycastHit closestHit = new() { distance = Mathf.Infinity };
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
            // ignore hits with triggers that don't have a Damageable component
            if (hit.collider.isTrigger && hit.collider.GetComponent<Target>() == null)
                return false;

            // ignore hits with specific ignored colliders (self colliders, by default)
            if (_ignoredColliders != null && _ignoredColliders.Contains(hit.collider))
                return false;

            return true;
        }

        void OnHit(Vector3 point, Vector3 normal, Collider collider)
        {
            // damage
            if (damageArea)
            {
                // area damage
                damageArea.InflictDamageInArea(damage, point, hitLayers, k_TriggerInteraction, Owner);
            }
            else
            {
                // point damage
                Target target = collider.GetComponent<Target>();
                if (target)
                    target.InflictDamage(new(damage, Owner, point, -normal));
            }

            // impact vfx
            if (impactVfx)
            {
                GameObject impactVfxInstance = Instantiate(impactVfx, point + (normal * impactVfxSpawnOffset),
                    Quaternion.LookRotation(normal));
                if (impactVfxDuration > 0)
                {
                    Destroy(impactVfxInstance, impactVfxDuration);
                }
            }

            // // impact sfx
            // if (impactSxfClip)
            // {
            //     AudioUtility.CreateSFX(impactSxfClip, point, AudioUtility.AudioGroups.Impact, 1f, 3f);
            // }

            // Self Destruct
            Destroy(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = debugDrawColor;
            Gizmos.DrawSphere(transform.position, hitRadius);
        }
    }
}