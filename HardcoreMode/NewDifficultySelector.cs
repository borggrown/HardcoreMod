using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HardcoreMode {

    /*
     * Replacement for DifficultySelector that supports more than 3 difficulties.
     */
    public class NewDifficultySelector: MonoBehaviour {
        public Color deselectedColor;

        public Color selectedColor;

        public Dictionary<Button, MyDifficulty> Buttons;

        private Dictionary<MyDifficulty, Action<Boolean>> _toggleButtonSelection;

        public void Start() {

            Buttons.ForEach((button, difficulty) => button.onClick.RemoveAllListeners());

            _toggleButtonSelection = Buttons.ToDictionary(kv => kv.Value, kv => {
                var button = kv.Key;
                var difficulty = kv.Value;

                var image = button.GetComponent<Image>();
                var textFields = button.GetComponentsInChildren<TMP_Text>();

                Action<bool> action = (selected) => {
                    var buttonColor = selected ? selectedColor : deselectedColor;
                    var textColor = selected ? Color.white : Color.black;
                    image.color = buttonColor;
                    textFields.ToList().ForEach(text => text.color = textColor);
                };

                return action;
            });

            Buttons.ForEach((button, difficulty) => {
                button.onClick.AddListener(() => SelectDifficulty(difficulty));
            });

            SelectDifficulty(MyDifficulty.Normal);
        }

        public void SelectDifficulty(MyDifficulty difficulty) {
            Singleton<GameManager>.Instance.gameModeData.currentDifficulty = (Difficulty)difficulty;
            _toggleButtonSelection.ForEach((d, a) => a(d == difficulty));
        }


    }

    // Replacement for Difficulty. Custom difficulties start from 10 upwards to leave space for future vanilla difficulties.
    public enum MyDifficulty: int {
        Relaxed = Difficulty.Relaxed,
        Normal = Difficulty.Normal,
        Hard = Difficulty.Hard,
        Expert = 10,
        Hardcore = 11,
        Dead = 20,
    }
}
