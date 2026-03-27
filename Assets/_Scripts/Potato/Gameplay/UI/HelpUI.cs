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
            pauseInputText.text = InputConstants.Game_Quit.ToString();
        }
    }
}