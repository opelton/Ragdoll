using UnityEngine;

namespace Potato.Gameplay
{
    public class PlayerDetector : MonoBehaviour
    {
        public enum AttentionState { Normal, Distracted, Searching, Zero }

        [SerializeField] private PlayerCharacterControllerReference playerRef;
        [SerializeField] private AttentionState attention = AttentionState.Normal;
        [SerializeField, Min(0f)] private float visionRange = 10f;
        [SerializeField, Range(0f, 360f)] private float visionAngle = 80f;
        [SerializeField, Min(0f)] private float hearingRange = 20f;
        [SerializeField, Range(0f, 1f)] private float distractionModifier = .25f;
        [SerializeField, Range(1f, 5f)] private float searchingModifier = 2f;
    }
}