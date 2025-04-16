using UnityEngine;
using UnityEngine.UI;
using System;
using Player;
using TMPro;

namespace UI
{
    public class Coins : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private Image _coinsImage;
        
        public static Action CoinsChanged;
        public Image CoinsImage => _coinsImage;
        
        private void OnEnable()
        {
            CoinsChanged += UpdateCoins;
            UpdateCoins();
        }

        private void OnDisable()
        {
            CoinsChanged -= UpdateCoins;
        }

        private void Start()
        {
            UpdateCoins();
        }

        private void UpdateCoins()
        {            
            _coinsText.text = PlayerData.Instance.Coins.ToString();
        }

        public void SetCoinsText(int coins)
        {
            _coinsText.text = coins.ToString();
        }

        public void AddCoinsText(int coins)
        {
            _coinsText.text = (int.Parse(_coinsText.text) + coins).ToString();
        }

    }
}