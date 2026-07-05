using UnityEngine;

namespace Potato.Utils
{
    public class UpVectorToNormalRGBHelper : MonoBehaviour
    {
        void Awake()
        {
            // World-space up vector
            Vector3 normal = transform.up.normalized;

            // Convert from [-1,1] to [0,1]
            Color normalColor = new(
                normal.x * 0.5f + 0.5f,
                normal.y * 0.5f + 0.5f,
                normal.z * 0.5f + 0.5f,
                1f
            );

            // Convert to 0-255 RGB values for Paint
            int r = Mathf.RoundToInt(normalColor.r * 255f);
            int g = Mathf.RoundToInt(normalColor.g * 255f);
            int b = Mathf.RoundToInt(normalColor.b * 255f);

            Debug.Log(
                $"{gameObject.name}\n" +
                $"Normal: {normal}\n" +
                $"RGB (0-1): ({normalColor.r:F3}, {normalColor.g:F3}, {normalColor.b:F3})\n" +
                $"RGB (0-255): ({r}, {g}, {b})"
            );
        }

//         // Optional: automatically report whenever the object is rotated in the editor
// #if UNITY_EDITOR
//         private void OnValidate()
//         {
//             if (!Application.isPlaying)
//                 ReportNormalMapColor();
//         }
// #endif
    }
}