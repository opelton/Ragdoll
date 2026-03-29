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
        [SerializeField] private Image fillElement;

        [Header("Settings")]
        [SerializeField] private bool interactable = true;
        [SerializeField] private bool intMode = false;
        [SerializeField] private string labelText;
        [SerializeField] private float minValue = 0f;
        [SerializeField] private float maxValue = 1f;
        [SerializeField] private Color enabledColor = Color.white;
        [SerializeField] private Color disabledColor = Color.grey;

        
        [Header("Settings")]
        [SerializeField] private bool displayAsInt = false;
        [SerializeField] private int multiplyDisplayValue = 1;

        public UnityEvent<float> OnSliderChanged;
        public float Value
        {
            get => sliderElement.value;
            set => sliderElement.value = value;
        }
        
        public bool Interactable
        {
            get => sliderElement.interactable;
            set => SetInteractable(value);
        }

        void Awake()
        {
            UpdateLabelElement();
            UpdateSliderElement();
            sliderElement.onValueChanged.AddListener(HandleSliderChanged);

            UpdateDisplay();
            SetInteractable(interactable);
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

        public void SetInteractable(bool canInteract)
        {
            interactable = canInteract;
            Color interactColor = interactable ? enabledColor : disabledColor;
            labelElement.color = interactColor;
            displayElement.color = interactColor;
            fillElement.color = interactColor;
            sliderElement.interactable = interactable;
        }

        void UpdateLabelElement() => labelElement.text = labelText;
        void UpdateSliderElement()
        {
            ColorBlock colors = sliderElement.colors;
            colors.normalColor = enabledColor;
            colors.disabledColor = disabledColor;
            sliderElement.colors = colors;
            
            sliderElement.minValue = minValue;
            sliderElement.maxValue = maxValue;
            sliderElement.wholeNumbers = intMode;
        }

        void OnValidate()
        {
            if(labelElement != null)
                UpdateLabelElement();

            if(sliderElement != null)
            {
                UpdateSliderElement();

                if(displayElement != null)
                    UpdateDisplay();
            }

            if(labelElement != null && sliderElement != null && displayElement != null)
                SetInteractable(interactable);
        }
    }
}