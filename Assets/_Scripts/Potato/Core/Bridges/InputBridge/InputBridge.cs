using Potato.Core;
using UnityEngine;

namespace Core.Potato
{
    public class InputBridge : MonoBehaviour
    {
        [SerializeField] InputContext DefaultInputContext;

        void Update()
        {
            // check all inputs the context cares about
            // have the context process input
        }
    }
}