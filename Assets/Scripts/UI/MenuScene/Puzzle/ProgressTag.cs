using UnityEngine;
using TMPro;

namespace UI.MenuScene.Puzzle
{
    public class ProgressTag : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _progressText;

        public void SetProgressText(int percents)
        {
            if (percents == 0)
            {
                percents = 1;
            }
            
            _progressText.text = percents + "%";
        }
    }
}