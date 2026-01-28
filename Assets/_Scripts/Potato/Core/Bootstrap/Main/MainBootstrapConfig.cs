using UnityEngine;

namespace Potato.Core
{
    // I'm going to be lazy and put all bootstrap injections in one config
    [CreateAssetMenu(menuName = "ScriptableObjects/Config/MainBootstrapConfig"), Tooltip("One of these must exist in Resources/Data, and it must be named MainBootstrapConfig")]
    public class MainBootstrapConfig : ScriptableObject
    {
        // Asset must exist, and it must have this name
        public static readonly string RelativePath = "Data/MainBootstrapConfig";

        // todo -- SO
        public string PersistentBridgeScene;
    }
}
