using UnityEngine;
using Potato.Core;

namespace Potato.Game
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Input/IntAxis")]
    public class InputIntAxis : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] internal string _description;
#endif
        public Vector2Int Value { get; private set;} = Vector2Int.zero;
        [SerializeField] internal Vec2IntEvent onAxisChanged;

        // dpad
        public void UpdateState(bool left, bool right, bool up, bool down)
        {
            Vector2Int newAxis = FromDirections(left, right, up, down);

            if(onAxisChanged != null && Value != newAxis)
                onAxisChanged.Invoke(newAxis, this);
            
            Value = newAxis;
        }

        Vector2Int FromDirections(bool left, bool right, bool up, bool down)
        {
            Vector2Int newAxis = Vector2Int.zero;

            if(right)   newAxis.x += 1;
            if(left)    newAxis.x -= 1;
            if(up)      newAxis.y += 1;
            if(down)    newAxis.y -= 1;

            return newAxis;
        }

        // stick
        public void UpdateState(float horizontal, float vertical, float deadzone = .2f)
        {
            Vector2Int newAxis = FromAxis(horizontal, vertical, deadzone);            

            if(onAxisChanged != null && Value != newAxis)
                onAxisChanged.Invoke(newAxis, this);
            
            Value = newAxis;
        }

        Vector2Int FromAxis(float horizontal, float vertical, float deadzone)
        {
            Vector2Int newAxis = Vector2Int.zero;
            if(horizontal > deadzone)       newAxis.x = 1;
            else if(horizontal < -deadzone) newAxis.x = -1;
            if(vertical > deadzone)         newAxis.y = 1;
            else if(vertical < -deadzone)   newAxis.y = -1;
            return newAxis;            
        }

        public void ResetState(bool left, bool right, bool up, bool down) => Value = FromDirections(left, right, up, down);
        public void ResetState(float horizontal, float vertical, float deadzone = .2f) => Value = FromAxis(horizontal, vertical, deadzone);
    }
}