using UnityEngine;

namespace Potato.Gameplay
{
    // for objects that should shatter like a tables hit by the dark souls dodge
    public class FragileProp : MonoBehaviour
    {
        // todo 
        public void OnDamaged(float damage, GameObject source)
        {
            Debug.Log("prop destroyed!");
        }
    }
}