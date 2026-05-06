using UnityEngine;
using UnityEngine.Events;

namespace Potato.Gameplay
{
    public class PlayerDetector : MonoBehaviour
    {
        //public enum AttentionState { Normal, Distracted, Searching, Zero }

        [SerializeField] private PlayerCharacterControllerReference playerRef;
        //[SerializeField] private AttentionState attention = AttentionState.Normal;
        [SerializeField, Min(0f)] private float visionRange = 10f;
        [SerializeField, Range(0f, 360f)] private float visionAngle = 80f;
        [SerializeField, Min(0f)] private float hearingRange = 20f;
        //[SerializeField] private float reactionSpeed = .5f;
        [SerializeField] private float searchDelay = .5f;
        //[SerializeField] private float targetLossDelay = 2f;
        //[SerializeField, Range(0f, 1f)] private float distractionModifier = .25f;
        //[SerializeField, Range(1f, 5f)] private float searchingModifier = 2f;

        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;
        public PlayerCharacterController DetectedTarget
        {
            get => _detectedTarget;
            private set
            {
                if (_detectedTarget != value)
                {
                    _detectedTarget = value;

                    if (_detectedTarget == null)
                        onLostTarget?.Invoke();
                    else
                        onDetectedTarget?.Invoke();
                }
            }
        }

        private PlayerCharacterController _detectedTarget = null;
        private float _searchTimer = 0;

        // precomputed math
        private float _sqViewRange;
        private float _sqHearingRange;
        private float _cosHalfAngle;
        private float _sqCosHalfAngle;

        void Start()
        {
            _sqViewRange = visionRange * visionRange;
            _sqHearingRange = hearingRange * hearingRange;
            _cosHalfAngle = Mathf.Cos(visionAngle * .5f * Mathf.Deg2Rad);
            _sqCosHalfAngle = _cosHalfAngle * _cosHalfAngle;
        }

        void Update()
        {
            _searchTimer -= Time.deltaTime;
            if (_searchTimer <= 0)
            {
                DetectedTarget = LookForPlayer() ? playerRef.Value : null;
                _searchTimer = searchDelay;
            }
        }

        // todo -- sound intensity?
        bool CheckAudioSource(Vector3 position)
        {
            Vector3 toTarget = position - transform.position;
            float sqDist = Vector3.Dot(toTarget, toTarget);

            return sqDist <= _sqHearingRange;
        }

        bool LookForPlayer()
        {
            Vector3 toTarget = playerRef.Value.transform.position - transform.position;
            float sqDist = Vector3.Dot(toTarget, toTarget);

            // target should be within view range
            if (sqDist > _sqViewRange)
            {
                //Debug.Log("player outside range");
                return false;
            }

            float scalar = Vector3.Dot(transform.forward, toTarget);

            // target should be in front
            if (scalar <= 0f)
            {
                //Debug.Log("player behind");
                return false;
            }

            // target should be within cone angle
            if (scalar * scalar < sqDist * _sqCosHalfAngle)
            {
                //Debug.Log("player outside view cone");
                return false;
            }

            // todo -- raycast for obstructions/walls/etc
            //Debug.Log("player seen");
            return true;
        }
    }
}