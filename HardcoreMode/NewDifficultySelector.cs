using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ColorPalette DeselectedColor;

        public ColorPalette SelectedColor;

        
        private List<SelectableDifficultyButton> _difficultyButtons;

        public Dictionary<Button, MyDifficulty> Buttons
        {
            set { _difficultyButtons = value.Select(kv => new SelectableDifficultyButton(kv.Key, kv.Value)).ToList(); }
        }


        public void Start() {

            _difficultyButtons.ForEach(b => b.Button.onClick.RemoveAllListeners());

            _difficultyButtons.ForEach(b => {
                b.Button.onClick.AddListener(() => SelectDifficulty(b.Difficulty));
            });

            SelectDifficulty(MyDifficulty.Normal);
        }

        public void SelectDifficulty(MyDifficulty difficulty) {
            Singleton<GameManager>.Instance.gameModeData.currentDifficulty = (Difficulty)difficulty;
            _difficultyButtons.ForEach(b => b.UpdateSelection(
                b.Difficulty == difficulty ? SelectedColor : DeselectedColor));
        }


        private class SelectableDifficultyButton {
            private readonly Image _image;
            private readonly TMP_Text[] _textFields;

            public MyDifficulty Difficulty { get; private set; }
            public Button Button { get; private set; }
            
            public SelectableDifficultyButton(Button button, MyDifficulty difficulty) {
                Button = button;
                Difficulty = difficulty;
                _image = button.GetComponent<Image>();
                _textFields = button.GetComponentsInChildren<TMP_Text>();
            }
            
            
            public void UpdateSelection(ColorPalette colorPalette) {
                _image.color = colorPalette.ImageColor;
                _textFields.ToList().ForEach(textField => textField.color = colorPalette.TextColor);
            }
        }

        public class ColorPalette {
            public Color TextColor { get; private set; }
            public Color ImageColor { get; private set; }

            public ColorPalette(Color imageColor, Color textColor) {
                ImageColor = imageColor;
                TextColor = textColor;
            }
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
