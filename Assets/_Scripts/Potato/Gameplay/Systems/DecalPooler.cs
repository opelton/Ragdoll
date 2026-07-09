using System;
using System.Collections.Generic;
using UnityEngine;

namespace Potato.Gameplay
{
    [Serializable]
    public class DecalSpawnData
    {
        public GameObject decalPrefab;
        public Vector3 position;
        public Quaternion rotation;
        public Transform parent;
        
        public DecalSpawnData(GameObject decal, Vector3 decalPosition, Quaternion decalRotation, Transform attachedTransform)
        {
            decalPrefab = decal;
            position = decalPosition;
            rotation = decalRotation;
            parent = attachedTransform;
        }
    }

    public class DecalPooler : MonoBehaviour
    {
        [SerializeField] private int poolSize = 50;
        private Dictionary<GameObject, GameObject[]> _decalPools = new();   // pooled decals are reused circularly
        private Dictionary<GameObject, int> _poolTracker = new();           // live index of each pool

        void OnDestroy()
        {
            foreach(var pool in _decalPools.Values)
                foreach(var obj in pool)
                    if(obj != null)
                        obj.SetActive(false);
        }

        public void CreateDecalPool(GameObject decalPrefab)
        {
            if(decalPrefab == null || _decalPools.ContainsKey(decalPrefab))
                return;

            var newPool = new GameObject[poolSize];
            for(int i = 0; i < poolSize; ++i)
            {
                var decal = Instantiate(decalPrefab, transform);
                decal.SetActive(false);
                newPool[i] = decal;
            }
            _decalPools.Add(decalPrefab, newPool);
            _poolTracker.Add(decalPrefab, 0);
        }

        public void SpawnDecal(DecalSpawnData data)
        {
            if(!_decalPools.ContainsKey(data.decalPrefab))
                Debug.LogWarning($"decal does not have a pool");
            
            var decal = GetPooledDecal(data.decalPrefab);
            decal.transform.SetPositionAndRotation(data.position, data.rotation);
            decal.transform.SetParent(data.parent);
            decal.SetActive(true);
        }

        GameObject GetPooledDecal(GameObject prefab)
        {
            if(_poolTracker[prefab] >= poolSize)
                _poolTracker[prefab] = 0;

            return _decalPools[prefab][_poolTracker[prefab]++];
        }
    }
}