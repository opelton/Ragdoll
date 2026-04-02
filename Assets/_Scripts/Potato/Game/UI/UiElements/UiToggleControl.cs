using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TMPro;

namespace Potato.Game.UI
{
    public class UiToggleControl : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text labelElement;
        [SerializeField] private Toggle toggleElement;
        [SerializeField] private Image fillElement;

        [Header("Settings")]
        [SerializeField] private string labelText;
        [SerializeField] private bool interactable = true;
        [SerializeField] private Color enabledColor = Color.white;
        [SerializeField] private Color disabledColor = Color.grey;

        public bool Value
        {
            get => toggleElement.isOn;
            set => toggleElement.isOn = value;
        }

        public bool Interactable
        {
            get => toggleElement.interactable;
            set => SetInteractable(value);
        }

        void Start()
        {
            Interactable = interactable;
            UpdateLabel();
        }

        public void SetInteractable(bool newValue)
        {
            interactable = newValue;
            Color interactColor = interactable ? enabledColor : disabledColor;
            labelElement.color = interactColor;
            fillElement.color = interactColor;

            toggleElement.interactable = interactable;
        }

        void UpdateLabel() => labelElement.text = labelText;

        void OnValidate()
        {
            if(labelElement != null)
                UpdateLabel();

            if(labelElement != null && toggleElement != null)
                Interactable = interactable;
        }
    }
}