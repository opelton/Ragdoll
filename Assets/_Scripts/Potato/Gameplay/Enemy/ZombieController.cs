using UnityEngine;
using UnityEngine.AI;
using Potato.Core;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(NavMeshAgent), typeof(ZombieAnimator), typeof(PlayerDetector))]
    [RequireComponent(typeof(ZombieAttackController))]
    public class ZombieController : MonoBehaviour
    {
        // public enum AiState { Idle, Chasing, Attacking }
        // public enum MotorState { Upright, Downed }
        [SerializeField] private float turnSpeed = 5f;
        [SerializeField] private float followRange = 0.5f;

        [Header("Hitbox layer switching")]
        [SerializeField][LayerIndex] private int defaultLayer;
        [SerializeField][LayerIndex] private int ragdollLayer;

        public bool IsAlive { get; private set; } = true;
        private NavMeshAgent _nav;
        private ZombieAnimator _animator;
        private PlayerDetector _zombieSenses;
        private ZombieAttackController _attackController;
        private Target[] _hitboxes;
        private int _startingLayer;

        void Start()
        {
            _nav = GetComponent<NavMeshAgent>();
            _animator = GetComponent<ZombieAnimator>();
            _zombieSenses = GetComponent<PlayerDetector>();
            _attackController = GetComponent<ZombieAttackController>();
            _hitboxes = GetComponentsInChildren<Target>();
            _animator.OnLimbAttacked += OnLimbsAttacked;
            _startingLayer = gameObject.layer;

            _zombieSenses.onDetectedTarget += HandlePlayerDetected;
            _zombieSenses.onLostTarget += HandlePlayerLost;

            ToggleHitboxes(true);
        }

        void Update()
        {
            if (!IsAlive)
                return;

            var dt = Time.deltaTime;
            _zombieSenses.UpdateSenses(dt);

            if (_zombieSenses.DetectedTarget != null)
            {
                // move to player
                Vector3 targetDir = _zombieSenses.VectorToTarget();

                if (targetDir.sqrMagnitude >= followRange * followRange)
                    _nav.SetDestination(_zombieSenses.DetectedTarget.transform.position);

                // rotate toward player
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * dt);
            }

            // [0f, 1f]
            _animator.SetZombieSpeed(_nav.velocity.magnitude / _nav.speed);
        }

        void HandlePlayerDetected()
        {
            _animator.OnPlayerDetected();
            _attackController.OnPlayerDetected();
        }

        void HandlePlayerLost()
        {
            _animator.OnPlayerLost();
            _attackController.OnPlayerLost();
        }

        void OnLimbsAttacked()
        {
            ToggleHitboxes(false);
            _animator.OnLimbAttacked -= OnLimbsAttacked;
            OnKilled();
        }

        void OnKilled()
        {
            IsAlive = false;
            _nav.isStopped = true;
            HandlePlayerLost();

            _zombieSenses.onDetectedTarget -= HandlePlayerDetected;
            _zombieSenses.onLostTarget -= HandlePlayerLost;
        }

        void ToggleHitboxes(bool alive)
        {
            var teamId = alive ? Target.Team.Hostile : Target.Team.Neutral;
            var layer = alive ? defaultLayer : ragdollLayer;

            foreach (var hitbox in _hitboxes)
            {
                hitbox.TeamId = teamId;
                hitbox.gameObject.layer = layer;
            }

            gameObject.layer = alive ? _startingLayer : layer;
        }
    }
}