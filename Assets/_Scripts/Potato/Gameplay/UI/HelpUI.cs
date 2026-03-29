using UnityEngine;
using Potato.Game;
using TMPro;

namespace Potato.Gameplay.UI
{
    public class HelpUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text pauseInputText;

        void Start()
        {
            var buttonName = InputConstants.Game_Quit.ToString();
            if(buttonName.Length >= 3)
                buttonName = buttonName[..3];
                
            pauseInputText.text = buttonName;
        }
    }
}