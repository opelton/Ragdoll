using UnityEngine;
using UnityEngine.AI;
using Potato.Core;

namespace Potato.Gameplay
{
    [RequireComponent(typeof(NavMeshAgent), typeof(ZombieAnimator), typeof(PlayerDetector))]
    public class ZombieController : MonoBehaviour
    {
        // public enum AiState { Idle, Chasing, Attacking }
        // public enum MotorState { Upright, Downed }
        //[SerializeField] private PlayerCharacterControllerReference playerRef;
        [SerializeField] private float turnSpeed = 5f;
        [SerializeField] private float followRange = 0.5f;

        [Header("Hitbox layer switching")]
        [SerializeField][LayerIndex] private int defaultLayer;
        [SerializeField][LayerIndex] private int ragdollLayer;

        public bool IsAlive { get; private set; } = true;
        private NavMeshAgent _nav;
        private ZombieAnimator _animator;
        private PlayerDetector _zombieSenses;
        private Target[] _hitboxes;
        private int _startingLayer;

        void Start()
        {
            _nav = GetComponent<NavMeshAgent>();
            _animator = GetComponent<ZombieAnimator>();
            _zombieSenses = GetComponent<PlayerDetector>();
            _hitboxes = GetComponentsInChildren<Target>();
            _animator.OnLimbAttacked += OnLimbsAttacked;
            _startingLayer = gameObject.layer;

            _zombieSenses.onDetectedTarget += _animator.OnDetectedPlayer;
            _zombieSenses.onLostTarget += _animator.OnLostPlayer;

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
                Vector3 targetDir = _zombieSenses.DetectedTarget.transform.position - transform.position;

                if (targetDir.sqrMagnitude >= followRange * followRange)
                    _nav.SetDestination(_zombieSenses.DetectedTarget.transform.position);

                // rotate toward player
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * dt);
            }

            // [0f, 1f]
            _animator.SetZombieSpeed(_nav.velocity.sqrMagnitude / (_nav.speed * _nav.speed));
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
            _animator.OnLostPlayer();

            _zombieSenses.onDetectedTarget -= _animator.OnDetectedPlayer;
            _zombieSenses.onLostTarget -= _animator.OnLostPlayer;
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