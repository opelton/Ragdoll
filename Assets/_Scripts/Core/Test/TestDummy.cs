using UnityEngine;

namespace Potato.Core
{
    // honsetly this is more so I remember how it's used later on
    public class TestDummy : MonoBehaviour
    {
        [SerializeField] FloatReference testFloat = 42;
        [SerializeField] IntReference testInt = 42;
        [SerializeField] BoolReference testBool = false;
        [SerializeField] StringReference testString = "test";
        [SerializeField] StringReference testString2 = new();
        [SerializeField] CardinalEnum testCardinalEnum;
        [SerializeField][LayerIndex] int testLayerIndex;
        [SerializeField] StartupMarker startupMarker;

        void OnEnable()
        {
            if(!startupMarker.WasPreInitialized)
                Debug.LogWarning("PreInitialization failed on starup marker");

            startupMarker.WasPreInitialized = false;
        }

        public void TestCallback()
        {
            Debug.Log("Bark");
        }
    }
}