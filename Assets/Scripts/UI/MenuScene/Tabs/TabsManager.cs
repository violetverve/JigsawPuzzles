using System.Collections.Generic;
using UnityEngine;

namespace JigsawPuzzles.UI.MenuScene.Tabs
{
    public class TabsManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _tabs = new List<GameObject>();
        [SerializeField] private List<ButtonTMP> _buttons = new List<ButtonTMP>();

        private void OnEnable()
        {
            ButtonTMP.TabSelected += HandleTabSelected;
        }

        private void OnDisable()
        {
            ButtonTMP.TabSelected -= HandleTabSelected;
        }
       
        private void HandleTabSelected(int index)
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                _tabs[i].SetActive(i == index);
                _buttons[i].SetInteractable(i != index);
            }
        }
    }
}