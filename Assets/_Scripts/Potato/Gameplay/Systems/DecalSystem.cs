using Potato.Core;
using UnityEngine;

namespace Potato.Gameplay
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Systems/Decals")]
    public class DecalSystem : ScriptableObject
    {
        [SerializeField] private DecalSpawnEvent decalRequest;
        [SerializeField] private GameObjectEvent requestPoolEvent;
        [SerializeField] private float decalPadding = .025f;

        public void RegisterDecal(GameObject prefab)
        {
            requestPoolEvent.Invoke(prefab, this);
        }

        public void CreateBulletHoleDecal(GameObject decal, Vector3 pos, Vector3 surfaceNormal, Transform parent)
        {
            var data = new DecalSpawnData(
                decal,
                pos + (decalPadding * surfaceNormal),
                Quaternion.LookRotation(-surfaceNormal) * Quaternion.Euler(0f, 0f, Random.Range(0, 180f)),
                parent);

            decalRequest.Invoke(data, this);
        }
    }
}