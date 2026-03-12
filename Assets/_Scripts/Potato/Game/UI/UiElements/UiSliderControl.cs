using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Potato.Game.UI
{
    public class UiSliderControl : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text labelElement;
        [SerializeField] private TMP_Text displayElement;
        [SerializeField] private Slider sliderElement;

        [Header("Settings")]
        [SerializeField] private bool intMode = false;
        [SerializeField] private string labelText;
        [SerializeField] private float minValue = 0f;
        [SerializeField] private float maxValue = 1f;

        
        [Header("Settings")]
        [SerializeField] private bool displayAsInt = false;
        [SerializeField] private int multiplyDisplayValue = 1;

        public UnityEvent<float> OnSliderChanged;
        public float Value => sliderElement.value;

        void Awake()
        {
            labelElement.text = labelText;
            sliderElement.minValue = minValue;
            sliderElement.maxValue = maxValue;
            sliderElement.wholeNumbers = intMode;
            sliderElement.onValueChanged.AddListener(HandleSliderChanged);

            UpdateDisplay();
        }

        void HandleSliderChanged(float value)
        {
            UpdateDisplay();
            OnSliderChanged.Invoke(Value);
        }

        void UpdateDisplay()
        {
            displayElement.text = displayAsInt
                ? Mathf.RoundToInt(Value * multiplyDisplayValue).ToString()
                : (Value * multiplyDisplayValue).ToString("F1");
        }

        void OnValidate()
        {
            if(labelElement != null)
                labelElement.text = labelText;

            if(sliderElement != null)
            {
                sliderElement.minValue = minValue;
                sliderElement.maxValue = maxValue;
                sliderElement.wholeNumbers = intMode;

                if(displayElement != null)
                    UpdateDisplay();
            }
        }
    }
}