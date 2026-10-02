using System;
using UnityEngine;
using UnityEngine.UI;

namespace DrinkAlcoholic
{
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] Slider bgmSlider;
        [SerializeField] Slider seSlider;
        [SerializeField] Button closeButton;

        public event Action OnClosed;

        void Awake()
        {
            bgmSlider.onValueChanged.AddListener(OnBgmChanged);
            seSlider.onValueChanged.AddListener(OnSeChanged);
            closeButton.onClick.AddListener(Close);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            bgmSlider.SetValueWithoutNotify(AudioManager.Instance.BgmVolume);
            seSlider.SetValueWithoutNotify(AudioManager.Instance.SeVolume);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            OnClosed?.Invoke();
        }

        void OnBgmChanged(float v) => AudioManager.Instance.SetBgmVolume(v);
        void OnSeChanged(float v) => AudioManager.Instance.SetSeVolume(v);
    }
}
