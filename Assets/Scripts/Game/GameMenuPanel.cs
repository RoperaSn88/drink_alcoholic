using System;
using UnityEngine;
using UnityEngine.UI;

namespace DrinkAlcoholic
{
    public class GameMenuPanel : MonoBehaviour
    {
        [SerializeField] Button continueButton;
        [SerializeField] Button exitButton;
        [SerializeField] Button settingsButton;
        [SerializeField] SettingsPanel settingsPanel;

        public event Action OnContinueClicked;
        public event Action OnExitClicked;

        void Awake()
        {
            if (!ValidateReferences())
            {
                enabled = false;
                return;
            }

            continueButton.onClick.AddListener(() => OnContinueClicked?.Invoke());
            exitButton.onClick.AddListener(() => OnExitClicked?.Invoke());
            settingsButton.onClick.AddListener(OpenSettings);
            settingsPanel.OnClosed += Show;
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

        void OpenSettings()
        {
            Hide();
            settingsPanel.Open();
        }

        bool ValidateReferences()
        {
            bool valid = true;
            valid &= Require(continueButton, nameof(continueButton));
            valid &= Require(exitButton, nameof(exitButton));
            valid &= Require(settingsButton, nameof(settingsButton));
            valid &= Require(settingsPanel, nameof(settingsPanel));
            return valid;
        }

        bool Require(UnityEngine.Object reference, string fieldName)
        {
            if (reference != null) return true;
            Debug.LogError($"{nameof(GameMenuPanel)} の {fieldName} が設定されていません。", this);
            return false;
        }
    }
}
