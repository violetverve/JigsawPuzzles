using System;
using UnityEngine;
using Player;

namespace UI.GameScene.Themes
{
    public class SingleChoiceManager : MonoBehaviour
    {
        public static event Action<ThemeToggle> OnThemeSelected;

        [SerializeField] private ThemesGrid _themesGrid;

        void Start()
        {
            foreach (var themeToggle in _themesGrid.Toggles)
            {
                themeToggle.Toggle.onValueChanged
                    .AddListener(delegate { OnToggleChanged(themeToggle); });
            }

            if (PlayerData.Instance != null)
            {
                SetTheme(PlayerData.Instance.ThemeID);
            }
        }


        private void SetTheme(int themeID)
        {
            if (themeID > _themesGrid.Toggles.Count - 1)
            {
                themeID = 0;
            }

            ThemeToggle themeToggle = _themesGrid.Toggles[themeID];

            themeToggle.Toggle.isOn = true;
            DeselectOtherToggles(themeToggle);

            OnThemeSelected(themeToggle);
        }

        private void OnToggleChanged(ThemeToggle changedThemeToggle)
        {
            if (changedThemeToggle.Toggle.isOn)
            {
                int themeIndex = _themesGrid.Toggles.IndexOf(changedThemeToggle);
                PlayerData.Instance.SaveThemeID(themeIndex);

                DeselectOtherToggles(changedThemeToggle);
                OnThemeSelected?.Invoke(changedThemeToggle);
            }
        }

        private void DeselectOtherToggles(ThemeToggle selectedToggle)
        {
            foreach (var themeToggle in _themesGrid.Toggles)
            {
                if (themeToggle != selectedToggle)
                {
                    themeToggle.Toggle.isOn = false;
                }
            }
        }
    }
}
