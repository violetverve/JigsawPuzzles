using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace UI.MenuScene
{
    public class RotationSwitchToggle : MonoBehaviour 
    {
        [SerializeField] private RectTransform _uiHandleRectTransform;
        [SerializeField] private Color _handleActiveColor;
        [SerializeField] private Color _onImageActiveColor;
        [SerializeField] private Color _offImageActiveColor;
        [SerializeField] private Color _defaultIconImageColor;
        
        [SerializeField] private Image _onImage;
        [SerializeField] private Image _offImage;

        [SerializeField] private TextMeshProUGUI _rotationEnabledText;

        [SerializeField] private float _animDuration = 0.4f;
        [SerializeField] private float _tiltAngle = 20f;

        private Image _backgroundImage;
        private Image _handleImage;

        private Color _backgroundDefaultColor;
        private Color _handleDefaultColor;
        private Toggle _toggle ;

        Vector2 handlePosition ;

        void Awake ( ) {
            _toggle = GetComponent<Toggle>();

            handlePosition = _uiHandleRectTransform.anchoredPosition;

            _backgroundImage = _uiHandleRectTransform.parent.GetComponent<Image>();
            _handleImage = _uiHandleRectTransform.GetComponent<Image>();

            _backgroundDefaultColor = _backgroundImage.color;
            _handleDefaultColor = _handleImage.color;

            _toggle.onValueChanged.AddListener(OnSwitch);

            if (_toggle.isOn)
                OnSwitch(true);
        }

        private void OnDisable()
        {
            _toggle.isOn = false;
            OnSwitch(false);
        }


        void OnSwitch (bool on) {
            _uiHandleRectTransform.DOAnchorPos(on ? handlePosition * -1 : handlePosition, _animDuration).SetEase(Ease.InOutBack);

            _handleImage.DOColor(on ? _handleActiveColor : _handleDefaultColor, _animDuration);

            _onImage.DOColor(on ? _onImageActiveColor : _defaultIconImageColor, _animDuration);
            _offImage.DOColor(on ? _defaultIconImageColor : _offImageActiveColor, _animDuration);

            TiltAnimation(on);
            SetText(on);
        }

        private void TiltAnimation(bool on) {
            Sequence sequence = DOTween.Sequence();

            float tiltAngle = on ? -_tiltAngle : _tiltAngle;
    
            sequence.Append(_onImage.transform.DORotate(new Vector3(0, 0, tiltAngle), _animDuration / 2).SetEase(Ease.InOutBack))
                    .Append(_onImage.transform.DORotate(new Vector3(0, 0, 0), _animDuration / 2).SetEase(Ease.InOutBack));
        }

        private void SetText(bool on) {
            _rotationEnabledText.text = "Rotate pieces: " + (on ? "Yes" : "No");
        }

        void OnDestroy ( ) {
            _toggle.onValueChanged.RemoveListener (OnSwitch) ;
        }
    }
}