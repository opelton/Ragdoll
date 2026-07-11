using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Potato.Gameplay
{
    public class ZombieAttackController : MonoBehaviour
    {
        bool _playerDetected = false;
        public void OnPlayerDetected()
        {
            _playerDetected = true;
        }
        
        public void OnPlayerLost()
        {
            _playerDetected = false;
        }
    }
}