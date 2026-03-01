using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Input/FloatAxis")]
    public class InputFloatAxis : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] internal string _description;
#endif
        public Vector2 Value { get; private set;} = Vector2.zero;
        [SerializeField] internal Vec2Event onAxisChanged;

        public void UpdateState(float horizontal, float vertical)
        {
            Vector2 newAxis = new (horizontal, vertical);

            if(onAxisChanged != null && Value != newAxis)
                onAxisChanged.Invoke(newAxis, this);
            
            Value = newAxis;
        }

        public void ResetState(float x = 0, float y = 0) => Value = new(x, y);
    }
}