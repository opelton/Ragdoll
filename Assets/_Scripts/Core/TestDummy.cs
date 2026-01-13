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
        [SerializeField] CardinalEnum testCardinalEnum;
        [SerializeField][LayerIndex] int testLayerIndex;

        public void TestCallback()
        {
            Debug.Log("Bark");
        }
    }
}