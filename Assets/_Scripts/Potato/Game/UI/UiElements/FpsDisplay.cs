using UnityEngine;
using TMPro;

namespace Potato.Game.UI
{
    public class FramerateCounter : MonoBehaviour
    {
        [SerializeField] private float pollingTime = 0.5f;
        [SerializeField] private TMP_Text displayText;

        private float dtCount = 0f;
        private int frameCount = 0;

        void Update()
        {
            dtCount += Time.deltaTime;
            frameCount++;

            if (dtCount >= pollingTime)
            {
                int framerate = Mathf.RoundToInt((float)frameCount / dtCount);
                displayText.text = framerate.ToString();

                dtCount -= pollingTime;
                frameCount = 0;
            }
        }
    }
}