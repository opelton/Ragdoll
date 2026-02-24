using UnityEngine;

namespace Potato.Core
{
    // I'm going to be lazy and put all bootstrap injections in one config
    [CreateAssetMenu(menuName = "ScriptableObjects/Config/MainBootstrapConfig"), Tooltip("One of these must exist in Resources/Data, and it must be named MainBootstrapConfig")]
    public class MainBootstrapConfig : ScriptableObject
    {
        // asset must be in /Resources/BootstrapConfig/, and it must be named MainBootstrapConfig
        public static readonly string RelativePath = "BootstrapConfig/MainBootstrapConfig";

        public string PersistentBridgeScene;
        public string PersistentUiScene;

        public StringReference ActiveSceneName;
    }
}
