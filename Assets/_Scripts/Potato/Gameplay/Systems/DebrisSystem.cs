using UnityEngine;
using Potato.Game;

namespace Potato.Gameplay
{
    // Owner of spawning junk in game or first-person space
    [CreateAssetMenu(menuName = "ScriptableObjects/Systems/Debris")]
    public class DebrisSystem : ScriptableObject
    {
        [SerializeField] private GameplayCameraDataReference gameCams;

        public GameObject SpawnWeaponSpacePrefabInWorldSpace(GameObject prefab, Vector3 weaponSpacePosition, Quaternion weaponSpaceRotation, Vector3 velocity, float spin = 0f)
        {
            GameObject obj = SpawnWeaponSpacePrefabInWorldSpace(prefab, weaponSpacePosition, weaponSpaceRotation);
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.velocity = velocity;
            
            if(spin != 0)
                rb.angularVelocity = Random.insideUnitSphere * spin;

            return obj;
        }

        // translates prefab in gameplayCamera's layer to the position it would occupy using fpsCamera's view matrix
        public GameObject SpawnWeaponSpacePrefabInWorldSpace(GameObject prefab, Vector3 weaponSpacePosition, Quaternion weaponSpaceRotation)
        {
            return Instantiate(prefab,
                WeaponToGameSpacePosition(weaponSpacePosition),
                WeaponToGameSpaceRotation(weaponSpaceRotation));
        }

        public Vector3 WeaponToGameSpacePosition(Vector3 weaponSpacePosition)
        {
            Vector3 screenPos = gameCams.Value.fpsCamera.WorldToViewportPoint(weaponSpacePosition);
            return gameCams.Value.gameplayCamera.ViewportToWorldPoint(screenPos);
        }

        public Quaternion WeaponToGameSpaceRotation(Quaternion weaponSpaceRotation)
        {
            Quaternion relativeRotation = Quaternion.Inverse(gameCams.Value.fpsCamera.transform.rotation) * weaponSpaceRotation;
            return gameCams.Value.gameplayCamera.transform.rotation * relativeRotation;
        }
    }
}