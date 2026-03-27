using UnityEngine;

namespace Potato.Gameplay
{
    public class TimedDestruction : MonoBehaviour
    {
        [SerializeField] float destroyAfterSeconds = 10f;
        void Start() => Destroy(gameObject, destroyAfterSeconds);
    }
}